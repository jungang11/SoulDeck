using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField idInputField;
    [SerializeField] TMP_InputField passwordInputField;

    public void OnClickBtn_LoginButton()
    {
        string id = idInputField.text;
        string password = passwordInputField.text;

        Login(id, password);
    }

    // Login 버튼 실행 함수
    private void Login(string id, string password)
    {
        try
        {
            LogApi.Log($"[Login] >> id: {id}, pwd: {password}");

            if(id != null)
            {
                PhotonNetwork.LocalPlayer.NickName = id;
                PhotonNetwork.ConnectUsingSettings();   // 접속 신청
            }
            else
            {
                LogApi.LogWarning("[Login] >> ID is Null");
                idInputField.text = "";
                passwordInputField.text = "";
            }
        }
        catch (Exception e)
        {
            LogApi.LogError(e);
        }
    }
}
