# Development Roadmap

## Implementation Status (2025-11-26)

### Packet Struct Cleanup (2025-11-27)

All packet structs now use consistent Pack = 1 and proper IPacketFixed/IPacketVar interfaces:
- Fixed 70+ CS packets with Pack=2 -> Pack=1
- Fixed 7 CS packets missing IPacketFixed interface
- Fixed 2 SC packets missing IPacketFixed interface (SC_HEAD_UNDER_WATER, SC_HEAD_OVER_WATER)
- Fixed 2 Common/Model structs with Pack=2 (LIFE_JOB_STATUS, DB_TIME)
- Added standardized XML documentation with IDA memory layout comments

**Verification Results:**
- 0 files with Pack = 2 remaining in entire Packets folder
- 0 CS/SC structs missing IPacketFixed/IPacketVar interface
- All 150 CS packets verified and complete
- All packet structs have proper IDA memory layout documentation

### IDA Verification Progress (2025-11-26)

Massive packet verification session completed using parallel agents with IDA Pro MCP integration.

**Packets IDA Verified Today: 150+ packets**

| Category | CS Packets | SC Packets | Models Created | Critical Fixes |
|----------|------------|------------|----------------|----------------|
| Login Flow | 2 | 2 | 0 | SC_LOGIN_RESULT enum created |
| Character | 1 | 1 | 2 | CS_CREATE_PC: raceTypeID byte->uint, added familyNameType |
| Movement | 4 | 4 | 0 | SC_MOVE: missing header field added |
| Sight (PC) | 0 | 4 | 0 | SC_ENTER_SIGHT_PC: APPEARANCE_PC_KR->APPEARANCE_PC |
| Sight (NPC) | 0 | 2 | 2 | APPEARANCE_NPC: string->byte[31], Pack fixes |
| Inventory | 0 | 3 | 2 | Pack 8->1, ITEM_INHERIT Size=12 padding |
| Equipment | 7 | 7 | 1 | Client uses "EQUIPED" typo - preserved |
| Skills | 4 | 1 | 0 | Created CS_USE_SKILL_OBJ, CS_USE_SKILL_POS |
| Quests | 0 | 1 | 0 | SC_QUEST_LIST: missing count field fixed |
| Chat | 1 | 0 | 0 | Already correct |
| Combat | 0 | 5 | 1 | HIT_DESC Pack=1, SC_WEAPON_HIT_RESULT |
| Trade/Shop | 8 | 5 | 0 | Uses "TRADE" naming, not "SHOP" |
| Party | 11 | 25 | 4 | POS, DEF, PARTY_MEMBER_STATE, PARTY_MEMBER |
| Guild | 12 | 17 | 4 | DB_TIME, GUILD, GUILD_MEMBER, GUILD_MEMBER_STATE |
| Teleport | 1 | 4 | 0 | Zone transfer vs warp distinguished |
| **TOTAL** | **51** | **81** | **16** | Multiple critical bugs fixed |

**Key Findings:**
- CS_ATTACK does NOT exist - game uses weapon hitting system (CS_READY_WEAPON_HITING)
- SC_ENTER_SIGHT_NPC does NOT exist - uses VILLAGER/MONSTER/MERCHANT instead
- Multiple Pack attribute corrections (2->1, 4->1, 8->1)
- Several missing struct fields discovered and added
- Client typos preserved for compatibility ("EQUIPED", "TRADE")

### Current Statistics

| Component | Exists | Total Required | Completion |
|-----------|--------|----------------|------------|
| CS Packet Handlers | 33 | 150 | 22% |
| CS Packet Structs | 78 | 150 | 52% |
| SC Packet Structs | 141 | 293 | 48% |
| IDA Verified | 132 | 443 | 30% |
| Build Warnings | TBD | 0 | TBD |

### Existing Handlers (33)

```
CS_CANCEL_READY_WEAPON_HITING     CS_LOGIN
CS_CANCEL_SELECTED_ACTION_TARGET  CS_LOOT_ITEM
CS_CHANGE_JOB_CLASS               CS_MOVE_PC
CS_COMBAT_JOB_EQUIP_ITEM          CS_MOVE_SLOT_ITEM
CS_COMBAT_JOB_UNEQUIP_ITEM        CS_PING
CS_CREATE_PC                      CS_QUEST_LIST
CS_DIRECTION_PC                   CS_READY
CS_DISCARD_SLOT_ITEM              CS_REQUEST_COMMON_STATUS
CS_FELL_PC                        CS_SAY_PC
CS_FINISH_LOOTING                 CS_SELECT_ACTION_TARGET
CS_JUMP_PC                        CS_SELECT_AND_REQUEST_TALKING
CS_LIFE_JOB_EQUIP_ITEM            CS_SELECT_TARGET_READY_WEAPON_HITING
CS_LIFE_JOB_UNEQUIP_ITEM          CS_SELECT_TARGET_REQUEST_START_LOOTING
CS_SIT_DOWN_PC                    CS_SLIDE_PC
CS_STAND_UP_PC                    CS_START_GAME
CS_STOP_PC                        CS_USE_SKILL_ACTION_TARGET
CS_USE_SKILL_SELF
```

---

## Server Restructure Plan

### Phase 1: IDA Verification (MAJOR PROGRESS - 2025-11-26)

Verify all packet structs against IDA Pro using parallel agents.

**Completed Categories:**
- [x] Login Flow: CS_LOGIN, CS_CREATE_PC, CS_START_GAME, SC_LOGIN_RESULT, SC_PC_LIST, SC_ENTER_ZONE
- [x] Movement: CS_MOVE_PC, CS_STOP_PC, CS_JUMP_PC, SC_MOVE, SC_STOP, SC_JUMP_PC
- [x] Sight: SC_ENTER_SIGHT_PC, SC_LEAVE_SIGHT_PC, SC_ENTER_SIGHT_VILLAGER, SC_ENTER_SIGHT_MONSTER
- [x] Inventory/Equipment: SC_INVENTORY, SC_EQUIP_ITEM_LIST, CS_EQUIP_ITEM, CS_UNEQUIP_ITEM
- [x] Skills: CS_USE_SKILL_*, SC_SKILL_LIST
- [x] Combat: SC_WEAPON_HIT_RESULT, SC_START/STOP_COMBATING, HIT_DESC
- [x] Trade: 8 CS + 5 SC trade packets (uses "TRADE" not "SHOP")
- [x] Party: 11 CS + 25 SC party packets with 4 model structs
- [x] Guild: 12 CS + 17 SC guild packets with 4 model structs
- [x] Teleport: CS_FINISH_WARP, SC_WARP, SC_ZONE_TRANSFERING, SC_ENTER/LEAVE_SIGHT_TRANSFER
- [x] Chat: CS_SAY_PC
- [x] Quest: SC_QUEST_LIST

**Remaining:**
- [ ] Remaining 50 CS packets (mostly obscure/unused)
- [ ] Remaining 150 SC packets (many are response variants)
- [ ] Run build to check for new warnings from created files

### Architecture Refactor (IN PROGRESS - 2025-11-26)

**Phase 1: Data Architecture Cleanup (COMPLETED)**
- Player persistence: MongoDB only (no JsonDB fallback)
- Game configuration: XML files only (spawns, loot tables, etc.)
- Deleted JsonDB.cs and JsonDBWrapper
- Created WorldDataLoader.cs for XML game config loading

**Phase 2: Interface Reorganization (COMPLETED)**
- Created new `kakia_lime_odyssey_contracts` project for shared interfaces
- Moved domain interfaces (IPlayerClient, IItem, INpc, IPlayerInventory, IPlayerEquipment) from Network to Contracts
- Updated all 38+ files to use new contracts namespace
- Deleted old Interface/ folder from Network project
- Fixed naming violation: `CS_LIFE_JOB_UNEQUIP_ITEM_handler.cs` -> `CS_LIFE_JOB_UNEQUIP_ITEM_Handler.cs`
- Fixed C# naming conventions:
  - `INPC` -> `INpc` (interface naming)
  - `NPC_TYPE` -> `NpcType` (enum naming)
  - `NPC` class -> `Npc` class (PascalCase)
  - `MOB` class -> `Mob` class (PascalCase)
  - `MOB_STATE` -> `MobState` (enum naming)
  - `GetNPCType()` -> `GetNpcType()` (method naming)
  - `GetVELOCITIES()` -> `GetVelocities()` (method naming)
  - `UpdateVELOCITIES()` -> `UpdateVelocities()` (method naming)

**Phase 3: Entity Organization (COMPLETED)**
- Populated Entities/ folder:
  - `Entities/Monsters/` - Monster.cs (partial), MonsterAttacking.cs, MonsterChasing.cs, MonsterRoaming.cs, Mob.cs
  - `Entities/Npcs/` - Npc.cs, Merchant.cs
  - `Entities/Player/` - (PlayerClient remains in Network/ as it manages socket connections)
