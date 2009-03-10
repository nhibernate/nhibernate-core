using System;
using NHibernate.Type;

namespace NHibernate.Engine.Query.Sql
{
	/// <summary> Describes a scalar return in a native SQL query. </summary>
	[Serializable]
	public class NativeSQLQueryScalarReturn : INativeSQLQueryReturn
	{
		private readonly string columnAlias;
		private readonly IType type;

		public NativeSQLQueryScalarReturn(string alias, IType type)
		{
			if (string.IsNullOrEmpty(alias))
				throw new ArgumentNullException("alias","A valid scalar alias must be specified.");

			columnAlias = alias;
			this.type = type;
		}

		public string ColumnAlias
		{
			get { return columnAlias; }
		}

		public IType Type
		{
			get { return type; }
		}
	}
}
