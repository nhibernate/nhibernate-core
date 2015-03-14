using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NHibernate.SqlCommand.Parser;
using System.Text.RegularExpressions;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// This is a non-modifiable SQL statement that is ready to be prepared 
	/// and sent to the Database for execution.
	/// </summary>
	/// <remarks>
	/// <para>A <see cref="SqlString"/> represents a (potentially partial) SQL query string 
	/// that may or may not contain query parameter references. A <see cref="SqlString"/>
	/// decomposes the underlying SQL query string into a list of parts. Each part is either
	/// 1) a string part, which represents a fragment of the underlying SQL query string that 
	/// does not contain any parameter references, or 2) a parameter part, which represents
	/// a single query parameter reference in the underlying SQL query string.
	/// </para>
	/// <para>The <see cref="SqlString"/> constructors ensure that the number of string parts 
	/// in a <see cref="SqlString"/> are kept to an absolute minimum (as compact as possible) 
	/// by concatenating any adjoining string parts into a single string part.
	/// </para>
	/// <para>
	/// Substring operations on a <see cref="SqlString"/> (such as <see cref="Substring(int,int)"/>, 
	/// <see cref="Split"/>, <see cref="Trim"/>) return a <see cref="SqlString"/> that reuses the parts 
	/// list of the <see cref="SqlString"/> instance on which the operation was performed. 
	/// Besides a reference to this parts list, the resulting <see cref="SqlString"/> instance 
	/// also stores the character offset into the original underlying SQL string at which the 
	/// substring starts and the length of the substring. By avoiding the unnecessary rebuilding 
	/// of part lists these operations have O(1) behaviour rather than O(n) behaviour.
	/// </para>
	/// <para>
	/// If you need to modify this object pass it to a <see cref="SqlStringBuilder"/> and
	/// get a new object back from it.
	/// </para>
	/// </remarks>
	[Serializable]
	public class SqlString : ICollection, IEnumerable<object>
	{
		/// <summary>
		/// Empty <see cref="SqlString"/> instance.
		/// </summary>
		public static readonly SqlString Empty = new SqlString(Enumerable.Empty<object>());

		#region Instance fields

		/// <summary>
		/// Immutable list of string and parameter parts that make up this <see cref="SqlString"/>.
		/// This list may be shared by multiple <see cref="SqlString"/> instances that present 
		/// different fragments of a common underlying SQL query string.
		/// </summary>
		private readonly List<Part> _parts;

		/// <summary>
		/// List of SQL query parameter references that occur in this <see cref="SqlString"/>.
		/// </summary>
		private readonly SortedList<int, Parameter> _parameters;

		/// <summary>
		/// Cached index of first part in <see cref="_parts"/> that contains (part of)
		/// a SQL fragment that falls within the scope of this <see cref="SqlString"/> instance.
		/// </summary>
		private readonly int _firstPartIndex;

		/// <summary>
		/// Cached index of last part in <see cref="_parts"/> that contains (part of)
		/// a SQL fragment that falls within the scope of this <see cref="SqlString"/> instance.
		/// </summary>
		private readonly int _lastPartIndex;

		/// <summary>
		/// Index of first character of the underlying SQL query string that is within scope of
		/// this <see cref="SqlString"/> instance.
		/// </summary>
		private readonly int _sqlStartIndex;

		/// <summary>
		/// Number of characters of the underlying SQL query string that are within scope of
		/// this <see cref="SqlString"/> instance from <see cref="_sqlStartIndex"/> onwards.
		/// </summary>
		private readonly int _length;

		#endregion

		#region Constructor(s)

		/// <summary>
		/// Creates copy of other <see cref="SqlString"/>.
		/// </summary>
		/// <param name="other"></param>
		private SqlString(SqlString other)
		{
			_parts = other._parts;
			_sqlStartIndex = other._sqlStartIndex;
			_length = other._length;
			_firstPartIndex = other._firstPartIndex;
			_lastPartIndex = other._lastPartIndex;

			var parameterCount = other._parameters.Count;
			if (parameterCount > 0)
			{
				_parameters = new SortedList<int, Parameter>(other._parameters.Count);
				foreach (var parameterByIndex in other._parameters)
				{
					var otherParameter = parameterByIndex.Value;
					var parameter = otherParameter.Clone();

					if (otherParameter.ParameterPosition < 0)
					{
						// placeholder for sub-query parameter
						parameter.ParameterPosition = otherParameter.ParameterPosition;
					}

					_parameters.Add(parameterByIndex.Key, parameter);
				}
			}
			else
			{
				_parameters = Empty._parameters;
			}
		}

		/// <summary>
		/// Creates substring of other <see cref="SqlString"/>.
		/// </summary>
		/// <param name="other"></param>
		/// <param name="sqlStartIndex"></param>
		/// <param name="length"></param>
		private SqlString(SqlString other, int sqlStartIndex, int length)
		{
			_parts = other._parts;
			_sqlStartIndex = sqlStartIndex;
			_length = length;
			_firstPartIndex = other.GetPartIndexForSqlIndex(sqlStartIndex);
			_lastPartIndex = other.GetPartIndexForSqlIndex(_sqlStartIndex + _length - 1);

			if (_firstPartIndex != _lastPartIndex || _parts[_firstPartIndex].IsParameter)
			{
				_parameters = new SortedList<int, Parameter>(other._parameters.Count);
				using (var otherParameterEnum = other._parameters.GetEnumerator())
				{
					while (otherParameterEnum.MoveNext())
					{
						if (otherParameterEnum.Current.Key >= _sqlStartIndex) break;
					}

					var sqlEndIndex = _sqlStartIndex + _length;
					do
					{
						if (otherParameterEnum.Current.Key > sqlEndIndex) break;
						_parameters.Add(otherParameterEnum.Current.Key, otherParameterEnum.Current.Value);
					} while (otherParameterEnum.MoveNext());
				}
			}
			else
			{
				_parameters = Empty._parameters;
			}
		}

		/// <summary>
		/// Creates <see cref="SqlString"/> consisting of single string part.
		/// </summary>
		/// <param name="sql">A SQL fragment</param>
		public SqlString(string sql)
		{
			if (sql == null) throw new ArgumentNullException("sql");

			_parts = new List<Part>(1) { new Part(0, sql) };
			_parameters = Empty._parameters;
			_length = sql.Length;
		}

		/// <summary>
		/// Creates <see cref="SqlString"/> consisting of single parameter part.
		/// </summary>
		/// <param name="parameter">A query parameter</param>
		public SqlString(Parameter parameter)
		{
			if (parameter == null) throw new ArgumentNullException("parameter");

			_parts = new List<Part>(1) { new Part(0) };
			_parameters = new SortedList<int, Parameter>(1) { { 0, parameter } };
			_length = _parts[0].Length;
		}

		/// <summary>
		/// Creates <see cref="SqlString"/> consisting of multiple parts.
		/// </summary>
		/// <param name="parts">Arbitrary number of parts, which must be 
		/// either <see cref="string"/>, <see cref="Parameter"/> or <see cref="SqlString"/>
		/// values.</param>
		/// <remarks>The <see cref="SqlString"/> instance is automatically compacted.</remarks>
		public SqlString(params object[] parts)
			: this((IEnumerable<object>)parts)
		{ }

		private SqlString(IEnumerable<object> parts)
		{
			_parts = new List<Part>();
			_parameters = new SortedList<int, Parameter>();

			var sqlIndex = 0;
			var pendingContent = new StringBuilder();  // Collect adjoining string parts (the compaction).
			foreach (var part in parts)
			{
				Add(part, pendingContent, ref sqlIndex);
			}
			AppendAndResetPendingContent(pendingContent, ref sqlIndex);

			_firstPartIndex = _parts.Count > 0 ? 0 : -1;
			_lastPartIndex = _parts.Count - 1;
			_length = sqlIndex;
		}

		#endregion

		#region Factory methods

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
			return new SqlString(ParseParts(sql));
		}

		private static IEnumerable<object> ParseParts(string text)
			{
			if (string.IsNullOrEmpty(text)) yield break;

			int offset = 0;
			int maxOffset = text.Length;
			int partOffset = 0;

			while (offset < maxOffset)
			{
				var ch = text[offset];
				switch (ch)
				{
					case '?':      // Parameter marker
						if (offset > partOffset)
						{
							yield return text.Substring(partOffset, offset - partOffset);
						}
						yield return Parameter.Placeholder;
						partOffset = offset += 1;
						break;
					case '\'':      // String literals
					case '\"':      // ANSI quoted identifiers
					case '[':       // Sql Server quoted indentifiers
						offset += SqlParserUtils.ReadDelimitedText(text, maxOffset, offset);
						continue;
					case '/':
						if (offset + 1 < maxOffset && text[offset + 1] == '*')
						{
							offset += SqlParserUtils.ReadMultilineComment(text, maxOffset, offset);
							continue;
						}
						break;
					case '-':
						if (offset + 1 < maxOffset && text[offset + 1] == '-')
							{
							offset += SqlParserUtils.ReadLineComment(text, maxOffset, offset);
							continue;
							}
						break;
						}

				offset++;
				}

			if (maxOffset > partOffset)
			{
				yield return text.Substring(partOffset, offset - partOffset);
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of SqlParts contained in this SqlString.
		/// </summary>
		/// <value>The number of SqlParts contained in this SqlString.</value>
		public int Count
		{
			get { return _length > 0 ? _lastPartIndex - _firstPartIndex + 1 : 0; }
		}

		public int Length
		{
			get { return _length; }
		}

		#endregion

		#region Operators

		public static SqlString operator +(SqlString lhs, SqlString rhs)
		{
			return lhs.Append(rhs);
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Appends the SqlString parameter to the end of the current SqlString to create a 
		/// new SqlString object.
		/// </summary>
		/// <param name="sql">The SqlString to append.</param>
		/// <returns>A new SqlString object.</returns>
		/// <remarks>
		/// A SqlString object is immutable so this returns a new SqlString.  If multiple Appends 
		/// are called it is better to use the SqlStringBuilder.
		/// </remarks>
		public SqlString Append(SqlString sql)
		{
			if (sql == null || sql._length == 0) return this;
			if (_length == 0) return sql;
			return new SqlString(new object[] { this, sql });
		}

		/// <summary>
		/// Appends the string parameter to the end of the current SqlString to create a 
		/// new SqlString object.
		/// </summary>
		/// <param name="text">The string to append.</param>
		/// <returns>A new SqlString object.</returns>
		/// <remarks>
		/// A SqlString object is immutable so this returns a new SqlString.  If multiple Appends 
		/// are called it is better to use the SqlStringBuilder.
		/// </remarks>
		public SqlString Append(string text)
		{
			if (string.IsNullOrEmpty(text)) return this;
			if (_length == 0) return new SqlString(text);
			return new SqlString(new object[] { this, text });
		}

		/// <summary>
		/// Compacts the SqlString into the fewest parts possible.
		/// </summary>
		/// <returns>A new SqlString.</returns>
		/// <remarks>
		/// Combines all SqlParts that are strings and next to each other into
		/// one SqlPart.
		/// </remarks>
		[Obsolete]
		public SqlString Compact()
		{
			// FIXME: As of january 2012, the SqlString is always in compact form. Once this is settled, perhaps we should remove SqlString.Compact()?
			return this;
		}

		/// <summary>
		/// Makes a copy of the SqlString, with new parameter references (Placeholders)
		/// </summary>
		public SqlString Copy()
		{
			return new SqlString(this);
		}

		/// <summary>
		/// Determines whether the end of this instance matches the specified String.
		/// </summary>
		/// <param name="value">A string to seek at the end.</param>
		/// <returns><see langword="true" /> if the end of this instance matches value; otherwise, <see langword="false" /></returns>
		public bool EndsWith(string value)
		{
			return value != null
				&& value.Length <= _length
				&& IndexOf(value, _length - value.Length, value.Length, StringComparison.InvariantCulture) >= 0;
		}

		public bool EndsWithCaseInsensitive(string value)
		{
			return value != null
				&& value.Length <= _length
				&& IndexOf(value, _length - value.Length, value.Length, StringComparison.CurrentCultureIgnoreCase) >= 0;
		}

		public IEnumerable<Parameter> GetParameters()
		{
			return _parameters.Values;
		}

		public int GetParameterCount()
		{
			return _parameters.Count;
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
			return IndexOf(text, 0, _length, StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// Returns the index of the first occurrence of <paramref name="text" />, case-insensitive.
		/// </summary>
		/// <param name="text">Text to look for in the <see cref="SqlString" />. Must be in lower</param>
		/// <param name="startIndex">The zero-based index of the search starting position.</param>
		/// <param name="length">The number of character positions to examine.</param>
		/// <param name="stringComparison">One of the enumeration values that specifies the rules for the search.</param>
		/// <remarks>
		/// The text must be located entirely in a string part of the <see cref="SqlString" />.
		/// Searching for <c>"a ? b"</c> in an <see cref="SqlString" /> consisting of
		/// <c>"a ", Parameter, " b"</c> will result in no matches.
		/// </remarks>
		/// <returns>The index of the first occurrence of <paramref name="text" />, or -1
		/// if not found.</returns>
		public int IndexOf(string text, int startIndex, int length, StringComparison stringComparison)
		{
			if (text == null) throw new ArgumentNullException("text");

			var sqlSearchStartIndex = _sqlStartIndex + startIndex;
			var maxSearchLength = Math.Min(length, _sqlStartIndex + _length - sqlSearchStartIndex);
			if (maxSearchLength >= text.Length)
			{
				var partIndex = GetPartIndexForSqlIndex(sqlSearchStartIndex);
				if (partIndex >= 0)
				{
					while (maxSearchLength > 0 && partIndex <= _lastPartIndex)
					{
						var part = _parts[partIndex];
						var partStartOffset = sqlSearchStartIndex - part.SqlIndex;
						var partLength = Math.Min(maxSearchLength, part.Length - partStartOffset);
						var partOffset = part.Content.IndexOf(text, partStartOffset, partLength, stringComparison);
						if (partOffset >= 0) return part.SqlIndex + partOffset - _sqlStartIndex;

						sqlSearchStartIndex += partLength;
						maxSearchLength -= partLength;
						partIndex++;
					}
				}
			}

			return -1;
		}

		public SqlString Insert(int index, string text)
		{
			if (string.IsNullOrEmpty(text)) return this;
			return new SqlString(new object[] { Substring(0, index), text, Substring(index, _length - index) });
		}

		public SqlString Insert(int index, SqlString sql)
		{
			if (sql == null || sql._length == 0) return this;
			return new SqlString(new object[] { Substring(0, index), sql, Substring(index, _length - index) });
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
		public int LastIndexOfCaseInsensitive(string text)
		{
			return LastIndexOf(text, 0, _length, StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// Returns the index of the first occurrence of <paramref name="text" />, case-insensitive.
		/// </summary>
		/// <param name="text">Text to look for in the <see cref="SqlString" />. Must be in lower case.</param>
		/// <param name="startIndex">The zero-based index of the search starting position.</param>
		/// <param name="length">The number of character positions to examine.</param>
		/// <param name="stringComparison">One of the enumeration values that specifies the rules for the search.</param>
		/// <remarks>
		/// The text must be located entirely in a string part of the <see cref="SqlString" />.
		/// Searching for <c>"a ? b"</c> in an <see cref="SqlString" /> consisting of
		/// <c>"a ", Parameter, " b"</c> will result in no matches.
		/// </remarks>
		/// <returns>The index of the first occurrence of <paramref name="text" />, or -1
		/// if not found.</returns>
		private int LastIndexOf(string text, int startIndex, int length, StringComparison stringComparison)
		{
			if (text == null) throw new ArgumentNullException("text");

			var sqlSearchEndIndex = _sqlStartIndex + Math.Min(_length, startIndex + length);
			var maxSearchLength = sqlSearchEndIndex - _sqlStartIndex - startIndex;
			if (maxSearchLength > text.Length)
			{
				var partIndex = GetPartIndexForSqlIndex(sqlSearchEndIndex - 1);
				if (partIndex >= 0)
				{
					while (maxSearchLength > 0 && partIndex >= _firstPartIndex)
					{
						var part = _parts[partIndex];
						var partEndOffset = sqlSearchEndIndex - part.SqlIndex;
						var partLength = Math.Min(maxSearchLength, partEndOffset);
						var partOffset = part.Content.LastIndexOf(text, partEndOffset - 1, partLength, stringComparison);
						if (partOffset >= 0) return part.SqlIndex + partOffset - _sqlStartIndex;

						sqlSearchEndIndex -= partLength;
						maxSearchLength -= partLength;
						partIndex--;
					}
				}
			}

			return -1;
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
			return new SqlString(ReplaceParts(oldValue, newValue));
		}

		private IEnumerable<object> ReplaceParts(string oldValue, string newValue)
		{
			foreach (var part in this)
			{
				var content = part as string;
				yield return content != null
					? content.Replace(oldValue, newValue)
					: part;
			}
		}

		public SqlString[] Split(string splitter)
		{
			return SplitParts(splitter).ToArray();
		}

		internal SqlString[] SplitWithRegex(string pattern)
		{
			var sql = Regex.Split(ToString(), pattern).Select(s => SqlString.Parse(s)).ToArray();
			var parameters = _parameters.Values;
			int i = 0;
			foreach (var p in sql.SelectMany(s => s.GetParameters()))
			{
				p.BackTrack = parameters[i].BackTrack;
				i++;
			}
			return sql;
		}

		private IEnumerable<SqlString> SplitParts(string splitter)
		{
			var startIndex = 0;
			while (startIndex < _length)
			{
				var splitterIndex = IndexOf(splitter, startIndex, _length - startIndex, StringComparison.InvariantCultureIgnoreCase);
				if (splitterIndex < 0) break;

				yield return new SqlString(this, _sqlStartIndex + startIndex, splitterIndex - startIndex);
				startIndex = splitterIndex + splitter.Length;
			}

			if (startIndex < _length)
			{
				yield return new SqlString(this, _sqlStartIndex + startIndex, _length - startIndex);
			}
		}

		/// <summary>
		/// Determines whether the beginning of this SqlString matches the specified System.String,
		/// using case-insensitive comparison.
		/// </summary>
		/// <param name="value">The System.String to seek</param>
		/// <returns>true if the SqlString starts with the value.</returns>
		public bool StartsWithCaseInsensitive(string value)
		{
			return value != null
				&& value.Length <= _length
				&& IndexOf(value, 0, value.Length, StringComparison.InvariantCultureIgnoreCase) >= 0;
		}

		/// <summary>
		/// Determines whether the sqlString matches the specified System.String,
		/// using case-insensitive comparison
		/// </summary>
		/// <param name="value">The System.String to match</param>
		/// <returns>true if the SqlString matches the value.</returns>
		public bool EqualsCaseInsensitive(string value)
		{
			return value != null
				&& value.Length == _length
				&& IndexOf(value, 0, value.Length, StringComparison.InvariantCultureIgnoreCase) >= 0;
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
			return Substring(startIndex, _length - startIndex);
		}

		public SqlString Substring(int startIndex, int length)
		{
			if (startIndex == 0 && length == _length) return this;

			length = Math.Min(_length - startIndex, length);
			return length > 0
				? new SqlString(this, _sqlStartIndex + startIndex, length)
				: Empty;
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

		/// <summary>
		/// Removes all occurrences of white space characters from the beginning and end of this instance.
		/// </summary>
		/// <returns>
		/// A new SqlString equivalent to this instance after white space characters 
		/// are removed from the beginning and end.
		/// </returns>
		public SqlString Trim()
		{
			if (_firstPartIndex < 0) return this;

			var firstPart = _parts[_firstPartIndex];
			var firstPartOffset = _sqlStartIndex - firstPart.SqlIndex;
			var firstPartLength = Math.Min(firstPart.Length - firstPartOffset, _length);
			while (firstPartLength > 0 && char.IsWhiteSpace(firstPart.Content[firstPartOffset]))
			{
				firstPartOffset++;
				firstPartLength--;
			}

			var lastPart = _parts[_lastPartIndex];
			var lastPartOffset = _sqlStartIndex + _length - 1 - lastPart.SqlIndex;
			var lastPartLength = Math.Min(lastPartOffset + 1, _length);
			while (lastPartLength > 0 && char.IsWhiteSpace(lastPart.Content[lastPartOffset]))
			{
				lastPartOffset--;
				lastPartLength--;
			}

			var sqlStartIndex = firstPart.SqlIndex + firstPartOffset;
			var length = lastPart.SqlIndex + lastPartOffset + 1 - sqlStartIndex;
			return length > 0
				? new SqlString(this, sqlStartIndex, length)
				: Empty;
		}

		public void Visit(ISqlStringVisitor visitor)
		{
			foreach (object part in this)
			{
				var partString = part as string;
				if (partString != null)
				{
					visitor.String(partString);
					continue;
				}

				var partSqlString = part as SqlString;
				if (partSqlString != null)
				{
					visitor.String(partSqlString);
					continue;
				}

				var partParameter = part as Parameter;
				if (partParameter != null)
				{
					visitor.Parameter(partParameter);
				}
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Locate the part that contains the requested character index, and return the
		/// part's index. Return -1 if the character position isn't found.
		/// </summary>
		private int GetPartIndexForSqlIndex(int sqlIndex)
		{
			if (sqlIndex < _sqlStartIndex || sqlIndex >= _sqlStartIndex + _length) return -1;

			var min = _firstPartIndex;
			var max = _lastPartIndex;
			while (min < max)
			{
				var i = (min + max + 1) / 2;
				if (sqlIndex < _parts[i].SqlIndex)
				{
					max = i - 1;
				}
				else
				{
					min = i;
				}
			}

			var part = _parts[min];
			return sqlIndex >= part.SqlIndex && sqlIndex < part.SqlIndex + part.Length ? min : -1;
		}

		private void Add(object part, StringBuilder pendingContent, ref int sqlIndex)
		{
			var stringPart = part as string;
			if (stringPart != null)
			{
				pendingContent.Append(stringPart);
				return;
			}

			var parameter = part as Parameter;
			if (parameter != null)
			{
				AppendAndResetPendingContent(pendingContent, ref sqlIndex);

				_parts.Add(new Part(sqlIndex));
				_parameters.Add(sqlIndex, parameter);
				sqlIndex += 1;
				return;
			}

			var sql = part as SqlString;
			if (sql != null)
			{
				foreach (var otherPart in sql)
				{
					Add(otherPart, pendingContent, ref sqlIndex);
				}
				return;
			}

			throw new ArgumentException("Only string, Parameter or SqlString values are supported as SqlString parts.");
		}

		/// <summary>
		/// It the pendingContent is non-empty, append it as a new part and reset the pendingContent
		/// to empty. The new part will be given the sqlIndex. After return, the sqlIndex will have
		/// been updated to the next available index.
		/// </summary>
		/// <param name="pendingContent"></param>
		/// <param name="sqlIndex"></param>
		private void AppendAndResetPendingContent(StringBuilder pendingContent, ref int sqlIndex)
		{
			if (pendingContent.Length > 0)
			{
				_parts.Add(new Part(sqlIndex, pendingContent.ToString()));
				sqlIndex += pendingContent.Length;
				pendingContent.Length = 0;
			}
		}

		#endregion

		#region ICollection Members

		void ICollection.CopyTo(Array array, int index)
		{
			foreach (var part in this)
			{
				array.SetValue(part, index++);
			}
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		object ICollection.SyncRoot
		{
			get { return null; }
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IEnumerable<object> Members

		public IEnumerator<object> GetEnumerator()
		{
			if (_firstPartIndex < 0) yield break;

			// Yield (substring of) first part
			var partIndex = _firstPartIndex;
			var part = _parts[partIndex++];
			if (part.IsParameter)
			{
				yield return _parameters[part.SqlIndex];
			}
			else
			{
				var firstPartOffset = _sqlStartIndex - part.SqlIndex;
				var firstPartLength = Math.Min(part.Length - firstPartOffset, _length);
				yield return part.Content.Substring(firstPartOffset, firstPartLength);
			}

			if (_firstPartIndex == _lastPartIndex) yield break;

			// Yield middle parts
			while (partIndex < _lastPartIndex)
			{
				part = _parts[partIndex++];
				yield return part.IsParameter
					? (object)_parameters[part.SqlIndex]
					: part.Content;
			}

			// Yield (substring of) last part
			part = _parts[partIndex];
			if (part.IsParameter)
			{
				yield return _parameters[part.SqlIndex];
			}
			else
			{
				var lastPartLength = _sqlStartIndex + _length - part.SqlIndex;
				yield return part.Content.Substring(0, lastPartLength);
			}
		}

		#endregion

		#region System.Object Members

		public override bool Equals(object obj)
		{
			var other = obj as SqlString;
			if (other == null) return false;
			if (other == this) return true;

			// Exit early if the length or number of parts differ - cannot be equal then.
			if (_length != other._length) return false;
			if (_lastPartIndex - _firstPartIndex != other._lastPartIndex - other._firstPartIndex) return false;
			if (_parameters.Count != other._parameters.Count) return false;

			using (var partEnum = this.GetEnumerator())
			using (var otherPartEnum = other.GetEnumerator())
			{
				while (partEnum.MoveNext())
				{
					if (!otherPartEnum.MoveNext()) return false;
					if (!Equals(partEnum.Current, otherPartEnum.Current)) return false;
				}
				if (otherPartEnum.MoveNext()) return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			const uint FNV_OFFSET_BASIS = 2166136261;
			const uint FNV_PRIME = 16777619;

			uint hashCode = FNV_OFFSET_BASIS;
			unchecked
			{
				// Iterate the calculated parts - cannot use the underlying _parts
				// directly, as that may contain elements no longer part of this string.
				foreach (var part in this)
				{
					hashCode ^= (uint)part.GetHashCode();
					hashCode *= FNV_PRIME;
				}

				return (int)hashCode;
			}
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
			return ToString(0, _length);
		}

		public string ToString(int startIndex)
		{
			return ToString(startIndex, _length - startIndex);
		}

		public string ToString(int startIndex, int length)
		{
			var sqlStartIndex = _sqlStartIndex + startIndex;
			var nextPartIndex = GetPartIndexForSqlIndex(sqlStartIndex);
			if (nextPartIndex < 0) return string.Empty;

			var maxSearchLength = Math.Min(_sqlStartIndex + _length - sqlStartIndex, length);

			var part = _parts[nextPartIndex++];
			var firstPartOffset = sqlStartIndex - part.SqlIndex;
			var firstPartLength = Math.Min(part.Length - firstPartOffset, maxSearchLength);

			// Shortcut when result can be taken from single part 
			if (firstPartLength == maxSearchLength)
			{
				return part.Length == firstPartLength
					? part.Content
					: part.Content.Substring(firstPartOffset, firstPartLength);
			}

			// Add (substring of) first part
			var result = new StringBuilder(length);
			result.Append(part.Content, firstPartOffset, firstPartLength);
			maxSearchLength -= firstPartLength;

			// Add middle parts
			part = _parts[nextPartIndex++];
			while (nextPartIndex <= _lastPartIndex && part.Length <= maxSearchLength)
			{
				result.Append(part.Content);
				maxSearchLength -= part.Length;
				part = _parts[nextPartIndex++];
			}

			// Add (substring of) last part
			if (maxSearchLength > 0)
			{
				result.Append(part.Content, 0, maxSearchLength);
			}

			return result.ToString();
		}

		#endregion

		public SqlString GetSubselectString()
		{
			return new SubselectClauseExtractor(this).GetSqlString();
		}

		[Serializable]
		private struct Part : IEquatable<Part>
		{
			public readonly int SqlIndex;
			public readonly string Content;
			public readonly bool IsParameter;

			public Part(int sqlIndex, string content)
			{
				SqlIndex = sqlIndex;
				Content = content;
				IsParameter = false;
			}

			public Part(int sqlIndex) : this(sqlIndex, "?")
			{
				IsParameter = true;
			}

			public int Length
			{
				get { return Content.Length; }
			}

			public bool Equals(Part other)
			{
				return IsParameter == other.IsParameter && Content == other.Content;
			}

			public override bool Equals(object obj)
			{
				return (obj is Part && Equals((Part)obj));
			}

			public override int GetHashCode()
			{
				return Content.GetHashCode();
			}

			public override string ToString()
			{
				return Content;
			}
		}
	}
}