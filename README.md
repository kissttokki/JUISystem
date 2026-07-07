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
- UniTask is required
- Addressables is optional

## Installation

### Package via Git URL
Use the Unity Package Manager and add the Git URL:

`https://github.com/kissttokki/JUISystem.git`

### Dependencies
This package depends on UniTask, uGUI, and TextMeshPro.

`com.unity.ugui` and `com.unity.textmeshpro` are declared in the package manifest.

Install UniTask in the consuming project before importing this package. Example Git URL:

`https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask`

## Basic setup
1. Add `UIManager` to the scene.
2. Add `MainCanvasSetter` to the main UI canvas.
3. Register a resource system during startup.

### Unity Resources
Place UI prefabs under `Resources/UI` by default.

```csharp
UIManager.Instance.SetResourceSystem(UIManager.ResourceSystemType.UnityResource);
```

### Addressables
Install `com.unity.addressables` and place UI prefabs under `AddressableAssets/Remote/UI` by default.

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

## Editor workflow
- Use the `CONTEXT/Button/Convert to CustomButton` menu on a `Button` component to replace its script with `CustomButton`.

## Notes
- `BaseResourceSystem` is registered publicly through `UIManager`, but direct external access to the active resource system is not exposed.
- `ReOpen` is intentionally preserved as a distinct panel lifecycle extension point for package consumers.
- `AddressableResourceSystem` is compiled only when `USE_ADDRESSABLES` is available through the Addressables package version define.

