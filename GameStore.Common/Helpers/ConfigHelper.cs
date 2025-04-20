using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace GameStore.Common.Helpers;

public static class ConfigHelper
{
    public static string GetSecret(IWebHostEnvironment environment, IConfiguration configuration, string secret)
    {
        if (environment.IsDevelopment())
        {
            secret = secret.Replace('-', ':');
            var configValue = configuration[secret];

            if (string.IsNullOrEmpty(configValue))
            {
                throw new InvalidOperationException($"Config error: config value is not set for {secret}");
            }

            return configValue;
        }

        var environmentVariable = Environment.GetEnvironmentVariable(secret);

        if (string.IsNullOrEmpty(environmentVariable))
        {
            throw new InvalidOperationException($"Environment error: environment variable is not set for {secret}");
        }

        return environmentVariable;
    }
}
