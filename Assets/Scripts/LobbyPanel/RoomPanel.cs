using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviour
{
    [SerializeField] RectTransform playerContent;
    [SerializeField] PlayerEntry playerEntryPrefab;
    [SerializeField] Button startButton;

    public void UpdatePlayerList()
    {
        // Clear player list
        for (int i = 0; i < playerContent.childCount; i++)
        {
            Destroy(playerContent.GetChild(i).gameObject);
        }

        // Update player list
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            PlayerEntry entry = Instantiate(playerEntryPrefab, playerContent);
            entry.SetPlayer(player);
        }

        // player add
        if (PhotonNetwork.IsMasterClient) // 내가 방장인지 확인 요청
            CheckPlayerReady();
        else
            startButton.gameObject.SetActive(false);
    }

    // Start Button
    public void StartGame(string sceneName)
    {
        GameManager.Scene.LoadScene($"{sceneName}");
    }

    // Leave Button
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void CheckPlayerReady()
    {
        int readyCount = 0;

        // 전체 플레이어의 레디 상황 확인
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetReady_InLobby())
                readyCount++;
        }

        startButton.gameObject.SetActive(readyCount == PhotonNetwork.PlayerList.Length);
    }
}
