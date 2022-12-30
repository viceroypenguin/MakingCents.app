if not exists (select * from sys.tables where name = 'VersionHistory')
begin

create table VersionHistory
(
	VersionHistoryId int not null identity(1,1)
		constraint [PK_VersionHistory] primary key,
	SqlFile varchar(50) not null,
	ExecutionStart datetimeoffset not null,
	ExecutionEnd datetimeoffset not null,
);

end
