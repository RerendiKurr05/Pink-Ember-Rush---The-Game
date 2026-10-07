using UnityEngine;
using UnityEngine.UI;

public class PlayerPaintManager : MonoBehaviour
{
    [Header("Paint Settings")]
    public float maxPaint = 100f;
    public float currentPaint = 0f;

    [Header("UI References")]
    public Image bucketFillImage;

    [Header("Powerup Reference")]
    public GameObject powerupObject;

    [Header("Attack Mode Settings")]
    public bool isAttackModeActive = false;
    public float attackDuration = 10f;
    private float attackTimer;

    [Header("Power Up Visual")]
    public Animator playerAnimatorComponent; // drag komponen Animator Player ke sini
    public RuntimeAnimatorController normalController;
    public RuntimeAnimatorController powerUpController;
    private bool wasAttackModeActive = false;

    public Animator playerAnimator;
    public Animator bucketAnimator;

    private SpriteRenderer spriteRenderer;

    // void Start()
    // {
    //     currentPaint = 0f;
    //     UpdateUI();
    //     spriteRenderer = GetComponent<SpriteRenderer>();
    // }
    void Start()
    {
        currentPaint = 0f;
        UpdateUI();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (isAttackModeActive)
        {
            attackTimer = attackDuration;
        }
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
        
        HandlePowerUpVisual();
    }

    void HandlePowerUpVisual()
    {
        if (isAttackModeActive != wasAttackModeActive)
        {
            if (playerAnimatorComponent != null)
            {
                playerAnimatorComponent.runtimeAnimatorController = isAttackModeActive ? powerUpController : normalController;
            }
            wasAttackModeActive = isAttackModeActive;
        }
    }

    public void AddPaint(float amount)
    {
        if (isAttackModeActive) return;

        if (currentPaint < maxPaint)
        {
            currentPaint += amount;
            currentPaint = Mathf.Clamp(currentPaint, 0f, maxPaint);

            UpdateUI();

            if (currentPaint >= maxPaint)
            {
                bucketAnimator.SetBool("isFull", true);

                if (powerupObject != null)
                {
                    powerupObject.SetActive(true);
                }
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

        ConsumePaint();

        if (playerAnimator != null)
            playerAnimator.SetBool("isArmed", true);

        if (GameJuiceManager.instance != null)
            GameJuiceManager.instance.SetPowerupState(true);

        if (bucketAnimator != null)
            bucketAnimator.SetBool("isFull", false);
    }

    private void DeactivateAttackMode()
    {
        isAttackModeActive = false;
        playerAnimator.SetBool("isArmed", false);

        if (powerupObject != null)
        {
            powerupObject.SetActive(false);
        }
        
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