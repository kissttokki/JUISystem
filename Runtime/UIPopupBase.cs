using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public abstract class UIPopupBase<T> : UIPopupBase where T : UIPopupBase<T>
{
    public static async UniTask<T> ShowPopup(bool isActive = true, Transform parent = null)
    {
        T popup = await UIManager.Instance.ResourceSystem.LoadPopup<T>(typeof(T).Name, parent);
        popup?.gameObject.SetActive(isActive);
        return popup;
    }
}

public class UIPopupBase : MonoBehaviour
{
    public enum PopupCloseCode
    {
        None,
        AccountCreated,
    }



    public bool IsCreated => this.gameObject != null;

    protected CancellationTokenSource _closeAwaiter;
    protected PopupCloseCode _closeCode = PopupCloseCode.None;

    private bool _isClosing;
    private bool _isClosed;

    private void OnApplicationQuit()
    {
        ReleaseCloseAwaiter();
    }

    protected virtual void Awake()
    {
        UIManager.Instance.OnShowPopup(this);
    }

    public virtual void Close()
    {
        if (_isClosing)
            return;

        _isClosing = true;
        UIManager.Instance.OnClosingPopup(this);

        if (this != null && this.gameObject != null)
        {
            Destroy(this.gameObject);
        }
    }

    public async UniTask<PopupCloseCode> WaitUntilClose()
    {
        await UniTask.Yield();

        if (this == null)
            return _closeCode;

        if (_closeAwaiter == null)
            _closeAwaiter = new CancellationTokenSource();

        if (_closeAwaiter.Token.IsCancellationRequested)
            return _closeCode;

        await _closeAwaiter.Token.WaitUntilCanceled();
        return _closeCode;
    }


    private void OnDestroy()
    {
        if (_isClosing == false)
        {
            _isClosing = true;
            UIManager.Instance.OnClosingPopup(this);
        }

        if (_isClosed == false)
        {
            _isClosed = true;
            UIManager.Instance.OnClosedPopup(this);
        }

        ReleaseCloseAwaiter();
    }

    private void ReleaseCloseAwaiter()
    {
        if (_closeAwaiter == null)
            return;

        if (_closeAwaiter.IsCancellationRequested == false)
        {
            _closeAwaiter.Cancel();
        }

        _closeAwaiter.Dispose();
        _closeAwaiter = null;
    }
}
