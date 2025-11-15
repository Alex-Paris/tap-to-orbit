using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private Transform _player;

    [Header("Layout")]
    [SerializeField] private float _startDistance = 18f;
    [SerializeField] private float _gapBuffer = 25f; // how far ahead to maintain planets
    [SerializeField] private float _lateralRange = 5f;
    
    [Header("Pooling")]
    [SerializeField] private GameObject[] _prefabs;
    [SerializeField] private int _preload = 10;

    private Queue<GameObject> _poolPlanets = new Queue<GameObject>();
    private Queue<GameObject> _activePlanets = new Queue<GameObject>();
    private float _nextY;
    private float _lastX;
    private float _nextCleanupTime;
    private const float _cleanupInterval = 0.25f;
    
    private void Awake()
    {
        if (_prefabs == null || _prefabs.Length == 0)
        {
            Debug.LogError("SpawnerManager: Nenhum prefab atribuído em _prefabs!");
            return;
        }
        
        for (int i = 0; i < _preload; i++)
        {
            var go = CreatePlanetInstance();
            go.SetActive(false);
            _poolPlanets.Enqueue(go);
        }
    }

    private void Start()
    {
        _lastX = 0f;
        _nextY = _player.position.y + _startDistance;
        _nextCleanupTime = Time.time + _cleanupInterval;
        FillForward();
    }

    private void Update()
    {
        FillForward();
        
        if (Time.time >= _nextCleanupTime)
        {
            CleanupBehind();
            _nextCleanupTime = Time.time + _cleanupInterval;
        }
    }
    
    private GameObject CreatePlanetInstance()
    {
        int index = Random.Range(0, _prefabs.Length);
        GameObject prefab = _prefabs[index];
        return Instantiate(prefab, transform);
    }

    private void FillForward()
    {
        float spacing = GameManager.Instance?.Difficulty.GetSpacing() ?? 3f;

        while (_nextY < _player.position.y + _gapBuffer)
        {
            _lastX = Random.Range(_lastX - _lateralRange, _lastX + _lateralRange);
            Spawn(new Vector3(_lastX, _nextY, 0f), Quaternion.identity);
            _nextY += spacing;
        }
    }
    
    private void CleanupBehind()
    {
        float cutoffY = _player.position.y - _gapBuffer;
        
        while (_activePlanets.Count > 0)
        {
            GameObject first = _activePlanets.Peek();
            if (first == null)
            {
                _activePlanets.Dequeue();
                continue;
            }

            float y = first.transform.position.y;
            if (y < cutoffY)
            {
                Despawn(first);
                _activePlanets.Dequeue();
            }
            else
                break; // if no cutOff, obviously the next on queue will be inside the buffer
        }
    }

    // (Opcional) Se tiver reset de fase/morte, chame isso
    public void ResetSpawner()
    {
        // Despawn de todos que sobraram
        while (_poolPlanets.Count > 0)
        {
            var go = _poolPlanets.Dequeue();
            if (go != null) Despawn(go);
        }

        _nextY = _player.position.y + _startDistance;
        FillForward();
        _nextCleanupTime = Time.time + _cleanupInterval;
    }
    
    private void Spawn(Vector3 pos, Quaternion rot)
    {
        var go = _poolPlanets.Count > 0 ? _poolPlanets.Dequeue() : CreatePlanetInstance();
        go.transform.SetPositionAndRotation(pos, rot);
        go.SetActive(true);
        _activePlanets.Enqueue(go);
    }

    private void Despawn(GameObject go)
    {
        go.SetActive(false);
        _poolPlanets.Enqueue(go);
    }
}
