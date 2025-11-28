/// <summary>
/// Server->Client packet broadcast when a player/entity stops moving.
/// </summary>
/// <remarks>
/// IDA Verified: 2025-11-26
/// IDA Struct: PACKET_SC_STOP
/// Size: 37 bytes (39 with PACKET_FIX header)
/// Triggered by: CS_STOP_PC
/// </remarks>
using kakia_lime_odyssey_packets.Packets.Interface;
using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_STOP : IPacketFixed
{
	/// <summary>Instance ID of the object that stopped</summary>
	public long objInstID;

	/// <summary>Final position where the object stopped</summary>
	public FPOS pos;

	/// <summary>Direction the object is facing after stopping</summary>
	public FPOS dir;

	/// <summary>Server tick when the stop occurred</summary>
	public uint tick;

	/// <summary>Type of stop (0=normal, 1=forced, etc.)</summary>
	public byte stopType;
}
