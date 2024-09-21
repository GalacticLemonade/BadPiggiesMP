# Specification
GET OUT OF MY HEAD GET OUT OF MY HEAD GET OUT OF MY HEAD GET OUT OF MY HEAD GET OUT OF MY HEAD GET OUT OF MY HEAD GET OUT OF MY HEAD GET OUT OF MY HEAD GET OUT OF MY HEAD GET OUT OF MY HEAD GET OUT OF MY HEAD GET OUT OF MY HEAD GET OUT OF MY HEAD GET OUT OF MY HEAD

Version 1.08

## Notes
1. Domain - bpmp.galacticlemon.dev
2. Port - 39182
3. ALL data transmitted over TCP
4. LZW Compression is not yet implemented

## Types
- byte       - 8-bit unsigned integer
- int16      - 16-bit signed integer (short)
- int32      - 32-bit signed integer (int)
- float32    - 32 bit floating point number (float)
- float64    - 64 bit floating point number (double)
- varint     - variable-length encoded int32 (see [wiki.vg](https://wiki.vg/Protocol#VarInt_and_VarLong))
- vec2       - 2 float32s, first is x, second is y
- str        - utf-8 encoded string prefixed by a varint for length in bytes
- bool       - boolean value (true or false you motherfucker)

## Definitions
- Server - The software all clients communicate to. Manages every game and its players
- Clients - All clients connected to individual game
- Client - Individual client connected to individual game
- Owner - The person who originally started a lobby - the "owner" of the lobby/game

## Packet Format
- string Game ID (special identifier of the specific game instance)
- byte Packet ID (identifier of which instruction)
- All following data is defined on a per-packet basis.

## Packets
- 0x00 - Place Part (Server -> Clients):
1. varint X - Location of the part on grid
2. varint Y - Location of the part on grid
3. int16 Type - BasePart.PartType cast to short. Value of -1 means the part was removed
4. varint NetID - Unique identifier of the part

- 0x01 - Request Place Part (Client -> Server):
1. varint X - Location of the part on grid
2. varint Y - Location of the part on grid
3. int16 - BasePart.PartType cast to short. Value of -1 means the part was removed

- 0x02 - Game Start (Server -> Clients):
1. No data

- 0x03 - Request Game Start (Owner -> Server):
1. No data

- 0x04 - Part Update (2-5 is repeated for #NumParts) (LZW) (Server -> Clients):
1. varint NumParts
2. varint NetID - Unique identifier of the part
3. vec2 Pos - New part position
4. float32 RotZ - New part rotation
5. float32 RotW - New part rotation

- 0x05 - Win (Server -> Clients):
1. No data - Tells all clients that the current level has been won

- 0x06 - PLACEHOLDER (PLACEHOLDER):
1. No data - PLACEHOLDER

- 0x07 - PLACEHOLDER (PLACEHOLDER):
1. No data - PLACEHOLDER

- 0x08 - Request Win (Owner -> Server):
1. No data

- 0x09 - Switch Level (Server -> Clients):
1. int32 LevelID - ID of the level to go to

- 0x0A - Request Level Switch (Owner -> Server):
1. int32 LevelID - ID of the level to go to

- 0x0B - Activate Part (Server -> Clients):
1. varint NetID - Unique identifier of the part to activate

- 0x0C - Request Activate Part (Client -> Server):
1. varint NetID - Unique identifier of the part to activate

- 0x0D - Disconnected (Server -> Client)
1. str Message - Reason why the user was disconnected

- 0x0E - Connect (Client -> Server)
1. str Username - Username of the connecting user

- 0x0F - Selected (Server -> Client)
1. No data - Tells a client it will provide all physics calculations

- 0x10 - Part Update (2-5 is repeated for #NumParts) (LZW) (Client -> Server):
1. varint NumParts
2. varint NetID - Unique identifier of the part
3. vec2 Pos - New part position
4. float32 RotZ - New part rotation
5. float32 RotW - New part rotation

- 0x11 - Is Owner (Server -> Owner)
1. No data - Tells the FIRST client that is in a lobby that it is the round owner

- 0x12 - Join Request (Server -> Owner)
1. str Username - Username of the user attempting to join the current game

- 0x13 - Join Request (Owner -> Server)
1. str Username - Username of the user attempting to join the current game
2. bool Allowed - Whether the user is allowed to connect