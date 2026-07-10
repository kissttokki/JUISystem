#if UNITASK_ENABLED
using System.Linq;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;


[CustomEditor(typeof(CustomButton), true)]
[CanEditMultipleObjects]
public class CustomButtonEditor : ButtonEditor
{
    private static MonoScript s_targetScript; 
    private SerializedProperty _clickSoundType;
    private SerializedProperty _costItemImage;
    private SerializedProperty _costLabel;
    private SerializedProperty _onLongClick;

    [MenuItem("CONTEXT/Button/Convert to CustomButton")]
    private static void ConvertToCustomButton(MenuCommand command)
    {
        var button = command.context as Button;
        if (button == null || button is CustomButton) return;

        var serializedObject = new SerializedObject(button);
        var scriptProperty = serializedObject.FindProperty("m_Script");

        if (s_targetScript == null)
        {
            var scriptGuids = AssetDatabase.FindAssets("t:MonoScript");

            s_targetScript = scriptGuids
                .Select(guid => AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guid)))
                .FirstOrDefault(script => script != null && script.GetClass() == typeof(CustomButton));
        }

        scriptProperty.objectReferenceValue = s_targetScript;
        serializedObject.ApplyModifiedProperties();

        Debug.Log($"스크립트가 CustomButton으로 교체되었습니다.");
    }



    protected override void OnEnable()
    {
        base.OnEnable();
        _costItemImage = serializedObject.FindProperty("_costItemImage");
        _clickSoundType = serializedObject.FindProperty("_clickSoundType");
        _costLabel = serializedObject.FindProperty("_costLabel");
        _onLongClick = serializedObject.FindProperty("_onLongClick");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.PropertyField(_clickSoundType);
        EditorGUILayout.PropertyField(_onLongClick);
        EditorGUILayout.PropertyField(_costItemImage);
        EditorGUILayout.PropertyField(_costLabel);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif