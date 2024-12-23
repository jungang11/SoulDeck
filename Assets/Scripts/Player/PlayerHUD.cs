using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    
    [SerializeField] private GameObject playerHUD_MasterHUD;
    [SerializeField] private GameObject playerHUD_CardHolder;
    
    // 카드 Get 테스트용
    public TMP_Text[] numberTexts;

    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (pv.IsMine)
        {
            ShowPlayerHUD();
        }
        else
        {
            this.enabled = false;
        }
    }
    
    public void ShowPlayerHUD()
    {
        EnableMasterHUD();
        EnableCardHolder();
    }

    public void HidePlayerHUD()
    {
        DisableCardHolder();
        
        if (PhotonNetwork.IsMasterClient)
            EnableMasterHUD();
    }

    private void EnableMasterHUD()
    {
        if (PhotonNetwork.IsMasterClient)
            playerHUD_MasterHUD.SetActive(true);
        else
        {
            LogApi.Log("Not Master Client >> Disable Master HUD");
            if (playerHUD_MasterHUD.activeSelf)
                playerHUD_MasterHUD.SetActive(false);
        }
    }

    private void DisableMasterHUD()
    {
        if (PhotonNetwork.IsMasterClient)
            playerHUD_MasterHUD.SetActive(false);
    }

    private void EnableCardHolder()
    {
        playerHUD_CardHolder.SetActive(true);
        UpdateCardHolder();
    }

    private void DisableCardHolder()
    {
        playerHUD_CardHolder.SetActive(false);
    }

    #region MasterClient HUD

    public void OnClickBtn_ChangeState_MovingPlaying()
    {
        pv.RPC("TestState_RPC", RpcTarget.All);
    }

    [PunRPC]
    public void TestState_RPC()
    {
        if(playerController.CurrentState == PlayerController.PlayerState.Playing)
            playerController.SetState(PlayerController.PlayerState.Moving);
        else if(playerController.CurrentState == PlayerController.PlayerState.Moving)
            playerController.SetState(PlayerController.PlayerState.Playing);
    }
    
    #endregion
    
    #region CardHolder HUD

    public void OnClickBtn_GetCardAllPlayer()
    {
        pv.RPC("GetCardAllPlayer_RPC", RpcTarget.All);
    }

    [PunRPC]
    public void GetCardAllPlayer_RPC()
    {
        GameManager.Data.GetCard();
        UpdateCardHolder();
    }
    /// <summary>
    /// PlayerData의 보유 CardList를 UI 표시
    /// </summary>
    public void UpdateCardHolder()
    {
        if (!playerHUD_CardHolder.activeSelf) return;
        
        int currentCardCount = GameManager.Data.LocalCardIndexArray.Length;
        for (int i = 0; i < currentCardCount; i++)
        {
            LogApi.Log($"[UpdateCardHolder] >> {i}");
            numberTexts[i].text = GameManager.Data.LocalCardIndexArray[i].ToString();
        }

        for (int i = currentCardCount; i < numberTexts.Length; i++)
        {
            numberTexts[i].text = "";
        }
    }

    public void OnClickCardHolder(int cardHolderIndex)
    {
        LogApi.Log($"Click Card Holder >> index: {cardHolderIndex}");
        
        // TODO. 카드 제출 로직 수행
        // DataManager 통해서 카드 제출
        // 제출 함수를 Master에게 전달 -> Master에서 모든 로직 계산 후 유저들에게 결과 뿌리기
        GameManager.Data.SubmitCard(cardHolderIndex);
    }
    #endregion
}
