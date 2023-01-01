using MakingCents.Admin.Services;
using MakingCents.Tests.Common;
using Microsoft.Extensions.DependencyInjection;

namespace MakingCents.Admin.Tests;

public sealed class AdminDiFixture : DiFixture
{
	protected override void RegisterTypes(IServiceCollection services) =>
		services.AutoRegisterFromAdmin();

	public DatabaseService DatabaseService =>
		ServiceProvider.GetRequiredService<DatabaseService>();
}
