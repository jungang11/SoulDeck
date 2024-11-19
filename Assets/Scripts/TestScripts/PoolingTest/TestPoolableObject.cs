using UnityEngine;

public class TestPoolableObject : MonoBehaviour, IPoolable
{
    public float speed = 1f;
    public float lifeTime = 5f; // 5초 후 자동 반환

    public void OnSpawn()
    {
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    public void OnDespawn()
    {
        CancelInvoke(nameof(ReturnToPool));
    }

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.forward);
    }

    private void ReturnToPool()
    {
        GameManager.Pool.Release(gameObject);
    }
}
