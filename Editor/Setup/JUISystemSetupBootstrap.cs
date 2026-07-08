using UnityEditor;

[InitializeOnLoad]
internal static class JUISystemSetupBootstrap
{
    static JUISystemSetupBootstrap()
    {
        EditorApplication.delayCall += TryOpenSetupWizard;
    }

    private static void TryOpenSetupWizard()
    {
        if (JUISystemSetupState.instance.HasShownWizard)
            return;

        JUISystemSetupState.instance.MarkWizardShown();
        JUISystemSetupWizard.Open();
    }
}
