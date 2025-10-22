# Credits and Asset Attributions

This project uses various third-party assets and libraries. Below is a comprehensive list of all external resources used.

## Third-Party Asset Packs

### Residential Buildings Pack
- **Author:** GabroMedia
- **Contact:** gabromedia@gmail.com
- **Version:** 2.3 (2016)
- **Source:** Unity Asset Store
- **Location:** `Assets/Residential Buildings Pack/`
- **Description:** Low-poly residential building models with props, roads, and streetlights
- **Contents:**
  - 50+ residential building prefabs (Houses A-E, Garages)
  - Road pieces and driveways
  - Props (mailboxes, fire hydrants, street lights, plants, fences)
  - Demo scenes (day and night lighting)
- **License:** Unity Asset Store License (standard asset store EULA)
- **Notes:** All prefabs fitted with colliders, proper pivot positions, and nested spotlights

### Military Cargo Aircraft
- **Author:** Unknown (purchased asset)
- **Source:** Unity Asset Store (presumed)
- **Location:** `Assets/Military Cargo Aircraft/`
- **Last Modified:** May 4, 2023
- **Description:** Military transport aircraft with cargo container
- **Contents:**
  - Military cargo aircraft model (FBX format)
  - Container and rope accessories
  - 4 material variants (A, B, C, Custom)
  - PBR shader graphs for URP/HDRP
  - Full texture sets (BaseColor, Normal, AO, Metallic, Roughness)
  - Paint mask for custom liveries
- **License:** Unity Asset Store License (presumed)

## Unity Packages

### TextMesh Pro
- **Publisher:** Unity Technologies
- **Version:** 3.0.6
- **Source:** Unity Package Manager
- **License:** Unity Companion License
- **Location:** `Assets/SV4/TextMesh Pro/`
- **Fonts Included:**
  - **Liberation Sans**
    - Copyright © 2010 Google Corporation
    - Copyright © 2012 Red Hat, Inc.
    - License: SIL Open Font License v1.1
    - License File: `Assets/SV4/TextMesh Pro/Fonts/LiberationSans - OFL.txt`
    - Website: http://scripts.sil.org/OFL
- **Sprites:**
  - **EmojiOne**
    - Source: https://www.emojione.com/
    - Attribution File: `Assets/SV4/TextMesh Pro/Sprites/EmojiOne Attribution.txt`
    - License: See EmojiOne website for licensing terms

### Unity Input System
- **Publisher:** Unity Technologies
- **Version:** 1.7.0
- **Source:** Unity Package Manager
- **License:** Unity Companion License
- **Usage:** Drone controller input handling (cyclic, pedals, throttle)

### Unity UI (uGUI)
- **Publisher:** Unity Technologies
- **Version:** 1.0.0
- **Source:** Unity Built-in Package
- **License:** Unity Companion License
- **Usage:** Mission UI, waypoint tracking, delivery counter

### Deform (Mesh Deformation Tool)
- **Location:** `Assets/Deform/`
- **Description:** Terrain and mesh deformation utilities
- **License:** Unknown (appears to be an asset store package)
- **Last Modified:** January 9, 2025

### Polybrush
- **Publisher:** Unity Technologies
- **Location:** `Assets/Polybrush Data/`
- **Description:** Terrain painting and texturing tool
- **License:** Unity Companion License
- **Last Modified:** January 14, 2025

## Fonts

### Liberation Sans
- **Copyright:**
  - © 2010 Google Corporation (Reserved Font Arimo, Tinos, Cousine)
  - © 2012 Red Hat, Inc. (Reserved Font Name Liberation)
- **License:** SIL Open Font License v1.1
- **License URL:** http://scripts.sil.org/OFL
- **Location:** `Assets/SV4/TextMesh Pro/Fonts/`
- **License File:** `Assets/SV4/TextMesh Pro/Fonts/LiberationSans - OFL.txt`
- **Usage:** UI text rendering via TextMesh Pro

## Emoji / Icons

### EmojiOne
- **Source:** https://www.emojione.com/
- **Location:** `Assets/SV4/TextMesh Pro/Sprites/`
- **Attribution:** See EmojiOne website for complete licensing
- **Usage:** Sample emoji sprites for TextMesh Pro

## Original Code and Scripts

### Stewart Platform Controller & Drone Simulation
- **Author:** Jasen Blevins (JBlevins52736)
- **Repository:** https://github.com/JBlevins52736/Unity-plugin
- **Location:** `Assets/SV4/`
- **License:** [Add your license here]
- **Components:**
  - Stewart Platform serial communication (`SerialThread.cs`, `PlatformController.cs`)
  - Drone flight controller (`DroneController.cs`, `Drone_Inputs.cs`)
  - Cargo delivery mission system (`UIManager.cs`, `WaypointManager.cs`)
  - Educational challenge scenes
  - Platform kinematics visualization

### ESP32 Firmware
- **Location:** `StewartPlatform_ESP32_2410S/`
- **Description:** ESP32 firmware for Stewart Platform hardware control
- **Last Modified:** September 26, 2024

## Unity Engine

This project is built with **Unity 2022.3.0f1** (LTS)
- **Publisher:** Unity Technologies
- **License:** Unity Software License
- **Website:** https://unity.com/

## Build Dependencies (Packages/manifest.json)

The following Unity packages are referenced:
- `com.unity.inputsystem` - 1.7.0
- `com.unity.textmeshpro` - 3.0.6
- `com.unity.ugui` - 1.0.0
- `com.unity.timeline` - 1.7.5
- `com.unity.visualscripting` - 1.8.0
- `com.unity.collab-proxy` - 2.0.5
- `com.unity.feature.development` - 1.0.1

All Unity packages follow the **Unity Companion License** unless otherwise noted.

## Notes on Asset Usage

### Commercial Use
- **Residential Buildings Pack** and **Military Cargo Aircraft** are Unity Asset Store purchases
- These assets are subject to the Unity Asset Store EULA
- Redistribution of original asset files is prohibited
- Usage within Unity projects (including built games) is permitted per Asset Store license

### Attribution Requirements
- **Liberation Sans Font:** Must include SIL OFL license notice
- **EmojiOne:** Attribution recommended, see their website for requirements
- **TextMesh Pro:** Covered by Unity Companion License, no additional attribution required

## Missing Information

If you are the original author or know the attribution for any of the following, please contact the repository owner:

1. **Military Cargo Aircraft** - Exact asset store link or original creator unknown
2. **Deform** - Full license and attribution information needed

## License Compliance

This project complies with all applicable licenses. If you discover any licensing issues or missing attributions, please open an issue at:
https://github.com/JBlevins52736/Unity-plugin/issues

---

**Last Updated:** October 22, 2025
