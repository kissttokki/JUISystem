# J UISystem

A lightweight Unity UI package for panel and popup lifecycle management with pluggable resource loading.

## Features
- Generic panel and popup loading APIs
- Distinct panel reopen lifecycle for back-stack style navigation
- Popup closing and closed lifecycle separation
- Pluggable resource loading through `BaseResourceSystem`
- Built-in Unity Resources and Addressables resource systems

## Requirements
- Unity 6000.5 or newer
- UniTask must be installed before importing this package
- Addressables is optional

## Installation

### Package via Git URL
Use the Unity Package Manager and add the Git URL:

`https://github.com/kissttokki/JUISystem.git`

### Required dependency
Install UniTask first. Example Git URL:

`https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask`

## Basic setup
1. Add `UIManager` to the scene.
2. Add `MainCanvasSetter` to the main UI canvas.
3. Register a resource system during startup.

### Unity Resources
```csharp
UIManager.Instance.SetResourceSystem(UIManager.ResourceSystemType.UnityResource);
```

### Addressables
```csharp
UIManager.Instance.SetResourceSystem(UIManager.ResourceSystemType.Addressable);
```

### Custom resource system
```csharp
public sealed class MyResourceSystem : BaseResourceSystem
{
    public override UniTask<T> LoadPanel<T>(string name, Transform parent = null)
    {
        throw new NotImplementedException();
    }

    public override UniTask<T> LoadPopup<T>(string name, Transform parent = null)
    {
        throw new NotImplementedException();
    }
}

UIManager.Instance.SetResourceSystem(new MyResourceSystem());
```

## Notes
- `BaseResourceSystem` is registered publicly through `UIManager`, but direct external access to the active resource system is not exposed.
- `ReOpen` is intentionally preserved as a distinct panel lifecycle extension point for package consumers.
- `AddressableResourceSystem` is compiled only when `UNITASK_ADDRESSABLE_SUPPORT` is available.

