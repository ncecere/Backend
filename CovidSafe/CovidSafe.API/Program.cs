﻿using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace CovidSafe.API
{
    /// <summary>
    /// Main application class
    /// </summary>
    /// <remarks>
    /// CS1591: Ignores missing documentation warnings.
    /// CodeCoverageExclusion: Generic program entry point.
    /// </remarks>
    [ExcludeFromCodeCoverage]
#pragma warning disable CS1591
    public class Program
    {
        /// <summary>
        /// Application entry point
        /// </summary>
        /// <param name="args">Command-line arguments</param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // We need to generate the original config before we can get the Key Vault URL
                    var builtConfig = config.Build();

                    // Is there a Key Vault URL specified?
                    if(!String.IsNullOrEmpty(builtConfig["KeyVaultUrl"]))
                    {
                        // Get Managed Service Identity token
                        AzureServiceTokenProvider tokenProvider = new AzureServiceTokenProvider();
                        KeyVaultClient kvClient = new KeyVaultClient(
                            new KeyVaultClient.AuthenticationCallback(
                                tokenProvider.KeyVaultTokenCallback
                            )
                        );

                        config.AddAzureKeyVault(
                            builtConfig["KeyVaultUrl"],
                            kvClient,
                            new DefaultKeyVaultSecretManager()
                        );
                    }
                })
                .UseStartup<Startup>();
    }
#pragma warning restore CS1591
}
