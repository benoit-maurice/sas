﻿using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using sas.Configurations;

namespace sas.configurations.azure;

public class FromKeyVaultConfiguration : IEnrichConfiguration
{
    private static readonly InteractiveBrowserCredential InteractiveBrowserCredential =
        new ();
    
    private readonly string _keyVaultName;

    public FromKeyVaultConfiguration(string keyVaultName)
    {
        _keyVaultName = keyVaultName;
    }
    
    public void Enrich(ConfigurationBuilder builder)
    {
        TokenCredential credentials = IsRunningInAzureDevOps() ? new AzureCliCredential() : InteractiveBrowserCredential;
        
        builder.AddAzureKeyVault(new Uri($"https://{_keyVaultName}.vault.azure.net/"), credentials);
    }

    private static bool IsRunningInAzureDevOps()
    {
        var tfBuild = Environment.GetEnvironmentVariable("TF_BUILD");
        return !string.IsNullOrEmpty(tfBuild);
    }
}