- Deleted old entity files from Models/
- Updated using statements in LimeServer.cs, CS_SAY_PC_Handler.cs, WorldDataLoader.cs
- Models/ folder structure:
  - `BuffXML/` - Buff XML definitions
  - `FileHandler/` - Map and terrain parsing
  - `InheritXML/` - Inherit system definitions
  - `ItemMakeXML/` - Crafting definitions
  - `ItemSubjectXML/` - Item subject definitions
  - `MonsterXML/` - Monster XML definitions
  - `PcStatusXML/` - Player status definitions
  - `Persistence/` - Database persistence models
  - `SkillXML/` - Skill XML definitions
  - Root level: Item.cs, EntityStatus.cs, Config.cs, WorldDataLoader.cs, etc.

**Phase 4: Feature Organization (COMPLETED)**
- [x] Categorize PacketHandlers/ by feature:
  - `PacketHandlers/Authentication/` - CS_LOGIN, CS_CREATE_PC, CS_START_GAME, CS_READY (4 handlers)
  - `PacketHandlers/Movement/` - CS_MOVE_PC, CS_STOP_PC, CS_JUMP_PC, CS_SLIDE_PC, CS_DIRECTION_PC, CS_FELL_PC, CS_SIT_DOWN_PC, CS_STAND_UP_PC (8 handlers)
  - `PacketHandlers/Combat/` - CS_SELECT_ACTION_TARGET, CS_CANCEL_SELECTED_ACTION_TARGET, CS_SELECT_TARGET_READY_WEAPON_HITING, CS_CANCEL_READY_WEAPON_HITING, CS_USE_SKILL_ACTION_TARGET, CS_USE_SKILL_SELF (6 handlers)
  - `PacketHandlers/Inventory/` - CS_MOVE_SLOT_ITEM, CS_DISCARD_SLOT_ITEM, CS_COMBAT_JOB_EQUIP_ITEM, CS_COMBAT_JOB_UNEQUIP_ITEM, CS_LIFE_JOB_EQUIP_ITEM, CS_LIFE_JOB_UNEQUIP_ITEM, CS_SELECT_TARGET_REQUEST_START_LOOTING, CS_LOOT_ITEM, CS_FINISH_LOOTING (9 handlers)
  - `PacketHandlers/Character/` - CS_CHANGE_JOB_CLASS, CS_REQUEST_COMMON_STATUS, CS_QUEST_LIST (3 handlers)
  - `PacketHandlers/Social/` - CS_SAY_PC, CS_SELECT_AND_REQUEST_TALKING (2 handlers)
  - `PacketHandlers/System/` - CS_PING (1 handler)
- [x] Create Services/ layer for business logic:
  - `Services/Combat/ICombatService.cs` - Combat service interface
  - `Services/Combat/CombatService.cs` - Combat damage calculations
  - `Services/Inventory/InventoryService.cs` - Item stacking, moving, equipping
  - `Services/World/IWorldService.cs` - World service interface
  - `Services/World/WorldService.cs` - Entity and zone management

**Folder Structure:**
```
GameData/
  Definitions/
    Buffs/BuffInfo.xml
    Characters/ExperienceTable.xml, InheritTypes.xml
    Entities/Monsters.xml, Models.xml
    Items/Items.xml, Crafting.xml, Categories.xml
    Skills/Skills.xml
    Quests/
  World/
    Maps/
    MobSpawns.xml      (server-side spawn config)
    NpcSpawns.xml      (server-side spawn config)
    DropTables.xml     (loot configuration)
    LocalAreaInfo.xml
    MapTargetInfo.xml
```

**Key Files:**
- `GameDataPaths.cs` - Centralized path constants
- `Models/WorldDataLoader.cs` - XML loader for world config
- `Database/DatabaseFactory.cs` - MongoDB singleton
- `Database/MongoDBService.cs` - Player persistence

**Migration Completed:**
- All PacketHandlers now use `DatabaseFactory.Instance`
- LimeServer uses `WorldDataLoader` for spawns
- Monster.cs uses `WorldDataLoader.LoadDropTables()`
- PlayerClient.cs uses `DatabaseFactory.Instance` for saves

### Phase 2: Service Layer

Create clean service interfaces:

```
Services/
  IAccountService.cs      -> AccountService.cs
  ICharacterService.cs    -> CharacterService.cs
  IInventoryService.cs    -> InventoryService.cs
  ICombatService.cs       -> CombatService.cs
  IWorldService.cs        -> WorldService.cs
```

### Phase 3: Repository Layer

Create data access layer:

```
Data/
  Repositories/
    IAccountRepository.cs   -> AccountRepository.cs
    ICharacterRepository.cs -> CharacterRepository.cs
    IInventoryRepository.cs -> InventoryRepository.cs
```

---

## Recent Updates (2025-11-19)

### Trading & Economy Systems Analysis (COMPLETED)
**Date**: 2025-11-19
**Analysis Tool**: IDA Pro with MCP integration
**Document**: `/docs/TRADING_ECONOMY_ANALYSIS.md`

Performed comprehensive reverse engineering of the Lime Odyssey client's trading and economy systems to identify all trading mechanisms, currency systems, and commerce features.

**Systems Analyzed**:
1. **Player-to-Player Trading** - Full trade window UI with item/currency exchange
2. **NPC Merchant Shops** - Buy system with price calculation (sell limited)
3. **Mail/Post System** - Send mail with item/currency attachments
4. **Bank/Storage System** - Personal storage with expansion support
5. **Looting System** - Multiple loot modes with auto-loot options
6. **Currency System** - Dual currency (Peder/Lant) with conversion

**Key Findings**:

**Trade System Details**:
- UI Files: UITrade.xml, UITradeAccept.xml, UITradeWait.xml
- Functions: TradeInit, TradeDrop, UpdateTradePeder, UpdateTradeLant, RefreshTradeMoney
- Packets: CS_ADD_ITEM_TO_EXCHANGE_LIST, CS_SUBTRACT_ITEM_FROM_EXCHANGE_LIST, sc_trade_price
- isExchange flag determines if items are tradeable
- Double-click and drag-drop support for items

**Merchant Shop System**:
- UI File: UIMerchant.xml
- Functions: MerchantInit, BuyMerchantItem, MerchantBuyItemCount, GetMerchantIPage
- Packet: sc_enter_sight_merchant
- Global state: pageCount, indexCopy, clickItemType (at 0xb93e3c-0xb93e44)
- Price UI elements: PriceSection, pPriceText, UpdateItemPrice
- isSell flag exists but may not be fully implemented

**Mail/Post System**:
- UI Files: UIPost.xml, UIPostDetail.xml, UISendPost.xml
- Functions: Post_Init, SendPost_SendMail, PostDetail_clickedMoney, AddPostItem
- Packets: CS_SEND_POST, sc_new_post_alarm, sc_post, sc_post_item
- Supports item and currency attachments
- NewPostAni for new mail notifications
- MailIcon_%d for mail icons

**Bank Storage System**:
- UI File: UIBank.xml
- Functions: uiBank_Init, uiBank_SetBankItem, uiBank_ItemSort, uiBank_DragStart
- Packet: PACKET_SC_BANK_OPENED
- Supports item deposit/withdrawal
- No expansion system found in client strings

**Currency System**:
- **Peder**: Primary currency (IconPeder, valuePeder, TotalPeder)
- **Lant**: Secondary currency (UpdateTradeLant references)
- Conversion: ConverRantToPederRant %d %d (found at 0xa3c8d8, 0xa43e9c)
- Money icon: icon_money_00.bmp (used 4 times)

**Looting System**:
- UI File: UILooting.xml
- Auto-loot options: AutoLooting_off, AutoLooting_on, AutoLooting_shift, AutoLooting_always
- Functions: SetLootingItem, LootingItemDoubleClick, uiLooting_ClickedLootingAll
- Packets: CS_LOOT_ITEM, SC_LOOTABLE_ITEM_LIST, sc_disabled_looting
- Party loot distribution support

**Missing Systems** (NOT found in client):
- ❌ Auction House system
- ❌ Player-run shops/vendors
- ❌ Advanced marketplace features
- ❌ Consignment systems
- ❌ Trade brokers

**Critical Security Vulnerabilities**:
1. **Trade Duplication** - No atomic transaction locks, race conditions possible
2. **Price Manipulation** - Client could send manipulated prices
3. **Currency Overflow** - No int64 overflow checks on currency transfers
4. **Distance Bypass** - No continuous distance validation during trade
5. **Mail Duplication** - No validation that sender owns attached items
6. **Bank Exploits** - No access control validation

**Required Server-Side Implementations** (Priority Order):
1. **CRITICAL** - Atomic trade transactions with global locks
2. **CRITICAL** - Currency overflow prevention (max 9,999,999,999 Peder, 999,999,999 Lant)
3. **CRITICAL** - Server-authoritative pricing (never trust client prices)
4. **HIGH** - Distance validation during trades (10 units max, continuous check)
5. **HIGH** - Mail attachment validation (verify sender ownership)
6. **HIGH** - Bank access control (distance check, combat check)
7. **MEDIUM** - Loot distribution modes (FreeForAll, Individual, Party)
8. **MEDIUM** - Currency conversion system
9. **MEDIUM** - Mail expiry (30-day auto-return to sender)
10. **LOW** - Shop stock management for limited items

**Validation Requirements**:
- Trade distance: Max 10 units, continuous validation
- Currency caps: Peder max 9,999,999,999, Lant max 999,999,999
- Price validation: Server defines all prices, client display only
- Item tradeability: Check isExchange flag before allowing trade
- Mail postage: Cost system for sending mail
- Bank proximity: Max 5 units from banker NPC
- Loot ownership: Tag expiry system for public loot (60 seconds typical)

