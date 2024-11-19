using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [Header("Scene UI")]
    public Canvas sceneCanvas;

    [Header("PopUp UI")]
    public Canvas popUpCanvas;
    private Stack<PopUpUI> popUpStack;
    public Stack<PopUpUI> PopUpStack { get { return popUpStack; } }

    [Header("Window UI")]
    public Canvas windowCanvas;
    private List<WindowUI> windowList;

    [Header("InGame UI")]
    public Canvas inGameCanvas;

    private void Awake()
    {
        sceneCanvas = InitCanvas("SceneCanvas", 150);
        popUpCanvas = InitCanvas("PopUpCanvas", 100);
        windowCanvas = InitCanvas("WindowCanvas", 50);
        inGameCanvas = InitCanvas("InGameCanvas", 0);

        popUpStack = new Stack<PopUpUI>();
        windowList = new List<WindowUI>();
    }

    private Canvas InitCanvas(string name, int sortingOrder)
    {
        Canvas canvas = GameManager.Resource.Instantiate<Canvas>("UI/Canvas");
        canvas.gameObject.name = name;
        canvas.transform.SetParent(transform);
        canvas.sortingOrder = sortingOrder;
        return canvas;
    }

#region PopUpUI
    public T ShowPopUpUI<T>(T popUpUI) where T : PopUpUI
    {
        if (popUpStack.Count > 0)
        {
            PopUpUI prevUI = popUpStack.Peek();
            prevUI.gameObject.SetActive(false);
        }

        T ui = GameManager.Pool.GetUI<T>(popUpUI);
        ui.transform.SetParent(popUpCanvas.transform, false);
        popUpStack.Push(ui);

        Time.timeScale = 0f;
        return ui;
    }

    public T ShowPopUpUI<T>(string path) where T : PopUpUI
    {
        T ui = GameManager.Resource.Load<T>(path);
        return ShowPopUpUI(ui);
    }

    public void ClosePopUpUI()
    {
        if(popUpStack.Count == 0)
            return;

        PopUpUI ui = popUpStack.Pop();
        if (ui != null && ui.gameObject != null)
        {
            GameManager.Pool.ReleaseUI(ui);
        }
        
        if (popUpStack.Count > 0)
        {
            PopUpUI currentUI = popUpStack.Peek();
            currentUI.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void ClosePopUpUIAll()
    {
        while(popUpStack.Count > 0)
        {
            ClosePopUpUI();
        }
    }
#endregion PopUpUI

#region WindowUI
    public T ShowWindowUI<T>(T windowUI) where T : WindowUI
    {
        windowList.Add(windowUI);

        T ui = GameManager.Pool.GetUI(windowUI);
        ui.transform.SetParent(windowCanvas.transform, false);

        RefreshWindowUI();
        return ui;
    }

    public T ShowWindowUI<T>(string path) where T : WindowUI
    {
        T ui = GameManager.Resource.Load<T>(path);
        return ShowWindowUI(ui);
    }

    public void RefreshWindowUI()
    {
        foreach(var window in windowList)
        {
            window.transform.SetAsFirstSibling();
        }
    }

    public void SelectWindowUI<T>(T windowUI) where T : WindowUI
    {
        // 선택 시 위치 조정
        windowUI.transform.SetAsLastSibling();
    }

    public void CloseWindowUI<T>(T windowUI) where T : WindowUI
    {
        windowList.Remove(windowUI);
        RefreshWindowUI();
        GameManager.Pool.ReleaseUI(windowUI.gameObject);
    }
#endregion WindowUI

#region InGameUI
    public T ShowInGameUI<T>(T inGameUI) where T : InGameUI
    {
        T ui = GameManager.Pool.GetUI(inGameUI);
        ui.transform.SetParent(inGameCanvas.transform, false);

        return ui;
    }

    public T ShowInGameUI<T>(string path) where T : InGameUI
    {
        T ui = GameManager.Resource.Load<T>(path);
        return ShowInGameUI(ui);
    }

    public void CloseInGameUI<T>(T inGameUI) where T : InGameUI
    {
        GameManager.Pool.ReleaseUI(inGameUI.gameObject);
    }
#endregion InGameUI
}