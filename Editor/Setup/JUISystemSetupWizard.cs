using UnityEditor;
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
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnGUI()
    {
        GUILayout.Space(8f);
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
        else if (_addRequest.Status >= StatusCode.Failure)
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
}