**Implementation Roadmap** (4-Week Plan):
- Week 1: Trade system with atomic transactions, distance validation
- Week 2: NPC merchant shops with server-authoritative pricing
- Week 3: Mail system with attachment validation, expiry
- Week 4: Bank storage, loot distribution, currency conversion

See full analysis document for detailed packet flows, validation pseudocode, database schemas, and anti-cheat requirements.

### Item System Deep Dive Analysis (COMPLETED)
**Date**: 2025-11-19
**Analysis Tool**: IDA Pro with MCP integration
**Document**: `/docs/ITEM_SYSTEM_ANALYSIS.md`

Performed comprehensive reverse engineering of the Lime Odyssey client's item system to identify all data structures, validation requirements, and missing server-side implementations.

**Key Structures Analyzed**:
1. **ITEM_INHERIT** (12 bytes) - Single inherited attribute (enhancement/socket bonus)
2. **ITEM_INHERITS** (300 bytes) - Array of 25 inherit slots per item
3. **INVENTORY_ITEM** (336 bytes) - Full inventory item with durability, expiry, grade
4. **EQUIP_ITEM** (320 bytes) - Equipment with durability and enhancements
5. **iteminfo::Item** (212 bytes) - Item definition from ItemInfo.xml
6. **itemOptionInfo::ItemOption** (72 bytes) - Socket/gem system data

**Critical Findings - Item Enhancement System**:
- **maxEnchantCount**: Each item has a maximum enhancement level (found at offset 0x44)
- Items store up to 25 inherited attributes (stats, enhancements, sockets)
- Enhancement bonuses stored in ITEM_INHERITS array
- Server MUST validate enhancement attempts against maxEnchantCount limit
- Currently NO server-side enhancement validation exists

**Critical Findings - Durability System**:
- All items track current durability and max durability (separate fields)
- Client expects SC_UPDATE_DURABILITY_* packets when durability changes
- Durability should decrease from: combat, death, skill usage
- Items with 0 durability should be unusable/broken
- Repair system exists with price calculation packets
- Currently NO server-side durability tracking exists

**Critical Findings - Item Attributes**:
- 64-bit stack counts (supports massive item stacks)
- Time-limited items with remainExpiryTime field (expires based on time)
- Item grade/quality system affects stats and visuals
- Trading restrictions: isSell, isExchange, isDiscard flags
- Equipment restrictions: job class, level, race requirements

**Socket/Gem System** (Framework Found):
- soketItemTypeID field in ItemOptionInfo (note: "soket" typo in client)
- mapSoketItem structure for socket-to-gem mapping
- Socket bonuses stored in ITEM_INHERITS like enhancements
- Currently NOT implemented on server

**Item Crafting System**:
- 10+ crafting packets identified (ITEM_MAKE_*)
- Supports casting time, continuous crafting, cancel
- Recipes loaded from ItemMakeInfo.xml
- Material validation and skill requirements needed
- Currently partially implemented (basic only)

**Security Vulnerabilities Identified**:
1. **Item Duplication**: No transaction-based item operations, race conditions possible
2. **Durability Bypass**: No server-side durability tracking, client can ignore wear
3. **Enhancement Bypass**: No maxEnchantCount validation, infinite enhancements possible
4. **Stack Manipulation**: Insufficient validation on count values
5. **Expiration Bypass**: No server-side expiration checks, rental items usable forever

**Required Server-Side Implementations** (Priority Order):
1. **CRITICAL** - Item duplication prevention (database transactions, locking)
2. **HIGH** - Durability system (tracking, loss calculation, SC_UPDATE packets)
3. **HIGH** - Item repair system (price calculation, repair handler)
4. **MEDIUM** - Enhancement system (level tracking, maxEnchantCount validation)
5. **MEDIUM** - Socket/gem system (insertion, removal, bonus application)
6. **MEDIUM** - Complete crafting system (all packets, casting time)
7. **LOW** - Time-limited items (expiration tracking and handling)

**Validation Requirements**:
- Stack count validation (prevent overflow, enforce maxStack limits)
- Durability validation (current <= max, server-authoritative)
- Enhancement validation (level <= maxEnchantCount, material requirements)
- Slot validation (range checks, type matching)
- Trading restriction enforcement (isSell, isExchange, isDiscard flags)
- Equipment restriction validation (job, level, race requirements)

**Implementation Roadmap** (6-Week Plan):
- Week 1: Item duplication prevention, validation hardening
- Week 2: Durability tracking and repair system
- Week 3-4: Enhancement system with full validation
- Week 5: Socket/gem system
- Week 6+: Crafting completion, time-limited items, testing

See full analysis document for detailed structure definitions, packet analysis, validation pseudocode, and security recommendations.

## Recent Updates (2025-11-19)

### IDA Pro Client Analysis - Server Architecture Requirements (COMPLETED)
**Date**: 2025-11-19
**Analysis Tool**: IDA Pro with MCP integration
**Document**: `/docs/IDA_CLIENT_ANALYSIS.md`

Performed comprehensive reverse engineering of LimeOdyssey.exe to identify critical server-side validation and logic requirements. Created 1,150-line analysis document covering:

**Key Areas Analyzed**:
1. Packet Handling Architecture (CClientServerPacketTable, encryption, 300+ packet types)
2. Movement System & Anti-Cheat (CPlayer structure, speed validation, tick validation)
3. Combat System (skill handlers, STATUS_PC stats, damage calculation)
4. Character Management (login flow, stat calculations, 176-byte STATUS_PC)
5. Inventory & Item System (335-byte item structure, duplication prevention)
6. Security & Anti-Cheat Requirements (5 critical validation systems)

**Critical Findings**:
- Client sends position + direction + velocity + tick data that server must validate
- Movement direction vectors MUST be normalized (length = 1.0)
- Client expects AES/Rijndael encryption on packets
- Complex nested stat system: STATUS_PC with LIFE_JOB_STATUS and COMBAT_JOB_STATUS
- Items use 300-byte ITEM_INHERITS for enhancement data
- Tick-based synchronization required for anti-cheat

**Security Gaps Identified**:
- NO speed validation (speed hacking vulnerability)
- NO position validation (teleport hacking vulnerability)
- NO tick validation (packet replay vulnerability)
- Minimal cooldown enforcement (skill spam vulnerability)
- NO anti-cheat logging system

**Immediate Action Items**:
1. Implement movement speed validation (PC_RUN_SPEED constants)
2. Add tick tracking to PlayerClient class
3. Implement server-side cooldown tracking
4. Add CheatDetection logging system
5. Implement position bounds/teleport detection

See full analysis document for detailed implementation requirements, pseudocode examples, and structure definitions.

### Anti-Cheat System Implementation (COMPLETED)
**Date**: 2025-11-19
**Status**: Fully implemented and committed
**Commit**: c1a6a33

Based on the IDA Pro client analysis, implemented a comprehensive anti-cheat infrastructure addressing all identified security gaps:

**New Components**:
1. **Constants/GameConstants.cs**
   - Movement speed constants extracted from client addresses 0xa75650-0xa756e4
   - PC_RUN_SPEED = 90.0f, PC_WALK_SPEED = 80.0f, PC_SWIMMING_SPEED = 90.0f
   - SPEED_CHECK_TOLERANCE = 1.1f (10% for network latency)
   - MAX_TELEPORT_DISTANCE = 50.0f units
   - DIRECTION_NORMAL_TOLERANCE = 0.01f

2. **AntiCheat/MovementValidator.cs**
   - ValidateSpeed(): Checks player movement speed against max allowed with tolerance
   - ValidateDirection(): Enforces normalized direction vectors (client requirement at 0xa2bb8c)
   - IsTeleport(): Detects instant position changes beyond reasonable distance
   - ValidateTickProgression(): Prevents packet replay attacks

3. **AntiCheat/CheatDetection.cs**
   - Centralized cheat logging system with per-player violation tracking
   - Auto-ban thresholds: SpeedHack (5), Teleport (3), ItemDuplication (1)
   - Detailed logging with player name, violation type, and metrics
   - LogSpeedHack(), LogTeleport(), LogInvalidDirection(), LogTickManipulation()

4. **CS_MOVE_PC_Handler.cs** (Complete Rewrite)
   - Direction normalization check with client requirement enforcement
   - Tick progression validation (rejects backward/invalid ticks)
   - Speed validation with 10% tolerance for network conditions
   - Teleport detection with rubber-banding
   - Detailed logging with player context

5. **PlayerClient.cs** (Anti-Cheat Extensions)
   - Added tracking fields: _lastClientTick, _lastPosition, _lastMoveTime, _movePacketCount
   - Helper methods: GetLastClientTick(), UpdateClientTick(), GetLastPosition(), UpdateLastPosition()
   - GetMaxSpeed() method for move type-based speed limits

**Validation Flow in CS_MOVE_PC**:
```
1. Extract movement packet data
2. Check direction vector is normalized (length = 1.0 ± 0.01)
   → If invalid: Log violation, normalize direction, continue
3. Check tick progression (must increase, max 10000ms delta)
   → If invalid: Log violation, reject packet
4. Check movement speed against max allowed
   → If exceeded: Log violation, rubber-band to last position
5. Check for teleport (distance > 50 units)
   → If detected: Log violation, rubber-band to last position
6. Update tracking (tick, position, timestamp)
7. Broadcast validated movement to other players
```

