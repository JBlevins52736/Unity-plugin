# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 2022.3.55f1 project for a drone delivery simulation integrated with a physical Stewart Platform (motion simulator). The project combines virtual drone flight control with real-time serial communication to a 6-DOF motion platform.

## Key Technologies

- **Unity 2022.3.55f1**
- **Unity Input System** - Flight control input handling
- **Serial Communication** - Real-time communication with Stewart Platform hardware
- **ProBuilder** - 3D environment construction
- **TextMesh Pro** - UI rendering
- **Deform Package** - Mesh deformation (Beans.Unity custom package)

## Development Commands

### Opening the Project
- Open the project in Unity Hub by selecting the directory `SV4 Jasen blevins`
- Unity will automatically compile scripts on load

### Building
- Unity Editor: `File > Build Settings > Build` (or Ctrl+Shift+B)
- Command line: Not commonly used for Unity projects

### Testing in Editor
- Press Play button in Unity Editor to enter Play Mode
- Connect Stewart Platform to COM7 (115200 baud) before testing
- The platform initializes automatically in `DroneController.Start()`

## Architecture

### Core Flight System

**DroneController** (`Assets/SV4/drone Controller/DroneController.cs`)
- Main controller inheriting from `Rigged_Body_Setup`
- Integrates with `PlatformController` singleton for motion platform output
- Handles pitch/roll/yaw physics and engine management
- Manages cargo pickup/drop mechanics
- Updates platform every frame: `PlatformController.singleton.Pitch/Roll/Yaw`

**Drone_Inputs** (`Assets/SV4/drone Controller/Drone_Inputs.cs`)
- Unity Input System wrapper providing:
  - `Cyclic` (Vector2) - Pitch and Roll input
  - `Pedals` (float) - Yaw input
  - `Throttle` (float) - Altitude/power control

**IEngine Interface** (`Assets/SV4/drone Controller/IEngine.cs`)
- Component-based engine system
- Engines attach to drone children implementing `IEngine`
- `DroneEngine` is the concrete implementation

### Stewart Platform Communication

**PlatformController** (`Assets/SV4/1.) Platform Controller/PlatformController.cs`)
- Original platform controller (superseded by `PlatformControllerAdv`)

**PlatformControllerAdv** (`Assets/SV4/Scripts/PlatformControllerAdv.cs`)
- **Singleton pattern** - Access via `PlatformControllerAdv.singleton`
- Communicates with 6-DOF Stewart Platform via serial
- Three communication modes:
  - `Mode_8Bit` - 6 bytes (0-255 range, 128=neutral)
  - `Mode_Float32` - 6 floats (-30 to +30 degrees/units)
  - `Mode_ASCII` - Human-readable "DOF=x,y,z,p,r,y" format
- **6-DOF Axis Order**: [Sway, Surge, Heave, Pitch, Roll, Yaw]
- Properties for direct axis control: `Sway`, `Surge`, `Heave`, `Pitch`, `Roll`, `Yaw`
- Fixed update rate: 50 FPS (0.02s delay between sends)
- **Must call `Init(comPort, baudRate)` before use** (currently hardcoded to COM7, 115200 in DroneController)

**SerialThread** (`Assets/SV4/Scripts/SerialThread.cs`)
- Threaded serial reading implementation (alternative approach)
- Singleton pattern with observer notification system
- Safer for bidirectional communication but not currently used by drone system

### Cargo Delivery System

**WaypointManager** (`Assets/SV4/UI And objective tracker/Waypoint Manager.cs`)
- Singleton managing delivery loop: pickup → dropoff → pickup
- Spawns random dropoff zones from `dropOffLocations` list
- Never repeats last dropoff location if multiple available
- Access via `WaypointManager.Instance`

**PickupPoint** / **DropOffPoint** (`Assets/SV4/UI And objective tracker/`)
- Zone trigger components with activation states
- DropOffPoint validates cargo drop with `CanDropCargo()`
- Communicate with WaypointManager via `OnPickup()` / `OnDropOff()`

**CargoSpawner** (`Assets/SV4/drone Controller/Cargo contorl/CargoSpawner.cs`)
- Manages cargo object lifecycle at pickup zones

### Challenges/Examples

The project contains 4 numbered challenge folders demonstrating platform concepts:

1. **Platform Controller** - Basic platform control
2. **Challenge - Bounce** - Physics simulation mapped to platform (bouncing ball)
3. **Challenge - Serial Read** - Reading sensor data from Arduino/hardware
4. **Challenge - Platform Builder** - Stewart platform geometry calculator using `StewartPlatformUtility`

**StewartPlatformUtility** calculates attachment points for physical platform construction:
- Generates top platform and base mounting coordinates
- Uses `UtilityLib` for triangular geometry calculations
- Outputs C++ and C# syntax for embedding coordinates

### Scene Structure

**Main Scene**: `Assets/finalScene.unity`
- Contains complete drone delivery simulation
- Includes terrain, buildings (Residential Buildings Pack, Military Cargo Aircraft)
- Pre-configured with waypoint system

## Serial Port Configuration

The drone system expects:
- **Port**: COM4 (ESP32 Silicon Labs CP210x USB-to-UART Bridge)
- **Baud Rate**: 115200
- **Data Format**: 8 data bits, no parity, 1 stop bit
- **Update Rate**: 100 Hz (0.01s between sends)

**To find your COM port**: Use Windows Device Manager or PowerShell:
```powershell
Get-CimInstance -ClassName Win32_SerialPort | Select-Object DeviceID, Name
```

**To change port**: Modify `DroneController.Start()` line 41:
```csharp
PlatformController.singleton.Init("COM4", 115200);
```

## Common Workflow

### Adding New Drone Behaviors
1. Create new component implementing behavior logic
2. If affecting flight dynamics, inherit from or modify `DroneController`
3. Update platform DOF via `PlatformControllerAdv.singleton` properties
4. Test without platform by commenting out `Init()` call in DroneController

### Modifying Platform Motion Response
1. Edit `DroneController.Update()` lines 51-55 (pitch/roll/yaw mapping)
2. Adjust lerp values for smoothness
3. Platform accepts -30 to +30 degree range in Float32 mode
4. Use `MapRange()` utility in PlatformControllerAdv for value scaling

### Testing Without Hardware
- Comment out `PlatformController.singleton.Init()` in DroneController.Start()
- Platform commands will safely no-op when port not open
- All flight mechanics work independently

## Input Configuration

Flight controls are defined in Unity's Input System:
- Located in Project Settings > Input Manager (legacy) or Input Actions asset
- Actions: Cyclic, Pedals, Throttle
- Supports both keyboard/mouse and joystick input
- Cargo drop: E key or Joystick Button 2

## Important Notes

- **Platform Safety**: Always call `HomePlatform()` before disconnecting - this centers the platform to prevent damage
- **Scene Persistence**: PlatformControllerAdv uses `DontDestroyOnLoad()` - persists across scene changes
- **Thread Safety**: SerialPort operations must not be called from Unity's main thread except in PlatformControllerAdv's Update() loop
- **COM Port Naming**: Windows requires `\\.\COMx` format for COM10+ (already handled in code)
