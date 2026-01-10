using System;

namespace ImperialBackend.Config;

public static class SecretReader
{
    // Reads KEY_FILE first (path to secret file), then KEY.
    public static string Get(string key, bool required = true, string? defaultValue = null)
    {
        var filePath = Environment.GetEnvironmentVariable($"{key}_FILE");
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            if (!File.Exists(filePath))
                throw new InvalidOperationException(
                    $"Secret file not found for {key}_FILE: {filePath}"
                );

            var value = File.ReadAllText(filePath).Trim();
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            if (required)
                throw new InvalidOperationException(
                    $"Secret file empty for {key}_FILE: {filePath}"
                );

            return defaultValue ?? "";
        }

        var env = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrWhiteSpace(env))
            return env;

        if (!required)
            return defaultValue ?? "";

        throw new InvalidOperationException($"Missing {key} (or {key}_FILE).");
    }
}