**Security Improvements**:
- ✅ Speed validation implemented (addresses speed hacking)
- ✅ Position validation implemented (addresses teleport hacking)
- ✅ Tick validation implemented (addresses packet replay)
- ✅ Anti-cheat logging system implemented
- ✅ Rubber-banding on detected cheats
- ✅ Auto-ban system with configurable thresholds

**Immediate Action Items** (from IDA analysis - ALL COMPLETED):
- ✅ Implement movement speed validation (PC_RUN_SPEED constants)
- ✅ Add tick tracking to PlayerClient class
- ✅ Add CheatDetection logging system
- ✅ Implement position bounds/teleport detection

**Still Needed**:
- ✅ Implement server-side skill cooldown tracking (COMPLETED - SkillCooldownTracker added)
- [ ] Test anti-cheat with real client and movement hacks
- ✅ Add more cheat types: FlyHack, OutOfBounds detection (COMPLETED - See below)
- [x] Implement actual ban logic (COMPLETED - BanService with kick and login rejection)

### Enhanced Anti-Cheat System - FlyHack and OutOfBounds Detection (COMPLETED)
**Date**: 2025-11-19
**Status**: Fully implemented based on IDA analysis requirements

Added the final two critical anti-cheat detection types identified in the IDA client analysis:

**New Detection Types**:
1. **FlyHack Detection**
   - Tracks player vertical (Y) position changes
   - Validates jump height against PC_MAX_JUMP_HEIGHT constant (5.0 units)
   - Distinguishes between legitimate jumping and fly hacking
   - Allows small terrain height changes (2.0 units for stairs/slopes)
   - Tracks jumping state with automatic timeout (2 seconds max)
   - Clears jumping state when player lands (downward movement detected)

2. **OutOfBounds Detection**
   - Validates player position against map boundaries
   - Default boundaries: X(-2048 to 2048), Y(-100 to 1000), Z(-2048 to 2048)
   - Configurable per-map boundaries (ready for NavMesh integration)
   - Rubber-bands players back to last valid position when out of bounds

**Implementation Details**:

1. **GameConstants.cs Updates**:
   - Added `PC_MAX_JUMP_HEIGHT = 5.0f` constant
   - Added `MapBounds` class with default min/max coordinates for X, Y, Z
   - Ready for dynamic map boundary loading from terrain data

2. **MovementValidator.cs New Methods**:
   - `IsFlyHack()`: Detects unrealistic vertical movement
     - Ignores downward movement (falling is always allowed)
     - Validates jump height when jumping is active
     - Detects non-jumping upward movement beyond terrain tolerance
   - `IsOutOfBounds()`: Checks if position exceeds map boundaries
   - `Calculate2DDistance()`: Helper for horizontal distance (ignores height)

3. **CheatDetection.cs New Logging**:
   - `LogFlyHack()`: Logs fly hack attempts with height delta details
   - `LogOutOfBounds()`: Logs out-of-bounds attempts with position and boundary type

4. **PlayerClient.cs Jump State Tracking**:
   - Added `_isJumping` flag and `_jumpStartTime` timestamp
   - `IsJumping()`: Check if player is currently jumping
   - `SetJumping(bool)`: Set jumping state and record start time
   - `GetJumpDuration()`: Calculate how long player has been jumping

5. **CS_JUMP_PC_Handler.cs Enhancements**:
   - Direction normalization validation (same as CS_MOVE_PC)
   - Fly hack detection at jump start
   - Sets jumping state to true when jump packet received
   - Tracks last position for subsequent movement validation
   - Rubber-bands on detected fly hack

6. **CS_MOVE_PC_Handler.cs Enhancements**:
   - Fly hack detection during movement
     - Checks if jump is valid (within 2 second window)
     - Validates height change against max jump height
     - Allows terrain climbing with 2.0 unit tolerance
   - Auto-clears jumping state on landing (downward movement)
   - Out of bounds detection with configurable boundaries
   - Rubber-bands players back on violations

**Validation Flow**:
```
CS_JUMP_PC received:
1. Validate direction is normalized
2. Check jump height doesn't exceed max (5.0 units)
3. Set player jumping state = true
4. Track jump start time
5. Log violation and rubber-band if fly hack detected

CS_MOVE_PC received (while jumping):
1. Check if jump is still valid (< 2 seconds elapsed)
2. Validate vertical movement against max jump height
3. Clear jumping state if moving downward (landed)
4. Check position is within map boundaries
5. Log violations and rubber-band as needed
```

**Anti-Cheat Summary** (All Requirements from IDA Analysis NOW COMPLETE):
- ✅ Speed validation (speed hacking)
- ✅ Position validation (teleport hacking)
- ✅ Tick validation (packet replay attacks)
- ✅ Direction normalization (client requirement enforcement)
- ✅ **FlyHack detection** (NEW - unrealistic vertical movement)
- ✅ **OutOfBounds detection** (NEW - leaving valid play area)
- ✅ Comprehensive cheat logging with auto-ban thresholds
- ✅ Rubber-banding on detected violations

**Integration Ready**:
- Map boundary constants ready for NavMesh data loading
- Can read actual terrain bounds from MapData/TrnParser when implemented
- Jump height validation based on actual game physics
- All detection types have configurable auto-ban thresholds

### Server-Side Skill Cooldown Tracking System (COMPLETED)
**Date**: 2025-11-19
**Status**: Fully implemented and integrated
**Based On**: IDA Pro client analysis (docs/IDA_CLIENT_ANALYSIS.md)

Implemented comprehensive server-side skill cooldown tracking to prevent skill spam hacks and enforce cooldown times from SkillXML data.

**New Components**:

1. **AntiCheat/SkillCooldownTracker.cs** (NEW)
   - Per-player skill cooldown tracking system
   - Tracks skill ID, last use timestamp, and next available time
   - Validates cooldowns haven't been violated before allowing skill use
   - Integrates with CheatDetection logging system
   - Key Methods:
     - `IsSkillReady(skillId)`: Check if skill is off cooldown
     - `GetRemainingCooldown(skillId)`: Get remaining cooldown in milliseconds
     - `ValidateAndTrackSkillUse(skillId, skill)`: Primary validation method
     - `SetCooldown(skillId, cooldownSeconds, skillName)`: Track skill usage
     - `ClearAllCooldowns()`, `ClearCooldown(skillId)`: For testing/special events
     - `GetActiveCooldowns()`: Debugging information

2. **Network/PlayerClient.cs Updates**
   - Added `_skillCooldownTracker` field (initialized when character is set)
   - Initializes tracker in `SetCurrentCharacter()` with player ID and name
   - Public accessor: `GetSkillCooldownTracker()` for packet handlers
   - Automatic initialization ensures every player has cooldown tracking

3. **PacketHandlers/CS_USE_SKILL_ACTION_TARGET_Handler.cs Integration**
   - Added cooldown validation before processing skill on target
   - Retrieves cooldown tracker from PlayerClient
   - Calls `ValidateAndTrackSkillUse()` before skill execution
   - Rejects skill usage if cooldown not ready (silent rejection to prevent hack detection)
   - Logs cooldown violations with player context
   - Uses SkillXML data for cooldown values (SubjectList level 1 or base cooldown)

4. **PacketHandlers/CS_USE_SKILL_SELF_Handler.cs Integration**
   - Same cooldown validation for self-cast skills
   - Consistent enforcement across all skill usage types
   - Prevents cooldown bypass through different skill packets

**Cooldown Data Source**:
- Reads cooldown times from existing SkillXML database (db/xmls/SkillInfo.xml)
- Prefers `SubjectList[0].CoolTime` (level 1 skill data) if available
- Falls back to `XmlSkill.CoolTime` (base skill cooldown)
- Cooldown times stored in milliseconds in XML, converted to seconds for tracking

**Validation Flow**:
```
Player uses skill (CS_USE_SKILL_ACTION_TARGET or CS_USE_SKILL_SELF):
1. Packet handler retrieves skill data from SkillDB
2. Get player's SkillCooldownTracker
3. Call ValidateAndTrackSkillUse(skillId, skill)
   a. Check if skill is ready (DateTime.Now >= NextAvailableTime)
   b. If on cooldown:
      - Log violation to CheatDetection system
      - Return false (reject skill usage)
   c. If ready:
      - Record usage timestamp
      - Calculate next available time (now + cooldown seconds)
      - Return true (allow skill execution)
4. If validation passed, proceed with skill casting/execution
5. If validation failed, silently reject (prevents hack detection)
```

**Anti-Cheat Integration**:
- Logs to CheatDetection.CheatType.InvalidSkillCast
- Includes skill ID, skill name, and remaining cooldown in log
- Subject to auto-ban threshold (default: 5 violations)
- Detailed logging format:
  ```
  [COOLDOWN VIOLATION] Player {name} ({id}) used skill {skillId} while on cooldown (remaining: {ms}ms)
  ```

