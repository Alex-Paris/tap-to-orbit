using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlanetGravityField : MonoBehaviour
{
    [Header("Gravity Model")]
    [SerializeField] private float _G = 50f; // game constant
    [SerializeField] private float _planetMass = 1000f;
    [SerializeField] private float _minRadius = 0.5f;
    [SerializeField] private float _maxAccel = 40f; // clamp

    private CircleCollider2D _trigger;
    private ParticleSystem _gravityParticles;

    private void Awake()
    {
        _gravityParticles = GetComponent<ParticleSystem>();
        _trigger = GetComponent<CircleCollider2D>();
        _trigger.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_gravityParticles) _gravityParticles.Play();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var rb = other.attachedRigidbody; if (!rb) return;
        if (!GameManager.Instance.IsRunning) return;
        
        Vector2 toCenter = (Vector2)transform.position - rb.worldCenterOfMass;
        float dist = Mathf.Max(toCenter.magnitude, _minRadius);
        float accelMag = Mathf.Min((_G * _planetMass) / (dist * dist), _maxAccel);
        Vector2 accel = toCenter.normalized * accelMag;
        rb.AddForce(accel * rb.mass, ForceMode2D.Force);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_gravityParticles) _gravityParticles.Stop();
    }
}
