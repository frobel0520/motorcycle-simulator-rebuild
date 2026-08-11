using MotorcycleSimulator.Runtime.Vehicles;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MotorcycleSimulator.Runtime.Input
{
    /// <summary>
    /// Temporary manual controller for validating the kinematic motorcycle.
    /// Attach only to a scene instance, never to the reusable production prefab.
    /// W/S controls acceleration and braking; A/D controls steering.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(KinematicMotorcycleBody))]
    public sealed class KeyboardMotorcycleController : MonoBehaviour
    {
        private KinematicMotorcycleBody motorcycleBody;

        private void Awake()
        {
            motorcycleBody = GetComponent<KinematicMotorcycleBody>();
        }

        private void FixedUpdate()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                motorcycleBody.ApplyControl(0f, 0f);
                return;
            }

            var acceleration = (keyboard.wKey.isPressed ? 1f : 0f)
                - (keyboard.sKey.isPressed ? 1f : 0f);
            var steering = (keyboard.dKey.isPressed ? 1f : 0f)
                - (keyboard.aKey.isPressed ? 1f : 0f);

            motorcycleBody.ApplyControl(acceleration, steering);
        }
    }
}