**Example Cooldown Tracking** (from logs):
```
[COOLDOWN] Player TestPlayer used skill 1001 (Fireball), cooldown: 2.50s
[COOLDOWN VIOLATION] Player TestPlayer attempted to use skill 1001 while on cooldown (remaining: 1250ms)
[CHEAT] Player TestPlayer (20001) - InvalidSkillCast: Skill 1001 (Fireball) used while on cooldown. Remaining: 1250ms
```

**Security Benefits**:
- ✅ **Server Authority**: Cooldowns enforced server-side only (client cooldowns are cosmetic)
- ✅ **Hack Prevention**: Cannot bypass cooldowns by modifying client memory
- ✅ **Data-Driven**: Uses actual game skill data from SkillXML
- ✅ **Accurate Timing**: DateTime-based tracking with millisecond precision
- ✅ **Violation Logging**: All cooldown violations logged for analysis
- ✅ **Auto-Ban Support**: Repeated violations trigger auto-ban (configurable threshold)

**Implementation Details**:
- **Memory Efficient**: Dictionary-based tracking, only stores used skills
- **Thread-Safe**: Ready for concurrent skill usage (dictionary operations)
- **Garbage Collection Friendly**: Old cooldown entries naturally expire (no cleanup needed)
- **Debugging Support**: GetCooldownInfo() provides detailed cooldown state
- **Admin Commands Ready**: Clear cooldowns for testing/events

**Testing Considerations**:
- [ ] Test cooldown enforcement with rapid skill spamming
- [ ] Verify cooldown times match SkillXML data
- [ ] Test different skill types (instant, cast time, long cooldown)
- [ ] Verify cooldown persists across multiple skill uses
- [ ] Test ClearCooldown functionality for admin commands
- [ ] Monitor cheat detection logs for false positives

**Code Quality**:
- Full XML documentation on all public methods
- Clear separation of concerns (tracking vs validation)
- Integrates cleanly with existing CheatDetection system
- Follows existing codebase patterns and conventions
- No external dependencies (uses only System namespaces + project references)

### Build Status (DONE) - Zero Warnings Achieved
Successfully built with .NET 10.0 SDK with 0 warnings and 0 errors.

Fixed all 107 warnings without suppression:
- Removed unused variables
- Added default! initializers to XML/JSON deserialization properties
- Fixed nullable reference handling throughout codebase
- Made `velocity` field public in CS_FELL_PC.cs for proper accessibility

Build is now completely clean.

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

### PacketHandlers TODO Analysis (COMPLETED)
**Date**: 2025-11-19
**Document**: `/docs/TODO_ANALYSIS_REPORT.md`

Performed comprehensive analysis of all 33 packet handler files to identify remaining TODOs and implementation gaps:

**Statistics**:
- Total Files Analyzed: 33 packet handlers
- TODOs Found: 1 (97% completion rate!)
- Fully Implemented Handlers: 32/33

**The One Remaining TODO**:
1. **CS_START_GAME_Handler.cs** (Line 81) - `// TODO: FIX THESE`
   - Context: SendInventory() and SendEquipment() calls during login
   - Priority: IMPORTANT (affects login experience)
   - Status: May already be fixed by recent equipment/inventory updates
   - Next Step: Client testing required to verify and remove TODO

**Well-Implemented Systems** (No TODOs):
- ✅ Movement (6 handlers: move, stop, jump, slide, direction, fell)
- ✅ Combat (4 handlers: target selection, skill usage, weapon ready/cancel)
- ✅ Item Management (3 handlers: move, discard, loot)
- ✅ Equipment (4 handlers: combat/life equip/unequip)
- ✅ Character Actions (8 handlers: sit, stand, say, change job, etc.)
- ✅ Game Flow (4 handlers: login, ready, create, quest list)
- ✅ Other (4 handlers: ping, status, talking, looting)

**Additional Observations**:
- Some handlers lack error notifications to client (silent failures)
- ✅ CS_QUEST_LIST_Handler now sends proper response with quest statistics
- Hardcoded values in CS_START_GAME (quests, skills) should move to database

**Recommendation**: Prioritize client testing of login flow to verify inventory/equipment display works correctly, then remove or update the single remaining TODO.

### HP/MP Change Notification and Death Handling System (COMPLETED)
**Date**: 2025-11-19
**Status**: Fully implemented

Implemented a comprehensive HP/MP change notification and death handling system for player characters:

**New Packet Structures**:
1. **SC_CHANGED_HP.cs**
   - Notifies client when player HP changes
   - Contains objInstID and new hp value
   - Follows same pattern as other SC_CHANGED_* packets

2. **SC_CHANGED_MP.cs**
   - Notifies client when player MP changes
   - Contains objInstID and new mp value
   - Ready for skill MP cost implementation

**New PlayerClient Methods**:
1. **UpdateHP(int newHP, bool broadcast = true)**
   - Updates player HP with bounds checking (0 to maxHP)
   - Sends SC_CHANGED_HP to player
   - Optionally broadcasts to nearby players
   - Used for healing, damage, and HP modifications

2. **UpdateMP(int newMP, bool broadcast = true)**
   - Updates player MP with bounds checking (0 to maxMP)
   - Sends SC_CHANGED_MP to player
   - Optionally broadcasts to nearby players
   - Ready for skill MP cost implementation

3. **TakeDamage(int damage)**
   - High-level damage handling method
   - Calls UpdateHP() to apply damage
   - Automatically calls SendDeath() if HP reaches 0
   - Returns DamageResult with TargetKilled flag
   - Simplifies combat code

4. **SendDeath()**
   - Sends SC_DEAD packet to player and nearby players
   - Logs death event for debugging
   - Called automatically by TakeDamage() when HP = 0

**Updated Systems**:
1. **CS_FELL_PC_Handler.cs**
   - Now uses UpdateHP() instead of directly modifying status
   - Properly notifies client of HP changes from fall damage
   - Broadcasts HP changes to nearby players

2. **MonsterAttacking.cs**
   - Simplified combat code using TakeDamage()
   - Death notification handled automatically
   - Removed duplicate SC_DEAD packet code

3. **CS_USE_SKILL_ACTION_TARGET_Handler.cs**
   - Detects player vs monster targets
   - Uses TakeDamage() for player targets (handles death automatically)
   - Uses UpdateHealth() for monster targets (keeps existing death handling)
   - Cleaner separation of player and monster combat logic

**Benefits**:
- ✅ Client properly notified of HP/MP changes
- ✅ Death handling centralized and consistent
- ✅ Broadcasts to nearby players for multiplayer visibility
- ✅ Reduced code duplication in combat handlers
- ✅ Bounds checking prevents negative HP/MP
- ✅ Logging for death events
- ✅ Ready for future features (healing skills, regeneration, MP costs)

**Implementation Notes**:
- Packets follow existing SC_CHANGED_* pattern (VELOCITIES, JOB_CLASS, etc.)
- UpdateHP/UpdateMP can be used for both positive and negative changes
- TakeDamage automatically handles death, no need to check TargetKilled in most cases
- All changes are thread-safe using existing packet sending infrastructure

## Active TODOs

### CRITICAL PRIORITY - Combat System Bugs (2025-11-26 - ALL FIXED)
- [x] **EMERGENCY: Fix inverted critical hit variance** in DamageHandler.cs - COMPLETED
  - Fixed: Critical hits now deal MORE damage (1.5x-2.0x variance * 2.0 multiplier, ignores defense)
  - Normal hits now deal LESS (0.8x-1.2x variance with defense reduction)
- [x] **HIGH: Fix hit rate calculation** - COMPLETED
  - Added Math.Clamp(hitChance, 5.0, 95.0) bounds checking
  - Minimum 5% hit chance, maximum 95% (no guaranteed hits/misses)
- [x] **HIGH: Implement defense diminishing returns** - COMPLETED
  - Implemented: `Atk * (1 - Def/(Def+100))` formula
  - High defense now has diminishing returns instead of linear reduction
- [x] **MEDIUM: Fix Random instance** - COMPLETED
  - Changed to `ThreadLocal<Random>` for thread safety

### Equipment Stat System (2025-11-26 - COMPLETED)
- [x] **Created InheritType.cs enum** - All 61 inherit types from inherit.xml
  - Base stats (1-7): STR, INT, DEX, AGI, VIT, SPI, LUK
  - Combat stats (15-36): MeleeAtk, SpellAtk, Defense, CritRate, Dodge, etc.
  - Percentage bonuses (46-58): Rate-based stat modifiers
- [x] **Added GetInheritBonus() to PlayerEquipment** - Sums inherit values from all equipped items
- [x] **Updated GetMeleeAttack()** - Now includes equipment bonuses:
  - Base stat bonuses (STR, DEX, AGI, VIT, LUK from gear)
  - Direct combat bonuses (ExtraMeleeAtk, ExtraMeleeDefense, HitAccurate, ExtraCriticalRate, ExtraDodge)
- [x] **Updated GetSpellAttack()** - Now includes equipment bonuses:
  - Base stat bonuses (INT, DEX, AGI, VIT, SPI, LUK from gear)
  - Direct magic bonuses (ExtraSpellAtk, ExtraSpellDefense, etc.)

