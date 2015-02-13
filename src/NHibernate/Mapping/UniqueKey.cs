using System;
using System.Text;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// An Unique Key constraint in the database.
	/// </summary>
	[Serializable]
	public class UniqueKey : Constraint
	{
		

		public override bool IsGenerated(Dialect.Dialect dialect)
		{
			if (dialect.SupportsNotNullUnique)
				return true;
			foreach (Column column in ColumnIterator)
			{
				if(column.IsNullable)
					return false;
			}
			return true;
		}
	}
}