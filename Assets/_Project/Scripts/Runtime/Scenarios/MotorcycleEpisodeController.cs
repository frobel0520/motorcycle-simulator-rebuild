using MotorcycleSimulator.Runtime.Agents;
using MotorcycleSimulator.Runtime.Settings;
using MotorcycleSimulator.Runtime.Vehicles;
using UnityEngine;

namespace MotorcycleSimulator.Runtime.Scenarios
{
    /// <summary>
    /// Owns one straight-road episode's terminal condition and reset. It finds
    /// the single motorcycle agent at startup, so no Inspector object dragging
    /// is needed. Future traffic and reward rules belong here, not in Agent.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MotorcycleEpisodeController : MonoBehaviour
    {
        private const string SettingsResourcePath = "TrainingScenarioSettings";

        private MotorcycleRlAgent agent;
        private KinematicMotorcycleBody motorcycleBody;
        private TrainingScenarioSettings settings;
        private EpisodeTerminalZone goalZone;
        private float previousGoalDistance;
        private int lastTerminalFrame = -1;

        public bool HasGoal => goalZone != null;
        public Vector3 GoalPosition => goalZone == null ? Vector3.zero : goalZone.transform.position;
        public int MaximumEpisodeSteps => settings == null ? 1000 : settings.MaximumEpisodeSteps;

        private void Start()
        {
            settings = Resources.Load<TrainingScenarioSettings>(SettingsResourcePath);
            if (settings == null)
            {
                throw new MissingReferenceException(
                    $"Create {nameof(TrainingScenarioSettings)} at Resources/{SettingsResourcePath}.asset.");
            }

            agent = FindFirstObjectByType<MotorcycleRlAgent>();
            if (agent == null)
            {
                throw new MissingReferenceException($"No {nameof(MotorcycleRlAgent)} exists in this scene.");
            }

            motorcycleBody = agent.GetComponent<KinematicMotorcycleBody>();
            var terminalZones = FindObjectsByType<EpisodeTerminalZone>(FindObjectsSortMode.None);
            foreach (var terminalZone in terminalZones)
            {
                if (terminalZone.TerminalType == EpisodeTerminalType.Goal)
                {
                    goalZone = terminalZone;
                    break;
                }
            }

            ResetMotorcycle();
        }

        private void FixedUpdate()
        {
            if (motorcycleBody == null || settings == null)
            {
                return;
            }

            if (HasGoal)
            {
                var currentGoalDistance = PlanarDistance(motorcycleBody.transform.position, GoalPosition);
                var progress = previousGoalDistance - currentGoalDistance;
                agent.AddReward(progress * settings.ProgressRewardPerMeter);
                previousGoalDistance = currentGoalDistance;
            }

            if (IsOutsideEpisodeBoundary(motorcycleBody.transform.position))
            {
                CompleteEpisode(settings.OutOfBoundsPenalty);
            }
        }

        /// <summary>Called by terminal zones such as collision obstacles and goals.</summary>
        public void CompleteEpisode(float terminalReward)
        {
            if (lastTerminalFrame == Time.frameCount)
            {
                return;
            }

            lastTerminalFrame = Time.frameCount;
            agent.AddReward(terminalReward);
            agent.EndEpisode();
        }

        /// <summary>Resets only scenario-owned state at the beginning of an episode.</summary>
        public void ResetEpisode()
        {
            if (motorcycleBody != null && settings != null)
            {
                ResetMotorcycle();
            }
        }

        private bool IsOutsideEpisodeBoundary(Vector3 position)
        {
            return position.x < settings.MinimumPosition.x
                || position.z < settings.MinimumPosition.y
                || position.x > settings.MaximumPosition.x
                || position.z > settings.MaximumPosition.y;
        }

        private void ResetMotorcycle()
        {
            motorcycleBody.ResetState(
                settings.SpawnPosition,
                settings.SpawnHeadingDegrees,
                speed: 0f);
            previousGoalDistance = HasGoal
                ? PlanarDistance(settings.SpawnPosition, GoalPosition)
                : 0f;
        }

        public Vector2 GetNormalizedLocalGoalOffset(Transform actor)
        {
            if (!HasGoal)
            {
                return Vector2.zero;
            }

            var localGoal = actor.InverseTransformPoint(GoalPosition);
            var scale = settings.GoalObservationDistanceScale;
            return new Vector2(
                Mathf.Clamp(localGoal.x / scale, -1f, 1f),
                Mathf.Clamp(localGoal.z / scale, -1f, 1f));
        }

        private static float PlanarDistance(Vector3 first, Vector3 second)
        {
            var offset = first - second;
            offset.y = 0f;
            return offset.magnitude;
        }
    }
}
