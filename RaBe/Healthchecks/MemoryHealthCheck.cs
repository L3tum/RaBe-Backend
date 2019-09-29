#region using

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

#endregion

namespace RaBe.Healthchecks
{
	public class MemoryHealthCheck : IHealthCheck
	{
		public string Name => "memory_check";

		public Task<HealthCheckResult> CheckHealthAsync(
			HealthCheckContext context,
			CancellationToken cancellationToken = default)
		{
			// Include GC information in the reported diagnostics.
			var allocated = GC.GetTotalMemory(false);
			var data = new Dictionary<string, object>
			{
				{"AllocatedBytes", allocated},
				{"Gen0Collections", GC.CollectionCount(0)},
				{"Gen1Collections", GC.CollectionCount(1)},
				{"Gen2Collections", GC.CollectionCount(2)}
			};

			return Task.FromResult(new HealthCheckResult(
				allocated > 500000000 ? HealthStatus.Degraded : HealthStatus.Healthy,
				"Reports degraded status if allocated bytes " +
				$">= {500000000} bytes.",
				null,
				data));
		}
	}
}