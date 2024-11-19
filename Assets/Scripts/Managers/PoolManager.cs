using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    Dictionary<string, ObjectPool<GameObject>> poolDic;
    Dictionary<string, Transform> poolContainer;
    Transform poolRoot;
    Canvas canvasRoot;

    private void Start()
    {
        OnInit();
    }

    public void OnInit()
    {
        poolDic = new Dictionary<string, ObjectPool<GameObject>>();
        poolContainer = new Dictionary<string, Transform>();

        poolRoot = new GameObject("PoolRoot").transform;
        poolRoot.SetParent(transform);

        canvasRoot = GameManager.Resource.Instantiate<Canvas>("UI/Canvas");
        canvasRoot.name = "PoolCanvasRoot";
        canvasRoot.transform.SetParent(transform);
    }

    // Get: return PooledObject GameObject/ Component
    public T Get<T>(T original, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where T : Object
    {
        GameObject prefab = GetObjectFromPrefab(original);
        if (prefab == null) 
        {
            Debug.LogWarning("[PoolManager] >> Get() Prefab is null");
            return null;
        }

        string key = prefab.name;
        if(!poolDic.ContainsKey(key))
        {
            CreatePool(key, prefab);
        }

        GameObject obj = poolDic[key].Get();
        obj.transform.SetParent(parent);
        obj.transform.SetPositionAndRotation(position, rotation);

        if (obj.TryGetComponent<IPoolable>(out IPoolable poolable))
            poolable?.OnSpawn();

        return obj.GetComponent<T>();
    }

    private GameObject GetObjectFromPrefab<T>(T original) where T : Object
    {
        return original is GameObject ? original as GameObject :
               original is Component component? component.gameObject : null;
    }

    public bool IsContain<T>(T original) where T : Object
    {
        GameObject prefab = GetObjectFromPrefab(original);
        return prefab != null && poolDic.ContainsKey(prefab.name);
    }

    public bool Release<T>(T instance) where T : Object
    {
        GameObject obj = instance is Component component ? component.gameObject : instance as GameObject;
        if (obj == null) return false;

        string key = obj.name;
        if (!poolDic.ContainsKey(key)) return false;

        if(obj.TryGetComponent<IPoolable>(out IPoolable poolable))
            poolable?.OnDespawn();

        poolDic[key].Release(obj);
        return true;
    }

    private void CreatePool(string key, GameObject prefab, bool isUI = false)
    {
        Transform container = new GameObject($"{key}Container").transform;
        container.SetParent(isUI? canvasRoot.transform : poolRoot);
        poolContainer[key] = container;

        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject obj = Instantiate(prefab);
                obj.name = key;
                return obj;
            },
            actionOnGet: (GameObject obj) =>
            {
                obj.SetActive(true);
                obj.transform.SetParent(null);
            },
            actionOnRelease: (GameObject obj) =>
            {
                obj.SetActive(false);
                obj.transform.SetParent(poolContainer[key]);
            },
            actionOnDestroy: (GameObject obj) =>
            {
                Destroy(obj);
            }
            );
        poolDic.Add(key, pool);
    }

    // Get >> UI
    public T GetUI<T>(T original, Vector3 position = default) where T : Object
    {
        T obj = Get(original, position);
        if(obj is GameObject go)
        {
            go.transform.SetParent(canvasRoot.transform, false);
        }
        return obj;
    }

    public bool ReleaseUI<T>(T instance) where T : Object
    {
        return Release(instance);
    }
}