using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [Header("Curves")]
    [Tooltip("Orbit angular speed added over time")] [SerializeField]
    private AnimationCurve _orbitSpeedOverTime = AnimationCurve.Linear(0, 0, 60, 60);

    [Tooltip("Planet spacing decreases over time")] [SerializeField]
    private AnimationCurve _spacingOverTime = AnimationCurve.Linear(0, 8, 60, 16);

    [Tooltip("Chance of obstacle spawning over time (0..1)")] [SerializeField]
    private AnimationCurve _obstacleChanceOverTime = AnimationCurve.Linear(0, 0.1f, 60, 0.5f);

    private float _elapsed;

    public void ResetDifficulty(){ _elapsed = 0f; }

    private void Update(){ if (GameManager.Instance.IsRunning) _elapsed += Time.deltaTime; }

    public float GetOrbitSpeedBonus() => _orbitSpeedOverTime.Evaluate(_elapsed);
    public float GetSpacing() => _spacingOverTime.Evaluate(_elapsed);
    public float GetObstacleChance() => _obstacleChanceOverTime.Evaluate(_elapsed);
}
