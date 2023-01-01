using LinqToDB.Data;
using Microsoft.Extensions.Logging;

namespace MakingCents.Database;

[RegisterTransient]
public sealed partial class DbContext : DataConnection
{
	private static bool s_dbInitialized;
	private static bool s_loaded;
	private readonly ILogger<DbContext> _logger;

	public DbContext(ILogger<DbContext> logger)
	{
		_logger = logger;

		if (s_loaded && !s_dbInitialized)
			throw new InvalidOperationException("Database must be initialized during startup.");
		s_loaded = true;
	}
}
