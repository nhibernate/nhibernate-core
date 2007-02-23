using System;
using System.Collections;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Represents an <c>... in (...)</c> expression
	/// </summary>
	public class InFragment
	{
		public static readonly string Null = "null";
		public static readonly string NotNull = "not null";

		private string columnName;
		private ArrayList values = new ArrayList();

		/// <summary>
		/// Add a value to the value list. Value may be a string,
		/// a <see cref="Parameter" />, or one of special values
		/// <see cref="Null" /> or <see cref="NotNull" />.
		/// </summary>
		public InFragment AddValue(object value)
		{
			values.Add(value);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public InFragment SetColumn(string columnName)
		{
			this.columnName = columnName;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public InFragment SetColumn(string alias, string columnName)
		{
			this.columnName = alias + StringHelper.Dot + columnName;
			return SetColumn(this.columnName);
		}

		public InFragment SetFormula(string alias, string formulaTemplate)
		{
			this.columnName = StringHelper.Replace(formulaTemplate, Template.Placeholder, alias);
			return SetColumn(this.columnName);
		}

		/// <summary></summary>
		public SqlString ToFragmentString()
		{
			SqlStringBuilder buf = new SqlStringBuilder(values.Count * 5);
			buf.Add(columnName);

			if (values.Count > 1)
			{
				// is a comma needed before the value that's about to be added - it
				// defaults to false because we don't need a comma right away.
				bool commaNeeded = false;

				// if a "null" is in the list of values then we need to manipulate
				// the SqlString a little bit more at the end.
				bool allowNull = false;

				buf.Add(" in (");
				for (int i = 0; i < values.Count; i++)
				{
					object value = values[i];
					if (Null.Equals(value))
					{
						allowNull = true;
					}
					else if (NotNull.Equals(value))
					{
						throw new ArgumentOutOfRangeException("not null makes no sense for in expression");
					}
					else
					{
						if (commaNeeded)
						{
							buf.Add(StringHelper.CommaSpace);
						}

						if (value is Parameter)
						{
							buf.Add((Parameter) value);
						}
						else
						{
							buf.Add((string) value);
						}

						// a value has been added into the IN clause so the next
						// one needs a comma before it
						commaNeeded = true;
					}
				}

				buf.Add(StringHelper.ClosedParen);

				// if "null" is in the list of values then add to the beginning of the
				// SqlString "is null or [column] (" + [rest of sqlstring here] + ")"
				if (allowNull)
				{
					buf.Insert(0, " is null or ")
						.Insert(0, columnName)
						.Insert(0, StringHelper.OpenParen)
						.Add(StringHelper.ClosedParen);
				}
			}
			else
			{
				object value = values[0];
				if (Null.Equals(value))
				{
					buf.Add(" is null");
				}
				else if (NotNull.Equals(value))
				{
					buf.Add(" is not null ");
				}
				else
				{
					buf.Add("=").AddObject(values[0]);
				}
			}
			return buf.ToSqlString();
		}
	}
}