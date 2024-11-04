using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntry : MonoBehaviour
{
    [SerializeField] TMP_Text roomName;
    [SerializeField] TMP_Text currentPlayer;
    [SerializeField] Button enterRoomButton;

    private RoomInfo info;

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        info = roomInfo;
        roomName.text = roomInfo.Name;
        currentPlayer.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
        enterRoomButton.interactable = roomInfo.PlayerCount < roomInfo.MaxPlayers; // 방이 다 차지 않았을 경우만 클릭 가능
    }

    public void JoinRoom()
    {
        // 로비에 있다가 방에 들어갈 경우 로비를 나가야함 (포톤 특징). 나가지 않을 경우 Lobby 에서 정보가 계속 갱신
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(info.Name);
    }
}
