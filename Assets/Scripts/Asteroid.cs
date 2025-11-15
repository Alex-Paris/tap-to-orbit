using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Asteroid : MonoBehaviour
{
    private float _rotSpeed = 90f;

    private void Update(){ transform.Rotate(0f, 0f, _rotSpeed * Time.deltaTime); }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var p = other.GetComponent<Player>();
        if (p) p.Kill();
    }
}
