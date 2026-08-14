using System.Collections;
using UnityEngine;

public class HeartEffect : MonoBehaviour
{
    [Header("Movement Settings")]
    public float popForceY = 5f;
    public float popForceX = 2f;
    
    [Header("Lifetime Settings")]
    public float lifetime = 6f; 
    public float blinkDuration = 2f;

    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        _spriteRenderer = GetComponent<SpriteRenderer>(); 
        
        if (rb != null)
        {
            float randomX = Random.Range(-popForceX, popForceX);
            rb.velocity = new Vector2(randomX, popForceY);
        }

        StartCoroutine(LifeTimeRoutine()); 
    }

    private IEnumerator LifeTimeRoutine()
    {
        yield return new WaitForSeconds(lifetime - blinkDuration);

        float timer = 0f;
        while (timer < blinkDuration)
        {
            _spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            
            _spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
            
            timer += 0.2f;
        }

        Destroy(gameObject);
    }
}