using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        // Hierarchy에서 GameManager를 검색
        GameManager existingManager = FindFirstObjectByType<GameManager>();
    
        if (existingManager == null)
        {
            // Hierarchy에 GameManager가 없으면 새로 생성
            GameObject gameManager = new GameObject() { name = "GameManager" };
            gameManager.AddComponent<GameManager>();
        }
        else
        {
            // Hierarchy에 GameManager가 이미 있으면 로깅
            LogApi.Log("Existing GameManager found. Using it as the singleton instance.");
        }
    }
}