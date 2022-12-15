using Microsoft.Extensions.Options;

namespace Teos.Autosigner.Workers
{
	abstract class TimerBasedWorkerServiceBase : BackgroundService
	{
		private readonly TimerBasedWorkerOptions _options;
		private readonly ILogger _logger;

		public TimerBasedWorkerServiceBase(IOptions<TimerBasedWorkerOptions> options, ILogger logger)
		{
			_options = options.Value;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var startTime = DateTime.UtcNow;

				await OnTimerAsync(stoppingToken);

				var endTime = DateTime.UtcNow;
				var executionDuration = endTime.Subtract(startTime);
				var nextStartDelay = executionDuration < _options.Interval
					? _options.Interval - executionDuration
					: TimeSpan.Zero;

				await Task.Delay(nextStartDelay, stoppingToken);
			}
		}

		protected virtual async Task OnTimerAsync(CancellationToken stoppingToken)
		{
			try
			{
				_logger.LogDebug("Start task processing");

				await ExecuteInternalAsync(stoppingToken);

				_logger.LogDebug("Successfully processed task");
			}
			catch (Exception ex) when (_options.ContinueOnException)
			{
				_logger.LogWarning(ex, "Error occurred while processing the task. The error is ingored.");
			}
			catch (Exception ex) when (!_options.ContinueOnException)
			{
				_logger.LogError(ex, "Error occurred while processing the task");
				throw;
			}
		}

		protected abstract Task ExecuteInternalAsync(CancellationToken stoppingToken);
	}

	public class TimerBasedWorkerOptions
	{
		public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(60);
		public bool ContinueOnException { get; set; } = true;
	}
}
