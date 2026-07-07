using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

internal sealed class UnityResourceSystem : BaseResourceSystem
{
    public override UniTask<T> LoadPanel<T>(string name, Transform parent = null)
    {
        return Load<T>(name, parent != null ? parent : UIManager.Instance.MainCanvas != null ? UIManager.Instance.MainCanvas.PanelParent : null);
    }

    public override UniTask<T> LoadPopup<T>(string name, Transform parent = null)
    {
        return Load<T>(name, parent != null ? parent : UIManager.Instance.MainCanvas != null ? UIManager.Instance.MainCanvas.PopupParent : null);
    }

    private static UniTask<T> Load<T>(string name, Transform parent)
    {
        string path = GetAssetPath(name);
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
            throw new InvalidOperationException($"UI prefab not found at path '{path}'.");

        GameObject instance = UnityEngine.Object.Instantiate(prefab, parent);
        T component = instance.GetComponent<T>();
        if (component == null)
        {
            UnityEngine.Object.Destroy(instance);
            throw new InvalidOperationException($"Component of type '{typeof(T).Name}' was not found on prefab '{prefab.name}'.");
        }

        return UniTask.FromResult(component);
    }

    private static string GetAssetPath(string name)
    {
        return string.IsNullOrEmpty(UISystemConfig.RESOURCES_PATH) ? name : $"{UISystemConfig.RESOURCES_PATH}/{name}";
    }
}
