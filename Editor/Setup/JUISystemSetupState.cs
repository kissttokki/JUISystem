using UnityEditor;

[FilePath("ProjectSettings/JUISystemSetupState.asset", FilePathAttribute.Location.ProjectFolder)]
internal sealed class JUISystemSetupState : ScriptableSingleton<JUISystemSetupState>
{
    public bool HasShownWizard;

    public void MarkWizardShown()
    {
        HasShownWizard = true;
        Save(true);
    }
}
