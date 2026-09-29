using UnityEngine;
using System.Collections;
using Cinemachine;

public class GameJuiceManager : MonoBehaviour
{
    public static GameJuiceManager instance;

    [Header("Cinemachine References")]
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachineNoise;

    [Header("Audio Filter Reference")]
    public AudioLowPassFilter audioLowPassFilter;
    public float normalCutoff = 22000f;
    public float muffleCutoff = 800f;

    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;
    
    private bool isHitStopping = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (virtualCamera != null)
        {
            cinemachineNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        if (audioLowPassFilter != null)
        {
            audioLowPassFilter.cutoffFrequency = normalCutoff;
            audioLowPassFilter.enabled = false;
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

    public void ShakeCamera(float intensity, float time)
    {
        if (cinemachineNoise == null) return;
        cinemachineNoise.m_AmplitudeGain = intensity;
        cinemachineNoise.m_FrequencyGain = defaultFrequencyForShake(intensity);
        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }
    
    private float defaultFrequencyForShake(float intensity) => 2f;

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

    public void TriggerSlowMotion(float slowScale, float duration)
    {
        StartCoroutine(SlowMotionRoutine(slowScale, duration));
    }

    private IEnumerator SlowMotionRoutine(float slowScale, float duration)
    {
        Time.timeScale = slowScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (audioLowPassFilter != null)
        {
            audioLowPassFilter.enabled = true;
            audioLowPassFilter.cutoffFrequency = muffleCutoff;
        }

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        if (audioLowPassFilter != null)
        {
            audioLowPassFilter.cutoffFrequency = normalCutoff;
            audioLowPassFilter.enabled = false;
        }
    }
    [Header("Powerup Visuals")]
    public AudioSource bgmNormal;
    public AudioSource bgmTense;
    public ParticleSystem speedLines;

    // public void SetPowerupState(bool isActive)
    // {
    //     if (isActive)
    //     {
    //         bgmNormal.Pause();
    //         bgmTense.Play();
    //         speedLines.Play();
    //     }
    //     else
    //     {
    //         bgmTense.Stop();
    //         bgmNormal.Play();
    //         speedLines.Stop();
    //     }
    // }

    public void SetPowerupState(bool isActive)
    {
        if (isActive)
        {
            if (bgmNormal != null) bgmNormal.Pause();
            if (bgmTense != null) bgmTense.Play();
            if (speedLines != null) speedLines.Play();
        }
        else
        {
            if (bgmTense != null) bgmTense.Stop();
            if (bgmNormal != null) bgmNormal.Play();
            if (speedLines != null) speedLines.Stop();
        }
   }
}