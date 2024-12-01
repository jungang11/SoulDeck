using Photon.Pun;
using UnityEngine;

/// <summary>
/// 전체 카드 관리, 카드 지급/제출 관리 스크립트, 계산 처리 과정은 Host 진행 지향
/// </summary>
public class CardManager : MonoBehaviour
{
    public void GetCardRPC(int cardIndex)
    {
        GetCard(cardIndex);
    }
    
    /// <summary>
    /// master client set cardIndex, Local Player Get Card 
    /// </summary>
    public void GetCard(int cardIndex)
    {
        
    }
}
