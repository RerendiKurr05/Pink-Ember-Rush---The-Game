using UnityEngine;

public class AutoDestroyAnimation : MonoBehaviour
{
    public float lifeTime = 1f; // sesuaikan durasi, kira2 selama animasi 8 frame itu muter

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}