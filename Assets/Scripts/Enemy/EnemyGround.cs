using UnityEngine;

public class EnemyGround : EnemyBase
{
    protected override void Move()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;

        // Ground enemy hanya bergerak secara horizontal
        direction.y = 0;

        transform.Translate(direction * moveSpeed * Time.deltaTime);

        // Membalik arah sprite
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(direction.x),
                1,
                1
            );
        }
    }
}