using UnityEngine;
using System.Collections;

public class PlayerAfterImage : MonoBehaviour
{
    public float spawnInterval = 0.05f;
    public float afterImageDuration = 0.2f;

    private float timer;
    private Controller.PlayerController player;
    private SpriteRenderer playerSprite;

    private void Start()
    {
        player = GetComponent<Controller.PlayerController>();
        playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (player.IsDashing)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                CreateAfterImage();
                timer = spawnInterval;
            }
        }
        else
        {
            timer = 0f;
        }
    }

    private void CreateAfterImage()
    {
        GameObject ghost = new GameObject("DashAfterImage");

        ghost.transform.position = playerSprite.transform.position;
        ghost.transform.rotation = playerSprite.transform.rotation;
        ghost.transform.localScale = playerSprite.transform.lossyScale;

        SpriteRenderer ghostSprite = ghost.AddComponent<SpriteRenderer>();

        ghostSprite.sprite = playerSprite.sprite;
        ghostSprite.flipX = playerSprite.flipX;
        ghostSprite.sortingLayerID = playerSprite.sortingLayerID;
        ghostSprite.sortingOrder = playerSprite.sortingOrder - 1;

        StartCoroutine(FadeOut(ghostSprite, ghost));
    }

    private IEnumerator FadeOut(SpriteRenderer ghostSprite, GameObject ghost)
    {
        float timer = 0f;
        Color originalColor = ghostSprite.color;

        while (timer < afterImageDuration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, timer / afterImageDuration);

            ghostSprite.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                alpha
            );

            yield return null;
        }

        Destroy(ghost);
    }
}