# Lime Odyssey Server Emulator

A server emulator for the Korean CBT3 client (rev 211) of Lime Odyssey Online, built in C# .NET 10.

## Requirements

- .NET 10 SDK
- Korean CBT3 Client (rev 211)

## Quick Start

1. Clone the repository
2. Build the solution: `dotnet build`
3. Run the server: `dotnet run --project kakia_lime_odyssey_server`
4. On first run, a `config.json` file will be generated

## Getting the Client

This emulator works with the Korean CBT3 client (rev 211). Download from kaitodomoto:
- [RageZone Thread](https://forum.ragezone.com/threads/lime-odyssey-obt-english-client.1160226/post-8951757)

Create a shortcut to `LimeOdyssey.exe` with `-localhost` argument:
```
<path>\LimeOdysseyOnline\LimeOdyssey.exe -localhost
```

## Implemented Features

### Core Systems
- Player authentication and character creation
- Zone loading and player spawning
- Movement and positioning
- NPC and monster spawning

### Combat
- Basic weapon attacks
- Skill system with casting and cooldowns
- Buff/debuff system
- HP/MP regeneration
- Death and resurrection

### Social
- Party system (create, invite, kick, leave, disband)
- Guild system (create, invite, promote, demote, disband)
- Chat (local, party, guild, whisper)

### Inventory
- Item management
- Equipment system (combat and life jobs)
- Looting system

### NPCs
- NPC dialogue system
- Monster AI (roaming, chasing, attacking)
- Respawn system

## Project Structure

```
kakia_lime_odyssey_server/      # Main server application
kakia_lime_odyssey_packets/     # Packet definitions (CS/SC)
kakia_lime_odyssey_network/     # Network layer
kakia_lime_odyssey_contracts/   # Shared interfaces
kakia_lime_odyssey_logging/     # Logging utilities
kakia_lime_odyssey_utils/       # Common utilities
```

## Contributing

Contributions are welcome. Please ensure:
- Code compiles with 0 warnings
- Follow existing code patterns
- Test changes before submitting

## License

This project is for educational and preservation purposes only.
