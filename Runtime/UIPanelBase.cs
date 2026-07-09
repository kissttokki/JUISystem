#if UNITASK_ENABLED
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



[RequireComponent(typeof(Canvas),typeof(GraphicRaycaster))]
public abstract class UIPanelBase<T> : UIPanelBase where T : UIPanelBase<T>
{
    public static T Instance => _Instance;
    protected static T _Instance;

    [field: SerializeField] public Canvas Canvas { get; private set; }

    public bool isEnable => gameObject.activeSelf;

    public async static UniTask<T> LoadPanel(Transform parent = null)
    {
        _Instance = await UIManager.Instance.ResourceSystem.LoadPanel<T>(typeof(T).Name, parent);
        return _Instance;
    }

    public async static UniTask<T> ShowPanel(Transform parent = null)
    {
        T panel = await LoadPanel(parent);
        panel.Open();
        return panel;
    }


#if UNITY_EDITOR
    virtual protected void OnValidate()
    {
        Canvas = GetComponent<Canvas>();
    }
#endif

    public override void Close()
    {
        base.Close();
        _Instance = null;
    }

  

    private void Awake()
    {
        if (_Instance == null) _Instance = this as T;
    }

    private void OnDestroy()
    {
        if (_Instance == this) _Instance = null;
    }
}


public class UIPanelBase : MonoBehaviour
{
    public static UIPanelBase CurrentPanel { get; private set; }

    protected static List<UIPanelBase> s_panelList = new List<UIPanelBase>();

    private bool _isClosing;
    private bool _isClosed;

    public virtual void Open()
    {
        if (_isClosing || _isClosed)
            return;

        OnOpening();
        CurrentPanel?.Hide();
        CurrentPanel = this;
        if (s_panelList.Contains(this) == false)
        {
            s_panelList.Add(this);
        }

        gameObject.SetActive(true);
        UIManager.Instance.OnChangedPanel(this);
        OnOpened();
    }

    protected virtual void ReOpen()
    {
        if (_isClosing || _isClosed)
            return;

        OnOpening();
        CurrentPanel?.Hide();
        CurrentPanel = this;
        gameObject.SetActive(true);
        UIManager.Instance.OnChangedPanel(this);
        OnOpened();
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    protected virtual void OnOpening()
    {

    }

    protected virtual void OnOpened()
    {

    }

    protected virtual void OnClosing()
    {

    }

    protected virtual void OnClosed()
    {

    }

    public virtual void Close()
    {
#if UNITY_EDITOR
        if (Application.isEditor == true)
        {
            if (UnityEditor.EditorApplication.isPlaying == false)
            {
                throw new OperationCanceledException();
            }
        }
#endif

        if (_isClosing || _isClosed)
            return;

        _isClosing = true;
        OnClosing();

        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_isClosing == false)
        {
            _isClosing = true;
            OnClosing();
        }

        s_panelList.Remove(this);

        if (CurrentPanel == this)
        {
            CurrentPanel = null;

            if (s_panelList.Count > 0)
            {
                var lastPanel = s_panelList[s_panelList.Count - 1];
                lastPanel.ReOpen();
            }
            else
            {
                UIManager.Instance.OnChangedPanel(null);
            }
        }

        if (_isClosed == false)
        {
            _isClosed = true;
            OnClosed();
        }
    }
}

#endif