using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using DocoptNet;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using MakingCents.Scaffold;
using Microsoft.Data.SqlClient;

static int ShowHelp(string help) { Console.WriteLine(help); return 0; }
static int OnError(string error) { Console.Error.WriteLine(error); return 1; }

return ProgramArguments.CreateParser()
	.Parse(args) switch
{
	IArgumentsResult<ProgramArguments> { Arguments: var arguments } => await Main(arguments),
	IHelpResult { Help: var help } => ShowHelp(help),
	IInputErrorResult { Error: var error } => OnError(error),
	var result => throw new SwitchExpressionException(result),
};

static async Task<int> Main(ProgramArguments args)
{
	try
	{
		var txt = await GenerateScaffold(args);

		var file = args.OptOutputFile!;
		Directory.CreateDirectory(Path.GetDirectoryName(file)!);
		await File.WriteAllTextAsync(file, txt);
		return 0;
	}
	catch
	{
		return 1;
	}
}

static async Task<string> GenerateScaffold(ProgramArguments args)
{
	await Task.Yield();
	return string.Empty;
}
