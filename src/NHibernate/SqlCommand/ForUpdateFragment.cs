using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Represents an SQL <c>for update of ... nowait</c> statement
	/// </summary>
	public class ForUpdateFragment
	{
		private readonly Dialect.Dialect dialect;
		private readonly StringBuilder aliases = new StringBuilder();
		private bool isNoWaitEnabled;

		public ForUpdateFragment(Dialect.Dialect dialect)
		{
			this.dialect = dialect;
		}

		public ForUpdateFragment(Dialect.Dialect dialect, IDictionary<string, LockMode> lockModes, IDictionary<string, string[]> keyColumnNames)
			: this(dialect)
		{
			LockMode upgradeType = null;

			foreach (KeyValuePair<string, LockMode> me in lockModes)
			{
				LockMode lockMode = me.Value;
				if (LockMode.Read.LessThan(lockMode))
				{
					string tableAlias = me.Key;
					if (dialect.ForUpdateOfColumns)
					{
						string[] keyColumns = keyColumnNames[tableAlias];
						if (keyColumns == null)
						{
							throw new ArgumentException("alias not found: " + tableAlias);
						}
						keyColumns = StringHelper.Qualify(tableAlias, keyColumns);
						for (int i = 0; i < keyColumns.Length; i++)
						{
							AddTableAlias(keyColumns[i]);
						}
					}
					else
					{
						AddTableAlias(tableAlias);
					}
					if (upgradeType != null && lockMode != upgradeType)
					{
						throw new QueryException("mixed LockModes");
					}
					upgradeType = lockMode;
				}

				if (upgradeType == LockMode.UpgradeNoWait)
				{
					IsNoWaitEnabled = true;
				}
			}
		}

		public bool IsNoWaitEnabled
		{
			get { return isNoWaitEnabled; }
			set { isNoWaitEnabled = value; }
		}

		public ForUpdateFragment AddTableAlias(string alias)
		{
			if (aliases.Length > 0)
			{
				aliases.Append(StringHelper.CommaSpace);
			}
			aliases.Append(alias);
			return this;
		}

		public string ToSqlStringFragment()
		{
			if (aliases.Length == 0)
			{
				return string.Empty;
			}

			return isNoWaitEnabled
			       	? dialect.GetForUpdateNowaitString(aliases.ToString())
			       	: dialect.GetForUpdateString(aliases.ToString());
		}
	}
}