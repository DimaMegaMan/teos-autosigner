using Microsoft.Extensions.Options;
using Teos.Autosigner.Services;

namespace Teos.Autosigner.Workers
{
	internal class AutosignerWorker : TimerBasedWorkerServiceBase
	{
		private readonly SignerService _signerService;

		public AutosignerWorker(ILogger<AutosignerWorker> logger, IOptions<AutosignerWorkerOptions> options, SignerService signerService)
			: base(options, logger)
		{
			_signerService = signerService;
		}

		protected override Task ExecuteInternalAsync(CancellationToken stoppingToken)
		{
			return _signerService.SignPendingTransactionsAsync();
		}
	}
}
