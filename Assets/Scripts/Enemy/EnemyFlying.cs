using UnityEngine;

public class EnemyFlying : EnemyBase 
{
    protected override void Move()
    {
        // Terbang mengejar pemain dari segala arah (sumbu X dan Y)
        Vector2 direction = (player.position - transform.position).normalized;
        
        // Membalikkan arah hadap sprite musuh
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        }

        // Menggerakkan posisi musuh melayang ke arah pemain
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
}