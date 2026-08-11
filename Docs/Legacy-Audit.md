# Legacy project audit - first pass

## Scope and method

This is a static audit of the former Unity 2021 project. It identifies code
referenced by `Assets/Scenes/MainScene.unity`, duplicated implementations, and
responsibilities that should be separated before migration. It is **not** a
deletion list: a Unity 2021 editor validation will still be required before any
legacy asset is removed.

## Findings

- The old project contains 113 project scripts and multiple third-party asset
  packages and demo scenes.
- `MainScene` references both the thesis-era `Assets/Scripts/ytwei` system and
  the later `Assets/Scripts/Simulator` framework. They must be treated as two
  overlapping prototypes rather than one coherent architecture.
- There are two distinct files named `KinematicBicycleModel.cs`:
  `Scripts/ytwei/KinematicBicycleModel.cs` and
  `Scripts/Simulator/Agent/Vehicle/Controller/KinematicBicycleModel.cs`.
- `Scripts/ytwei/MotorcycleAgent.cs` is 2,194 lines long. It combines agent
  actions and observations, reward calculation, curriculum transitions,
  subgoal setup, collision handling, UI updates, trajectory logging, CSV
  output, and evaluation metrics.
- `Scripts/Simulator/Agent/Vehicle/VehicleInterface.cs` is 996 lines long and
  similarly mixes vehicle state, rule-based control, collision handling,
  reward logic, intersection state, and lifecycle management.

## Migration decisions

### Rebuild as small, testable modules

| Legacy responsibility | Legacy source | New home | Reason |
| --- | --- | --- | --- |
| PID steering math | `ytwei/PIDController.cs` | `Scripts/Core/Control` | Pure logic; no scene dependency is needed. |
| Intelligent Driver Model | `ytwei/IDM.cs` | `Scripts/Core/Traffic` | Separate IDM equations and parameters from Unity perception and spawners. |
| Risk and waypoint evaluation | `ytwei/WaypointGenerator.cs` | `Scripts/Core/Risk` | Keep the research idea, but return data instead of creating waypoints and debug objects. |
| Signed-distance guidance | `ytwei/SDFCalculator.cs` | `Scripts/Core/Guidance` | Retain only if it remains part of the new research question. |
| RL observation, action, and reward adapter | `ytwei/MotorcycleAgent.cs` | `Scripts/Runtime/Agents` | Rebuild around ML-Agents 4.0.0 after the core models exist. |
| Experiment configuration | scattered public fields and YAML | `Settings` ScriptableObjects and versioned trainer YAML | Make each experiment reproducible and diffable. |

### Preserve as reference only for now

- `ytwei/MotorcycleAgent.cs`, `WaypointGenerator.cs`, `IDM.cs`,
  `SDFCalculator.cs`, `SDFController.cs`, and the legacy kinematic model.
- The traffic, road, and vehicle prefabs used by the final demonstration scene.
- Training YAML files and recorded trajectory/attribute data.

### Do not migrate directly

- `Simulator/` manager and interface layer: useful as a design reference, but
  it is a second prototype with overlapping responsibilities.
- `ImportedPackages/` demo scenes and editor tooling.
- `Plugins/Editor/`, `SDFTextureGenerator/Example/`, and other package samples.
- Old ML-Agents agents and trained models. ML-Agents 4.0.0 requires a new
  adapter and retraining.

## First implementation slice

1. Create a pure `PidController` class and its unit tests.
2. Create immutable traffic-state data types and a pure IDM model.
3. Recreate the five-direction risk/waypoint calculation as data-only logic.
4. Add Unity adapters for perception and a motorcycle prefab only after the
   preceding models are testable without a scene.

This order intentionally postpones visual assets, UI, traffic spawning, and RL
training. It establishes a reliable simulation core before introducing Unity
object references.
