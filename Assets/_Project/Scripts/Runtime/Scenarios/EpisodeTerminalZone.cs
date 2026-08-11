using MotorcycleSimulator.Runtime.Agents;
using UnityEngine;

namespace MotorcycleSimulator.Runtime.Scenarios
{
    public enum EpisodeTerminalType
    {
        Collision,
        Goal,
    }

    /// <summary>
    /// Marks a trigger volume as an episode terminal. Attach it to an obstacle
    /// or goal collider; it finds the scenario controller automatically.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public sealed class EpisodeTerminalZone : MonoBehaviour
    {
        [SerializeField] private EpisodeTerminalType terminalType = EpisodeTerminalType.Collision;

        [SerializeField, Tooltip("Negative for collisions; positive for goals.")]
        private float terminalReward = -1f;

        public EpisodeTerminalType TerminalType => terminalType;

        private MotorcycleEpisodeController episodeController;

        private void Awake()
        {
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        private void Start()
        {
            episodeController = FindFirstObjectByType<MotorcycleEpisodeController>();
            if (episodeController == null)
            {
                throw new MissingReferenceException(
                    $"No {nameof(MotorcycleEpisodeController)} exists in this scene.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var agent = other.GetComponent<MotorcycleRlAgent>();
            if (agent != null)
            {
                episodeController.CompleteEpisode(terminalReward);
            }
        }
    }
}
