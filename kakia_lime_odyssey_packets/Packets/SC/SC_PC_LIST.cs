using kakia_lime_odyssey_packets.Packets.Models;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_packets.Packets.SC;

/// <summary>
/// Server sends the list of characters for the logged-in account.
/// Sent immediately after SC_LOGIN_RESULT when login is successful.
/// Contains count followed by variable-length array of CLIENT_PC structs.
/// </summary>
/// <remarks>
/// IDA Verified: Yes (2025-11-26)
/// IDA Struct: PACKET_SC_PC_LIST
/// Size: 5 bytes (1 byte count) + (count * 220 bytes per CLIENT_PC)
/// Total with header: 5 + (count * 220) bytes
/// Triggered by: CS_LOGIN (after successful authentication)
/// Handler: CSeparateHandler::sc_pc_list @ 0x5b304c
/// Note: The packet header (PACKET_VAR) contains size and opcode (4 bytes).
/// The count field is at offset 4, followed by CLIENT_PC array at offset 5.
/// Each CLIENT_PC is 220 bytes (68 bytes SAVED_STATUS_PC + 152 bytes APPEARANCE_PC).
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SC_PC_LIST
{
    public byte count;
    public CLIENT_PC[] pc_list;
}
