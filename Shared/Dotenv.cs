using System.Collections;

namespace gs.shared;

public static class Dotenv
{
    static IReadOnlyDictionary<string?, string?>? Cache;
    private static readonly char[] separator = ['='];

    public static string? Get(string key) => Cache.TryGetValue(key, out string v) ? v : null;

    public static IEnumerable<string?> Gets(params string[] keys) => keys.Select(k => Get(k));

    public static void Load(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path) + "is empty");

        if (!File.Exists(path))
            throw new FileNotFoundException(nameof(path));

        foreach (string line in File.ReadAllLines(path))
        {
            string[] tokens = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length != 2)
                continue;

            Environment.SetEnvironmentVariable(tokens[0], tokens[1]);
        }

        Cache = Environment.GetEnvironmentVariables().Cast<DictionaryEntry>()
            .ToDictionary(e => e.Key.ToString(), e => e.Value?.ToString() ?? "");
    }
}
