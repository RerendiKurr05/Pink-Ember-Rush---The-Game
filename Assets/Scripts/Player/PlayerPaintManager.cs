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

    public Animator playerAnimator;
    public Animator bucketAnimator;

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
            UpdateUI();

            if (currentPaint >= maxPaint)
            {
                bucketAnimator.SetBool("isFull", true);
            }
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

        playerAnimator.SetBool("isArmed", true);

        GameJuiceManager.instance.SetPowerupState(true);

        bucketAnimator.SetBool("isFull", false);
    }

    private void DeactivateAttackMode()
    {
        isAttackModeActive = false;
        playerAnimator.SetBool("isArmed", false);
        GameJuiceManager.instance.SetPowerupState(false);
    }

    private void UpdateUI()
    {
        if (bucketFillImage != null)
        {
            bucketFillImage.fillAmount = currentPaint / maxPaint;
        }



    }

}