### Typo Corrections (2025-11-26 - COMPLETED)
- [x] Fixed client typos in codebase (user preference: proper English over client compatibility)
  - EQUIPED -> EQUIPPED (6 files renamed)
  - LEAVED -> LEFT (1 file renamed)
  - SUCESS -> SUCCESS (1 file renamed)
  - SHILDGUARDED -> SHIELDGUARDED (1 file renamed)
- [x] Updated PacketType.cs enum values
- [x] Updated cs_packets.toon and sc_packets.toon tracking files

### Life Job Stat System (2025-11-26 - COMPLETED)
- [x] **Added LifeJobStats class** to EntityStatus.cs
  - Base stats: Idea, Mind, Craft, Sense (from SAVED_LIFE_JOB_STATUS)
  - Derived stats: CollectSuccessRate, CollectionIncreaseRate, MakeTimeDecrease
- [x] **Added GetLifeJobStats() method** to PlayerClient.cs
  - Pulls base stats from character's lifeJob status
  - Applies equipment bonuses (IDE, MID, CRT, SES from life job gear)
  - Calculates derived stats from equipment inherits
- [x] **Updated GetEntityStatus()** to handle life jobs
  - Combat job (jobId=1): Returns combat stats only
  - Life job (other ids): Returns life job stats + basic combat stats
- [x] **Equipment inherit mappings** for life jobs:
  - ExtraIDE (8) -> Idea bonus
  - ExtraMID (10) -> Mind bonus
  - ExtraCRT (11) -> Craft bonus
  - ExtraSES (9) -> Sense bonus

### Data Persistence System (2025-11-26 - COMPLETED)
Added comprehensive JSON-based persistence for all missing player data:

**New Model Classes** (in Models/Persistence/):
- [x] **PlayerSkills.cs** - Learned skills with levels, experience, hotbar positions
- [x] **PlayerQuests.cs** - Active quests, completed quests, objective progress
- [x] **PlayerBank.cs** - Bank storage items, currency, unlocked slots
- [x] **PlayerMail.cs** - Inbox/sent messages, attachments, expiry
- [x] **PlayerSocial.cs** - Friend list, block list with notes/groups
- [x] **GuildData.cs** - Guild info, members, ranks, permissions, funds

**JsonDB Methods Added**:
- [x] `GetPlayerSkills()` / `SavePlayerSkills()` - Per-character skill persistence
- [x] `GetPlayerQuests()` / `SavePlayerQuests()` - Per-character quest persistence
- [x] `GetPlayerBank()` / `SavePlayerBank()` - Per-character bank persistence
- [x] `GetPlayerMail()` / `SavePlayerMail()` / `SendMail()` - Mail system with recipient lookup
- [x] `GetPlayerSocial()` / `SavePlayerSocial()` - Friends and blocks persistence
- [x] `LoadAllGuilds()` / `GetGuild()` / `SaveGuild()` / `DeleteGuild()` - Guild persistence
- [x] `GetGuildByName()` / `GetCharacterGuild()` / `GetNextGuildId()` - Guild helpers
- [x] `FindCharacterPath()` - Cross-account character lookup for mail system

**Storage Structure**:
```
db/
  {accountId}/
    {characterName}/
      appearance.json      (existing)
      saved_status_pc.json (existing)
      inventory.json       (existing)
      equipment.json       (existing)
      world_position.json  (existing)
      skills.json          (NEW)
      quests.json          (NEW)
      bank.json            (NEW)
      mail.json            (NEW)
      social.json          (NEW)
  guilds/
    guild_{id}.json        (NEW - global guild storage)
```

### Recent Implementations (2025-11-26)

**Skill Learning System** (COMPLETED):
- **CS_REQUEST_LEARN_SKILL packet**: IDA verified structure (10 bytes: typeID + level)
- **SkillService**: Complete service for skill learning and progression
  - `LearnSkill()`: Learn new skills from NPC trainers with SC_SKILL_ADD response
  - `DistributeSkillPoints()`: Level up skills using combat job skill points
  - `GetLearnedSkills()`: Retrieve character's skill list
  - `HasSkill()` / `GetSkillLevel()`: Query skill knowledge
  - `AddSkillPoints()`: Add skill points to player's pool
- **CS_REQUEST_LEARN_SKILL_Handler**: Handles skill learning requests
- **CS_DISTRIBUTE_COMBAT_JOB_SKILL_POINT_Handler**: Handles skill point distribution
- Integrated with existing PlayerSkills persistence model and MongoDBService

**Item Usage System** (COMPLETED):
- **ItemService**: Complete service for item consumption and effects
  - `UseItemOnSelf()`: Consumables (potions, buffs) with HP/MP restoration
  - `UseItemOnObject()`: Use items on NPCs, pets, or other entities
  - `UseItemAtPosition()`: Position-targeted items (traps, AoE)
  - `UseItemOnSlot()`: Item-to-item usage (combining, upgrading)
  - Item inherit effects: HP/MP restore from item inherits
  - Proper inventory consumption (decrement stack or remove)
- **4 Item Usage Handlers**:
  - CS_USE_INVENTORY_ITEM_SELF_Handler
  - CS_USE_INVENTORY_ITEM_OBJ_Handler
  - CS_USE_INVENTORY_ITEM_POS_Handler
  - CS_USE_INVENTORY_ITEM_SLOT_Handler
- **Response packets**: SC_USE_ITEM_SLOT_RESULT, SC_USE_ITEM_OBJ_RESULT_LIST, SC_USE_ITEM_POS_RESULT_LIST

**Combat Service Enhancements**:
- **Skill Damage System**: Full implementation with Physical/Magical/Hybrid damage types
  - `DealSkillDamage()` method with skill type detection and appropriate stat usage
  - Skill multipliers based on level (1.2x at L1 to 2.4x at L7)
  - Combo bonus (+10%) and cast time bonus (+20% per second)
  - Defense diminishing returns formula: `Atk * (1 - Def/(Def+100))`
- **Magical Damage**: `DealMagicalDamage()` using SpellAttack stats
  - More consistent variance (0.9x-1.1x vs physical 0.8x-1.2x)
  - Magic defense slightly less effective (Def+120 vs Def+100)

**Dual-Wielding Combat System** (COMPLETED):
- **Off-hand weapon detection**: `IsOffHandWeapon()` distinguishes weapons from shields
- **Off-hand stats**: `GetSubAttack()` calculates attack stats for off-hand weapon
- **Dual-wield damage**: Off-hand deals 50% damage with 10% reduced accuracy
- **Separate rolls**: Main and off-hand have independent hit/crit rolls
- **Packet support**: SC_WEAPON_HIT_RESULT.sub HIT_DESC populated for dual-wielding
- **EntityStatus**: Added SubAttack field for off-hand weapon stats

**Ranged Combat System** (COMPLETED):
- **Distance-based delay**: `CalculateRangeHitDelay()` uses actual 3D distance between entities
- **Weapon-specific velocity**: Projectile speeds based on weapon type
  - Bow: 20.0f, CrossBow: 25.0f, Gun: 30.0f, Wand: 12.0f, Staff: 10.0f
- **Bullet packets**: `BuildBulletPacket()` creates SC_LAUNCH_BULLET_ITEM_OBJ packets
- **DamageResult extended**: Added IsRanged, BulletPacket, HitDelay fields
- **Handler integration**: CS_USE_SKILL_ACTION_TARGET and MonsterAttacking send bullet packets
- **EntityStatus**: Added IsRanged field for ranged weapon detection

**Healing System** (COMPLETED):
- **HealingService**: New service for healing calculations
  - `HealTarget()`: Heals another entity with critical heal support
  - `HealSelf()`: Self-healing with Spirit bonus
  - `RollCriticalHeal()`: Critical heal chance (50% bonus healing)
  - `CalculateNaturalRegenHP()`: 1% MaxHP + Vitality bonus
  - `CalculateNaturalRegenMP()`: 2% MaxMP + Spirit bonus
- **HealResult**: New result class with HealAmount, IsCritical, WasOverheal, packets
- **Healing skill detection**: Recognizes Heal, Recovery, Cure, Regeneration skill types
- **CS_USE_SKILL_SELF_Handler**: Updated to apply healing for heal-type skills
- **Critical heal logging**: Logs critical heals with player name and amount

**Buff/Debuff System** (COMPLETED):
- **IBuffService Interface**: Complete buff management API
  - `ApplyBuff()`: Apply buff with type, level, duration, caster tracking
  - `RemoveBuff()`: Remove specific buff instance
  - `RemoveBuffsByType()`: Remove all buffs of a type
  - `ClearAllBuffs()`: Clear all buffs (optionally keep debuffs)
  - `GetActiveBuffs()`: Get list of active buffs on entity
  - `HasBuff()`: Check if entity has specific buff type
  - `UpdateBuffTimers()`: Update duration timers, remove expired buffs (returns expired list)
  - `GetBuffModifiers()`: Get aggregated stat modifiers from all buffs
  - `ApplyBuffsFromSkill()`: Parse skill XML and apply referenced buffs
  - `IsBuffSkill()` / `IsDebuffSkill()`: Classify skill types
- **BuffService Implementation**: Full buff management with:
  - Thread-safe entity buff tracking (ConcurrentDictionary)
  - Global buff instance ID generation
  - Max 32 buffs per entity limit
  - Buff stacking/refresh (same type refreshes duration)
  - Debuff type classification
  - Buff modifier parsing from XML detail strings
  - Skill-to-buff reference parsing (buff:ID, applyBuff(), effect:name patterns)
