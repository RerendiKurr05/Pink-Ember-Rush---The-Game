using UnityEngine;

public class Powerup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayerPaintManager paintManager = collision.GetComponent<PlayerPaintManager>();

        if (paintManager == null)
            return;

        if (paintManager.isAttackModeActive)
            return;

        if (paintManager.currentPaint >= paintManager.maxPaint)
        {
            paintManager.ConsumePaint();
            paintManager.ActivateAttackMode();

            gameObject.SetActive(false);
        }
    }
}