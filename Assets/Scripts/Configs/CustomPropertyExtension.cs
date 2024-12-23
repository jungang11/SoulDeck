using Photon.Pun;
using Photon.Realtime;
using System.Diagnostics;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public static class CustomPropertyExtension
{
#region LobbyScene
    public static bool GetReady_InLobby(this Player player)
    {
        PhotonHashtable property = player.CustomProperties;
        if (property.ContainsKey("ReadyInLobby"))
            return (bool)property["ReadyInLobby"];
        else
            return false;
    }

    public static void SetReady_InLobby(this Player player, bool ready)
    {
        PhotonHashtable property = new PhotonHashtable();
        property["ReadyInLobby"] = ready;
        player.SetCustomProperties(property);
    }
#endregion

#region GameScene
    public static bool GetReady_InGame(this Player player)
    {
        PhotonHashtable property = player.CustomProperties;
        if (property.ContainsKey("ReadyInGame"))
            return (bool)property["ReadyInGame"];
        else
            return false;
    }

    public static void SetReady_InGame(this Player player, bool ready)
    {
        PhotonHashtable property = new PhotonHashtable();
        property["ReadyInGame"] = ready;
        player.SetCustomProperties(property);
    }

    public static bool GetLoad(this Player player)
    {
        PhotonHashtable property = player.CustomProperties;
        if (property.ContainsKey("Load"))
            return (bool)property["Load"];
        else
            return false;
    }

    public static void SetLoad(this Player player, bool load)
    {
        PhotonHashtable property = new PhotonHashtable();
        property["Load"] = load;
        player.SetCustomProperties(property);
    }

    public static int GetLoadTime(this Room room)
    {
        PhotonHashtable property = room.CustomProperties;
        if (property.ContainsKey("LoadTime"))
            return (int)property["LoadTime"];
        else
            return -1;
    }

    public static void SetLoadTime(this Room room, int loadTime)
    {
        PhotonHashtable property = new PhotonHashtable();
        property["LoadTime"] = loadTime;
        room.SetCustomProperties(property);
    }

    public static int GetPlayerID(this Player player)
    {
        PhotonHashtable property = player.CustomProperties;
        if (property.ContainsKey("PlayerID"))
            return (int)property["PlayerID"];
        else
            return -1;
    }

    public static void SetPlayerID(this Player player, int playerID)
    {
        PhotonHashtable property = player.CustomProperties;
        property["PlayerID"] = playerID;
        player.SetCustomProperties(property);
    }
#endregion
}