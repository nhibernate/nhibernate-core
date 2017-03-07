using System;
using System.Collections;
using System.Collections.Generic;

using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// The SqlStringBuilder is used to construct a SqlString.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The SqlString is a nonmutable class so it can't have sql parts added
	/// to it.  Instead this class should be used to generate a new SqlString.
	/// The SqlStringBuilder is to SqlString what the StringBuilder is to
	/// a String.
	/// </para>
	/// <para>
	/// This is different from the original version of SqlString because this does not
	/// hold the sql string in the form of "column1=@column1" instead it uses an array to
	/// build the sql statement such that 
	/// object[0] = "column1="
	/// object[1] = ref to column1 parameter
	/// </para>
	/// <para>
	/// What this allows us to do is to delay the generating of the parameter for the sql
	/// until the very end - making testing dialect indifferent.  Right now all of our test
	/// to make sure the correct sql is getting built are specific to MsSql2000Dialect.
	/// </para>
	/// </remarks>
	public class SqlStringBuilder : ISqlStringBuilder
	{
		// this holds the strings and parameters that make up the full sql statement.
		private List<object> sqlParts;

		private AddingSqlStringVisitor addingVisitor;

		private AddingSqlStringVisitor AddingVisitor
		{
			get
			{
				if (addingVisitor == null)
				{
					addingVisitor = new AddingSqlStringVisitor(this);
				}
				return addingVisitor;
			}
		}

		/// <summary>
		/// Create an empty StringBuilder with the default capacity.  
		/// </summary>
		public SqlStringBuilder() : this(16)
		{
		}

		/// <summary>
		/// Create a StringBuilder with a specific capacity.
		/// </summary>
		/// <param name="partsCapacity">The number of parts expected.</param>
		public SqlStringBuilder(int partsCapacity)
		{
			sqlParts = new List<object>(partsCapacity);
		}

		/// <summary>
		/// Create a StringBuilder to modify the SqlString
		/// </summary>
		/// <param name="sqlString">The SqlString to modify.</param>
		public SqlStringBuilder(SqlString sqlString)
		{
			sqlParts = new List<object>(sqlString.Count);
			Add(sqlString);
		}

		/// <summary>
		/// Adds the preformatted sql to the SqlString that is being built.
		/// </summary>
		/// <param name="sql">The string to add.</param>
		/// <returns>This SqlStringBuilder</returns>
		public SqlStringBuilder Add(string sql)
		{
			if (StringHelper.IsNotEmpty(sql))
			{
				sqlParts.Add(sql);
			}
			return this;
		}

		/// <summary>
		/// Adds the Parameter to the SqlString that is being built.
		/// The correct operator should be added before the Add(Parameter) is called
		/// because there will be no operator ( such as "=" ) placed between the last Add call
		/// and this Add call.
		/// </summary>
		/// <param name="parameter">The Parameter to add.</param>
		/// <returns>This SqlStringBuilder</returns>
		public SqlStringBuilder Add(Parameter parameter)
		{
			if (parameter != null)
			{
				sqlParts.Add(parameter);
			}
			return this;
		}

		public SqlStringBuilder AddParameter()
		{
			return Add(Parameter.Placeholder);
		}

		/// <summary>
		/// Attempts to discover what type of object this is and calls the appropriate
		/// method.
		/// </summary>
		/// <param name="part">The part to add when it is not known if it is a Parameter, String, or SqlString.</param>
		/// <returns>This SqlStringBuilder.</returns>
		/// <exception cref="ArgumentException">Thrown when the part is not a Parameter, String, or SqlString.</exception>
		public SqlStringBuilder AddObject(object part)
		{
			if (part == null)
			{
				return this;
			}
			Parameter paramPart = part as Parameter;
			if (paramPart != null)
			{
				return Add(paramPart); // EARLY EXIT
			}

			string stringPart = part as string;
			if (StringHelper.IsNotEmpty(stringPart))
			{
				return Add(stringPart);
			}

			SqlString sqlPart = part as SqlString;
			if (SqlStringHelper.IsNotEmpty(sqlPart))
			{
				return Add(sqlPart);
			}

			// remarks - we should not get to here - this is a problem with the 
			// SQL being generated.
			if (paramPart == null && stringPart == null && sqlPart == null)
			{
				throw new ArgumentException("Part was not a Parameter, String, or SqlString.");
			}

			return this;
		}

		/// <summary>
		/// Adds an existing SqlString to this SqlStringBuilder.  It does NOT add any
		/// prefix, postfix, operator, or wrap around this.  It is equivalent to just 
		/// adding a string.
		/// </summary>
		/// <param name="sqlString">The SqlString to add to this SqlStringBuilder</param>
		/// <returns>This SqlStringBuilder</returns>
		/// <remarks>This calls the overloaded Add(sqlString, null, null, null, false)</remarks>
		public SqlStringBuilder Add(SqlString sqlString)
		{
			sqlString.Visit(AddingVisitor);
			return this;
		}


		/// <summary>
		/// Adds an existing SqlString to this SqlStringBuilder
		/// </summary>
		/// <param name="sqlString">The SqlString to add to this SqlStringBuilder</param>
		/// <param name="prefix">String to put at the beginning of the combined SqlString.</param>
		/// <param name="op">How these Statements should be junctioned "AND" or "OR"</param>
		/// <param name="postfix">String to put at the end of the combined SqlString.</param>
		/// <returns>This SqlStringBuilder</returns>
		/// <remarks>
		/// This calls the overloaded Add method with an array of SqlStrings and wrapStatement=false
		/// so it will not be wrapped with a "(" and ")"
		/// </remarks>
		public SqlStringBuilder Add(SqlString sqlString, string prefix, string op, string postfix)
		{
			return Add(new SqlString[] {sqlString}, prefix, op, postfix, false);
		}

		/// <summary>
		/// Adds existing SqlStrings to this SqlStringBuilder
		/// </summary>
		/// <param name="sqlStrings">The SqlStrings to combine.</param>
		/// <param name="prefix">String to put at the beginning of the combined SqlString.</param>
		/// <param name="op">How these SqlStrings should be junctioned "AND" or "OR"</param>
		/// <param name="postfix">String to put at the end of the combined SqlStrings.</param>
		/// <returns>This SqlStringBuilder</returns>
		/// <remarks>This calls the overloaded Add method with wrapStatement=true</remarks>
		public SqlStringBuilder Add(SqlString[] sqlStrings, string prefix, string op, string postfix)
		{
			return Add(sqlStrings, prefix, op, postfix, true);
		}

		/// <summary>
		/// Adds existing SqlStrings to this SqlStringBuilder
		/// </summary>
		/// <param name="sqlStrings">The SqlStrings to combine.</param>
		/// <param name="prefix">String to put at the beginning of the combined SqlStrings.</param>
		/// <param name="op">How these SqlStrings should be junctioned "AND" or "OR"</param>
		/// <param name="postfix">String to put at the end of the combined SqlStrings.</param>
		/// <param name="wrapStatement">Wrap each SqlStrings with "(" and ")"</param>
		/// <returns>This SqlStringBuilder</returns>
		public SqlStringBuilder Add(SqlString[] sqlStrings, string prefix, string op, string postfix, bool wrapStatement)
		{
			if (StringHelper.IsNotEmpty(prefix))
			{
				sqlParts.Add(prefix);
			}

			bool opNeeded = false;

			foreach (SqlString sqlString in sqlStrings)
			{
				if (sqlString.Count == 0)
				{
					continue;
				}

				if (opNeeded)
				{
					sqlParts.Add(" " + op + " ");
				}

				opNeeded = true;

				if (wrapStatement)
				{
					sqlParts.Add("(");
				}

				Add(sqlString);

				if (wrapStatement)
				{
					sqlParts.Add(")");
				}
			}

			if (postfix != null)
			{
				sqlParts.Add(postfix);
			}

			return this;
		}

		/// <summary>
		/// Gets the number of SqlParts in this SqlStringBuilder.
		/// </summary>
		/// <returns>
		/// The number of SqlParts in this SqlStringBuilder.
		/// </returns>
		public int Count
		{
			get { return sqlParts.Count; }
		}

		/// <summary>
		/// Gets or Sets the element at the index
		/// </summary>
		/// <value>Returns a string or Parameter.</value>
		/// <remarks></remarks>
		public object this[int index]
		{
			get { return sqlParts[index]; }
			set { sqlParts[index] = value; }
		}

		/// <summary>
		/// Insert a string containing sql into the SqlStringBuilder at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which the sql should be inserted.</param>
		/// <param name="sql">The string containing sql to insert.</param>
		/// <returns>This SqlStringBuilder</returns>
		public SqlStringBuilder Insert(int index, string sql)
		{
			sqlParts.Insert(index, sql);
			return this;
		}

		/// <summary>
		/// Insert a Parameter into the SqlStringBuilder at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which the Parameter should be inserted.</param>
		/// <param name="param">The Parameter to insert.</param>
		/// <returns>This SqlStringBuilder</returns>
		public SqlStringBuilder Insert(int index, Parameter param)
		{
			sqlParts.Insert(index, param);
			return this;
		}

		/// <summary>
		/// Removes the string or Parameter at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		/// <returns>This SqlStringBuilder</returns>
		public SqlStringBuilder RemoveAt(int index)
		{
			sqlParts.RemoveAt(index);
			return this;
		}

		/// <summary>
		/// Converts the mutable SqlStringBuilder into the immutable SqlString.
		/// </summary>
		/// <returns>The SqlString that was built.</returns>
		public SqlString ToSqlString()
		{
			return new SqlString(sqlParts.ToArray());
		}

		public override string ToString()
		{
			return ToSqlString().ToString();
		}

		private class AddingSqlStringVisitor : ISqlStringVisitor
		{
			private SqlStringBuilder parent;

			public AddingSqlStringVisitor(SqlStringBuilder parent)
			{
				this.parent = parent;
			}

			public void String(string text)
			{
				parent.Add(text);
			}

			public void String(SqlString sqlString)
			{
				parent.Add(sqlString);
			}

			public void Parameter(Parameter parameter)
			{
				parent.Add(parameter);
			}
		}

		public void Clear()
		{
			sqlParts.Clear();
		}
	}
}
