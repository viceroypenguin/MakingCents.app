using CommunityToolkit.Diagnostics;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using MakingCents.Database;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MakingCents.Tests.Common;

public abstract class DiFixture : IDisposable
{
	public IServiceProvider ServiceProvider { get; }
	protected abstract void RegisterTypes(IServiceCollection services);

	#region Database
	private readonly string _databaseName;
	private readonly string _connectionString;

	private DataConnection GetSqlServerConnection() =>
		SqlServerTools.CreateDataConnection(
			_connectionString,
			SqlServerVersion.v2019,
			SqlServerProvider.MicrosoftDataSqlClient);
	#endregion

	#region Setup
	protected DiFixture()
	{
		var configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: true)
			.AddEnvironmentVariables()
			.Build();

		var connectionString = configuration["ConnectionString"];
		Guard.IsNotNullOrWhiteSpace(connectionString);
		_connectionString = ReplaceDatabase(connectionString, "master");
		_databaseName = "MakingCents_Tests_" + Guid.NewGuid().ToString().Replace("-", "", StringComparison.Ordinal);

		using (var conn = GetSqlServerConnection())
			conn.Execute($"use master; create database {_databaseName};");

		var collection = new ServiceCollection();
		collection.AddLogging(l => l.AddProvider(NullLoggerProvider.Instance));
		collection.AddSingleton(configuration);
		collection.AddSingleton<IConfiguration>(configuration);
		collection.AutoRegisterFromCommon();
		collection.AutoRegisterFromDatabase();
		collection.Configure<DbContextOptions>(o =>
			o.ConnectionString = ReplaceDatabase(connectionString, _databaseName));

#pragma warning disable CA2214 // Do not call overridable methods in constructors
		RegisterTypes(collection);
#pragma warning restore CA2214 // Do not call overridable methods in constructors

		var sp = collection.BuildServiceProvider();

		using (sp.CreateScope())
		{
			var db = sp.GetRequiredService<DbContext>();
			db.InitializeDatabase();
		}

		ServiceProvider = sp;
	}

	private static string ReplaceDatabase(string connectionString, string database)
	{
		var builder = new SqlConnectionStringBuilder(connectionString)
		{
			InitialCatalog = database,
		};
		return builder.ToString();
	}
	#endregion

	#region Teardown
	private int _dispose;
	protected virtual void Dispose(bool disposing)
	{
		if (Interlocked.CompareExchange(ref _dispose, 1, 0) == 1)
			return;

		using var conn = GetSqlServerConnection();
		conn.Execute($"""
			use master;
			if exists (select * from sys.databases where name = '{_databaseName}')
			begin
				alter database {_databaseName} set single_user with rollback immediate;
				drop database {_databaseName};
			end
			""");
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
	#endregion
}
