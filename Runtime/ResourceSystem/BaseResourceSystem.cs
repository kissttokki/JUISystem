using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseResourceSystem
{
    public abstract UniTask<T> LoadPanel<T>(string name, Transform parent = null);

    public abstract UniTask<T> LoadPopup<T>(string name, Transform parent = null);
}
