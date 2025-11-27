
using kakia_lime_odyssey_packets.Packets.CS;
using System.Text;

namespace kakia_lime_odyssey_packets
{
	public class PacketReader : BinaryReader
	{
		public PacketReader(byte[] bytes) : base(new MemoryStream(bytes)) { }

		public string ReadFixedLengthAscii(ushort charCount)
		{
			return ReadFixedLengthAscii((uint)charCount);
		}

		public string ReadFixedLengthAscii(short charCount)
		{
			return ReadFixedLengthAscii((uint)charCount);
		}

		public string ReadFixedLengthAscii(int charCount)
		{
			return ReadFixedLengthAscii((uint)charCount);
		}

		public string ReadFixedLengthAscii(uint charCount)
		{
			var data = ReadBytes((int)charCount);
			var str = Encoding.ASCII.GetString(data);

			var endAt = str.IndexOf('\0');
			if (endAt == -1)
				return str;
			return str[..endAt];
		}

		public string ReadFixedLengthUtf16(int charCount)
		{
			var data = ReadBytes(charCount * 2);
			var str = Encoding.GetEncoding("UTF-16").GetString(data);

			var endAt = str.IndexOf('\0');
			if (endAt == -1)
				return str;
			return str[..endAt];
		}

		public CS_SAY_PC Read_CS_SAY_PC(int size)
		{
			ReadBytes(2); // Skip 2
			CS_SAY_PC say = new()
			{
				maintainTime = ReadUInt32(),
				type = ReadInt32()
			};
			say.message = ReadFixedLengthAscii(size - 8);
			return say;
		}

		public CS_WHISPER_PC Read_CS_WHISPER_PC(int size)
		{
			// CS_WHISPER_PC structure:
			// +0x00: targetPCName (char[26]) - 26 bytes
			// +0x1A: message (variable length)
			CS_WHISPER_PC whisper = new()
			{
				targetPCName = ReadFixedLengthAscii(26)
			};
			// Remaining bytes are the message (size - 26 for the name)
			int messageLength = size - 26;
			if (messageLength > 0)
			{
				whisper.message = ReadFixedLengthAscii(messageLength);
			}
			else
			{
				whisper.message = string.Empty;
			}
			return whisper;
		}
	}
}
