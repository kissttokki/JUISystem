#if UNITASK_ENABLED
using TMPro;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UI;



public static class RectTransformExtension
{
    [MenuItem("CONTEXT/RectTransform/Set all RaycastTarget off")]
    private static void ConvertToCustomButton(MenuCommand command)
    {
        if (command.context is RectTransform rt)
        {
            var graphics = rt.GetComponentsInChildren<Graphic>(true);

            foreach (var graphic in graphics)
            {
                Undo.RecordObject(graphic, "Disable RaycastTarget"); 
                graphic.raycastTarget = false;
                EditorUtility.SetDirty(graphic); 
            }

            Debug.Log($"RaycastTarget OFF: {graphics.Length}개 Graphic 컴포넌트 처리됨");

        }
    }

    [MenuItem("GameObject/UI/Create UGUI Text", false, 10)]
    [Shortcut("JUISystem/Create UGUI Text", KeyCode.T, ShortcutModifiers.Shift)]
    private static void CreateTextUnderSelected()
    {
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            return;
        }

        // Canvas가 없으면 생성
        Canvas parentCanvas = selected.GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas));
            canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            selected = canvasGO;
        }

        // Text 오브젝트 생성
        GameObject textGO = new GameObject("New Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(selected.transform, false);

        // Text 기본 설정
        var text = textGO.GetComponent<TMP_Text>();
        text.text = "New Text";
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.Midline;
        text.enableAutoSizing = true;

        var rect = textGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;



        // 생성된 오브젝트 선택
        Selection.activeObject = (textGO);
    }

    [MenuItem("GameObject/UI/Create Sprite", false, 10)]
    [Shortcut("JUISystem/Create Sprite", KeyCode.S, ShortcutModifiers.Shift)]
    private static void CreateSprite()
    {
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            return;
        }

        // Canvas가 없으면 생성
        Canvas parentCanvas = selected.GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas));
            canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            selected = canvasGO;
        }

        // Text 오브젝트 생성
        GameObject textGO = new GameObject("New Sprite", typeof(RectTransform), typeof(Image));
        textGO.transform.SetParent(selected.transform, false);

        var rect = textGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // 생성된 오브젝트 선택
        Selection.activeObject = (textGO);
    }

    [MenuItem("GameObject/UI/Create Button", false, 10)]
    [Shortcut("JUISystem/Create Button", KeyCode.B, ShortcutModifiers.Shift)]
    private static void CreateButton()
    {
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            return;
        }

        // Canvas가 없으면 생성
        Canvas parentCanvas = selected.GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas));
            canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            selected = canvasGO;
        }

        // Text 오브젝트 생성
        GameObject button = new GameObject("New Button", typeof(RectTransform), typeof(CustomButton), typeof(Image));
        button.transform.SetParent(selected.transform, false);
        var buttonRect = button.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(200, 100);

        // Text 오브젝트 생성
        GameObject textGO = new GameObject("New Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(button.transform, false);

        // Text 기본 설정
        var text = textGO.GetComponent<TMP_Text>();
        text.text = "New Text";
        text.color = Color.black;
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.offsetMin = Vector2.zero;
        text.rectTransform.offsetMax = Vector2.zero;
        text.alignment = TextAlignmentOptions.Midline;
        //text.autoSizeTextContainer = true;

        // 생성된 오브젝트 선택
        Selection.activeObject = (button);
    }
}
#endif