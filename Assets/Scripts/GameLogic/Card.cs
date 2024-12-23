using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int currentCardIndex;    // 카드 숫자

    public Image cardBackImage;
    public Image cardFrontImage;
    public TMP_Text cardIndexText;

    public Transform cardTransform;
    private Vector3 frontVector = new Vector3(90f, 0f, 0f);
    private Vector3 backVector = new Vector3(-90f, 0f, 0f);

    private void Awake()
    {
        cardTransform = GetComponent<Transform>();
    }
    
    private void Start()
    {
        if (testRoutine == null)
        {
            testRoutine = StartCoroutine(TestRoutine());
        }
    }

    private void InitCardData()
    {
        currentCardIndex = 0;
        SetCardIndexText(currentCardIndex);
    }

    private Coroutine testRoutine = null;
    private IEnumerator TestRoutine()
    {
        while (true)
        {
            SetCardFront();
            SetCardIndexText(currentCardIndex++);
            yield return new WaitForSeconds(3f);
            SetCardBack();
            SetCardIndexText(currentCardIndex++);
            yield return new WaitForSeconds(3f);
        }
    }

    /// <summary>
    /// Set Card Index Text(TMP)
    /// </summary>
    public void SetCardIndexText(int cardIndex)
    {
        cardIndexText.text = $"{cardIndex}";
    }

    /// <summary>
    /// Set Card Front
    /// </summary>
    public void SetCardFront()
    {
        cardTransform.DORotate(frontVector, 1.5f, RotateMode.FastBeyond360);
    }

    /// <summary>
    /// Set Card Back with Animation
    /// </summary>
    public void SetCardBack()
    {
        cardTransform.DORotate(backVector, 1.5f, RotateMode.FastBeyond360);
    }
}