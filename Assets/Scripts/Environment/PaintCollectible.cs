using UnityEngine;

public class PaintCollectible : MonoBehaviour
{
    public float paintValue = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerPaintManager paintManager = collision.GetComponent<PlayerPaintManager>();
            
            if (paintManager != null)
            {
                if (paintManager.currentPaint < paintManager.maxPaint)
                {
                    paintManager.AddPaint(paintValue);
                    Destroy(gameObject);
                }
            }
        }
    }
}