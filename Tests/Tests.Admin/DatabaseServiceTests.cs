using CommunityToolkit.Diagnostics;
using MakingCents.Database;
using Microsoft.Extensions.DependencyInjection;

namespace MakingCents.Admin.Tests;

public sealed class DatabaseServiceTests : IClassFixture<AdminDiFixture>, IDisposable
{
	private readonly AdminDiFixture _diFixture;
	private readonly IServiceScope _scope;

	public DatabaseServiceTests(AdminDiFixture diFixture)
	{
		Guard.IsNotNull(diFixture);

		_diFixture = diFixture;
		_scope = _diFixture.ServiceProvider.CreateScope();
	}

	public void Dispose() => _scope.Dispose();

	[Fact]
	public async Task AllScriptsHaveBeenRun()
	{
		var service = _diFixture.DatabaseService;
		var runScripts = await service.GetVersionHistories();

		var dbAssembly = typeof(DbContext).Assembly;
		var allScripts = dbAssembly.GetManifestResourceNames()
			.Where(n => Path.GetExtension(n) == ".sql")
			.Select(n => n.Replace("MakingCents.Database.Scripts.", "", StringComparison.OrdinalIgnoreCase))
			.ToHashSet();

		Assert.True(allScripts.SetEquals(runScripts.Select(r => r.SqlFile)));
	}
}
