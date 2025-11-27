using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using System.Reflection;

namespace kakia_lime_odyssey_network.Handler;

public class PacketHandlerAttr : Attribute
{
	public PacketType packetId;
	public PacketHandlerAttr(PacketType id)
	{
		this.packetId = id;
	}
}

public abstract class PacketHandler
{
	public abstract void HandlePacket(IPlayerClient client, RawPacket p);
}

public static class PacketHandlers
{
	private static readonly Dictionary<PacketType, PacketHandler> Handlers = new Dictionary<PacketType, PacketHandler>();

	public static void LoadPacketHandlers(string namespaceName)
	{
		var classes = from t in Assembly.GetCallingAssembly().GetTypes()
					  where
						  t.IsClass && t.Namespace == namespaceName &&
						  t.IsSubclassOf(typeof(PacketHandler))
					  select t;

		foreach (var t in classes.ToList())
		{
			var attrs = (Attribute[])t.GetCustomAttributes(typeof(PacketHandlerAttr), false);

			if (attrs.Length > 0)
			{
				var attr = (PacketHandlerAttr)attrs[0];
				if (!Handlers.ContainsKey(attr.packetId))
				{
					var instance = Activator.CreateInstance(t) as PacketHandler;
					if (instance != null)
						Handlers.Add(attr.packetId, instance);
				}
			}
		}
	}

	public static void LoadPacketHandlersFromPlugins(List<Assembly> plugins)
	{
		foreach (var p in plugins)
			LoadPacketHandlers(p);
	}

	private static void LoadPacketHandlers(Assembly plugin)
	{
		var classes = from t in plugin.GetTypes()
					  where
						  t.IsClass && t.Namespace == "kakia_lime_odyssey_plugin.Handlers" &&
						  t.IsSubclassOf(typeof(PacketHandler))
					  select t;

		foreach (var t in classes.ToList())
		{
			var attrs = (Attribute[])t.GetCustomAttributes(typeof(PacketHandlerAttr), false);

			if (attrs.Length > 0)
			{
				var attr = (PacketHandlerAttr)attrs[0];
				if (!Handlers.ContainsKey(attr.packetId))
				{
					var instance = Activator.CreateInstance(t) as PacketHandler;
					if (instance != null)
						Handlers.Add(attr.packetId, instance);
				}
			}
		}
	}

	public static PacketHandler? GetHandlerFor(PacketType id)
	{
		Handlers.TryGetValue(id, out var handler);
		return handler;
	}

	public static PacketHandler[] GetLoadedHandlers()
	{
		return Handlers.Values.ToArray();
	}
}
