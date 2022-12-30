using LinqToDB.Data;
using Microsoft.Extensions.Logging;

namespace MakingCents.Database;

public partial class DbContext : DataConnection
{
	private readonly ILogger<DbContext> _logger;

	public DbContext(ILogger<DbContext> logger)
	{
		_logger = logger;
	}
}
