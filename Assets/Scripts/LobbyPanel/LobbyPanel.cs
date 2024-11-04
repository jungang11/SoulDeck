using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] RectTransform roomContent; // RoomEntry가 들어갈 자리
    [SerializeField] RoomEntry roomEntryPrefab;

    Dictionary<string, RoomInfo> roomDictionary; // 방들의 목록을 관리하기 위한 Dictionary

    public void Awake()
    {
        roomDictionary = new Dictionary<string, RoomInfo>();
    }

    private void OnDisable()
    {
        // 로비에 들어갔을 때 이전 로비의 상황이 유지되는 것을 방지
        roomDictionary.Clear();
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        // Clear room list
        for (int i = 0; i < roomContent.childCount; i++)
        {
            Destroy(roomContent.GetChild(i).gameObject);
        }

        // Update room info
        foreach (RoomInfo roomInfo in roomList)
        {
            // 방이 곧 사라질 예정(RemovedFromList) + 방이 비공개 됐을 경우 + 방이 닫혔을 경우(선택사항)
            if (roomInfo.RemovedFromList || !roomInfo.IsVisible || !roomInfo.IsOpen)
            {
                // 방이 있었던 경우에만
                if (roomDictionary.ContainsKey(roomInfo.Name))
                {
                    // Dictionary에서 해당 이름의 방을 지워줌
                    roomDictionary.Remove(roomInfo.Name);
                }
                continue;
            }

            // 방이 있었으면 최신으로 갱신
            if (roomDictionary.ContainsKey(roomInfo.Name))
            {
                roomDictionary[roomInfo.Name] = roomInfo;
            }
            // 방이 없었으면 (방이 새로 생성된 경우)
            else
            {
                roomDictionary.Add(roomInfo.Name, roomInfo);
            }
        }

        // Create room list
        foreach (RoomInfo roomInfo in roomDictionary.Values)
        {
            // roomContent 자리에 roomEntryPrefab 생성
            RoomEntry entry = Instantiate(roomEntryPrefab, roomContent);
            entry.SetRoomInfo(roomInfo); // 방 정보 설정
        }
    }

    // Leave Button
    public void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
    }
}
