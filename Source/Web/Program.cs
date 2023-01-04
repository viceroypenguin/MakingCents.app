using DryIoc.Microsoft.DependencyInjection;
using Serilog;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Destructurers;
using Serilog.Exceptions.MsSqlServer.Destructurers;
using Serilog.Exceptions.Refit.Destructurers;
using Serilog.Exceptions;
using MakingCents.Database;
using Serilog.Events;
using System.Diagnostics;


#pragma warning disable CA1852 // Type can be sealed because it has no subtypes in its containing assembly and is not externally visible

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console(formatProvider: null)
	.CreateBootstrapLogger();

try
{
	var builder = WebApplication.CreateBuilder(args);

	builder.Host.UseServiceProviderFactory(
		new DryIocServiceProviderFactory());

	builder.Host.UseSerilog((ctx, lc) => lc
		.ReadFrom.Configuration(ctx.Configuration)
		.Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
			.WithDefaultDestructurers()
			.WithDestructurers(new IExceptionDestructurer[]
			{
				new SqlExceptionDestructurer(),
				new ApiExceptionDestructurer(),
			})));

	builder.Services.AutoRegisterFromCommon();
	builder.Services.AutoRegisterFromDatabase();

	builder.Services.AutoRegisterFromAdmin();

	builder.Services.Configure<DbContextOptions>(
		builder.Configuration.GetSection("DbContextOptions"));

	builder.Services.AddControllers();

	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	builder.Services.AddResponseCompression(options =>
	{
		options.EnableForHttps = true;
	});

	var app = builder.Build();

	using (var scope = app.Services.CreateScope())
	{
		var db = scope.ServiceProvider.GetRequiredService<DbContext>();
		db.InitializeDatabase();
	}

	app.UseSwagger();
	app.UseSwaggerUI();

	app.UseHttpsRedirection();

	app.UseStaticFiles();

	app.UseAuthorization();

	app.MapControllers();

	app.UseSerilogRequestLogging(o =>
	{
		o.GetLevel = static (httpContext, _, _) =>
			httpContext.Response.StatusCode >= 500 ? LogEventLevel.Error : LogEventLevel.Information;

		o.EnrichDiagnosticContext = static (diagnosticContext, httpContext) =>
		{
			diagnosticContext.Set("User", httpContext.User?.Identity?.Name);
			diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress);
			diagnosticContext.Set("ConnectingIP", httpContext.Request.Headers["CF-Connecting-IP"]);
		};
	});

	app.Run();
}
catch (Exception ex) when (
	ex is not HostAbortedException
	&& !ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
{
	Log.Fatal(ex, "unhandled exception");
}
finally
{
	if (new StackTrace().FrameCount == 1)
	{
		Log.Information("Shut down complete");
		Log.CloseAndFlush();
	}
}
