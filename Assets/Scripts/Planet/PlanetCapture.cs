using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlanetCapture : MonoBehaviour
{
    private CircleCollider2D _capture; // slightly smaller than gravity field

    private void Awake()
    {
        _capture = GetComponent<CircleCollider2D>();
        _capture.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player && !player.GetIsOrbiting())
        {
            player.Capture(transform);
        }
    }
}
