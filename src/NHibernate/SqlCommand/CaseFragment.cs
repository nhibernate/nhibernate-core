using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary> Abstract SQL case fragment renderer </summary>
	public abstract class CaseFragment
	{
		protected internal readonly Dialect.Dialect dialect;
		protected internal string returnColumnName;

		protected internal IDictionary<string,string > cases = new LinkedHashMap<string,string>();

		protected CaseFragment(Dialect.Dialect dialect)
		{
			this.dialect = dialect;
		}

		public virtual CaseFragment SetReturnColumnName(string returnColumnName)
		{
			this.returnColumnName = returnColumnName;
			return this;
		}

		public virtual CaseFragment SetReturnColumnName(string returnColumnName, string suffix)
		{
			return SetReturnColumnName(new Alias(suffix).ToAliasString(returnColumnName, dialect));
		}

		public virtual CaseFragment AddWhenColumnNotNull(string alias, string columnName, string value)
		{
			cases[StringHelper.Qualify(alias, columnName)] = value;
			return this;
		}

		public abstract string ToSqlStringFragment();
	}
}