using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private float bobAmplitude = 0.15f;
    [SerializeField] private float bobSpeed = 2f;
    
    private Vector3 basePos;

    private void Start(){ basePos = transform.position; }

    private void Update()
    {
        transform.position = basePos + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobAmplitude);
        transform.Rotate(0f, 0f, 50f * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        // Hook to your economy; for now we add score
        GameManager.Instance.AddScore(value);
        Destroy(gameObject);
    }
}
