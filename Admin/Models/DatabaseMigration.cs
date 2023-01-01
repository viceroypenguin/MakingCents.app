namespace MakingCents.Admin.Models;

public sealed record DatabaseMigration(
	VersionHistoryId VersionHistoryId,
	string SqlFile,
	DateTimeOffset ApplicationTime,
	TimeSpan ExecutionTime);
