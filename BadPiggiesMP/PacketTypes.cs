using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketTypes
{
    /// <summary>
    /// Enums for Client (outgoing) only packets.
    /// </summary>
    public enum ClientPackets
    {
        PLACE_PART = 0x00,
        GAME_START = 0x02,
        PART_UPDATE = 0x04,
        WIN = 0x05,
        LEVEL_SWITCH = 0x06,
        ACTIVATE_PART = 0x08,
        KICK = 0x0A,
        SELECTED = 0x0C
    }

    /// <summary>
    /// Enums for Server (incoming) only packets. 
    /// </summary>
    public enum ServerPackets
    {
        REQ_PLACE_PART = 0x01,
        REQ_GAME_START = 0x03,
        REQ_LEVEL_SWITCH = 0x07,
        REQ_ACTIVATE_PART = 0x09,
        CONNECT = 0x0B,
        PART_UPDATE = 0x0D
    }
}
