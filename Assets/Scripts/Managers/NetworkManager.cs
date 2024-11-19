using Photon.Pun;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private float lastKeepAliveTime;
    private float keepAliveInterval = 10f; // Keep Alive 패킷 전송 간격 설정

    private void Awake()
    {
        SetPhotonNetworkSetting();
    }

    private void Update()
    {
        // SendKeepAlive();
    }

    private void SetPhotonNetworkSetting(int sendRate = 30, int serializationRate = 15, int disconnectTimeout = 10000)
    {
        LogApi.Log($"[Set Network Setting] >> sendRate: {sendRate}, serializationRate: {serializationRate}, disconnectTimeout: {disconnectTimeout}");
        
        PhotonNetwork.SendRate = sendRate;
        PhotonNetwork.SerializationRate = serializationRate;
        PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout = disconnectTimeout;
    }

    private void SendKeepAlive()
    {
        if (PhotonNetwork.IsConnected && Time.time - lastKeepAliveTime >= keepAliveInterval)
        {
            lastKeepAliveTime = Time.time;
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }
}
