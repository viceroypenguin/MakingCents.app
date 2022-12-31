namespace MakingCents.Admin.Models;
public record DatabaseMigration(
	int VersionHistoryId,
	string SqlFile,
	DateTimeOffset ApplicationTime,
	TimeSpan ExecutionTime);