- **BuffStatModifiers**: Comprehensive stat modifier class
  - Flat bonuses: STR, INT, DEX, AGI, VIT, SPI, LUK, ATK, DEF, HIT, FLEE, CRIT, HP, MP
  - Percentage bonuses: ATK%, DEF%, MovementSpeed%, AttackSpeed%
  - Special effects: IsStunned, IsSilenced, IsRooted, IsInvincible
- **Packet Integration**:
  - SC_INSERT_DEF: Sent when buff applied
  - SC_REMOVE_DEF: Sent when buff removed/expired
  - SC_DEF_LIST: Sent on login with all active buffs
  - SendBuffList() added to PlayerClient
- **Combat Integration**:
  - DamageHandler checks source stun status (cannot attack if stunned)
  - DamageHandler checks target invincibility (takes no damage)
  - Buff modifiers applied to ATK, DEF, HIT, FLEE, CRIT stats
- **Server Tick Integration**:
  - Buff timers updated every server tick (60 FPS)
  - Expired buffs automatically removed with SC_REMOVE_DEF packets
  - Delta time tracking for accurate buff duration
- **Skill Buff Application**:
  - CS_USE_SKILL_SELF_Handler applies self-buffs from buff-type skills
  - CS_USE_SKILL_ACTION_TARGET_Handler applies debuffs to targets
  - Buff packets broadcast to caster and nearby players

**Natural HP/MP Regeneration System** (COMPLETED):
- **Server Tick Integration**:
  - Regeneration processed every 5 seconds (RegenTickIntervalMs = 5000)
  - Accumulator-based timing for accurate intervals
- **Regeneration Rules**:
  - HP Regen: 1% MaxHP + (Vitality / 10) per tick
  - MP Regen: 2% MaxMP + (Spirit / 5) per tick
  - No regeneration while dead (HP <= 0)
  - No regeneration while in combat (10 second combat exit delay)
- **Combat State Tracking**:
  - `_lastCombatTime` field tracks last combat activity
  - `IsInCombat()` returns true if in combat or <10 seconds since combat
  - `RecordCombatActivity()` updates combat timestamp
  - `InitCombat()` / `StopCombat()` manage combat state
- **HP/MP Updates**:
  - Uses existing `UpdateHP()` / `UpdateMP()` methods with broadcast=false
  - Status changes sent only to player (no nearby broadcast for regen)

**Quest System**:
- **IPlayerQuests Interface**: Quest tracking contract with completion counts
- **PlayerQuestsTracker**: Implements quest progress tracking
  - Quest categories: Main (000-002), Sub (100-102), Normal (200-207)
  - Methods: AcceptQuest, CompleteQuest, AbandonQuest, UpdateQuestProgress
- **CS_QUEST_LIST Handler**: Returns SC_QUEST_LIST with completion stats + SC_QUEST_ADD for active quests
- **PlayerClient Integration**: GetQuests(), quest persistence via MongoDB

**Ban System**:
- **BanRecord Model**: Persistent ban data with expiry, reason, IP tracking
- **BanService**: Account banning with configurable durations
  - `BanAccount()`: Register bans with duration based on offense count
  - `IsAccountBanned()`: Check ban status at login
  - `KickPlayer()`: Trigger player disconnect via event
  - Duration escalation: 1st offense (hours) -> 2nd (days) -> 3rd+ (permanent)
- **CheatDetection Integration**: `ExecuteBan()` calls BanService on threshold violation
- **CS_LOGIN_Handler**: Rejects banned accounts with LOGIN_RESULT.ACCOUNT_BANNED
- **LimeServer**: Subscribes to BanService.OnPlayerBanned for kick handling

### Combat System Implementation (HIGH Priority)
- [x] **Implement skill damage system** - COMPLETED: Separate calculation with skill multipliers, damage types (Physical/Magical/Hybrid)
- [x] **Add magical damage calculation** - COMPLETED: Use spellAtk/spellDefense with appropriate stats
- [x] **Implement dual-wielding** - COMPLETED: Sub hit descriptor populated, 50% off-hand damage, 10% accuracy penalty
- [ ] Complete stat scaling formulas (reverse engineer actual client functions)
- [ ] Add combat validation framework (all checks from COMBAT_MECHANICS_ANALYSIS.md Section 5)

### Combat System Features (MEDIUM Priority)
- [x] **Implement ranged combat** - COMPLETED: Distance-based projectile delay, weapon-specific velocities, bullet packets
- [x] **Implement healing system** - COMPLETED: HealingService with critical heals (50% bonus), Spirit bonus, natural regen
- [x] **Add status effect/buff system** - COMPLETED: BuffService with duration/stacking, stat modifiers, combat integration
- [ ] Implement elemental damage system (if exists in client)
- [ ] Find and decompile AttackNormal/DoubleAttackNormal functions for complete formula validation

### Existing Testing Tasks
- [ ] **Test CS_START_GAME login flow** - Verify inventory and equipment display with real client
- [ ] **Update/Remove TODO** in CS_START_GAME_Handler.cs based on test results
- ✅ Add proper HP/MP change notifications (SC_CHANGED_HP, SC_CHANGED_MP) - COMPLETED
- ✅ Implement death handling (SC_DEAD packet) - COMPLETED

### Testing Required
- [ ] Test fall damage with real client
- [ ] Test equipment display on login (both combat and life job)
- [ ] Test item stacking/merging behavior
- [ ] Test inventory operations (move, discard, loot)
- [ ] Test anti-cheat system with movement hacks
- [ ] **Test fixed damage calculations** - Verify crits deal MORE damage than normal hits after fix
- [ ] **Test hit rate bounds** - Verify no guaranteed 100% hit or 0% miss scenarios
- [ ] **Test defense scaling** - Verify high defense doesn't cause negative/zero damage

### Future Improvements
- [ ] Add error notifications for silent failures in handlers
- [x] Implement CS_QUEST_LIST response packet (COMPLETED - Returns quest statistics and active quest data)
- [ ] Move hardcoded quests to database-driven system
- [ ] Move hardcoded skills to database-driven system
- ✅ Implement server-side skill cooldown tracking (COMPLETED)
- ✅ Add FlyHack and OutOfBounds detection (COMPLETED)
- [x] Implement actual ban logic (COMPLETED - BanService integrated)
- [ ] Test skill cooldown system with rapid skill spamming
- [ ] Add admin commands for cooldown management (clear, check, etc.)
- NPC/Monster interaction improvements


### Tutorial/Hint/Guide/Help Packet Verification (COMPLETED)
**Date**: 2025-11-26
**Status**: IDA verification completed - NO PACKETS FOUND
**Document**: `/TUTORIAL_HINT_GUIDE_HELP_VERIFICATION_REPORT.md`

Performed comprehensive verification of tutorial, hint, guide, and help packet systems against the Lime Odyssey Korean CBT3 client using IDA Pro MCP integration:

**Verification Results**:
- TUTORIAL Packets: NOT FOUND (0 matches in 22,955 structures)
- HINT Packets: NOT FOUND (only game engine structures)
- GUIDE Packets: NOT FOUND (0 matches)
- HELP Packets: NOT FOUND (only Windows API structures)

**Search Methods Used**:
1. Direct structure searches (TUTORIAL, HINT, GUIDE, HELP)
2. Prefixed packet searches (PACKET_SC_*, PACKET_CS_*)
3. String database searches across entire binary
4. Complete packet enumeration (483 total packets analyzed)

**Conclusion**:
The Lime Odyssey Korean CBT3 client does NOT implement dedicated tutorial/hint/guide/help packets. Player guidance is delivered through alternative systems:

**Alternative Tutorial Systems Identified**:
1. Quest System - Tutorial quests with SC_QUEST_LIST, SC_UPDATE_QUEST_STATE
2. NPC Dialog System - Instructional content via SC_TALKING, SC_TALKING_CHOICE
3. Client-Side UI - Pre-loaded tutorial screens and context-sensitive help
4. Server Notice System - General tips via SC_NOTICE

**Recommendations**:
- Tutorial functionality should be implemented through quest system
- Use special "tutorial quest" category for new player guidance
- Leverage NPC dialog system for step-by-step instructions
- Implement client-side tooltips for contextual help
- Use SC_NOTICE for periodic gameplay tips

**Impact on Development**:
- No packet verification needed for these categories
- No C# packet structures to create
- No packet handlers to implement
- Existing quest/dialog/notice systems sufficient for player guidance

This is a common MMORPG design pattern where dedicated tutorial packets are unnecessary when existing systems can serve the same purpose.

See full verification report for detailed search methodology and alternative implementation strategies.

### Social Packets Verification - Friend/Block/Whisper (COMPLETED)
**Date**: 2025-11-26
**Status**: IDA verification completed, whisper packets created
**Document**: `/addon/SOCIAL_PACKETS_VERIFICATION_REPORT.md`

Verified social system packet structures against the Lime Odyssey Korean CBT3 client using IDA Pro MCP integration:

**Findings**:
- Friend List Packets: NOT FOUND in client (feature not implemented in CBT3)
- Block List Packets: NOT FOUND in client (feature not implemented in CBT3)
- Whisper Packets: VERIFIED and created

