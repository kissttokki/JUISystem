// UI 프리팹과 기반 스크립트를 자동 생성하는 에디터 창입니다.
#if UNITASK_ENABLED
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public sealed class UICreatorEditor : EditorWindow
{
    private const string WindowTitle = "UI Creator";
    private const string SettingsFileName = "UICreatorEditorSettings.json";
    private const string PendingScriptNameKey = "JUISystem.UICreatorEditor.PendingScriptName";
    private const string PendingPrefabPathKey = "JUISystem.UICreatorEditor.PendingPrefabPath";
    private const string PendingBaseTypeKey = "JUISystem.UICreatorEditor.PendingBaseType";
    private const string DefaultPanelPrefabPath = "Assets/Resources/UI/Panels";
    private const string DefaultPopupPrefabPath = "Assets/Resources/UI/Popups";
    private const string DefaultPanelScriptPath = "Assets/Scripts/UI/Panels";
    private const string DefaultPopupScriptPath = "Assets/Scripts/UI/Popups";

    private string _panelPrefabSavePath;
    private string _popupPrefabSavePath;
    private string _panelScriptSavePath;
    private string _popupScriptSavePath;
    private string _scriptName;
    private UIBaseType _baseType;

    private static string SettingsPath => Path.Combine(ProjectPath, "Custom", "JUISystem", SettingsFileName);

    private static string ProjectPath => Path.GetDirectoryName(Application.dataPath)?.Replace('\\', '/');

    private enum UIBaseType
    {
        Panel,
        Popup,
    }

    [MenuItem("Tools/JUISystem/UI Creator")]
    public static void Open()
    {
        var window = GetWindow<UICreatorEditor>(WindowTitle);
        window.minSize = new Vector2(420f, 220f);
        window.Show();
    }

    private void OnEnable()
    {
        LoadSettings();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("UI Prefab Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        _baseType = (UIBaseType)EditorGUILayout.EnumPopup("UI Type", _baseType);
        _scriptName = FilterEnglishLetters(EditorGUILayout.TextField("Script Name", _scriptName));

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Panel Paths", EditorStyles.boldLabel);
        DrawPathField("Prefab Save Path", ref _panelPrefabSavePath);
        DrawPathField("Script Save Path", ref _panelScriptSavePath);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Popup Paths", EditorStyles.boldLabel);
        DrawPathField("Prefab Save Path", ref _popupPrefabSavePath);
        DrawPathField("Script Save Path", ref _popupScriptSavePath);

        if (EditorGUI.EndChangeCheck())
        {
            SaveSettings();
        }

        EditorGUILayout.Space();
        DrawPreview();

        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(_scriptName)))
        {
            if (GUILayout.Button("Create UI Prefab"))
            {
                CreateUI();
            }
        }
    }

    private static void DrawPathField(string label, ref string path)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            path = EditorGUILayout.TextField(label, path);

            if (GUILayout.Button("Select", GUILayout.Width(60f)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel(label, Application.dataPath, string.Empty);
                if (string.IsNullOrEmpty(selectedPath) == false)
                {
                    path = ConvertToAssetPath(selectedPath);
                }
            }
        }
    }

    private void CreateUI()
    {
        string scriptName = GetCompletedScriptName();
        if (IsValidScriptName(scriptName) == false)
        {
            EditorUtility.DisplayDialog(WindowTitle, "스크립트 이름은 영문자만 입력할 수 있습니다.", "확인");
            return;
        }

        string prefabFolderPath = _baseType == UIBaseType.Panel ? _panelPrefabSavePath : _popupPrefabSavePath;
        string scriptFolderPath = _baseType == UIBaseType.Panel ? _panelScriptSavePath : _popupScriptSavePath;
        prefabFolderPath = NormalizeAssetPath(prefabFolderPath);
        scriptFolderPath = NormalizeAssetPath(scriptFolderPath);

        if (string.IsNullOrEmpty(prefabFolderPath))
        {
            EditorUtility.DisplayDialog(WindowTitle, "프리팹 저장 경로는 Assets 폴더 안이어야 합니다.", "확인");
            return;
        }

        if (string.IsNullOrEmpty(scriptFolderPath))
        {
            EditorUtility.DisplayDialog(WindowTitle, "스크립트 저장 경로는 Assets 폴더 안이어야 합니다.", "확인");
            return;
        }

        string scriptPath = $"{scriptFolderPath}/{scriptName}.cs";
        string prefabPath = $"{prefabFolderPath}/{scriptName}.prefab";
        if (AssetExists(scriptPath) || AssetExists(prefabPath))
        {
            EditorUtility.DisplayDialog(WindowTitle, "생성 대상 경로에 이미 같은 이름의 리소스가 있습니다.", "확인");
            return;
        }

        SaveSettings();
        EnsureFolder(prefabFolderPath);
        EnsureFolder(scriptFolderPath);

        string baseTypeName = _baseType == UIBaseType.Panel ? "UIPanelBase" : "UIPopupBase";

        File.WriteAllText(scriptPath, CreateScriptContent(scriptName, baseTypeName), Encoding.UTF8);
        AssetDatabase.ImportAsset(scriptPath);
        AssetDatabase.Refresh();

        RequestPrefabCreation(scriptName, prefabPath, _baseType);
        TryCreatePendingPrefab();
    }

    private void LoadSettings()
    {
        var settings = new UICreatorEditorSettings();
        if (File.Exists(SettingsPath))
        {
            string json = File.ReadAllText(SettingsPath, Encoding.UTF8);
            settings = JsonUtility.FromJson<UICreatorEditorSettings>(json) ?? settings;
        }

        _panelPrefabSavePath = string.IsNullOrWhiteSpace(settings.PanelPrefabSavePath) ? DefaultPanelPrefabPath : settings.PanelPrefabSavePath;
        _popupPrefabSavePath = string.IsNullOrWhiteSpace(settings.PopupPrefabSavePath) ? DefaultPopupPrefabPath : settings.PopupPrefabSavePath;
        _panelScriptSavePath = string.IsNullOrWhiteSpace(settings.PanelScriptSavePath) ? DefaultPanelScriptPath : settings.PanelScriptSavePath;
        _popupScriptSavePath = string.IsNullOrWhiteSpace(settings.PopupScriptSavePath) ? DefaultPopupScriptPath : settings.PopupScriptSavePath;
    }

    private void SaveSettings()
    {
        var settings = new UICreatorEditorSettings
        {
            PanelPrefabSavePath = NormalizeAssetPath(_panelPrefabSavePath),
            PopupPrefabSavePath = NormalizeAssetPath(_popupPrefabSavePath),
            PanelScriptSavePath = NormalizeAssetPath(_panelScriptSavePath),
            PopupScriptSavePath = NormalizeAssetPath(_popupScriptSavePath),
        };

        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
        File.WriteAllText(SettingsPath, JsonUtility.ToJson(settings, true), Encoding.UTF8);
    }

    private void DrawPreview()
    {
        string scriptName = GetCompletedScriptName();
        string prefabFolderPath = NormalizeAssetPath(_baseType == UIBaseType.Panel ? _panelPrefabSavePath : _popupPrefabSavePath);
        string scriptFolderPath = NormalizeAssetPath(_baseType == UIBaseType.Panel ? _panelScriptSavePath : _popupScriptSavePath);

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

            if (IsValidScriptName(scriptName) == false)
            {
                EditorGUILayout.HelpBox("스크립트 이름을 입력하면 생성될 파일 경로를 미리 볼 수 있습니다.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("Script", string.IsNullOrEmpty(scriptFolderPath) ? "Invalid Script Path" : $"{scriptFolderPath}/{scriptName}.cs");
            EditorGUILayout.LabelField("Prefab", string.IsNullOrEmpty(prefabFolderPath) ? "Invalid Prefab Path" : $"{prefabFolderPath}/{scriptName}.prefab");
        }
    }

    private string GetCompletedScriptName()
    {
        return CompleteScriptName(_scriptName, _baseType);
    }

    private static string CompleteScriptName(string scriptName, UIBaseType baseType)
    {
        if (string.IsNullOrWhiteSpace(scriptName))
            return string.Empty;

        string completedName = scriptName.Trim();
        if (completedName.StartsWith("UI", StringComparison.Ordinal) == false)
        {
            completedName = $"UI{completedName}";
        }

        string suffix = baseType == UIBaseType.Panel ? "Panel" : "Popup";
        if (completedName.EndsWith(suffix, StringComparison.Ordinal) == false)
        {
            completedName = $"{completedName}{suffix}";
        }

        return completedName;
    }

    private static string CreateScriptContent(string scriptName, string baseTypeName)
    {
        return $"using UnityEngine;\n\npublic class {scriptName} : {baseTypeName}<{scriptName}>\n{{\n\n}}\n";
    }

    private static void RequestPrefabCreation(string scriptName, string prefabPath, UIBaseType baseType)
    {
        EditorPrefs.SetString(PendingScriptNameKey, scriptName);
        EditorPrefs.SetString(PendingPrefabPathKey, prefabPath);
        EditorPrefs.SetString(PendingBaseTypeKey, baseType.ToString());
    }

    private static void TryCreatePendingPrefab()
    {
        string scriptName = EditorPrefs.GetString(PendingScriptNameKey, string.Empty);
        string prefabPath = EditorPrefs.GetString(PendingPrefabPathKey, string.Empty);
        string baseTypeText = EditorPrefs.GetString(PendingBaseTypeKey, string.Empty);

        if (string.IsNullOrEmpty(scriptName) || string.IsNullOrEmpty(prefabPath) || string.IsNullOrEmpty(baseTypeText))
            return;

        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            return;

        if (Enum.TryParse(baseTypeText, out UIBaseType baseType) == false)
        {
            ClearPendingPrefabCreation();
            return;
        }

        Type scriptType = FindType(scriptName);
        if (scriptType == null)
            return;

        Type requiredBaseType = baseType == UIBaseType.Panel ? typeof(UIPanelBase) : typeof(UIPopupBase);
        if (requiredBaseType.IsAssignableFrom(scriptType) == false)
        {
            Debug.LogError($"{scriptName} 스크립트가 {requiredBaseType.Name} 타입을 상속하지 않습니다.");
            ClearPendingPrefabCreation();
            return;
        }

        CreatePrefab(scriptName, prefabPath, scriptType, baseType);
        ClearPendingPrefabCreation();
    }

    private static void CreatePrefab(string prefabName, string prefabPath, Type componentType, UIBaseType baseType)
    {
        EnsureFolder(Path.GetDirectoryName(prefabPath)?.Replace('\\', '/'));

        GameObject prefabObject = new GameObject(prefabName, typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasRenderer), typeof(Image));
        prefabObject.AddComponent(componentType);

        RectTransform rectTransform = prefabObject.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;

        if (baseType == UIBaseType.Panel)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        PrefabUtility.SaveAsPrefabAsset(prefabObject, prefabPath);
        DestroyImmediate(prefabObject);
        AssetDatabase.Refresh();

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);

        Debug.Log($"UI 프리팹 생성 완료: {prefabPath}");
    }

    private static Type FindType(string typeName)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(typeName);
            if (type != null)
                return type;
        }

        return null;
    }

    private static bool IsValidScriptName(string scriptName)
    {
        if (string.IsNullOrWhiteSpace(scriptName))
            return false;

        for (int i = 0; i < scriptName.Length; i++)
        {
            if (IsEnglishLetter(scriptName[i]) == false)
                return false;
        }

        return true;
    }

    private static string FilterEnglishLetters(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var builder = new StringBuilder(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            if (IsEnglishLetter(value[i]))
            {
                builder.Append(value[i]);
            }
        }

        return builder.ToString();
    }

    private static bool IsEnglishLetter(char value)
    {
        return (value >= 'A' && value <= 'Z') || (value >= 'a' && value <= 'z');
    }

    private static bool AssetExists(string assetPath)
    {
        return string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(assetPath)) == false || File.Exists(assetPath);
    }

    private static string ConvertToAssetPath(string fullPath)
    {
        string normalizedFullPath = fullPath.Replace('\\', '/');
        string normalizedDataPath = Application.dataPath.Replace('\\', '/');

        if (normalizedFullPath.StartsWith(normalizedDataPath, StringComparison.OrdinalIgnoreCase) == false)
            return string.Empty;

        return "Assets" + normalizedFullPath.Substring(normalizedDataPath.Length);
    }

    private static string NormalizeAssetPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        string normalizedPath = path.Trim().Replace('\\', '/');
        if (normalizedPath.StartsWith("Assets", StringComparison.Ordinal) == false)
            return string.Empty;

        return normalizedPath.TrimEnd('/');
    }

    private static void EnsureFolder(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath) || AssetDatabase.IsValidFolder(folderPath))
            return;

        string[] folders = folderPath.Split('/');
        string currentPath = folders[0];

        for (int i = 1; i < folders.Length; i++)
        {
            string nextPath = $"{currentPath}/{folders[i]}";
            if (AssetDatabase.IsValidFolder(nextPath) == false)
            {
                AssetDatabase.CreateFolder(currentPath, folders[i]);
            }

            currentPath = nextPath;
        }
    }

    private static void ClearPendingPrefabCreation()
    {
        EditorPrefs.DeleteKey(PendingScriptNameKey);
        EditorPrefs.DeleteKey(PendingPrefabPathKey);
        EditorPrefs.DeleteKey(PendingBaseTypeKey);
    }

    [InitializeOnLoadMethod]
    private static void OnScriptsReloaded()
    {
        EditorApplication.delayCall += TryCreatePendingPrefab;
    }

    [Serializable]
    private sealed class UICreatorEditorSettings
    {
        public string PanelPrefabSavePath = DefaultPanelPrefabPath;
        public string PopupPrefabSavePath = DefaultPopupPrefabPath;
        public string PanelScriptSavePath = DefaultPanelScriptPath;
        public string PopupScriptSavePath = DefaultPopupScriptPath;
    }
}
#endif
