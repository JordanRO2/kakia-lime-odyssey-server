using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network.Crypto;
using kakia_lime_odyssey_packets;

namespace kakia_lime_odyssey_network;

public class RawPacket
{
	private const int MAX_PACKET_SIZE = 4096;

	public PacketType PacketId { get; set; }
	public UInt16 Size { get; set; }
	public byte[] Payload { get; set; } = Array.Empty<byte>();

	public static List<RawPacket> ParsePackets(byte[] data)
	{
		List<RawPacket> packets = new();
		int pos = 0;
		while(pos < data.Length)
		{
			try
			{
				int totalSize = BitConverter.ToUInt16(data, pos);

				if (totalSize > MAX_PACKET_SIZE)
				{
					throw new InvalidOperationException($"Packet exceeds maximum size: {totalSize} bytes (max: {MAX_PACKET_SIZE})");
				}

				var p = new RawPacket()
				{
					Size = BitConverter.ToUInt16(data.AsSpan(pos + 2, 2))
				};

				var temp = AesLime.Decrypt(data[(pos + 4)..(pos + p.Size + 4)]);

				if (temp.Length > 2)
					p.Payload = temp[2..].ToArray();

				p.PacketId = (PacketType)BitConverter.ToUInt16(temp.AsSpan(0, 2));

				pos += totalSize;
				packets.Add(p);
			}
			catch (Exception ex)
			{
				Logger.Log(ex);
				throw new Exception("Crypto failed.", ex);
			}
		}
		return packets;
	}

	public string GetName()
	{
		return Enum.GetName(typeof(PacketType), PacketId)!;
	}
}
