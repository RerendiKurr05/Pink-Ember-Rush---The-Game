using UnityEngine;

public class Powerup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerPaintManager paintManager = collision.GetComponent<PlayerPaintManager>();
            
            if (paintManager != null)
            {

                if (paintManager.currentPaint >= paintManager.maxPaint)
                {

                    paintManager.ConsumePaint();
                    paintManager.ActivateAttackMode();
                    
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Cat belum penuh! Kumpulkan cat pink lagi!");
                }
            }
        }
    }
}