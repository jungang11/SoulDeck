using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseUI : MonoBehaviour
{
    protected Dictionary<string, RectTransform> transforms = new();
    protected Dictionary<string, Button> buttons = new();
    protected Dictionary<string, TMP_Text> texts = new();
    protected Dictionary<string, TMP_InputField> inputFields = new();
    protected Dictionary<string, Image> images = new();

    protected virtual void Awake()
    {
        InitializeUIElements();
    }

    private void InitializeUIElements()
    {
        // GetComponentsInChildren 호출을 통해 모든 자식 객체 가져오기
        RectTransform[] children = GetComponentsInChildren<RectTransform>(true);
        
        foreach (RectTransform child in children)
        {
            string key = child.gameObject.name;
            
            if (transforms.ContainsKey(key)) 
                continue; // 이미 등록된 키라면 무시
            
            transforms[key] = child;

            Button button = child.GetComponent<Button>();
            if (button != null)
                buttons[key] = button;

            TMP_Text text = child.GetComponent<TMP_Text>();
            if (text != null)
                texts[key] = text;

            TMP_InputField inputField = child.GetComponent<TMP_InputField>();
            if (inputField != null)
                inputFields[key] = inputField;

            Image image = child.GetComponent<Image>();
            if (image != null)
                images[key] = image;
        }
    }

    // Get Component From Dictionary
    public RectTransform GetRectTransform(string key) => transforms.TryGetValue(key, out var rect) ? rect : null;
    public Button GetButton(string key) => buttons.TryGetValue(key, out var button) ? button : null;
    public TMP_Text GetTMPText(string key) => texts.TryGetValue(key, out var text) ? text : null;
    public Image GetImage(string key) => images.TryGetValue(key, out var image) ? image : null;

    /// <summary>
    /// (buttonName).onClick.AddListener(() => onClickAction());
    /// </summary>
    public void SetButtonOnClickEvent(string buttonName, Action onClickAction)
    {
        if (buttons.TryGetValue(buttonName, out var button) && onClickAction != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClickAction());
        }
        else
        {
            Debug.LogWarning($"[SetButtonOnClickEvent] >> buttons[buttonName] value Error");
        }
    }
}