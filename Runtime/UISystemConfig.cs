using UnityEngine;

public static class UISystemConfig
{
    private const string DefaultResourcesPath = "UI";
    private const string DefaultAddressablePath = "AddressableAssets/Remote/UI";
    private const string SettingsResourceName = "UISystemSettings";

    public static string RESOURCES_PATH => LoadSettings().resourcesPath;

    public static string ADDRESSABLE_PATH => LoadSettings().addressablePath;

    private static UISystemConfigData LoadSettings()
    {
        var settingsAsset = Resources.Load<TextAsset>(SettingsResourceName);
        if (settingsAsset == null || string.IsNullOrWhiteSpace(settingsAsset.text))
            return new UISystemConfigData(DefaultResourcesPath, DefaultAddressablePath);

        var settings = JsonUtility.FromJson<UISystemConfigData>(settingsAsset.text);
        if (settings == null)
            return new UISystemConfigData(DefaultResourcesPath, DefaultAddressablePath);

        if (string.IsNullOrWhiteSpace(settings.resourcesPath))
            settings.resourcesPath = DefaultResourcesPath;

        if (string.IsNullOrWhiteSpace(settings.addressablePath))
            settings.addressablePath = DefaultAddressablePath;

        return settings;
    }
}

[System.Serializable]
public sealed class UISystemConfigData
{
    public string resourcesPath;
    public string addressablePath;

    public UISystemConfigData(string resourcesPath, string addressablePath)
    {
        this.resourcesPath = resourcesPath;
        this.addressablePath = addressablePath;
    }
}
