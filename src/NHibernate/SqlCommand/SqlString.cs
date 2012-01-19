using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace NHibernate.SqlCommand
{
	using NHibernate.Exceptions;

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
	public class SqlString: ICollection, IEnumerable<object>
	{
		public static readonly SqlString Empty = new SqlString(Enumerable.Empty<object>());

		#region Instance fields

		private readonly List<Part> _parts;
		private readonly SortedList<int, Parameter> _parameters;
		private readonly int _firstPartIndex;
		private readonly int _lastPartIndex;
		private readonly int _sqlStartIndex;
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

		public SqlString(string sql)
		{
			if (sql == null) throw new ArgumentNullException("sql");

			_parts = new List<Part>(1) { new Part(0, sql) };
			_parameters = Empty._parameters;
			_length = sql.Length;
		}

		public SqlString(Parameter parameter)
		{
			if (parameter == null) throw new ArgumentNullException("parameter");

			_parts = new List<Part>(1) { new Part(0) };
			_parameters = new SortedList<int, Parameter>(1) { { 0, parameter} };
			_length = _parts[0].Length;
		}

		public SqlString(params object[] parts)
			: this((IEnumerable<object>)parts)
		{}

		private SqlString(IEnumerable<object> parts)
		{
			_parts = new List<Part>();
			_parameters = new SortedList<int, Parameter>();

			var sqlIndex = 0;
			var content = new StringBuilder();
			foreach (var part in parts)
			{
				Add(part, content, ref sqlIndex);
			}
			FlushContent(content, ref sqlIndex);

			_firstPartIndex = _parts.Count > 0 ? 0 : -1;
			_lastPartIndex = _parts.Count - 1;
			_length = sqlIndex;
		}

		private void Add(object part, StringBuilder content, ref int sqlIndex)
		{
			var stringPart = part as string;
			if (stringPart != null)
			{
				content.Append(stringPart);
				return;
			}

			var parameter = part as Parameter;
			if (parameter != null)
			{
				FlushContent(content, ref sqlIndex);

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
					Add(otherPart, content, ref sqlIndex);
				}
				return;
			}

			throw new ArgumentException("Only string, Parameter or SqlString values are supported as SqlString parts.");
		}

		private void FlushContent(StringBuilder content, ref int sqlIndex)
		{
			if (content.Length > 0)
			{
				_parts.Add(new Part(sqlIndex, content.ToString()));
				sqlIndex += content.Length;
				content.Length = 0;
			}
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
			var result = new SqlStringBuilder();
			var content = new StringBuilder();

			bool inQuote = false;
			foreach (char ch in sql)
			{
				switch (ch)
				{
					case '?':
						if (inQuote)
						{
							content.Append(ch);
						}
						else
						{
							if (content.Length > 0)
							{
								result.Add(content.ToString());
								content.Length = 0;
							}
							result.AddParameter();
						}
						break;

					case '\'':
						inQuote = !inQuote;
						content.Append(ch);
						break;

					default:
						content.Append(ch);
						break;
				}
			}

			if (content.Length > 0)
			{
				result.Add(content.ToString());
			}

			return result.ToSqlString();
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

		[Obsolete("Use SqlString.Count and SqlString.GetEnumerator properties")]
		public ICollection Parts
		{
			get { return this; }
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
		public SqlString Compact()
		{
			return this;
		}
		
		/// <summary>
		/// Make a copy of the SqlString, with new parameter references (Placeholders)
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

		public int IndexOf(string value, int startIndex, int length, StringComparison stringComparison)
		{
			if (value == null) throw new ArgumentNullException("value");

			var sqlSearchStartIndex = _sqlStartIndex + startIndex;
			var maxSearchLength = Math.Min(length, _sqlStartIndex + _length - sqlSearchStartIndex);
			if (maxSearchLength >= value.Length)
			{
				var partIndex = GetPartIndexForSqlIndex(sqlSearchStartIndex);
				if (partIndex >= 0)
				{
					while (maxSearchLength > 0 && partIndex <= _lastPartIndex)
					{
						var part = _parts[partIndex];
						var partStartOffset = sqlSearchStartIndex - part.SqlIndex;
						var partLength = Math.Min(maxSearchLength, part.Length - partStartOffset);
						var partOffset = part.Content.IndexOf(value, partStartOffset, partLength, stringComparison);
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
		
		public int LastIndexOfCaseInsensitive(string text)
		{
			return LastIndexOf(text, 0, _length, StringComparison.InvariantCultureIgnoreCase);
		}

		private int LastIndexOf(string value, int startIndex, int length, StringComparison stringComparison)
		{
			if (value == null) throw new ArgumentNullException("value");

			var sqlSearchEndIndex = _sqlStartIndex + Math.Min(_length, startIndex + length);
			var maxSearchLength = sqlSearchEndIndex - _sqlStartIndex - startIndex;
			if (maxSearchLength > value.Length)
			{
				var partIndex = GetPartIndexForSqlIndex(sqlSearchEndIndex - 1);
				if (partIndex >= 0)
				{
					while (maxSearchLength > 0 && partIndex >= _firstPartIndex)
					{
						var part = _parts[partIndex];
						var partEndOffset = sqlSearchEndIndex - part.SqlIndex;
						var partLength = Math.Min(maxSearchLength, partEndOffset);
						var partOffset = part.Content.LastIndexOf(value, partEndOffset - 1, partLength, stringComparison);
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

		public IEnumerable<SqlToken> Tokenize()
		{
			return new Tokenizer(this, SqlTokenType.All);
		}

		public IEnumerable<SqlToken> Tokenize(SqlTokenType includeTokens)
		{
			return new Tokenizer(this, includeTokens);
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

		internal static int LengthOfPart(object part)
		{
			var partString = part as string;
			return partString == null ? 1 : partString.Length;
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
					? (object)this._parameters[part.SqlIndex]
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
			int hashCode = 0;
			unchecked
			{
				for (int i = 0; i < _parts.Count; i++)
				{
					hashCode += _parts[i].GetHashCode();
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
			return ToString(0, _length);
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
		private struct Part: IEquatable<Part>
		{
			public readonly int SqlIndex;
			public readonly string Content;
			public readonly bool IsParameter;

			public Part(int sqlIndex, string content)
			{
				this.SqlIndex = sqlIndex;
				this.Content = content;
				this.IsParameter = false;
			}

			public Part(int sqlIndex)
			{
				this.SqlIndex = sqlIndex;
				this.Content = "?";
				this.IsParameter = true;
			}

			public int Length
			{
				get { return this.Content.Length; }
			}

			public bool Equals(Part other)
			{
				return this.IsParameter == other.IsParameter
					&& this.Content == other.Content;
			}

			public override bool Equals(object obj)
			{
				return (obj is Part && this.Equals((Part)obj));
			}

			public override int GetHashCode()
			{
				return this.Content.GetHashCode();
			}

			public override string ToString()
			{
				return this.Content;
			}
		}

		/// <summary>
		/// Splits a <see cref="SqlString"/> into <see cref="SqlToken"/>s.
		/// </summary>
		private class Tokenizer : IEnumerable<SqlToken>
		{
			private readonly SqlString _sql;
			private readonly SqlTokenType _includeTokens; 

			public Tokenizer(SqlString sql, SqlTokenType includeTokens)
			{
				if (sql == null) throw new ArgumentNullException("sql");
				_sql = sql;
				_includeTokens = includeTokens;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public IEnumerator<SqlToken> GetEnumerator()
			{
				int sqlIndex = 0;
				foreach (var part in _sql)
				{
					var parameter = part as Parameter;
					if (parameter != null)
					{
						if (CanYield(SqlTokenType.Parameter))
						{
							yield return new SqlToken(SqlTokenType.Parameter, _sql, sqlIndex, 1);
						}
						sqlIndex++;
						continue;
					}

					var text = part as string;
					if (text != null)
					{
						int offset = 0;
						int maxOffset = text.Length;
						int tokenOffset = 0;
						SqlTokenType nextTokenType = 0;
						int nextTokenOffset = -1;
						int nextTokenLength = 0;

						while (offset < maxOffset)
						{
							var ch = text[offset];
							switch (ch)
							{
								case '(':
									nextTokenType = SqlTokenType.BlockBegin;
									nextTokenOffset = offset;
									nextTokenLength = 1;
									break;
								case ')':
									nextTokenType = SqlTokenType.BlockEnd;
									nextTokenOffset = offset;
									nextTokenLength = 1;
									break;
								case '\'':      // String literals
								case '\"':      // ANSI quoted identifiers
								case '[':       // Sql Server quoted indentifiers
									nextTokenType = SqlTokenType.QuotedText;
									nextTokenOffset = offset;
									nextTokenLength = ReadQuotedText(text, maxOffset, ref offset);
									break;
								case ',':
									nextTokenType = SqlTokenType.ListSeparator;
									nextTokenOffset = offset;
									nextTokenLength = 1;
									break;
								case '*':
									if (offset > 0 && text[offset - 1] == '/')
									{
										nextTokenType = SqlTokenType.Comment;
										nextTokenOffset = offset - 1;
										nextTokenLength = ReadMultilineComment(text, maxOffset, ref offset);
									}
									break;
								case '-':
									if (offset > 0 && text[offset - 1] == '-')
									{
										nextTokenType = SqlTokenType.Comment;
										nextTokenOffset = offset - 1;
										nextTokenLength = ReadLineComment(text, maxOffset, ref offset);
									}
									break;
								default:
									if (char.IsWhiteSpace(ch))
									{
										nextTokenType = SqlTokenType.Whitespace;
										nextTokenOffset = offset;
										nextTokenLength = ReadWhitespace(text, maxOffset, ref offset);
									}
									break;
							}

							if (nextTokenType != 0)
							{
								if (nextTokenOffset > tokenOffset)
								{
									if (CanYield(SqlTokenType.UnquotedText))
									{
										yield return new SqlToken(SqlTokenType.UnquotedText, _sql, sqlIndex + tokenOffset, nextTokenOffset - tokenOffset);
									}
									tokenOffset = nextTokenOffset;
								}

								if (CanYield(nextTokenType))
								{
									yield return new SqlToken(nextTokenType, _sql, sqlIndex + nextTokenOffset, nextTokenLength);
								}
								tokenOffset += nextTokenLength;

								nextTokenType = 0;
								nextTokenOffset = -1;
								nextTokenLength = 0;
							}

							offset++;
						}

						if (offset > tokenOffset && CanYield(SqlTokenType.UnquotedText))
						{
							yield return new SqlToken(SqlTokenType.UnquotedText, _sql, sqlIndex + tokenOffset, maxOffset - tokenOffset);
						}
						sqlIndex += maxOffset;
					}
				}
			}

			private bool CanYield(SqlTokenType tokenType)
			{
				return (_includeTokens & tokenType) == tokenType;
			}

			private static int ReadQuotedText(string text, int maxOffset, ref int offset)
			{
				var startOffset = offset;
				char quoteEndChar;

				var quoteChar = text[offset++];
				switch (quoteChar)
				{
					case '\'':
					case '"':
						quoteEndChar = quoteChar;
						break;
					case '[':
						quoteEndChar = ']';
						break;
					default:
						throw new SqlParseException(string.Format("Quoted text cannot start with '{0}' character", text[offset]));
				}

				// TODO: handle quote escaping
				for (; offset < maxOffset; offset++)
				{
					if (text[offset] == quoteEndChar)
					{
						return offset + 1 - startOffset;
					}
				}

				throw new SqlParseException(string.Format("Cannot find terminating '{0}' character for quoted text.", quoteEndChar));
			}

			private static int ReadLineComment(string text, int maxOffset, ref int offset)
			{
				var startOffset = offset - 1;

				offset++;
				for (; offset < maxOffset; offset++)
				{
					switch (text[offset])
					{
						case '\r':
						case '\n':
							return offset + 1 - startOffset;
					}
				}

				return offset - startOffset;
			}

			private static int ReadMultilineComment(string text, int maxOffset, ref int offset)
			{
				var startOffset = offset - 1;

				var prevChar = text[offset++];
				for (; offset < maxOffset; offset++)
				{
					var ch = text[offset];
					if (ch == '/' && prevChar == '*')
					{
						return offset + 1 - startOffset;
					}

					prevChar = ch;
				}

				throw new SqlParseException(string.Format("Cannot find terminating '*/' string for multiline comment."));
			}

			private static int ReadWhitespace(string text, int maxOffset, ref int offset)
			{
				var startOffset = offset;

				offset++;
				while (offset < maxOffset)
				{
					if (!char.IsWhiteSpace(text[offset])) break;
					offset++;
				}

				var result = offset - startOffset;
				offset--;
				return result;
			}
		}
	}
}
