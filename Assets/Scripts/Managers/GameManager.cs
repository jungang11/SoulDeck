using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance { get { return instance; } }

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this);
        instance = this;
    }
}