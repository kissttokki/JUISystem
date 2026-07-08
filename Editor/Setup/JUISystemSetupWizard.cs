using UnityEditor;
using System.IO;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

internal sealed class JUISystemSetupWizard : EditorWindow
{
    private const string WindowTitle = "JUISystem Setup";
    private const string UniTaskPackageName = "com.cysharp.unitask";
    private const string UniTaskGitUrl = "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask";

    private AddRequest _addRequest;
    private ListRequest _listRequest;
    private string _statusMessage;
    private bool _isUniTaskInstalled;
    private string _resourcesPath;
    private string _addressablePath;
    private string _settingsAssetPath;

    [MenuItem("Tools/JUISystem/Setup Wizard")]
    public static void Open()
    {
        var window = GetWindow<JUISystemSetupWizard>(true, WindowTitle);
        window.minSize = new Vector2(460f, 220f);
        window.RefreshStatus();
        window.Show();
    }

    private void OnEnable()
    {
        RefreshStatus();
        LoadSettings();
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnGUI()
    {
        GUILayout.Space(8f);
      

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("UISystem Paths", EditorStyles.boldLabel);

            _resourcesPath = EditorGUILayout.TextField("Resources Path", _resourcesPath);
            _addressablePath = EditorGUILayout.TextField("Addressable Path", _addressablePath);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save Settings"))
                {
                    SaveSettings();
                }

                if (GUILayout.Button("Reload Settings"))
                {
                    LoadSettings();
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("JUISystem Setup Wizard", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "Checks whether UniTask is installed and allows installing it if needed.",
            MessageType.Info);

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("UniTask Status", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(_statusMessage ?? "Checking status...");
        }

        EditorGUILayout.Space();
        using (new EditorGUI.DisabledScope(_isUniTaskInstalled || _addRequest != null))
        {
            if (GUILayout.Button("Install UniTask", GUILayout.Height(30f)))
            {
                InstallUniTask();
            }
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Refresh"))
            {
                RefreshStatus();
            }

            if (GUILayout.Button("Close"))
            {
                Close();
            }
        }
    }

    private void InstallUniTask()
    {
        if (_addRequest != null || _listRequest != null)
            return;
        _statusMessage = "Requesting UniTask installation...";
        _addRequest = Client.Add(UniTaskGitUrl);
        Repaint();
    }

    private void OnEditorUpdate()
    {
        if (_listRequest != null && _listRequest.IsCompleted)
        {
            if (_listRequest.Status == StatusCode.Success)
            {
                _isUniTaskInstalled = false;

                foreach (var package in _listRequest.Result)
                {
                    if (package.name == UniTaskPackageName)
                    {
                        _isUniTaskInstalled = true;
                        break;
                    }
                }

                _statusMessage = _isUniTaskInstalled
                    ? "UniTask is already installed."
                    : "UniTask is not installed.";
            }
            else if (_listRequest.Status >= StatusCode.Failure)
            {
                _statusMessage = $"Failed to list packages: {_listRequest.Error.message}";
                _isUniTaskInstalled = false;
            }

            _listRequest = null;
            Repaint();
        }

        if (_addRequest == null || _addRequest.IsCompleted == false)
            return;
        
        if (_addRequest.Status == StatusCode.Success)
        {
            _statusMessage = $"UniTask installation completed: {_addRequest.Result.displayName} ({_addRequest.Result.version})";
        }
        else if (_addRequest.Error != null && _addRequest.Status >= StatusCode.Failure)
        {
            _statusMessage = $"UniTask installation failed: {_addRequest.Error.message}";
        }

        _addRequest = null;
        RefreshStatus();
        Repaint();
    }

    private void RefreshStatus()
    {
        if (_addRequest != null)
            return;

        _statusMessage = "Checking package list...";
        _listRequest = Client.List(true, false);
        Repaint();
    }

    private void LoadSettings()
    {
        _settingsAssetPath = GetSettingsAssetPath();

        if (!File.Exists(_settingsAssetPath))
        {
            _resourcesPath = "UI";
            _addressablePath = "AddressableAssets/Remote/UI";
            return;
        }

        var json = File.ReadAllText(_settingsAssetPath);
        if (string.IsNullOrWhiteSpace(json))
        {
            _resourcesPath = "UI";
            _addressablePath = "AddressableAssets/Remote/UI";
            return;
        }

        var settings = JsonUtility.FromJson<SetupWizardConfigData>(json);
        _resourcesPath = string.IsNullOrWhiteSpace(settings?.resourcesPath) ? "UI" : settings.resourcesPath;
        _addressablePath = string.IsNullOrWhiteSpace(settings?.addressablePath) ? "AddressableAssets/Remote/UI" : settings.addressablePath;
    }

    private void SaveSettings()
    {
        _settingsAssetPath = GetSettingsAssetPath();
        EnsureResourcesFolder();

        var settings = new SetupWizardConfigData(
            string.IsNullOrWhiteSpace(_resourcesPath) ? "UI" : _resourcesPath,
            string.IsNullOrWhiteSpace(_addressablePath) ? "AddressableAssets/Remote/UI" : _addressablePath);

        File.WriteAllText(_settingsAssetPath, JsonUtility.ToJson(settings, true));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        LoadSettings();
    }

    private static string GetSettingsAssetPath()
    {
        return Path.Combine(Application.dataPath, "Resources", "UISystemSettings.json");
    }

    private static void EnsureResourcesFolder()
    {
        const string resourcesFolder = "Assets/Resources";
        if (!AssetDatabase.IsValidFolder(resourcesFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
    }

    [System.Serializable]
    private sealed class SetupWizardConfigData
    {
        public string resourcesPath;
        public string addressablePath;

        public SetupWizardConfigData(string resourcesPath, string addressablePath)
        {
            this.resourcesPath = resourcesPath;
            this.addressablePath = addressablePath;
        }
    }
}
