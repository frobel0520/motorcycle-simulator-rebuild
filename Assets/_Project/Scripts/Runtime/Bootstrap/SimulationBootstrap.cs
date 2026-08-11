using UnityEngine;

namespace MotorcycleSimulator.Runtime.Bootstrap
{
    /// <summary>
    /// Root entry point for a simulation scene.
    /// Scene systems are discovered from the fixed hierarchy below this object,
    /// rather than assigned manually through the Inspector.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    [DisallowMultipleComponent]
    public sealed class SimulationBootstrap : MonoBehaviour
    {
        private const string SystemsRootName = "Systems";

        public Transform SystemsRoot { get; private set; }

        private void Awake()
        {
            SystemsRoot = transform.Find(SystemsRootName);

            if (SystemsRoot == null)
            {
                throw new MissingReferenceException(
                    $"{nameof(SimulationBootstrap)} requires a child named '{SystemsRootName}'.");
            }
        }
    }
}
