using UnityEngine;

namespace MotorcycleSimulator.Runtime.Settings
{
    /// <summary>
    /// Shared parameters for the first straight-road training scenario.
    /// Store the asset at Resources/TrainingScenarioSettings so the scenario
    /// controller can load it without scene-object references.
    /// </summary>
    [CreateAssetMenu(
        fileName = "TrainingScenarioSettings",
        menuName = "Motorcycle Simulator/Training Scenario Settings")]
    public sealed class TrainingScenarioSettings : ScriptableObject
    {
        [Header("Episode start")]
        [SerializeField] private Vector3 spawnPosition = new(0f, 1f, 0f);
        [SerializeField, Range(-180f, 180f)] private float spawnHeadingDegrees;

        [Header("Planar episode boundary")]
        [SerializeField] private Vector2 minimumPosition = new(-20f, -20f);
        [SerializeField] private Vector2 maximumPosition = new(20f, 20f);

        [Header("Terminal reward")]
        [SerializeField] private float outOfBoundsPenalty = -1f;

        [Header("Episode limit")]
        [SerializeField, Min(1)] private int maximumEpisodeSteps = 1000;

        [Header("Dense reward and observations")]
        [SerializeField, Min(0f)] private float progressRewardPerMeter = 0.1f;
        [SerializeField, Min(0.01f)] private float goalObservationDistanceScale = 25f;

        public Vector3 SpawnPosition => spawnPosition;
        public float SpawnHeadingDegrees => spawnHeadingDegrees;
        public Vector2 MinimumPosition => minimumPosition;
        public Vector2 MaximumPosition => maximumPosition;
        public float OutOfBoundsPenalty => outOfBoundsPenalty;
        public int MaximumEpisodeSteps => maximumEpisodeSteps;
        public float ProgressRewardPerMeter => progressRewardPerMeter;
        public float GoalObservationDistanceScale => goalObservationDistanceScale;

        private void OnValidate()
        {
            maximumPosition = Vector2.Max(maximumPosition, minimumPosition + Vector2.one * 0.01f);
        }
    }
}
