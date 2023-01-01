// ---------------------------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by LinqToDB scaffolding tool (https://github.com/linq2db/linq2db).
// Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
// ---------------------------------------------------------------------------------------------------

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using MakingCents.Common.Ids;
using MakingCents.Database.Models;
using System;

#pragma warning disable 1573, 1591
#nullable enable

namespace MakingCents.Database
{
	public partial class DbContext : DataConnection
	{
		partial void InitDataContext();

		public ITable<VersionHistory> VersionHistories => this.GetTable<VersionHistory>();
	}
}
namespace MakingCents.Database.Models
{
	[Table("VersionHistory")]
	public class VersionHistory
	{
		[ValueConverter(                  ConverterType = typeof(MakingCents.Common.Ids.VersionHistoryId.LinqToDbValueConverter)), Column("VersionHistoryId", IsPrimaryKey = true, IsIdentity = true, SkipOnInsert = true, SkipOnUpdate = true)] public VersionHistoryId VersionHistoryId { get; set; } // int
		[Column        ("SqlFile"       , CanBeNull     = false                       )                                                                                                              ] public string           SqlFile          { get; set; } = null!; // varchar(50)
		[Column        ("ExecutionStart"                                              )                                                                                                              ] public DateTimeOffset   ExecutionStart   { get; set; } // datetimeoffset(7)
		[Column        ("ExecutionEnd"                                                )                                                                                                              ] public DateTimeOffset   ExecutionEnd     { get; set; } // datetimeoffset(7)
	}
}
