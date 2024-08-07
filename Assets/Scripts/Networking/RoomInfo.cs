using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo
{
    public const int MAX_PLAYER = 6;

    public string RoomID { get; private set; }
    public string Name { get; private set; }
    public int MaxPlayer { get; private set; }
    public string HostNickname { get; private set; }

    public RoomInfo(string RoomID, string Name, int MaxPlayer, string hostNickname) {
        this.RoomID = RoomID;
        this.Name = Name;
        this.MaxPlayer = MaxPlayer;
        this.HostNickname = hostNickname;
    }

    public void SetRoomID(string RoomID) => this.RoomID = RoomID;
}
