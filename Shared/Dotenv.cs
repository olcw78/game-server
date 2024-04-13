using System.Collections;

namespace Shared;

public static class Dotenv {
  private static readonly Dictionary<string, string?> LoadedEnvItemDict = new();
  private static readonly char[] Separator = ['=',];
  private static readonly char[] TrimCharArr = ['\n', '\t', '\b', '\0',];

  public static string? Get(string key) =>
    LoadedEnvItemDict.GetValueOrDefault(key);

  public static IEnumerable<string?> Gets(params string[] keys) =>
    keys.Select(Get);

  public static int? GetI(string key) {
    string? v = Get(key);
    if (v == null) return null;
    if (int.TryParse(v, out int ret)) return ret;
    return null;
  }

  public static float? GetF(string key) {
    string? v = Get(key);
    if (v == null) return null;
    if (float.TryParse(v, out float ret)) return ret;
    return null;
  }

  public static void Load(string path, bool loadSystemEnv = false) {
    if (string.IsNullOrEmpty(path))
      throw new ArgumentNullException(nameof(path));

    if (!File.Exists(path)) throw new FileNotFoundException(nameof(path));

    foreach (string line in File.ReadAllLines(path)) {
      string[] tokens
        = line.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

      if (tokens.Length != 2)
        continue;

      string k = tokens[0].Trim().Trim(TrimCharArr);
      string v = tokens[1].Trim().Trim(TrimCharArr);

      Environment.SetEnvironmentVariable(k, v);
      if (!loadSystemEnv) {
        LoadedEnvItemDict.Add(k, v);
      }
    }

    if (loadSystemEnv) {
      foreach (DictionaryEntry e in Environment.GetEnvironmentVariables()) {
        string? k = e.Key.ToString();
        if (string.IsNullOrEmpty(k)) continue;

        string? v = e.Value?.ToString();
        if (string.IsNullOrEmpty(v)) continue;

        LoadedEnvItemDict.Add(k, v);
      }
    }
  }
}