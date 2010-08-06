using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// This is a non-modifiable SQL statement that is ready to be prepared 
	/// and sent to the Database for execution.
	/// </summary>
	/// <remarks>
	/// <para>
	/// If you need to modify this object pass it to a <see cref="SqlStringBuilder"/> and
	/// get a new object back from it.
	/// </para>
	/// </remarks>
	[Serializable]
	public class SqlString
	{
		private bool isCompacted = false;
		private readonly object[] sqlParts;

		public static readonly SqlString Empty = new SqlString(new object[0]);
		
		public static SqlString Parameter
		{
			get { return new SqlString(SqlCommand.Parameter.Placeholder); }
		}

		public SqlString(string sqlPart)
		{
			if (StringHelper.IsNotEmpty(sqlPart))
			{
				sqlParts = new object[] {sqlPart};
			}
			else
			{
				sqlParts = new object[0];
			}
		}

		public SqlString(params object[] sqlParts)
		{
#if DEBUG
			foreach (object obj in sqlParts)
			{
				Debug.Assert(obj is string || obj is SqlString || obj is Parameter);
			}
#endif
			this.sqlParts = sqlParts;
		}

		/// <summary>
		/// Appends the SqlString parameter to the end of the current SqlString to create a 
		/// new SqlString object.
		/// </summary>
		/// <param name="rhs">The SqlString to append.</param>
		/// <returns>A new SqlString object.</returns>
		/// <remarks>
		/// A SqlString object is immutable so this returns a new SqlString.  If multiple Appends 
		/// are called it is better to use the SqlStringBuilder.
		/// </remarks>
		public SqlString Append(SqlString rhs)
		{
			return new SqlString(ArrayHelper.Join(sqlParts, rhs.sqlParts));
		}

		/// <summary>
		/// Appends the string parameter to the end of the current SqlString to create a 
		/// new SqlString object.
		/// </summary>
		/// <param name="rhs">The string to append.</param>
		/// <returns>A new SqlString object.</returns>
		/// <remarks>
		/// A SqlString object is immutable so this returns a new SqlString.  If multiple Appends 
		/// are called it is better to use the SqlStringBuilder.
		/// </remarks>
		public SqlString Append(string rhs)
		{
			if (StringHelper.IsNotEmpty(rhs))
			{
				object[] temp = new object[sqlParts.Length + 1];
				Array.Copy(sqlParts, temp, sqlParts.Length);
				temp[sqlParts.Length] = rhs;
				return new SqlString(temp);
			}
			else
			{
				return this;
			}
		}

		/// <summary>
		/// Compacts the SqlString into the fewest parts possible.
		/// </summary>
		/// <returns>A new SqlString.</returns>
		/// <remarks>
		/// Combines all SqlParts that are strings and next to each other into
		/// one SqlPart.
		/// </remarks>
		private SqlString Compact()
		{
			if (isCompacted)
			{
				return this;
			}

			StringBuilder builder = new StringBuilder();
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			foreach (object part in sqlParts)
			{
				SqlString sqlStringPart = part as SqlString;
				string stringPart = part as string;
				if (sqlStringPart != null)
				{
					sqlBuilder.Add(sqlStringPart.Compact());
				}
				else if (stringPart != null)
				{
					builder.Append(stringPart);
				}
				else
				{
					// don't add an empty string into the new compacted SqlString
					if (builder.Length > 0)
					{
						sqlBuilder.Add(builder.ToString());
					}

					builder.Length = 0;
					sqlBuilder.Add((Parameter)part);
				}
			}

			// make sure the contents of the builder have been added to the sqlBuilder
			if (builder.Length > 0)
			{
				sqlBuilder.Add(builder.ToString());
			}

			SqlString result = sqlBuilder.ToSqlString();
			result.isCompacted = true;
			return result;
		}

		/// <summary>
		/// Gets the number of SqlParts contained in this SqlString.
		/// </summary>
		/// <value>The number of SqlParts contained in this SqlString.</value>
		public int Count
		{
			get { return sqlParts.Length; }
		}

		public int Length
		{
			get
			{
				int result = 0;
				foreach (object part in sqlParts)
				{
					result += LengthOfPart(part);
				}
				return result;
			}
		}

		/// <summary>
		/// Determines whether the end of this instance matches the specified String.
		/// </summary>
		/// <param name="value">A string to seek at the end.</param>
		/// <returns><see langword="true" /> if the end of this instance matches value; otherwise, <see langword="false" /></returns>
		public bool EndsWith(string value)
		{
			SqlString tempSql = Compact();
			if (tempSql.Count == 0)
			{
				return false;
			}

			string lastPart = tempSql.sqlParts[tempSql.Count - 1] as string;

			return lastPart != null && lastPart.EndsWith(value);
		}

		/// <summary>
		/// Replaces all occurrences of a specified <see cref="String"/> in this instance, 
		/// with another specified <see cref="String"/> .
		/// </summary>
		/// <param name="oldValue">A String to be replaced.</param>
		/// <param name="newValue">A String to replace all occurrences of oldValue. </param>
		/// <returns>
		/// A new SqlString with oldValue replaced by the newValue.  The new SqlString is 
		/// in the compacted form.
		/// </returns>
		public SqlString Replace(string oldValue, string newValue)
		{
			SqlString compacted = Compact();
			if (compacted == this)
			{
				// Ensure we have a new SqlString to work with
				compacted = Clone();
			}

			for (int i = 0; i < compacted.sqlParts.Length; i++)
			{
				string sqlPart = compacted.sqlParts[i] as string;
				if (sqlPart != null)
				{
					compacted.sqlParts[i] = sqlPart.Replace(oldValue, newValue);
				}
			}

			return compacted;
		}

		/// <summary>
		/// Determines whether the beginning of this SqlString matches the specified System.String,
		/// using case-insensitive comparison.
		/// </summary>
		/// <param name="value">The System.String to seek</param>
		/// <returns>true if the SqlString starts with the value.</returns>
		public bool StartsWithCaseInsensitive(string value)
		{
			SqlString tempSql = Compact();
			if (tempSql.Count == 0)
			{
				return value.Length == 0;
			}

			string firstPart = tempSql.sqlParts[0] as string;
			if (firstPart == null)
			{
				return false;
			}

			return StringHelper.StartsWithCaseInsensitive(firstPart, value);
		}

		private SqlStringBuilder BuildSubstring(int startIndex)
		{
			SqlStringBuilder builder = new SqlStringBuilder(this);

			int offset = 0;

			while (builder.Count > 0)
			{
				int nextOffset = offset + LengthOfPart(builder[0]);

				if (nextOffset > startIndex)
				{
					break;
				}

				builder.RemoveAt(0);
				offset = nextOffset;
			}

			if (builder.Count > 0 && offset < startIndex)
			{
				builder[0] = ((string) builder[0]).Substring(startIndex - offset);
			}

			return builder;
		}

		/// <summary>
		/// Retrieves a substring from this instance. The substring starts at a specified character position. 
		/// </summary>
		/// <param name="startIndex">The starting character position of a substring in this instance.</param>
		/// <returns>
		/// A new SqlString to the substring that begins at startIndex in this instance. 
		/// </returns>
		/// <remarks>
		/// If the startIndex is greater than the length of the SqlString then <see cref="SqlString.Empty" /> is returned.
		/// </remarks>
		public SqlString Substring(int startIndex)
		{
			if (startIndex < 0)
			{
				throw new ArgumentException("startIndex should be greater than or equal to 0", "startIndex");
			}

			SqlStringBuilder builder = BuildSubstring(startIndex);

			if (builder.Count == 0)
			{
				return Empty;
			}

			SqlString result = builder.ToSqlString();
			if (isCompacted)
			{
				result.SetCompacted();
			}
			return result;
		}

		public SqlString Substring(int startIndex, int length)
		{
			if (startIndex < 0)
			{
				throw new ArgumentException("startIndex should be greater than or equal to 0", "startIndex");
			}

			if (length < 0)
			{
				throw new ArgumentException("length should be greater than or equal to 0", "length");
			}

			SqlStringBuilder builder = BuildSubstring(startIndex);

			if (builder.Count == 0)
			{
				return builder.ToSqlString();
			}

			int offset = 0;
			int nextOffset = -1;
			int count = int.MaxValue;

			for (int i = 0; i < builder.Count; i++)
			{
				nextOffset = offset + LengthOfPart(builder[i]);
				if (nextOffset < length)
				{
					offset = nextOffset;
					continue;
				}
				else if (nextOffset >= length)
				{
					count = i + 1;
					break;
				}
			}

			while (builder.Count > count)
			{
				builder.RemoveAt(builder.Count - 1);
			}

			if (length < nextOffset)
			{
				string lastPart = (string) builder[builder.Count - 1];
				builder[builder.Count - 1] = lastPart.Substring(0, length - offset);
			}

			SqlString result = builder.ToSqlString();
			if (isCompacted)
			{
				result.SetCompacted();
			}
			return result;
		}

		private void SetCompacted()
		{
			isCompacted = true;
			return;
		}

		private static int LengthOfPart(object part)
		{
			string partString = part as string;
			return partString == null ? 1 : partString.Length;
		}

		/// <summary>
		/// Returns the index of the first occurrence of <paramref name="text" />, case-insensitive.
		/// </summary>
		/// <param name="text">Text to look for in the <see cref="SqlString" />. Must be in lower
		/// case.</param>
		/// <remarks>
		/// The text must be located entirely in a string part of the <see cref="SqlString" />.
		/// Searching for <c>"a ? b"</c> in an <see cref="SqlString" /> consisting of
		/// <c>"a ", Parameter, " b"</c> will result in no matches.
		/// </remarks>
		/// <returns>The index of the first occurrence of <paramref name="text" />, or -1
		/// if not found.</returns>
		public int IndexOfCaseInsensitive(string text)
		{
			SqlString compacted = Compact();
			int offset = 0;
			foreach (object part in compacted.sqlParts)
			{
				string partString = part as string;
				if (partString != null)
				{
					int indexOf = StringHelper.IndexOfCaseInsensitive(partString, text);

					if (indexOf >= 0)
					{
						// Found
						return offset + indexOf;
					}
				}

				offset += LengthOfPart(part);
			}

			// Not found
			return -1;
		}

		public int LastIndexOfCaseInsensitive(string text)
		{
			SqlString compacted = Compact();
			int offset = 0;
			int foundOffset = -1;
			foreach (object part in compacted.sqlParts)
			{
				string partString = part as string;
				if (partString != null)
				{
					int indexOf = StringHelper.LastIndexOfCaseInsensitive(partString, text);

					if (indexOf >= 0)
					{
						// Found
						foundOffset = offset + indexOf;
					}
				}

				offset += LengthOfPart(part);
			}

			return foundOffset;
		}

		/// <summary>
		/// Removes all occurrences of white space characters from the beginning and end of this instance.
		/// </summary>
		/// <returns>
		/// A new SqlString equivalent to this instance after white space characters 
		/// are removed from the beginning and end.
		/// </returns>
		public SqlString Trim()
		{
			SqlStringBuilder builder = new SqlStringBuilder(Compact());

			// there is nothing in the builder to Trim 
			if (builder.Count == 0)
			{
				return builder.ToSqlString();
			}

			string begin = builder[0] as string;
			int endIndex = builder.Count - 1;
			string end = builder[endIndex] as string;

			if (endIndex == 0 && begin != null)
			{
				builder[0] = begin.Trim();
			}
			else
			{
				if (begin != null)
				{
					builder[0] = begin.TrimStart();
				}

				if (end != null)
				{
					builder[builder.Count - 1] = end.TrimEnd();
				}
			}

			return builder.ToSqlString();
		}

		public static SqlString operator +(SqlString lhs, SqlString rhs)
		{
			return lhs.Append(rhs);
		}

		#region System.Object Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			SqlString rhs;

			// Step1: Perform an equals test
			if (obj == this)
			{
				return true;
			}

			// Step	2: Instance of check
			rhs = obj as SqlString;
			if (rhs == null)
			{
				return false;
			}

			//Step 3: Check each important field

			// if they don't contain the same number of parts then we
			// can exit early because they are different
			if (sqlParts.Length != rhs.sqlParts.Length)
			{
				return false;
			}

			// they have the same number of parts - so compare each
			// part for equallity.
			for (int i = 0; i < sqlParts.Length; i++)
			{
				if (!sqlParts[i].Equals(rhs.sqlParts[i]))
				{
					return false;
				}
			}

			// nothing has been found that is different - so they are equal.
			return true;
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			int hashCode = 0;

			unchecked
			{
				for (int i = 0; i < sqlParts.Length; i++)
				{
					hashCode += sqlParts[i].GetHashCode();
				}
			}

			return hashCode;
		}

		/// <summary>
		/// Returns the SqlString in a string where it looks like
		/// SELECT col1, col2 FROM table WHERE col1 = ?
		/// </summary>
		/// <remarks>
		/// The question mark is used as the indicator of a parameter because at
		/// this point we are not using the specific provider so we don't know
		/// how that provider wants our parameters formatted.
		/// </remarks>
		/// <returns>A provider-neutral version of the CommandText</returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder(sqlParts.Length * 15);

			for (int i = 0; i < sqlParts.Length; i++)
			{
				builder.Append(sqlParts[i].ToString());
			}

			return builder.ToString();
		}

		#endregion

		private SqlString Clone()
		{
			object[] clonedParts = new object[sqlParts.Length];
			Array.Copy(sqlParts, clonedParts, sqlParts.Length);
			return new SqlString(clonedParts);
		}

		/// <summary>
		/// Make a copy of the SqlString, with new parameter references (Placeholders)
		/// </summary>
		public SqlString Copy()
		{
			SqlString clone = Clone();

			for (int i=0; i<clone.sqlParts.Length; i++)
			{
				if (clone.sqlParts[i] is Parameter)
				{
					var originalParameter = (Parameter)clone.sqlParts[i];
					var copyParameter = SqlCommand.Parameter.Placeholder;

					if (originalParameter.ParameterPosition < 0)
					{
						// placeholder for sub-query parameter
						copyParameter.ParameterPosition = originalParameter.ParameterPosition;
					}

					clone.sqlParts[i] = copyParameter;
				}
			}

			return clone;
		}

		/// <summary>
		/// Returns substring of this SqlString starting with the specified
		/// <paramref name="text" />. If the text is not found, returns an
		/// empty, not-null SqlString.
		/// </summary>
		/// <remarks>
		/// The method performs case-insensitive comparison, so the <paramref name="text" />
		/// passed should be in lower case.
		/// </remarks>
		public SqlString SubstringStartingWithLast(string text)
		{
			int lastIndex = LastIndexOfCaseInsensitive(text);
			return lastIndex >= 0 ? Substring(lastIndex) : Empty;
		}

		public SqlString Insert(int index, string text)
		{
			if (index < 0)
			{
				throw new ArgumentException("index should be greater than or equal to 0", "index");
			}

			SqlStringBuilder result = new SqlStringBuilder();

			int offset = 0;
			bool inserted = false;
			foreach (object part in sqlParts)
			{
				if (inserted)
				{
					result.AddObject(part);
					continue;
				}

				int nextOffset = offset + LengthOfPart(part);
				if (nextOffset < index)
				{
					result.AddObject(part);
					offset = nextOffset;
				}
				else if (nextOffset == index)
				{
					result.AddObject(part);
					result.Add(text);
					inserted = true;
				}
				else if (offset == index)
				{
					result.Add(text);
					result.AddObject(part);
					inserted = true;
				}
				else if (index > offset && index < nextOffset)
				{
					string partString = (string) part;
					result.Add(partString.Insert(index - offset, text));
					inserted = true;
				}
				else
				{
					throw new ArgumentException("index too large", "index");
				}
			}

			return result.ToSqlString();
		}

		public int GetParameterCount()
		{
			int count = 0;
			foreach (object part in sqlParts)
			{
				if (part is Parameter)
				{
					count++;
				}
			}

			return count;
		}

		public void Visit(ISqlStringVisitor visitor)
		{
			foreach (object part in sqlParts)
			{
				string partString = part as string;
				SqlString partSqlString = part as SqlString;
				if (partString != null)
				{
					visitor.String(partString);
				}
				else if (partSqlString != null && !SqlString.Parameter.Equals(partSqlString))
				{
					visitor.String(partSqlString);
				}
				else
				{
					visitor.Parameter((Parameter)part);
				}
			}
		}

		/// <summary>
		/// Parse SQL in <paramref name="sql" /> and create a SqlString representing it.
		/// </summary>
		/// <remarks>
		/// Parameter marks in single quotes will be correctly skipped, but otherwise the
		/// lexer is very simple and will not parse double quotes or escape sequences
		/// correctly, for example.
		/// </remarks>
		public static SqlString Parse(string sql)
		{
			SqlStringBuilder result = new SqlStringBuilder();

			bool inQuote = false;

			StringBuilder stringPart = new StringBuilder();

			foreach (char ch in sql)
			{
				switch (ch)
				{
					case '?':
						if (inQuote)
						{
							stringPart.Append(ch);
						}
						else
						{
							if (stringPart.Length > 0)
							{
								result.Add(stringPart.ToString());
								stringPart.Length = 0;
							}
							result.AddParameter();
						}
						break;

					case '\'':
						inQuote = !inQuote;
						stringPart.Append(ch);
						break;

					default:
						stringPart.Append(ch);
						break;
				}
			}

			if (stringPart.Length > 0)
			{
				result.Add(stringPart.ToString());
			}

			return result.ToSqlString();
		}

		// Exposing the internal parts now because I'm too lazy to write SqlStringTokenizer.
		// Use is strongly discouraged.
		public ICollection Parts
		{
			get { return sqlParts; }
		}

		public SqlString GetSubselectString()
		{
			return new SubselectClauseExtractor(Compact().sqlParts).GetSqlString();
		}

	    public bool EndsWithCaseInsensitive(string value)
	    {
            SqlString tempSql = Compact();
            if (tempSql.Count == 0)
            {
                return false;
            }

            string lastPart = tempSql.sqlParts[tempSql.Count - 1] as string;

            return lastPart != null && lastPart.EndsWith(value,StringComparison.InvariantCultureIgnoreCase);
		
	    }

	    public SqlString[] Split(string splitter)
	    {
	        int iterations = 0;
	        SqlString temp = Compact();
            List<SqlString> results = new List<SqlString>();
            int index;
	        do
            {
	            index = temp.IndexOfCaseInsensitive(splitter);
                int locationOfComma = index == -1 ?
                    temp.Length :
                    index;
                if (iterations++ > 100)
                    Debugger.Break();

                results.Add(temp.Substring(0, locationOfComma));
                temp = temp.Substring(locationOfComma+1);
	        } while (index != -1);
	        return results.ToArray();
	    }
	}
}
