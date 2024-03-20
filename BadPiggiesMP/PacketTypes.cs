namespace PacketTypes
{
    /// <summary>
    /// Enums for Client (outgoing) only packets.
    /// </summary>
    public enum ClientPackets
    {
        /// <summary>
        /// Packet for when a part is placed on the grid.
        /// </summary>
        PLACE_PART = 0x00,

        /// <summary>
        /// Packet for when a game is started.
        /// </summary>
        GAME_START = 0x02,

        /// <summary>
        /// Packet for updating the data of all parts, like rotation and position.
        /// </summary>
        PART_UPDATE = 0x04,

        /// <summary>
        /// Packet for when the level is won.
        /// </summary>
        WIN = 0x05,

        /// <summary>
        /// Packet for when the level is switched.
        /// </summary>
        LEVEL_SWITCH = 0x06,

        /// <summary>
        /// Packet for when a part is activated.
        /// </summary>
        ACTIVATE_PART = 0x08,

        /// <summary>
        /// Packet for when a client is disconnected.
        /// </summary>
        KICK = 0x0A,

        /// <summary>
        /// Packet for when a specific client is selected to relay their physics data.
        /// </summary>
        SELECTED = 0x0C
    }

    /// <summary>
    /// Enums for Server (incoming) only packets. 
    /// </summary>
    public enum ServerPackets
    {
        /// <summary>
        /// Packet for when a client requests that a part be placed.
        /// </summary>
        REQ_PLACE_PART = 0x01,

        /// <summary>
        /// Packet for when a client requests to start the game.
        /// </summary>
        REQ_GAME_START = 0x03,

        /// <summary>
        /// Packet for when a client requests that the level is switched.
        /// </summary>
        REQ_LEVEL_SWITCH = 0x07,

        /// <summary>
        /// Packet for when a client requests that a part is activated.
        /// </summary>
        REQ_ACTIVATE_PART = 0x09,

        /// <summary>
        /// Packet for when a client is connecting to the requested game.
        /// </summary>
        CONNECT = 0x0B,

        /// <summary>
        /// Packet for when the selected client updates part info.
        /// </summary>
        PART_UPDATE = 0x0D
    }
}
