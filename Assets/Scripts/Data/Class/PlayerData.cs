using System.Collections.Generic;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerData
{
    public int PlayerID { get; set; }
    public string Nickname { get; set; }
    public List<Card> cardList = new List<Card>(); // Player가 가지고 있는 Card List
    
    

    /// <summary>
    /// Return Photon Hashtable
    /// </summary>
    public PhotonHashtable ToHashtable()
    {
        PhotonHashtable hashtable = new PhotonHashtable
        {
            { "PlayerID", PlayerID },
            { "Nickname", Nickname },
        };
        return hashtable;
    }

    /// <summary>
    /// Return PlayerData
    /// </summary>
    public static PlayerData FromHashtable(PhotonHashtable hashtable)
    {
        return new PlayerData
        {
            PlayerID = hashtable.ContainsKey("PlayerID") ? (int)hashtable["PlayerID"] : -1,
            Nickname = hashtable.ContainsKey("Nickname") ? (string)hashtable["Nickname"] : string.Empty,
        };
    }
}
