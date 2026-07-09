#if UNITASK_ENABLED
using UnityEngine;

public class MainCanvasSetter : MonoBehaviour
{

    [field: SerializeField] public Canvas Canvas { get; private set; }
    [field: SerializeField] public Transform PanelParent { get; private set; }
    [field: SerializeField] public Transform PopupParent { get; private set; }
    [field: SerializeField] public Transform ToastParent { get; private set; }
    

#if UNITY_EDITOR
    private void OnValidate()
    {
        Canvas = GetComponent<Canvas>();
    }
#endif

    private void Awake()
    {
        UIManager.Instance.MainCanvas = this;
    }
}
#endif