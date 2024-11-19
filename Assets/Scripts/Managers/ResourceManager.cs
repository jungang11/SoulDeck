using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    Dictionary<string, Object> resources = new Dictionary<string, Object>();

    public T Load<T>(string path) where T : Object
    {
        string key = $"{typeof(T)}.{path}";

        if (resources.TryGetValue(key, out Object cachedResource))
            return cachedResource as T;

        T resource = Resources.Load<T>(path);
        resources.Add(key, resource);
        return resource;
    }

    /// <summary>
    /// Instantiate Object From original Object
    /// </summary>
    public T Instantiate<T>(T original, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool pooling = false) where T : Object
    {
        if (pooling)
            return GameManager.Pool.Get(original, position, rotation, parent);
        else
            return Object.Instantiate(original, position, rotation, parent);
    }

    /// <summary>
    /// Instantiate Object Form path(Resources path: string)
    /// </summary>
    public T Instantiate<T>(string path, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool pooling = false) where T : Object
    {
        T original = Load<T>(path);
        return Instantiate<T>(original, position, rotation, parent, pooling);
    }

    public void Destroy(GameObject go, float delay = 0f)
    {
        if (delay <= 0f)
        {
            if (!GameManager.Pool.Release(go))
            {
                GameObject.Destroy(go);
            }
        }
        else
        {
            StartCoroutine(DelayedDestroy(go, delay));
        }
    }

    private IEnumerator DelayedDestroy(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(go);
    }

    public void Destroy(Component component, float delay = 0f)
    {
        Component.Destroy(component, delay);
    }
}