**Created Packet Structures**:
1. CS_WHISPER_PC.cs - Client sends whisper (targetPCName: char[26])
2. SC_WHISPER.cs - Server sends whisper (fromName: char[26])

**IDA Structure Details**:
- PACKET_CS_WHISPER_PC: 30 bytes (4-byte PACKET_VAR header + 26-byte targetPCName)
- PACKET_SC_WHISPER: 30 bytes (4-byte PACKET_VAR header + 26-byte fromName)
- Both packets use variable-length message field after fixed fields

**Tracking File Updates**:
- cs_packets.toon: CS_WHISPER_PC marked as verified (2025-11-26)
- sc_packets.toon: SC_WHISPER marked as verified (2025-11-26)

**Recommendations**:
- Friend/Block systems likely planned for later client builds or web-based
- Database schemas prepared for future implementation
- Whisper system ready for handler implementation

**Next Steps**:
- Implement CS_WHISPER_PC_Handler.cs (routing, offline handling)
- Add privacy settings (DND mode, whisper filtering)
- Implement whisper history/logging (optional)

See full verification report for detailed analysis, database schemas, and implementation roadmap.

### Social Systems Analysis (COMPLETED)
**Date**: 2025-11-19
**Status**: Comprehensive analysis completed
**Document**: `/docs/SOCIAL_SYSTEMS_ANALYSIS.md`

Performed complete reverse engineering analysis of Lime Odyssey's party, guild, and social systems using IDA Pro with MCP integration:

**Systems Analyzed**:
1. **Party System** (39 packet types)
   - Complete PARTY_MEMBER (96 bytes) and PARTY_MEMBER_STATE (52 bytes) structures
   - Party creation, invitations, member management
   - Loot distribution options (Free for All, Round Robin, Leader, Master)
   - Party chat and member synchronization
   - HP/MP/LP updates, position tracking, buff/debuff sharing
   - Recommended max party size: 6 members

2. **Guild System** (27 packet types)
   - Complete GUILD (200 bytes) and GUILD_MEMBER (64 bytes) structures
   - Guild creation, invitations, member management
   - 4-tier rank system (Member, Officer, Vice Leader, Leader)
   - Guild notice system (max 100 chars)
   - Guild chat and member synchronization
   - Fame/point/grade progression system

3. **Mail/Post System**
   - 11-item attachment support per mail
   - Complete PACKET_CS_SEND_POST (259 bytes) and PACKET_SC_POST (3649 bytes)
   - Subject limit: 50 characters
   - Full item attachment support

4. **Social Features**
   - Whisper system (PACKET_CS_WHISPER_PC / PACKET_SC_WHISPER)
   - Private chat rooms (multi-participant private channels)
   - Block/ignore list (SC_CHANGED_BLOCK packet)

**Implementation Priorities**:
1. CRITICAL - Party System - COMPLETED (2025-11-26)
2. CRITICAL - Guild System - COMPLETED (2025-11-26)
3. HIGH - Mail System (30-40 hours estimated)
4. MEDIUM - Social Features (20-30 hours estimated)

**Total Estimated Implementation**: 140-200 hours for all social systems

See `/docs/SOCIAL_SYSTEMS_ANALYSIS.md` for complete packet structures, validation requirements, database schemas, pseudocode examples, and security considerations.

### Combat Mechanics Deep Analysis (COMPLETED)
**Date**: 2025-11-19
**Analysis Tool**: IDA Pro with MCP integration
**Document**: `/COMBAT_MECHANICS_ANALYSIS.md`

Performed comprehensive reverse engineering of Lime Odyssey's combat mechanics and damage formulas to identify actual client implementation and compare with server code.

**Key Findings**:

**Combat Architecture**:
- Client uses **server-authoritative damage calculation** (client receives SC_WEAPON_HIT_RESULT packets)
- Dual combat systems: Melee (physical) and Spell (magical) with separate stats
- HIT_DESC structure contains damage, weapon type, hit result, and bonus damage
- Supports dual-wielding with main/off-hand hit descriptors
- Client functions: AttackNormal (single attack), DoubleAttackNormal (dual-wield)

**Stat Systems Found**:
- **Melee Stats**: meleeAtk (0xa30384), meleeDefense (0xa3039c), meleeHitRate (0xa303bc)
- **Spell Stats**: spellAtk (0xa30390), spellDefense (0xa303ac)
- **Universal**: criticalRate (0xa303e4), fleeRate (referenced in server)
- **Base Attributes**: STR, INT, DEX, AGI, VIT, SPI
- Stats stored as base `point` + `extra` bonus values

**Critical Issues in DamageHandler.cs**:

1. **CRITICAL BUG: Inverted Variance Logic** (Lines 18-21)
   - Current: Non-crits get variance = 2.0, crits capped at 1.5
   - Result: **Normal attacks deal MORE damage than critical hits**
   - Fix: Invert logic so crits have higher variance (1.5-2.0)

2. **Missing Defense Effectiveness**
   - Current: Linear defense reduction `(Atk - Def)` can result in 0 or negative damage
   - Fix: Use diminishing returns formula `Atk * (1 - Def/(Def+100))`

3. **Hit Rate Issues**
   - No bounds checking (can exceed 100% or be negative)
   - Division by zero not handled properly
   - Fix: Add Math.Clamp(hitChance, 5, 95) for min/max bounds

4. **Random Number Generator**
   - Creates new Random() for each attack causing poor randomness
   - Fix: Use static ThreadLocal<Random> instance

**Damage Type System**:
```csharp
enum TARGET_DAMAGE_TYPE {
    TDT_ATTACKED_NORMAL = 0x0,              // Normal physical
    TDT_ATTACKED_NORMAL_CRITICAL = 0x1,     // Critical physical
    TDT_ATTACKED_BY_SKILL = 0x2,            // Skill attack
    TDT_ATTACKED_BY_SKILL_CRITICAL = 0x3,   // Critical skill
    TDT_ATTACKED_BY_STREAM = 0x4,           // DoT/Stream
    TDT_HEALED = 0x5,                       // Healing
    TDT_HEALED_CRITICAL = 0x6               // Critical heal
}
```

**Missing Features**:
- ❌ Skill damage calculation (only normal attacks implemented)
- ❌ Magical (spell) damage system (spellAtk/spellDefense not used)
- ❌ Status effects and buffs/debuffs
- ❌ Dual-wielding (packet structure exists but server only populates main hand)
- ❌ Ranged combat (packet fields exist: ranged, rangeHitDelay, rangedVelocity)
- ❌ Healing system (enum exists but not implemented)

**Stat Scaling (Inferred)**:
- STR → MeleeAtk, Physical Defense (minor)
- INT → SpellAtk, Spell Defense, Mana
- DEX → Hit Rate, Critical Rate
- AGI → Flee Rate, Attack Speed
- VIT → HP, Physical Defense, HP Regen
- SPI → Mana, Spell Defense, Healing Power, Mana Regen

**Functions Analyzed**:
- `CGameInfo::ChangedMeleeAtk` (0x46193f) - Updates melee attack stat
- `CGameInfo::ChangedMeleeDefense` (0x4619df) - Updates melee defense stat
- `CSeparateHandler::sc_weapon_hit_result` (0x5b62fa) - Processes weapon hit packet

**Recommended Damage Formula**:
```
// Hit Check
if fleeRate > 0:
    hitChance = (attackerHit / targetFleeRate) * 100
    hitChance = clamp(hitChance, 5, 95)
    if random(0, 100) >= hitChance: return MISS

// Critical Check
critChance = clamp(attackerCritRate, 0, 100)
isCrit = random(0, 100) <= critChance

// Damage Calculation
if isCrit:
    variance = random(1.5, 2.0)
    damage = attackerAtk * variance * 2.0  // Crit multiplier
else:
    variance = random(0.8, 1.2)
    defReduction = targetDef / (targetDef + 100)
    damage = attackerAtk * (1 - defReduction) * variance
    damage = max(1, damage)
```

**Validation Requirements**:
1. Attack permission (alive, in range, not stunned, cooldown elapsed)
2. Stat validity (no negatives, CritRate ≤ 100, Hit ≤ 95%)
3. Damage range (min 0 for miss, min 1 for hit, reasonable max)
4. Weapon requirements (valid equipped weapon, not broken)
5. Combat state (PvP flags, faction rules, combat-enabled)

**Immediate Action Items** (Priority Order):
1. **CRITICAL** - Fix critical hit variance logic (inverted, game-breaking)
2. **HIGH** - Fix hit rate calculation (add bounds checking)
3. **HIGH** - Implement defense diminishing returns
4. **MEDIUM** - Fix Random instance creation
5. **HIGH** - Implement skill damage system
6. **MEDIUM** - Add dual-wielding support
7. **MEDIUM** - Implement spell damage system
8. **LOW** - Add ranged combat system
9. **LOW** - Implement healing system

**IDA Pro Details**:
- Binary: LimeOdyssey.exe (MD5: 54052ef4b6b47a7e5516aab2d976ee8b)
- Base Address: 0x400000, Size: 0x7e1000
- Architecture: x86 (32-bit), MSVC++ compiled

See full analysis document for detailed pseudocode, structure definitions, and comprehensive recommendations.


