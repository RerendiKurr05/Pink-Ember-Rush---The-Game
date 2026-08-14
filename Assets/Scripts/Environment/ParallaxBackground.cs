using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Referensi Kamera")]
    public GameObject cam;

    [Header("Pengaturan Kecepatan")]
    [Tooltip("0 = Diam (Langit), 1 = Bergerak secepat pemain (Foreground)")]
    public float parallaxEffectMultiplier;

    private float length, startpos;

    void Start()
    {
        if (cam == null) cam = Camera.main.gameObject;

        startpos = transform.position.x;
        
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {

        float temp = (cam.transform.position.x * (1 - parallaxEffectMultiplier));
        float dist = (cam.transform.position.x * parallaxEffectMultiplier);

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        if (temp > startpos + length)
        {
            startpos += length;
        }
        else if (temp < startpos - length)
        {
            startpos -= length;
        }
    }
}