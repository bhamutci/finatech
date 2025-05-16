namespace FinaTech.Core.Configuration;

using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;


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
    /// Retrieves an <see cref="IConfigurationRoot"/> instance, either from the cache or by building it
    /// based on the specified configuration file path and optional environment name.
    /// </summary>
    /// <param name="path">The directory path containing the configuration file.</param>
    /// <param name="environmentName">An optional environment name to use when loading environment-specific configurations.</param>
    /// <returns>An <see cref="IConfigurationRoot"/> instance containing the application configuration settings.</returns>
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
