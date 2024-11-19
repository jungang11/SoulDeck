using System.Collections;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using DG.Tweening;

public class SceneChangeManager : MonoBehaviour
{
    [Header("Loading UI")]
    public LoadingUI loadingUI;
    private CanvasGroup fade_Loading;

    private BaseScene curScene;
    public BaseScene CurScene
    {
        get
        {
            if (curScene == null)
                curScene = FindFirstObjectByType<BaseScene>();

            return curScene;
        }
    }

    private void Start()
    {
        if (loadingUI == null)
        {
            loadingUI = GameManager.Resource.Instantiate<LoadingUI>("UI/loadingUI");
            loadingUI.transform.SetParent(GameManager.UI.sceneCanvas.transform, false);
        }

        fade_Loading = loadingUI.GetComponent<CanvasGroup>();
        fade_Loading.alpha = 0f;
        fade_Loading.blocksRaycasts = false;
    }

    public void LoadScene(string sceneName)
    {
        if(GameManager.UI.PopUpStack.Count > 0)
            GameManager.UI.ClosePopUpUIAll();

        ChangeScene(sceneName);
    }

    public void ChangeScene(string sceneName)
    {
        loadingUI.gameObject.SetActive(true);

        fade_Loading.DOFade(1, 0.4f)
        .OnStart(() =>
        {
            fade_Loading.blocksRaycasts = true; // 아래 레이캐스트 막기
            LogApi.Log($"[Load Scene Start] >> {sceneName}");
        })
        .OnComplete(() =>
        {
            StartCoroutine(nameof(LoadSceneRoutine), sceneName);
        });
    }

    IEnumerator LoadSceneRoutine(string sceneName)
    {
        loadingUI.gameObject.SetActive(true);

        AsyncOperation async = UnitySceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false; // 퍼센트 딜레이용

        float past_time = 0;
        float percentage = 0;

        while(!async.isDone)
        {
            yield return null;

            past_time += Time.deltaTime;

            if(percentage >= 90)
            {
                percentage = Mathf.Lerp(percentage, 100, past_time);

                if(percentage == 100)
                {
                    async.allowSceneActivation = true; //씬 전환 준비 완료
                }
            }
            else
            {
                percentage = Mathf.Lerp(percentage, async.progress * 100f, past_time);
                if(percentage >= 90) past_time = 0;
            }
        }
    }

    private void OnEnable()
    {
        UnitySceneManager.sceneLoaded += OnSceneLoaded; // 이벤트에 추가
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        fade_Loading.DOFade(0, 0.4f)
        .OnStart(() =>
        {
            fade_Loading.blocksRaycasts = false;
        })
        .OnComplete(() =>
        {
            if(scene.name.Length == 3)
            {
                loadingUI.gameObject.SetActive(false);
            }
        });
    }

    private void OnDestroy()
    {
        UnitySceneManager.sceneLoaded -= OnSceneLoaded; // 이벤트에서 제거*
    }
}