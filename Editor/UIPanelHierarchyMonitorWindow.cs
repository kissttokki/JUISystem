// UIPanel 계층 상태를 에디터에서 확인하는 모니터 창입니다.
#if UNITASK_ENABLED
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public sealed class UIPanelHierarchyMonitorWindow : EditorWindow
{
    private const string WindowTitle = "UIPanel Monitor";
    private static readonly FieldInfo s_panelListField = typeof(UIPanelBase).GetField("s_panelList", BindingFlags.Static | BindingFlags.NonPublic);

    private Vector2 _scrollPosition;
    private bool _autoRefresh = true;

    [MenuItem("Tools/JUISystem/UIPanel Monitor")]
    public static void Open()
    {
        var window = GetWindow<UIPanelHierarchyMonitorWindow>(WindowTitle);
        window.minSize = new Vector2(360f, 240f);
        window.Show();
    }

    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnGUI()
    {
        DrawToolbar();

        if (Application.isPlaying == false)
        {
            EditorGUILayout.HelpBox("UIPanel 모니터는 Play Mode에서 패널 계층을 표시합니다.", MessageType.Info);
            return;
        }

        IReadOnlyList<UIPanelBase> panels = GetPanels();
        UIPanelBase currentPanel = UIPanelBase.CurrentPanel;

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("Current Panel", EditorStyles.boldLabel);
            DrawPanelObjectField(currentPanel);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Panel Stack ({panels.Count})", EditorStyles.boldLabel);

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        for (int i = panels.Count - 1; i >= 0; i--)
        {
            DrawPanelRow(panels[i], i, panels[i] == currentPanel);
        }
        EditorGUILayout.EndScrollView();
    }

    private void DrawToolbar()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            _autoRefresh = GUILayout.Toggle(_autoRefresh, "Auto Refresh", EditorStyles.toolbarButton, GUILayout.Width(100f));

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70f)))
            {
                Repaint();
            }

            GUILayout.FlexibleSpace();
        }
    }

    private void DrawPanelRow(UIPanelBase panel, int stackIndex, bool isCurrent)
    {
        if (panel == null)
            return;

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                string title = isCurrent ? $"▶ [{stackIndex}] {panel.name}" : $"  [{stackIndex}] {panel.name}";
                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

                if (GUILayout.Button("Select", GUILayout.Width(60f)))
                {
                    Selection.activeObject = panel.gameObject;
                    EditorGUIUtility.PingObject(panel.gameObject);
                }
            }

            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Type", panel.GetType().Name);
            EditorGUILayout.LabelField("Active", panel.gameObject.activeSelf.ToString());
            EditorGUILayout.LabelField("Path", GetTransformPath(panel.transform));
            EditorGUI.indentLevel--;
        }
    }

    private void DrawPanelObjectField(UIPanelBase panel)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.ObjectField(panel, typeof(UIPanelBase), true);

            using (new EditorGUI.DisabledScope(panel == null))
            {
                if (GUILayout.Button("Select", GUILayout.Width(60f)))
                {
                    Selection.activeObject = panel.gameObject;
                    EditorGUIUtility.PingObject(panel.gameObject);
                }
            }
        }
    }

    private static IReadOnlyList<UIPanelBase> GetPanels()
    {
        if (s_panelListField == null)
            return System.Array.Empty<UIPanelBase>();

        if (s_panelListField.GetValue(null) is List<UIPanelBase> panels)
            return panels;

        return System.Array.Empty<UIPanelBase>();
    }

    private static string GetTransformPath(Transform target)
    {
        if (target == null)
            return string.Empty;

        string path = target.name;
        Transform parent = target.parent;

        while (parent != null)
        {
            path = $"{parent.name}/{path}";
            parent = parent.parent;
        }

        return path;
    }

    private void OnEditorUpdate()
    {
        if (_autoRefresh && Application.isPlaying)
        {
            Repaint();
        }
    }
}
#endif
