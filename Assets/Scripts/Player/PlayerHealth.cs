using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public GameManager gameManager; 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Pemain Kalah!");
        
        if (gameManager != null)
        {
            gameManager.GameOver();
        }

        gameObject.SetActive(false); 
    }
}