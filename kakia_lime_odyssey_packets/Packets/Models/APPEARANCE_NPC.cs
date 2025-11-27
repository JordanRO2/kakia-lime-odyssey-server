using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.Models;

/// <summary>
/// Base NPC appearance structure used for NPCs, villagers, and as base for monsters.
/// Contains visual appearance, animation state, and identification data.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: APPEARANCE_NPC
/// Size: 60 bytes
/// Memory Layout (IDA):
/// - 0x00-0x1E: char[31] name (31 bytes) - NPC display name (null-terminated string)
/// - 0x1F: padding (1 byte) - alignment padding
/// - 0x20-0x23: unsigned int action (4 bytes) - Current animation/action ID
/// - 0x24-0x27: unsigned int actionStartTick (4 bytes) - Animation start time
/// - 0x28-0x2B: float scale (4 bytes) - Model scale multiplier
/// - 0x2C-0x2F: float transparent (4 bytes) - Transparency value (0.0-1.0)
/// - 0x30-0x33: unsigned int modelTypeID (4 bytes) - Model type identifier
/// - 0x34-0x36: COLOR color (3 bytes) - RGB color tint (r, g, b bytes)
/// - 0x37: padding (1 byte) - alignment padding
/// - 0x38-0x3B: unsigned int typeID (4 bytes) - NPC type/template ID
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct APPEARANCE_NPC
{
	/// <summary>NPC display name (31 bytes, null-terminated)</summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
	public byte[] name;

	/// <summary>Padding byte for alignment (offset 0x1F)</summary>
	public byte padding;

	/// <summary>Current animation/action ID (offset 0x20)</summary>
	public uint action;

	/// <summary>Animation start time (offset 0x24)</summary>
	public uint actionStartTick;

	/// <summary>Model scale multiplier (offset 0x28)</summary>
	public float scale;

	/// <summary>Transparency value 0.0-1.0 (offset 0x2C)</summary>
	public float transparent;

	/// <summary>Model type identifier (offset 0x30)</summary>
	public uint modelTypeID;

	/// <summary>RGB color tint (offset 0x34, 3 bytes)</summary>
	public COLOR color;

	/// <summary>Padding byte for alignment (offset 0x37)</summary>
	public byte padding2;

	/// <summary>NPC type/template ID (offset 0x38)</summary>
	public uint typeID;
}
