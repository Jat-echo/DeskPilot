using System.Security.Cryptography;
using System.Text;

namespace DeskPilot.Infrastructure.Security;

public static class SecureStorage
{
    public static void Save(string key, string value)
    {
        var data = Encoding.UTF8.GetBytes(value);
        var encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
        var base64 = Convert.ToBase64String(encrypted);

        var settings = LoadAll();
        settings[key] = base64;
        SaveAll(settings);
    }

    public static string? Load(string key)
    {
        var settings = LoadAll();
        if (settings.TryGetValue(key, out var base64))
        {
            var encrypted = Convert.FromBase64String(base64);
            var data = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(data);
        }
        return null;
    }

    public static void Delete(string key)
    {
        var settings = LoadAll();
        settings.Remove(key);
        SaveAll(settings);
    }

    private static Dictionary<string, string> LoadAll()
    {
        var path = GetSettingsPath();
        if (!File.Exists(path))
            return new Dictionary<string, string>();

        var json = File.ReadAllText(path);
        return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json)
               ?? new Dictionary<string, string>();
    }

    private static void SaveAll(Dictionary<string, string> settings)
    {
        var path = GetSettingsPath();
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var json = System.Text.Json.JsonSerializer.Serialize(settings);
        File.WriteAllText(path, json);
    }

    private static string GetSettingsPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(appData, "DeskPilot", "secure.dat");
    }
}
