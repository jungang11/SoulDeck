using System.Collections.Generic;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

[System.Serializable]
public class PlayerData
{
    public int PlayerID { get; set; }
    public string Nickname { get; set; }
    public List<int> CardIndexList; // Player가 가지고 있는 Card List

    public PlayerData(int playerID, string nickName)
    {
        this.PlayerID = playerID;
        this.Nickname = nickName;
        this.CardIndexList = new List<int>();
    }
}
