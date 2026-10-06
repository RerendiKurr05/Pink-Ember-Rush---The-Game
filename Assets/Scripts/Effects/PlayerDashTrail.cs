using UnityEngine;

public class PlayerDashTrail : MonoBehaviour
{
    private Controller.PlayerController player;
    private TrailRenderer trail;

    private void Start()
    {
        player = GetComponent<Controller.PlayerController>();
        trail = GetComponent<TrailRenderer>();

        trail.emitting = false;
    }

    private void Update()
    {
        trail.emitting = player.IsDashing;
    }
}