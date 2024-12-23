using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private static ResourceManager resourceManager;
    private static PoolManager poolManager;
    private static DataManager dataManager;
    private static UIManager uiManager;
    private static AudioManager audioManager;
    private static SceneChangeManager sceneManager;
    private static NetworkManager networkManager;

    public static GameManager Instance => instance;
    public static ResourceManager Resource => resourceManager;
    public static PoolManager Pool => poolManager;
    public static DataManager Data => dataManager;
    public static UIManager UI => uiManager;
    public static AudioManager Audio => audioManager;
    public static SceneChangeManager Scene => sceneManager;
    public static NetworkManager Network => networkManager;

    public PhotonView photonView;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            // Instance가 이미 다른 객체로 설정되어 있다면, 중복 GameManager 제거
            Destroy(this.gameObject);
        }
        
        DontDestroyOnLoad(this);
        InitManagers();

        photonView = GetComponent<PhotonView>();
        if (photonView != null && photonView.ViewID != 990)
            photonView.ViewID = 990;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void InitManagers()
    {
        resourceManager = CreateManager<ResourceManager>("ResourceManager");
        poolManager = CreateManager<PoolManager>("PoolManager");
        dataManager = CreateManager<DataManager>("DataManager");
        uiManager = CreateManager<UIManager>("UIManager");
        audioManager = CreateManager<AudioManager>("AudioManager");
        sceneManager = CreateManager<SceneChangeManager>("SceneChangeManager");
        networkManager = CreateManager<NetworkManager>("NetworkManager");
    }

    private T CreateManager<T>(string name) where T : Component
    {
        GameObject managerObj = new GameObject($"@{name}");
        managerObj.transform.SetParent(transform);
        return managerObj.AddComponent<T>();
    }
}