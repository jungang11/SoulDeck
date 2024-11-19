using UnityEngine;

/// <summary>
/// 게임 씬 내 Local Canvas UI 설정
/// </summary>
public class GameSceneUIManager : MonoBehaviour
{
    [Header("Game Scene Local UI Canvas")]
    [SerializeField] private Canvas hudCanvas;  // Canvas - Overlay

    private void Awake()
    {
        InitHUDCanvas();
    }

    private void InitHUDCanvas()
    {
        if (hudCanvas != null)
        {
            hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            hudCanvas.sortingOrder = 5; // 다른 Canvas보다 위에 표시
        }
        else
        {
            Debug.LogError("[GameSceneUIManager] >> HUD Canvas is Null");
        }
    }

    public void ShowHUD(bool show)
    {
        hudCanvas.gameObject.SetActive(show);
    }
}
