# Development Roadmap

## Recent Updates (2025-11-19)

### Build Status (DONE)
Successfully built with .NET 10.0 SDK. Build completed with 107 warnings (mostly nullable reference warnings) but no errors.

Fixed: Made `velocity` field public in CS_FELL_PC.cs for proper accessibility.

### Fall Damage System (DONE)
**File**: `PacketHandlers/CS_FELL_PC_Handler.cs`

The CS_FELL_PC packet was just a TODO comment before. Now it actually does something:
- Calculates fall damage based on velocity (threshold at 10.0f)
- Updates player HP when they fall too hard
- Logs fall events for debugging

Still need to:
- Send SC_CHANGED_HP to notify client about HP changes
- Send SC_DEAD if player dies from fall
- Test with actual client

### Equipment Sending Fix (DONE)
**File**: `Network/PlayerClient.cs`

SendEquipment() was only sending combat gear, completely ignoring life job equipment. Fixed by:
- Added GetLifeEquipList() method to PlayerEquipment
- Now sends both SC_COMBAT_JOB_EQUIP_ITEM_LIST and SC_LIFE_JOB_EQUIP_ITEM_LIST
- Should actually show equipped items on login now

Need to test with client to verify everything shows up.

### Item Stacking Fix (DONE)
**File**: `PacketHandlers/CS_MOVE_SLOT_ITEM_Handler.cs`

The item stacking code had a workaround using SC_UPDATE_SLOT_ITEM because it wasn't working right. Found the issue:
- fromCount/toCount mean "amount AFTER move", not "amount being moved"
- Was calculating it backwards before
- Removed the hacky workaround packet

Fixed:
- Moving items to empty slots sets fromCount to 0 (source is empty)
- Stacking items correctly reports final amounts in both slots
- No more duplicate update packets

Still needs client testing to make sure stacks merge properly.

## Active TODOs

### Packet Handlers
- [ ] Finish implementing remaining CS_* handlers with TODOs
- [ ] Add proper HP/MP change notifications (SC_CHANGED_HP, SC_CHANGED_MP)
- [ ] Implement death handling (SC_DEAD packet)

### Testing
- [ ] Test fall damage with real client
- [ ] Test equipment display on login
- [ ] Test item stacking/merging
- [ ] Test inventory operations

### Future
- Look into quest system (currently POC with hardcoded quests)
- Improve skill system
- NPC/Monster interactions
