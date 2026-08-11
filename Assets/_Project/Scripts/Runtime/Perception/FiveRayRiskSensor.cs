using MotorcycleSimulator.Core.Risk;
using MotorcycleSimulator.Runtime.Scenarios;
using MotorcycleSimulator.Runtime.Settings;
using UnityEngine;

namespace MotorcycleSimulator.Runtime.Perception
{
    /// <summary>
    /// Unity adapter for the five-direction risk map. Attach it to a vehicle
    /// with a Rigidbody; no controller or scene-object reference is required.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class FiveRayRiskSensor : MonoBehaviour
    {
        private static readonly float[] RelativeAngles = { -60f, -30f, 0f, 30f, 60f };

        private const string SettingsResourcePath = "RayRiskSensorSettings";

        private Rigidbody body;
        private Collider[] ownColliders;
        private RayRiskSensorSettings settings;

        public RayRiskMap CurrentRiskMap { get; private set; }

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            ownColliders = GetComponentsInChildren<Collider>();
            settings = Resources.Load<RayRiskSensorSettings>(SettingsResourcePath);
            if (settings == null)
            {
                throw new MissingReferenceException(
                    $"Create {nameof(RayRiskSensorSettings)} at Resources/{SettingsResourcePath}.asset.");
            }

            CurrentRiskMap = Sample();
        }

        private void FixedUpdate()
        {
            CurrentRiskMap = Sample();
        }

        public RayRiskMap Sample()
        {
            var observations = new RayObservation[RelativeAngles.Length];
            var egoSpeedAlongHeading = Vector3.Dot(body.linearVelocity, transform.forward);

            for (var index = 0; index < RelativeAngles.Length; index++)
            {
                var direction = Quaternion.AngleAxis(RelativeAngles[index], Vector3.up) * transform.forward;
                var hits = Physics.SphereCastAll(
                    transform.position,
                    settings.SphereRadius,
                    direction,
                    settings.MaximumDistance,
                    settings.ObstacleLayers,
                    QueryTriggerInteraction.Collide);

                var hasHit = false;
                var closestDistance = float.PositiveInfinity;
                var closestHit = new RaycastHit();
                foreach (var hit in hits)
                {
                    if (IsOwnCollider(hit.collider))
                    {
                        continue;
                    }

                    var terminalZone = hit.collider.GetComponent<EpisodeTerminalZone>();
                    if (terminalZone != null && terminalZone.TerminalType == EpisodeTerminalType.Goal)
                    {
                        continue;
                    }

                    if (hit.distance < closestDistance)
                    {
                        hasHit = true;
                        closestDistance = hit.distance;
                        closestHit = hit;
                    }
                }

                if (!hasHit)
                {
                    observations[index] = new RayObservation(false, settings.MaximumDistance, 0f);
                    Debug.DrawRay(transform.position, direction * settings.MaximumDistance, Color.green);
                    continue;
                }

                var detectedVelocity = closestHit.rigidbody == null
                    ? Vector3.zero
                    : closestHit.rigidbody.linearVelocity;
                var relativeSpeed = Vector3.Dot(detectedVelocity, transform.forward) - egoSpeedAlongHeading;
                observations[index] = new RayObservation(true, closestHit.distance, relativeSpeed);

                Debug.DrawRay(transform.position, direction * closestHit.distance, Color.red);
            }

            return RayRiskMap.Evaluate(
                settings.CreateRiskSettings(),
                observations);
        }

        private bool IsOwnCollider(Collider collider)
        {
            foreach (var ownCollider in ownColliders)
            {
                if (collider == ownCollider)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
