#if UNITASK_ENABLED
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static readonly Dictionary<ResourceSystemType, Func<BaseResourceSystem>> s_builtinResourceSystemFactories = new Dictionary<ResourceSystemType, Func<BaseResourceSystem>>();

    public enum ResourceSystemType
    {
        UnityResource,
#if USE_ADDRESSABLES
        Addressable,
#endif
    }

    static UIManager()
    {
        RegisterBuiltinResourceSystem(ResourceSystemType.UnityResource, () => new UnityResourceSystem());
    }

    #region SingleTon
    public static UIManager Instance 
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindAnyObjectByType<UIManager>();
            }

            if (_Instance == null)
            {
                _Instance = new GameObject(nameof(UIManager)).AddComponent<UIManager>();
            }
            return _Instance;
        }
    }
    private static UIManager _Instance;

    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        else if (_Instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public MainCanvasSetter MainCanvas;
    public event Action<UIPanelBase> OnChangePanel;

    private BaseResourceSystem _resourceSystem;

    internal BaseResourceSystem ResourceSystem => _resourceSystem ?? throw new InvalidOperationException("UI ResourceSystem is not initialized. Call UIManager.SetResourceSystem(...) before loading panels or popups.");

    private List<UIPopupBase> _popups = new List<UIPopupBase>();


    public void SetResourceSystem(BaseResourceSystem resourceSystem)
    {
        if (resourceSystem == null)
            throw new ArgumentNullException(nameof(resourceSystem));

        _resourceSystem = resourceSystem;
    }

    public void SetResourceSystem(ResourceSystemType resourceSystemType)
    {
        if (s_builtinResourceSystemFactories.TryGetValue(resourceSystemType, out var factory) == false || factory == null)
        {
            throw new InvalidOperationException($"Built-in resource system '{resourceSystemType}' is not available in the current project.");
        }

        SetResourceSystem(factory());
    }

    public static void RegisterBuiltinResourceSystem(ResourceSystemType resourceSystemType, Func<BaseResourceSystem> factory)
    {
        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        s_builtinResourceSystemFactories[resourceSystemType] = factory;
    }


    public void ShowToast(string msg)
    {

    }


    public void OnShowPopup(UIPopupBase popup)
    {
        if (_popups.Contains(popup) == false)
        {
            _popups.Add(popup);
        }
    }

    public void OnClosingPopup(UIPopupBase popup)
    {

    }

    public void OnClosedPopup(UIPopupBase popup)
    {
        _popups.Remove(popup);
    }

    internal void OnChangedPanel(UIPanelBase panel)
    {
        OnChangePanel?.Invoke(panel);
    }

    public void CloseAllPopup()
    {
        for(int i = _popups.Count - 1; i >= 0; i--)
        {
            _popups[i].Close();
        }
    }
}
#endif
