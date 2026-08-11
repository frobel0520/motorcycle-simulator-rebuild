using MotorcycleSimulator.Core.Control;
using MotorcycleSimulator.Runtime.Settings;
using UnityEngine;

namespace MotorcycleSimulator.Runtime.Vehicles
{
    /// <summary>
    /// Unity adapter that applies a pure kinematic-bicycle state to a Rigidbody.
    /// ML-Agents and other controllers interact through ApplyControl only.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class KinematicMotorcycleBody : MonoBehaviour
    {
        private const string SettingsResourcePath = "MotorcycleKinematicSettings";

        private Rigidbody body;
        private MotorcycleKinematicSettings settings;
        private KinematicBicycleParameters parameters;
        private KinematicBicycleState state;
        private KinematicBicycleControl control;

        public KinematicBicycleState State => state;
        public float NormalizedSpeed => settings == null || settings.MaximumSpeed <= 0f
            ? 0f
            : state.Speed / settings.MaximumSpeed;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            settings = Resources.Load<MotorcycleKinematicSettings>(SettingsResourcePath);
            if (settings == null)
            {
                throw new MissingReferenceException(
                    $"Create {nameof(MotorcycleKinematicSettings)} at Resources/{SettingsResourcePath}.asset.");
            }

            parameters = settings.CreateParameters();
            ResetState(transform.position, transform.eulerAngles.y, 0f);
        }

        private void FixedUpdate()
        {
            state = KinematicBicycleModel.Step(parameters, state, control, Time.fixedDeltaTime);

            var position = new Vector3(state.X, transform.position.y, state.Z);
            var rotation = Quaternion.Euler(0f, state.HeadingRadians * Mathf.Rad2Deg, 0f);
            body.MovePosition(position);
            body.MoveRotation(rotation);
        }

        public void ApplyControl(float normalizedAcceleration, float normalizedSteering)
        {
            control = new KinematicBicycleControl(normalizedAcceleration, normalizedSteering);
        }

        public void ResetState(Vector3 position, float headingDegrees, float speed)
        {
            state = new KinematicBicycleState(
                position.x,
                position.z,
                headingDegrees * Mathf.Deg2Rad,
                Mathf.Clamp(speed, 0f, settings.MaximumSpeed));
            control = new KinematicBicycleControl(0f, 0f);
            body.position = position;
            body.rotation = Quaternion.Euler(0f, headingDegrees, 0f);
        }
    }
}
