using Teos.Autosigner;
using Teos.Autosigner.Services;
using Teos.Autosigner.Workers;

var hostBuilder = Host.CreateDefaultBuilder(args);
IHost host = hostBuilder
	.ConfigureServices((context, services) =>
	{
		services.ConfigureServices(context.Configuration);
	})
	.Build();

await host.RunAsync();

namespace Teos.Autosigner
{
	public static class ServiceConfiguration
	{
		public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddSingleton<SocketsHttpHandler>();
			services.Configure<TeosApiClientOptions>(configuration.GetSection(nameof(TeosApiClientOptions)));
			services.AddSingleton<TeosApi_v1_Client>();

			services.Configure<SignerServiceOptions>(configuration.GetSection(nameof(SignerServiceOptions)));
			services.AddSingleton<SignerService>();

			services.Configure<AutosignerWorkerOptions>(configuration.GetSection(nameof(AutosignerWorkerOptions)));
			services.AddHostedService<AutosignerWorker>();

			return services;
		}
	}
}