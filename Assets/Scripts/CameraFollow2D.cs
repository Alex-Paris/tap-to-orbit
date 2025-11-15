using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private float _smooth = 6f;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 6f, -10f);

    private void LateUpdate()
    {
        if (!_player) return;
        
        if (!_player.GetIsOrbiting()) return;

        Transform targetTransform = _player.GetCurrentPlanet().transform;
        // targetTransform = !_player.GetIsOrbiting() ? _player.transform : _player.GetCurrentPlanet().transform;
        
        Vector3  desired = new Vector3(targetTransform.position.x, targetTransform.position.y, 0f) + _offset;
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * _smooth);
    }
}
