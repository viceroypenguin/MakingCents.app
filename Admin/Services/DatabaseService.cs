using LinqToDB;
using MakingCents.Admin.Models;
using MakingCents.Database;

namespace MakingCents.Admin.Services;

public sealed class DatabaseService
{
	private readonly DbContext _context;

	public DatabaseService(DbContext context)
	{
		_context = context;
	}

	public async Task<List<DatabaseMigration>> GetVersionHistories() =>
		await _context.VersionHistories
			.Select(vh => new DatabaseMigration(
				vh.VersionHistoryId,
				vh.SqlFile,
				vh.ExecutionStart,
				vh.ExecutionEnd - vh.ExecutionStart))
			.ToListAsync();
}
