using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] GameObject baseMenuPanel;
    [SerializeField] GameObject createRoomPanel;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_InputField maxPlayerInputField;

    private void OnEnable()
    {
        baseMenuPanel.SetActive(true);
        createRoomPanel.SetActive(false);
    }

    #region Create Room Panel
    public void OnClickBtn_CreateRoom()
    {
        baseMenuPanel.SetActive(false);
        createRoomPanel.SetActive(true);
    }

    public void OnClickBtn_CreateRoomConfirm()
    {
        string roomName = roomNameInputField.text;
        // 방 이름이 비어있으면 방 제목 랜덤 생성
        if (roomName == "")
            roomName = $"Room {Random.Range(1000, 1000)}";

        // 최대 인원 수 설정하지 않으면 8
        int maxPlayer = maxPlayerInputField.text == "" ? 8 : int.Parse(maxPlayerInputField.text);
        // 최대 인원 수 설정 제한은 8
        maxPlayer = Mathf.Clamp(maxPlayer, 1, 8);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayer };   // 인원 수 설정
        PhotonNetwork.CreateRoom(roomName, options); // 방의 설정 적용 가능 (패스워드, 레벨제한, 인원 수 등)
    }

    public void OnClickBtn_CreateRoomCancel()
    {
        baseMenuPanel.SetActive(true);
        createRoomPanel.SetActive(false);
    }
    #endregion

    public void OnClickBtn_RandomMatching()
    {
        string name = $"Room {Random.Range(1000, 10000)}";
        RoomOptions options = new RoomOptions { MaxPlayers = 8 };
        // 빠른 참가 / 방 생성
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: name, roomOptions: options);
    }

    public void OnClickBtn_JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public void OnClickBtn_ExitGame()
    {
        PhotonNetwork.Disconnect(); // 닉네임 설정 필요 X, 서버에 접속 해제 신청
    }
}
