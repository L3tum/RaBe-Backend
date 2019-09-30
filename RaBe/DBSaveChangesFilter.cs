#region using

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

#endregion

namespace RaBe
{
	public class DBSaveChangesFilter : IAsyncActionFilter
	{
		private readonly RaBeContext _dbContext;

		public DBSaveChangesFilter(RaBeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task OnActionExecutionAsync(
			ActionExecutingContext context,
			ActionExecutionDelegate next)
		{
			var result = await next().ConfigureAwait(false);

			if (result.Exception == null || result.ExceptionHandled)
			{
				await _dbContext.SaveChangesAsync().ConfigureAwait(false);
			}
		}
	}
}