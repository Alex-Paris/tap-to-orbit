using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))] 
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [SerializeField] private float _baseOrbitAngularSpeed = 90f; // deg/s
    [SerializeField] private float _launchSpeed = 10f;

    // TODO: fix FX sysmtem
    [Header("FX")]
    [SerializeField] private ParticleSystem _launchBurst;
    [SerializeField] private ParticleSystem _launchStars;
    [SerializeField] private ParticleSystem _deathExplosion;
    [SerializeField] private TrailRenderer _trail;
    
    // Game Input
    private PlayerInput _input;
    private InputAction _tapAction; 
    
    // Misc
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Transform _currentPlanet; // set by capture
    private Vector2 _tangentialDir;
    private bool _isOrbiting = false;
    private float _orbitSign = +1f; // define the orbit side
    
    public bool GetIsOrbiting() => _isOrbiting;
    public Transform GetCurrentPlanet() => _currentPlanet;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _input = GetComponent<PlayerInput>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _tangentialDir = Vector2.up; // Apply movement upwards
    }

    private void Start()
    {
        _rb.gravityScale = 0f; // No Earth gravity
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        var asset = _input.ActionAsset;
        _tapAction = asset.FindAction("Player/Attack", throwIfNotFound: true);
        _tapAction.performed += OnTapAction_performed;
        _tapAction.Enable();
    }
    
    private void OnEnable()
    {
        if (_tapAction != null) _tapAction.Enable();
    }
    
    private void OnDisable()
    {
        if (_tapAction != null) _tapAction.Disable();
    }

    private void Update()
    {
        if (!GameManager.Instance.IsRunning) return;
        
        if (_isOrbiting && _currentPlanet)
        {
            float orbitSpeed = _baseOrbitAngularSpeed + (GameManager.Instance?.Difficulty?.GetOrbitSpeedBonus() ?? 0f);

            // Apply sign of orbit RotateAround
            transform.RotateAround(_currentPlanet.position, Vector3.forward, _orbitSign * orbitSpeed * Time.deltaTime);

            // Tangential related to the orbit
            Vector2 radial = (Vector2)(transform.position - _currentPlanet.position);
            Vector2 tangent = new Vector2(-radial.y, radial.x); // counterclockwise
            if (_orbitSign < 0f) tangent = -tangent;            // invert to clockwise

            _tangentialDir = tangent.normalized;

            // Rocket front = +Y local (adjust -90°); if front is +X, change to 0f
            float ang = Mathf.Atan2(_tangentialDir.y, _tangentialDir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, ang);
        }
    }

    private void OnDestroy()
    {
        _tapAction.performed -= OnTapAction_performed;
    }


    private void OnTapAction_performed(InputAction.CallbackContext obj)
    {
        if (!GameManager.Instance.IsRunning) return;

        if (!_isOrbiting) return;
        
        Launch();
    }

    private void LaunchBurstLifetime(float lifeTime)
    {
        var main = _launchBurst.main;
        var shape = _launchBurst.shape;
        main.startLifetime = lifeTime;
        shape.angle = 20f * lifeTime;
    }
    
    public void Launch()
    {
        _isOrbiting = false;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.linearVelocity = _tangentialDir.normalized * _launchSpeed;
        // _launchStars.Play();
        
        LaunchBurstLifetime(0.5f);
        // _trail.Clear();
    }

    public void Capture(Transform planet)
    {
        _currentPlanet = planet;
        _launchStars.Stop();

        // Calc sign before reduce velocity
        Vector2 r = (Vector2)(transform.position - _currentPlanet.position);
        Vector2 v = _rb.linearVelocity; // approach speed
        float crossZ = r.x * v.y - r.y * v.x;   // z of cross(r, v)
        if (Mathf.Abs(crossZ) > 1e-4f)
            _orbitSign = Mathf.Sign(crossZ);    // +1 counterclockwise, -1 clockwise
        // fallback: keeps last signal if crossZ ≈ 0

        _isOrbiting = true;
        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Kinematic;

        LaunchBurstLifetime(0.1f);

        GameManager.Instance.AddScore(1);
    }

    public void Kill()
    {
        GameManager.Instance.GameOver();
        _launchBurst.Stop();
        _deathExplosion.Play();
        _spriteRenderer.enabled = false;
        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Static;
    }
}
