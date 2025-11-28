/// <summary>
/// Packet handler infrastructure for routing CS packets to their handlers.
/// </summary>
/// <remarks>
/// Provides auto-discovery of packet handlers via reflection.
/// Handlers are identified by the PacketHandlerAttr attribute.
/// </remarks>
using kakia_lime_odyssey_contracts.Interfaces;
using kakia_lime_odyssey_packets;
using System.Reflection;

namespace kakia_lime_odyssey_network.Handler;

/// <summary>
/// Attribute to mark a class as a packet handler for a specific packet type.
/// </summary>
/// <remarks>
/// Apply this attribute to classes that extend PacketHandler to register
/// them for automatic discovery and routing.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PacketHandlerAttr : Attribute
{
	/// <summary>The packet type this handler processes.</summary>
	public PacketType packetId;

	/// <summary>
	/// Creates a new packet handler attribute.
	/// </summary>
	/// <param name="id">The packet type to handle.</param>
	public PacketHandlerAttr(PacketType id)
	{
		this.packetId = id;
	}
}

/// <summary>
/// Base class for all packet handlers.
/// </summary>
public abstract class PacketHandler
{
	/// <summary>
	/// Handles an incoming packet from a client.
	/// </summary>
	/// <param name="client">The client that sent the packet.</param>
	/// <param name="p">The raw packet data.</param>
	public abstract void HandlePacket(IPlayerClient client, RawPacket p);
}

/// <summary>
/// Registry and auto-discovery system for packet handlers.
/// </summary>
/// <remarks>
/// Automatically discovers and registers packet handlers using reflection.
/// Handlers must extend PacketHandler and have the PacketHandlerAttr attribute.
/// Supports scanning namespaces and sub-namespaces for complete coverage.
/// </remarks>
public static class PacketHandlers
{
	private static readonly Dictionary<PacketType, PacketHandler> Handlers = new Dictionary<PacketType, PacketHandler>();

	/// <summary>
	/// Loads all packet handlers from a namespace and its sub-namespaces.
	/// </summary>
	/// <param name="namespacePrefix">The namespace prefix to scan (e.g., "MyApp.PacketHandlers").</param>
	/// <remarks>
	/// Scans all types in the calling assembly whose namespace starts with the given prefix.
	/// This allows handlers to be organized in sub-folders/sub-namespaces.
	/// </remarks>
	public static void LoadPacketHandlers(string namespacePrefix)
	{
		LoadPacketHandlersFromAssembly(Assembly.GetCallingAssembly(), namespacePrefix);
	}

	/// <summary>
	/// Loads packet handlers from a specific assembly.
	/// </summary>
	/// <param name="assembly">The assembly to scan.</param>
	/// <param name="namespacePrefix">The namespace prefix to filter by.</param>
	private static void LoadPacketHandlersFromAssembly(Assembly assembly, string namespacePrefix)
	{
		var classes = from t in assembly.GetTypes()
					  where t.IsClass &&
							!t.IsAbstract &&
							t.Namespace != null &&
							t.Namespace.StartsWith(namespacePrefix) &&
							t.IsSubclassOf(typeof(PacketHandler))
					  select t;

		int loadedCount = 0;
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
					{
						Handlers.Add(attr.packetId, instance);
						loadedCount++;
					}
				}
			}
		}
	}

	/// <summary>
	/// Loads packet handlers from plugin assemblies.
	/// </summary>
	/// <param name="plugins">List of plugin assemblies to scan.</param>
	public static void LoadPacketHandlersFromPlugins(List<Assembly> plugins)
	{
		foreach (var p in plugins)
			LoadPacketHandlersFromAssembly(p, "kakia_lime_odyssey_plugin.Handlers");
	}

	/// <summary>
	/// Gets the handler registered for a specific packet type.
	/// </summary>
	/// <param name="id">The packet type to look up.</param>
	/// <returns>The registered handler, or null if none exists.</returns>
	public static PacketHandler? GetHandlerFor(PacketType id)
	{
		Handlers.TryGetValue(id, out var handler);
		return handler;
	}

	/// <summary>
	/// Gets all currently loaded packet handlers.
	/// </summary>
	/// <returns>Array of all registered handlers.</returns>
	public static PacketHandler[] GetLoadedHandlers()
	{
		return Handlers.Values.ToArray();
	}

	/// <summary>
	/// Gets the count of registered handlers.
	/// </summary>
	/// <returns>Number of registered packet handlers.</returns>
	public static int GetHandlerCount()
	{
		return Handlers.Count;
	}

	/// <summary>
	/// Checks if a handler is registered for a specific packet type.
	/// </summary>
	/// <param name="id">The packet type to check.</param>
	/// <returns>True if a handler exists for this packet type.</returns>
	public static bool HasHandler(PacketType id)
	{
		return Handlers.ContainsKey(id);
	}
}
