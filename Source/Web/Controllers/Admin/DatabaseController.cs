using MakingCents.Admin.Models;
using MakingCents.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace MakingCents.Web.Controllers.Admin;

[ApiController]
[Route("~/api/admin/database")]
public sealed class DatabaseController : ControllerBase
{
	private readonly DatabaseService _databaseService;

	public DatabaseController(
		DatabaseService databaseService)
	{
		_databaseService = databaseService;
	}

	[HttpGet("migrations")]
	public async Task<IEnumerable<DatabaseMigration>> GetDatabaseMigrations() =>
		await _databaseService.GetVersionHistories();
}
