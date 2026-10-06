using UnityEngine;

public class PaintCollectible : MonoBehaviour
{
    public float paintValue = 10f;

    public static event System.Action OnPaintCollected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayerPaintManager paintManager = collision.GetComponent<PlayerPaintManager>();

        if (paintManager == null)
            return;

        if (paintManager.isAttackModeActive)
            return;

        if (paintManager.currentPaint < paintManager.maxPaint)
        {
            paintManager.AddPaint(paintValue);
            OnPaintCollected?.Invoke();
        }

        Destroy(gameObject);
    }
}