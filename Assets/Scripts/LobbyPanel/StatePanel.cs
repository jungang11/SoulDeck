using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class StatePanel : MonoBehaviour
{
    [SerializeField] RectTransform content; // LogText 가 들어갈 자리
    [SerializeField] TMP_Text logPrefab;    // LogText Prefab

    private ClientState state;  // 서버 상태

    void Update()
    {
        // PhotonNetwork Class -> 서버에 무언가 요청, 신청, 확인할 때 사용
        // 내 상태가 바뀌지 않았다면 (기존 상태와 같다면) return
        if (state == PhotonNetwork.NetworkClientState)
            return;

        // 상태가 바뀌었으면 LogText 출력
        state = PhotonNetwork.NetworkClientState;

        TMP_Text newLog = Instantiate(logPrefab, content);
        newLog.text = string.Format("[Photon] {0} : {1}", System.DateTime.Now.ToString("HH:mm:ss.ff"), state);
        Debug.Log(string.Format("[Photon] {0}", state));
    }

    // Log Message
    public void AddMessage(string message)
    {
        TMP_Text newLog = Instantiate(logPrefab, content);
        newLog.text = string.Format("[Photon] {0} : {1}", System.DateTime.Now.ToString("HH:mm:ss.ff"), message);
        Debug.Log(string.Format("[Photon] {0}", message));
    }
}
