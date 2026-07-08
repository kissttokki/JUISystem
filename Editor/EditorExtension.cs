using System;
using TMPro;
using UnityEditor;
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

    [MenuItem("GameObject/UI/Create UGUI Text #T", false, 10)]
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

    [MenuItem("GameObject/UI/Create Sprite #S", false, 10)]
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

    [MenuItem("GameObject/UI/Create Button #B", false, 10)]
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

        // Text 오브젝트 생성
        GameObject textGO = new GameObject("New Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(button.transform, false);

        // Text 기본 설정
        var text = textGO.GetComponent<TMP_Text>();
        text.text = "New Text";
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.Midline;
        text.autoSizeTextContainer = true;

        var rect = textGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // 생성된 오브젝트 선택
        Selection.activeObject = (button);
    }
}
