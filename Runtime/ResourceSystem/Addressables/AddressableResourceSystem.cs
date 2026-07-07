#if USE_ADDRESSABLES
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

internal sealed class AddressableResourceSystem : BaseResourceSystem
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        UIManager.RegisterBuiltinResourceSystem(UIManager.ResourceSystemType.Addressable, () => new AddressableResourceSystem());
    }

    public override UniTask<T> LoadPanel<T>(string name, Transform parent = null)
    {
        return Load<T>(name, parent != null ? parent : UIManager.Instance.MainCanvas != null ? UIManager.Instance.MainCanvas.PanelParent : null);
    }

    public override UniTask<T> LoadPopup<T>(string name, Transform parent = null)
    {
        return Load<T>(name, parent != null ? parent : UIManager.Instance.MainCanvas != null ? UIManager.Instance.MainCanvas.PopupParent : null);
    }

    private static async UniTask<T> Load<T>(string name, Transform parent)
    {
        string key = GetAssetPath(name);
        var handle = Addressables.InstantiateAsync(key, parent, false, false);
        GameObject instance = await handle.Task.AsUniTask();

        T component = instance.GetComponent<T>();
        if (component == null)
        {
            Addressables.ReleaseInstance(instance);
            throw new InvalidOperationException($"Component of type '{typeof(T).Name}' was not found on addressable '{key}'.");
        }

        return component;
    }

    private static string GetAssetPath(string name)
    {
        return string.IsNullOrEmpty(UISystemConfig.ADDRESSABLE_PATH) ? name : $"{UISystemConfig.ADDRESSABLE_PATH}/{name}";
    }
}
#endif
