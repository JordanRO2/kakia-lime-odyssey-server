using kakia_lime_odyssey_logging;
using System.Net;
using System.Net.Sockets;

namespace kakia_lime_odyssey_network
{
	public class SocketClient
	{
		public Func<RawPacket, Task>? PacketReceived;

		private Socket _socket { get; set; }
		private byte[] _buffer { get; set; }
		private int _position = 0;
		public bool IsAlive { get; set; }

		public int Id { get; set; }		

		public SocketClient(Socket socket)
		{
			IsAlive = true;
			_socket = socket;
			_buffer = new byte[1024 * 10];
			Id = socket.Handle.ToInt32();
		}

		public async Task BeginRead()
		{
			try
			{
				var seg = new ArraySegment<byte>(_buffer, _position, _buffer.Length - _position);
				var len = await _socket.ReceiveAsync(seg);
				if (len <= 0)
				{
					IsAlive = false;
					Logger.Log($"{GetIP()} disconnected.");
					return;
				}
				await HandleData(len);
			}
			catch (SocketException ex) when (IsNormalDisconnect(ex.SocketErrorCode))
			{
				// Normal client disconnect - log as info, not error
				IsAlive = false;
				Logger.Log($"{GetIP()} disconnected (client closed connection).");
			}
			catch (Exception ex)
			{
				Logger.Log(ex);
				IsAlive = false;
			}
		}

		/// <summary>
		/// Checks if the socket error indicates a normal client disconnect
		/// </summary>
		private static bool IsNormalDisconnect(SocketError error)
		{
			return error == SocketError.ConnectionReset ||
			       error == SocketError.ConnectionAborted ||
			       error == SocketError.Shutdown ||
			       error == SocketError.OperationAborted;
		}

		private async Task HandleData(int len)
		{
			try
			{
				var packets = RawPacket.ParsePackets(_buffer[_position..len]);
				foreach(var packet in packets)
					await PacketReceived!.Invoke(packet);
			}
			catch (Exception ex)
			{
				Logger.Log(ex);
			}
			await BeginRead();
		}

		public async Task Send(byte[] packet)
		{
			await _socket.SendAsync(packet);
		}

		public string GetIP()
		{
			if (!_socket.Connected)
				return string.Empty;
			var ip = _socket.RemoteEndPoint as IPEndPoint;
			return ip?.Address.ToString() ?? string.Empty;
		}

		/// <summary>
		/// Forcibly disconnect this client
		/// </summary>
		public void Disconnect()
		{
			try
			{
				IsAlive = false;
				_socket.Shutdown(SocketShutdown.Both);
				_socket.Close();
				Logger.Log($"[DISCONNECT] Client {GetIP()} forcibly disconnected");
			}
			catch (Exception ex)
			{
				Logger.Log($"[DISCONNECT] Error disconnecting client: {ex.Message}");
			}
		}
	}
}