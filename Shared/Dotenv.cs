using System.Collections;

namespace gs.shared;

public static class Dotenv {
  static readonly Dictionary<string, string?> Cache = new();
  private static readonly char[] separator = ['='];

  public static string? Get(string key) {
    if (Cache == null) return "";
    if (Cache.TryGetValue(key, out string? v))
      return v;
    return null;
  }

  public static IEnumerable<string?> Gets(params string[] keys)
    => keys.Select(Get);

  public static int? GetI(string key) {
    if (!int.TryParse(Get(key), out int val))
      return null;

    return val;
  }

  public static void Load(string path) {
    if (string.IsNullOrEmpty(path))
      throw new ArgumentNullException(nameof(path));

    if (!File.Exists(path))
      throw new FileNotFoundException(nameof(path));

    foreach (string line in File.ReadAllLines(path)) {
      string[] tokens = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);

      if (tokens.Length != 2)
        continue;

      Environment.SetEnvironmentVariable(tokens[0], tokens[1]);
    }

    foreach (DictionaryEntry e in Environment.GetEnvironmentVariables()) {
      string? k = e.Key.ToString();
      if (string.IsNullOrEmpty(k))
        continue;

      string? v = e.Value?.ToString();
      if (string.IsNullOrEmpty(v))
        continue;

      Cache.Add(k, v);
    }
  }
}