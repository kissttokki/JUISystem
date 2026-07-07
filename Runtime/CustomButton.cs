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
    public enum ClickSoundType
    {
        None,
        Ui_Button_Common,
        Ui_Button_Tap,
        Ui_GetReward,
        Sfx_PackBall,
        Sfx_Defeat,
        Sfx_Victory,
        Sfx_Excavation_Break_1,
        Sfx_Excavator_Pickax,
        Hit_Skill_DragonBreath_6,
        UI_Level_up,
        Hit_Basic_swish_01,
        Ui_Button_Confirm,
    }

    private const float LONGCLICK_THRESHOLD = 0.5f; // 롱클릭 판정 시간(초)

    [SerializeField] private ClickSoundType _clickSoundType = ClickSoundType.Ui_Button_Common;
    [SerializeField] private Image _costItemImage;
    [SerializeField] private TMP_Text _costLabel;

    public Image CostItemImage => _costItemImage;
    public TMP_Text CostLabel => _costLabel;


    private bool _isClicked = false;

    [SerializeField] private UnityEvent _onLongClick;
    public UnityEvent OnLongClick => _onLongClick;

    private bool _isPointerDown = false;
    private bool _longClickTriggered = false;
    private CancellationTokenSource _cts;

    public void SetCost(Sprite sprite, int cost)
    {
        _costLabel.text = cost.ToString();

        if(_costItemImage != null)
            _costItemImage.sprite = sprite;
    }


    public void SetCost(int cost)
    {
        _costLabel.text = cost.ToString();
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

        if(_clickSoundType != ClickSoundType.None && this.interactable == true)
        {
            //AudioManager.Instance.PlaySFX(_clickSoundType.ToString());
        }
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
