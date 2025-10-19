using AuthorizationGateway.Core.Interfaces;
using AuthorizationGateway.Core.Services;
using AuthorizationGateway.Infra.Crypto;
using AuthorizationGateway.Infra.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuthorizationGateway.Infra.IoC
{
    /// <summary>
    /// Provides extension methods for registering services related to the authorization gateway in the dependency
    /// injection container.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddAuthorizationGatewayServices(this IServiceCollection services, string aesKey, string aesIv, string hmacSecret)
        {
            // Core crypto services
            services.AddSingleton<IEncryptionService>(sp =>
                new AesEncryptionService(aesKey, aesIv,
                    sp.GetRequiredService<ILogger<AesEncryptionService>>()));

            services.AddSingleton<IIntegrityService>(new HmacIntegrityService(hmacSecret));

            // Data e domain services
            services.AddSingleton<ITransactionRepository, InMemoryTransactionRepository>();
            services.AddScoped<ITransactionService, TransactionService>();

            return services;
        }
    }
}
