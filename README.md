# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a **Unity plugin for Stewart Platform control and drone delivery simulation**. The codebase combines:
- A 6-DOF Stewart hexapod platform controller with serial communication
- A physics-based drone flight controller
- A cargo delivery mission system with waypoint management
- Educational challenge scenes for learning platform control

The main plugin code is located in `Assets/SV4/`.

## Core Architecture

### Stewart Platform Communication

The platform controller uses serial communication to interface with physical Stewart platform hardware:

**Communication Protocol:**
- Start frame: `!` (ASCII 33)
- End frame: `#` (ASCII 35)
- Two modes available:
  - **8-Bit Mode**: 6 bytes (0-255 range), 8 bytes total
  - **32-Bit Float Mode**: 6 floats (24 bytes), 26 bytes total

**6-DOF Axis Order**: [Sway, Surge, Heave, Pitch, Roll, Yaw]

**Serial Communication Pattern:**
- **Blocking approach** (UnitySerial.cs): NOT RECOMMENDED - causes frame drops
- **Non-blocking approach** (SerialThread.cs): RECOMMENDED - uses threading with message queue

### Key Singleton Pattern Usage

Three critical singletons manage global state:
- `PlatformController.singleton` - Stewart platform control (Assets/SV4/1.) Platform Controller/PlatformController.cs:271)
- `SerialThread.singleton` - Threaded serial I/O (Assets/SV4/Scripts/SerialThread.cs:29)
- `UIManager.Instance` - Mission UI management
- `WaypointManager.Instance` - Waypoint tracking

### Drone-Platform Integration

The DroneController integrates with the platform controller to provide motion feedback:
```csharp
// In DroneController.cs:41-55
PlatformController.singleton.Init("COM7", 115200);
PlatformController.singleton.Pitch = -finalPitch;
PlatformController.singleton.Roll = finalRoll;
PlatformController.singleton.Yaw = Mathf.Lerp(PlatformController.singleton.Yaw, input.Pedals * 10, 0.02f);
```

### Input System Architecture

The project uses Unity's **new Input System** (not legacy Input Manager):
- Input handled via `Drone_Inputs.cs` with InputActionMap
- Three control axes: Cyclic (2D), Pedals (1D), Throttle (1D)
- Supports both keyboard and gamepad input

### Observer Pattern for Serial Messages

SerialThread implements an observer pattern for distributing incoming serial messages:
- Observers implement `ISerialReader` interface
- Messages are safely queued from background thread to Unity main thread using `lock(queueLock)`
- Messages distributed via `SendMessage("ReceiveMessage", msg)` to all registered observers

## Directory Structure

```
Assets/SV4/
├── 1.) Platform Controller/        # Basic platform control demo scene
├── 2.) Challenge - Bounce/          # Physics challenge: bouncing ball
├── 3.) Challenge - Serial Read/     # Serial communication learning challenge
├── 4.) Challenge - Platform Builder/ # Platform kinematics visualization & IK solver
├── Scripts/                         # Core utilities & serial communication
│   ├── SerialThread.cs              # Non-blocking serial reader (RECOMMENDED)
│   ├── UnitySerial.cs               # Blocking serial reader (NOT RECOMMENDED)
│   ├── ISerialReader.cs             # Observer interface for serial messages
│   ├── UtilityLib.cs                # Math utilities for platform geometry
│   └── LineRenderGroup.cs           # Visualization utilities
├── drone Controller/                # Drone flight system
│   ├── DroneController.cs           # Main drone controller
│   ├── Drone_Inputs.cs              # Input handler (new Input System)
│   ├── DroneEngine.cs               # Engine physics & thrust
│   ├── IEngine.cs                   # Engine interface
│   ├── DroneCamera.cs               # Third-person camera
│   ├── Rigded_Body_Setup.cs         # Base class for physics objects
│   └── Cargo control/               # Cargo spawning & management
└── UI And objective tracker/        # Mission UI & waypoint system
    ├── UIManager.cs                 # Mission timer, delivery counter, prompts
    ├── WaypointManager.cs           # Pickup/dropoff waypoint management
    ├── PickupPoint.cs               # Cargo pickup trigger
    └── DropOffPoint.cs              # Cargo drop-off validation
```

## Common Development Patterns

### Initializing Serial Communication

When working with the platform controller:
```csharp
// Non-blocking threaded approach (RECOMMENDED)
SerialThread.singleton.Init("COM7", 115200, this); // 'this' as observer
```

### Adding Serial Message Observers

To receive serial messages in your script:
1. Implement `ISerialReader` interface (if using interface pattern)
2. Or add a `ReceiveMessage(string msg)` method to receive SendMessage calls
3. Register as observer: `SerialThread.singleton.AddObserver(this)`

### Platform Control

Access platform via singleton:
```csharp
PlatformController.singleton.Init("COM7", 115200);
PlatformController.singleton.Pitch = 10f;  // -30 to 30 degrees
PlatformController.singleton.Roll = -5f;
PlatformController.singleton.HomePlatform(); // Reset to neutral
```

### Cargo Drop Logic

Cargo can only be dropped in valid drop-off zones:
```csharp
// Check if drop is valid
bool canDrop = waypointManager.CurrentDropOffPoint != null &&
               waypointManager.CurrentDropOffPoint.CanDropCargo();

// Drop cargo (DroneController.cs:88-109)
if (HasCargo && canDrop) {
    DropCargo();
    UIManager.Instance.UpdateDeliverysCount();
    WaypointManager.Instance.OnDropOff(currentDropOffPoint);
}
```

## Testing & Development

**No automated test suite exists.** Testing is done via challenge scenes:
- Scene 1: Platform Controller basic demo
- Scene 2: Physics bounce challenge
- Scene 3: Serial communication challenge
- Scene 4: Platform kinematics visualization

## Important Implementation Notes

### Thread Safety
- SerialThread uses `lock(queueLock)` for thread-safe message queuing (SerialThread.cs:134, 193)
- Always access queues within lock blocks when working with threaded serial I/O

### Serial Port Formatting
- Use special formatting for COM ports > COM9: `@"\\.\" + comPort`
- See PlatformController.cs:58 and SerialThread.cs:99

### Mass Conversion
- DroneController extends Rigged_Body_Setup which handles mass unit conversion
- Rigidbody mass set in lbs, converted to kg internally

### Platform Send Rate
- Platform updates sent at fixed rate: 0.02s (50 FPS)
- Controlled via `nextSendDelay` in PlatformController.cs:30

### Cleanup on Exit
- Both SerialThread and PlatformController implement `OnApplicationQuit()`
- Platform automatically homes (resets to neutral) and closes serial port on exit

## Key Files Reference

| Component | File Path |
|-----------|-----------|
| Platform Controller | Assets/SV4/1.) Platform Controller/PlatformController.cs |
| Serial Threading | Assets/SV4/Scripts/SerialThread.cs |
| Drone Controller | Assets/SV4/drone Controller/DroneController.cs |
| Drone Input | Assets/SV4/drone Controller/Drone_Inputs.cs |
| Platform Kinematics | Assets/SV4/4.) Challenge - Platform Builder/PlatformBuilder.cs |
| Mission UI | Assets/SV4/UI And objective tracker/UIManager.cs |
| Waypoint System | Assets/SV4/UI And objective tracker/WaypointManager.cs |

## Unity Version & Dependencies

- Uses Unity's new Input System (not legacy)
- Requires TextMesh Pro for UI
- External asset packs included (Residential Buildings, Military Cargo Aircraft)
- No assembly definition files (.asmdef) - compiles to default Assembly-CSharp
