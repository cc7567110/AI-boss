using System.Text.Json;
using AIBoss.Models;

namespace AIBoss.Services;

public sealed class LocalDataService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private readonly string _dataDirectory;

    public LocalDataService()
    {
        var customDirectory = Environment.GetEnvironmentVariable("AIBOSS_DATA_DIR");
        _dataDirectory = string.IsNullOrWhiteSpace(customDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AIBoss")
            : customDirectory;
    }

    public string DataDirectory => _dataDirectory;
    public string DataFilePath => Path.Combine(_dataDirectory, "ai-boss-data.json");

    public AppData Load()
    {
        if (!File.Exists(DataFilePath))
        {
            return new AppData();
        }

        try
        {
            var json = File.ReadAllText(DataFilePath);
            var data = JsonSerializer.Deserialize<AppData>(json, JsonOptions) ?? new AppData();
            data.Normalize();
            return data;
        }
        catch (JsonException)
        {
            var corruptPath = Path.Combine(_dataDirectory, $"ai-boss-data-corrupt-{DateTime.Now:yyyyMMdd-HHmmss}.json");
            File.Copy(DataFilePath, corruptPath, overwrite: true);
            return new AppData();
        }
    }

    public void Save(AppData data)
    {
        Directory.CreateDirectory(_dataDirectory);
        var temporaryFile = DataFilePath + ".tmp";
        File.WriteAllText(temporaryFile, Serialize(data));
        File.Move(temporaryFile, DataFilePath, overwrite: true);
    }

    public string Serialize(AppData data) => JsonSerializer.Serialize(data, JsonOptions);
}
