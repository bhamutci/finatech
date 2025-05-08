using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;

namespace FinaTech.Core;

/// <summary>
/// Provides mechanisms to retrieve and manage application configuration settings.
/// </summary>
public class ApplicationConfigurations
{
    private static readonly ConcurrentDictionary<string, IConfigurationRoot> ConfigurationCache;

    /// <summary>
    /// Represents a utility class for retrieving and managing application configuration data.
    /// This class supports caching configurations for optimized retrieval and provides
    /// functionality to build configurations based on application settings files and environment variables.
    /// </summary>
    static ApplicationConfigurations()
    {
        ConfigurationCache = new ConcurrentDictionary<string, IConfigurationRoot>();
    }

    /// <summary>
    /// Retrieves a cached application configuration or builds a new one if not already cached.
    /// </summary>
    /// <param name="path">The base path where the configuration file is located.</param>
    /// <param name="environmentName">The optional name of the environment for environment-specific configuration files.</param>
    /// <param name="addUserSecrets">Indicates whether user secrets should be included in the configuration.</param>
    /// <returns>The application configuration as an <see cref="IConfigurationRoot"/> instance.</returns>
    public static IConfigurationRoot Get(string path, string? environmentName = null)
    {
        var cacheKey = path + "#" + environmentName + "#";
        return ConfigurationCache.GetOrAdd(
            cacheKey,
            _ => BuildConfiguration(path, environmentName)
        );
    }

    /// <summary>
    /// Constructs a new application configuration by combining settings from
    /// JSON files, environment variables, and optionally user secrets.
    /// </summary>
    /// <param name="path">The base path where the configuration files are located.</param>
    /// <param name="environmentName">The optional name of the environment for environment-specific configuration files.</param>
    /// <returns>The constructed application configuration as an <see cref="IConfigurationRoot"/> instance.</returns>
    private static IConfigurationRoot BuildConfiguration(string path, string? environmentName = null)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        if (!string.IsNullOrWhiteSpace(environmentName))
        {
            builder = builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
        }

        builder = builder.AddEnvironmentVariables();

        return builder.Build();
    }
}
