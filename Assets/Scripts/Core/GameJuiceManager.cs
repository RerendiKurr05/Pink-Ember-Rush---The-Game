using UnityEngine;
using System.Collections;
using Cinemachine;

public class GameJuiceManager : MonoBehaviour
{
    public static GameJuiceManager instance;

    [Header("Cinemachine References")]
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachineNoise;

    [Header("Shake Settings")]
    public float defaultShakeIntensity = 3f;
    public float defaultShakeFrequency = 2f;

    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;

    private bool isHitStopping = false;

    void Awake()
    {
        // Setup Singleton
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (virtualCamera != null)
        {
            cinemachineNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.unscaledDeltaTime;

            if (shakeTimer <= 0f)
            {
                cinemachineNoise.m_AmplitudeGain = 0f;
                cinemachineNoise.m_FrequencyGain = 0f;
            }
            else
            {

                cinemachineNoise.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
            }
        }
    }

    public void TriggerSlowMotion(float slowScale, float duration)
    {
        StartCoroutine(SlowMotionRoutine(slowScale, duration));
    }

    private System.Collections.IEnumerator SlowMotionRoutine(float slowScale, float duration)
    {
        // Pelambatan waktu
        Time.timeScale = slowScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(duration);

        // Kembalikan ke normal
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
    public void ShakeCamera(float intensity, float time)
    {
        if (cinemachineNoise == null) return;

        cinemachineNoise.m_AmplitudeGain = intensity;
        cinemachineNoise.m_FrequencyGain = defaultShakeFrequency;

        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }
    public void HitStop(float duration)
    {
        if (isHitStopping) return;

        StartCoroutine(HitStopRoutine(duration));
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        isHitStopping = true;

        Time.timeScale = 0.05f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        isHitStopping = false;
    }

    [Header("Powerup Visuals")]
    public AudioSource bgmNormal;
    public AudioSource bgmTense;
    public ParticleSystem speedLines;

    public void SetPowerupState(bool isActive)
    {
        if (isActive)
        {
            bgmNormal.Pause();
            bgmTense.Play();
            speedLines.Play();
        }
        else
        {
            bgmTense.Stop();
            bgmNormal.Play();
            speedLines.Stop();
        }
    }

}