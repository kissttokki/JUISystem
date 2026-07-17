#if UNITASK_ENABLED
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button, IPointerClickHandler, ISubmitHandler
{
    private const float LONGCLICK_THRESHOLD = 0.5f; // 롱클릭 판정 시간(초)

    private bool _isClicked = false;

    [SerializeField] private UnityEvent _onLongClick;
    public UnityEvent OnLongClick => _onLongClick;

    [SerializeField] private UnityEvent _onMouseOver;
    public UnityEvent OnMouseOver => _onMouseOver;

    [SerializeField] private UnityEvent _onMouseLeave;
    public UnityEvent OnMouseLeave => _onMouseLeave;

    private bool _isPointerDown = false;
    private bool _longClickTriggered = false;
    private CancellationTokenSource _cts;


    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (IsActive() == false || IsInteractable() == false)
            return;

        _onMouseOver?.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (IsActive() == false || IsInteractable() == false)
            return;

        _onMouseLeave?.Invoke();
    }
   
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        base.OnPointerDown(eventData);

        _isPointerDown = true;
        _longClickTriggered = false;
        _cts = new CancellationTokenSource();
        CheckLongClickAsync(_cts.Token).Forget();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        base.OnPointerUp(eventData);

        _isPointerDown = false;
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    public override async void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // 롱클릭이 이미 발생했다면 일반 클릭은 무시
        if (_longClickTriggered)
            return;

        if (_isClicked) return;
        _isClicked = true;
        await HandleClickAsync(eventData);
    }

    private async UniTaskVoid CheckLongClickAsync(CancellationToken token)
    {
        try
        {
            float elapsed = 0f;
            while (elapsed < LONGCLICK_THRESHOLD)
            {
                if (_isPointerDown == false)
                    return;
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                elapsed += Time.unscaledDeltaTime;
            }

            // 롱클릭 판정
            _longClickTriggered = true;
            _onLongClick?.Invoke();
        }
        catch (OperationCanceledException)
        {
            // 취소 시 아무것도 하지 않음
        }
    }

    private async UniTask HandleClickAsync(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        base.OnPointerClick(eventData);

        await UniTask.Yield();
        _isClicked = false;
    }
}
#endif