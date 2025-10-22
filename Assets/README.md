# Assets Folder Structure

This folder contains all Unity project assets organized by type.

## Standard Unity Folders

### **Scripts/**
C# scripts and code files. Currently, main plugin scripts are in `SV4/` subdirectories.

### **Prefabs/**
Reusable game objects. Store drone prefabs, cargo prefabs, and platform controller prefabs here.

### **Scenes/**
Unity scene files (.unity). Contains:
- `finalScene.unity` - Main drone delivery scene

### **Materials/**
Unity material assets (.mat). Contains terrain materials and object materials.

### **Textures/**
Texture files for materials and sprites.

### **Sprites/**
2D sprite assets. Contains:
- `DropOffSprite.png` - Drop-off zone indicator

### **Models/**
3D model files (.fbx, .obj, etc.). Store custom 3D models here.

### **Animations/**
Animation clips and animator controllers.

### **Audio/**
- **Music/** - Background music files
- **SFX/** - Sound effects

### **Resources/**
Assets loaded at runtime via `Resources.Load()`.

### **StreamingAssets/**
Assets that need to be accessed by file path at runtime.

### **Editor/**
Editor-only scripts and tools.

### **Plugins/**
Native plugins and third-party DLLs.

## Project-Specific Folders

### **SV4/**
Main Stewart Platform Variant 4 plugin. Contains:
- Platform controller scripts
- Drone controller system
- Challenge scenes
- UI and objective tracking
- Serial communication utilities

See root README.md for detailed architecture documentation.

### **Residential Buildings Pack/**
Third-party asset pack for building models.

### **Military Cargo Aircraft/**
Third-party asset pack for aircraft models.

### **UI Toolkit/**
UI framework resources and styles.

### **Deform/**
Terrain deformation tools and resources.

### **Polybrush Data/**
Polybrush terrain painting data.

### **_TerrainAutoUpgrade/**
Unity terrain system auto-upgrade data.
