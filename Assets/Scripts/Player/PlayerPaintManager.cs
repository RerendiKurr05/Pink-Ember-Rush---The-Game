using UnityEngine;
using UnityEngine.UI;

public class PlayerPaintManager : MonoBehaviour
{
    [Header("Paint Settings")]
    public float maxPaint = 100f;
    public float currentPaint = 0f;

    [Header("UI References")]
    public Image bucketFillImage;

    [Header("Attack Mode Settings")]
    public bool isAttackModeActive = false;
    public float attackDuration = 10f;
    private float attackTimer;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentPaint = 0f;
        UpdateUI();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {

        if (isAttackModeActive)
        {
            attackTimer -= Time.deltaTime;
            
            spriteRenderer.color = Color.Lerp(Color.white, Color.magenta, Mathf.PingPong(Time.time * 5f, 1f));

            if (attackTimer <= 0)
            {
                DeactivateAttackMode();
            }
        }
    }
    public void AddPaint(float amount)
    {
        if (currentPaint < maxPaint)
        {
            currentPaint += amount;
            if (currentPaint > maxPaint) currentPaint = maxPaint;
            UpdateUI();
        }
    }
    public void ConsumePaint()
    {
        currentPaint = 0f;
        UpdateUI();
    }

    public void ActivateAttackMode()
    {
        isAttackModeActive = true;
        attackTimer = attackDuration;
        Debug.Log("Powerup Diambil! Mode Serang Aktif!");
    }

    private void DeactivateAttackMode()
    {
        isAttackModeActive = false;
        spriteRenderer.color = Color.white;
        Debug.Log("Waktu Powerup Habis.");
    }

    private void UpdateUI()
    {
        if (bucketFillImage != null)
        {
            bucketFillImage.fillAmount = currentPaint / maxPaint;
        }
    }
}