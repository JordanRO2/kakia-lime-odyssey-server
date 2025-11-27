using kakia_lime_odyssey_logging;
using kakia_lime_odyssey_network;
using kakia_lime_odyssey_network.Handler;
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using kakia_lime_odyssey_packets.Packets.CS;
using kakia_lime_odyssey_packets.Packets.Enums;
using kakia_lime_odyssey_packets.Packets.Models;
using kakia_lime_odyssey_packets.Packets.SC;
using kakia_lime_odyssey_server.Database;
using kakia_lime_odyssey_server.Services.Ban;
using kakia_lime_odyssey_utils.Extensions;
using System.Runtime.InteropServices;

namespace kakia_lime_odyssey_server.PacketHandlers.Authentication;

[PacketHandlerAttr(PacketType.CS_LOGIN)]
class CS_LOGIN_Handler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		var login = PacketConverter.Extract<CS_LOGIN>(p.Payload);
		client.SetAccountId(login.id);
		client.SetClientRevision(login.revision);

		Logger.Log($"Account login [{login.id}:{login.pw}][REV: {login.revision}]");

		// Check if account is banned
		if (BanService.IsAccountBanned(login.id))
		{
			var banRecord = BanService.GetBanRecord(login.id);
			Logger.Log($"[BAN] Rejected login for banned account {login.id}. Reason: {banRecord?.Reason ?? "Unknown"}", LogLevel.Warning);

			SC_LOGIN_RESULT banResult = new()
			{
				result = LOGIN_RESULT.ACCOUNT_BANNED,
				revision = login.revision
			};

			using PacketWriter banPw = new();
			banPw.Write(banResult);
			client.Send(banPw.ToPacket(), default).Wait();
			return;
		}

		var db = DatabaseFactory.Instance;
		var characters = db.LoadPC(login.id);

		var updated = new List<CLIENT_PC>();

		foreach (var character in characters)
		{
			var characterName = global::System.Text.Encoding.ASCII.GetString(character.appearance.name).TrimEnd('\0');
			var equip = db.GetPlayerEquipment(login.id, characterName);
			var equipped = equip.Combat.GetEquipped();
			var modApp = new ModAppearance(character.appearance);
			modApp.equiped = new ModEquipped(equipped);
			modApp.playingJobClass = 1;

			updated.Add(new CLIENT_PC()
			{
				status = character.status,
				appearance = modApp.AsStruct()
			});
		}

		SC_PC_LIST char_list = new()
		{
			count = (byte)characters.Count,
			pc_list = updated.ToArray()
		};

		using PacketWriter pw = new();
		pw.Write(char_list);

		client.Send(pw.ToSizedPacket(), default);
	}
}
