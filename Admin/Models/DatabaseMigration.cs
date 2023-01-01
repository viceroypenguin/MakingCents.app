namespace MakingCents.Admin.Models;

public record DatabaseMigration(
	VersionHistoryId VersionHistoryId,
	string SqlFile,
	DateTimeOffset ApplicationTime,
	TimeSpan ExecutionTime);
