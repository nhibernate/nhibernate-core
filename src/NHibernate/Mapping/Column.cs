using System;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Represents the mapping to a column in a database.
	/// </summary>
	[Serializable]
	public class Column : ISelectable, ICloneable
	{
		public const int DefaultLength = 255;
		public const int DefaultPrecision = 19;
		public const int DefaultScale = 2;

		private int? _length;
		private int? _precision;
		private int? _scale;
		private IValue _value;
		private int _typeIndex;
		private string _name;
		private bool _nullable = true;
		private bool _unique;
		private string _sqlType;
		private SqlType _sqlTypeCode;
		private bool _quoted;
		internal int UniqueInteger;
		private string _checkConstraint;
		private string _comment;
		private string _defaultValue;

		/// <summary>
		/// Initializes a new instance of <see cref="Column"/>.
		/// </summary>
		public Column()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="Column"/>.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		public Column(string columnName)
		{
			Name = columnName;
		}

		/// <summary>
		/// Gets or sets the length of the datatype in the database.
		/// </summary>
		/// <value>The length of the datatype in the database.</value>
		public int Length
		{
			get { return _length.GetValueOrDefault(DefaultLength); }
			set { _length = value; }
		}

		/// <summary>
		/// Gets or sets the name of the column in the database.
		/// </summary>
		/// <value>
		/// The name of the column in the database.  The get does 
		/// not return a Quoted column name.
		/// </value>
		/// <remarks>
		/// <p>
		/// If a value is passed in that is wrapped by <c>`</c> then 
		/// NHibernate will Quote the column whenever SQL is generated
		/// for it.  How the column is quoted depends on the Dialect.
		/// </p>
		/// <p>
		/// The value returned by the getter is not Quoted.  To get the
		/// column name in quoted form use <see cref="GetQuotedName(Dialect.Dialect)"/>.
		/// </p>
		/// </remarks>
		public string Name
		{
			get { return _name; }
			set
			{
				if (value[0] == '`')
				{
					_quoted = true;
					_name = value.Substring(1, value.Length - 2);
				}
				else
				{
					_name = value;
				}
			}
		}

		public string CanonicalName
		{
			get { return _quoted ? _name : _name.ToLowerInvariant(); }
		}

		/// <summary>
		/// Gets the name of this Column in quoted form if it is necessary.
		/// </summary>
		/// <param name="d">
		/// The <see cref="Dialect.Dialect"/> that knows how to quote
		/// the column name.
		/// </param>
		/// <returns>
		/// The column name in a form that is safe to use inside of a SQL statement.
		/// Quoted if it needs to be, not quoted if it does not need to be.
		/// </returns>
		public string GetQuotedName(Dialect.Dialect d)
		{
			return IsQuoted ? d.QuoteForColumnName(_name) : _name;
		}


		/// <summary>
		/// For any column name, generate an alias that is unique to that
		/// column name, and also take Dialect.MaxAliasLength into account.
		/// </summary>
		public string GetAlias(Dialect.Dialect dialect)
		{
			return GetAlias(dialect.MaxAliasLength);
		}

		private string GetAlias(int maxAliasLength)
		{
			string alias = _name;
			string suffix = UniqueInteger.ToString() + StringHelper.Underscore;

			int lastLetter = StringHelper.LastIndexOfLetter(_name);
			if (lastLetter == -1)
			{
				alias = "column";
			}
			else if (lastLetter < _name.Length - 1)
			{
				alias = _name.Substring(0, lastLetter + 1);
			}

			// Updated logic ported from Hibernate's fix for HHH-8073.
			//  https://github.com/hibernate/hibernate-orm/commit/79073a98f0e4ed225fe4608b67594196f86d48d7
			// To my mind it is weird - since the suffix is now always used, it
			// seems "useRawName" is a misleading choice of variable name. For the same
			// reason, the checks for "_quoted" and "rowid" looks redundant. If you remove
			// those checks, then the double checks for total length can be reduced to one.
			//    But I will leave it like this for now to make it look similar. /Oskar 2016-08-20
			bool useRawName = _name.Length + suffix.Length <= maxAliasLength &&
			                  !_quoted &&
			                  !StringHelper.EqualsCaseInsensitive(_name, "rowid");
			if (!useRawName)
			{
				if (suffix.Length >= maxAliasLength)
				{
					throw new MappingException(
						string.Format(
							"Unique suffix {0} length must be less than maximum {1} characters.",
							suffix,
							maxAliasLength));
				}
				if (alias.Length + suffix.Length > maxAliasLength)
					alias = alias.Substring(0, maxAliasLength - suffix.Length);
			}
			return alias + suffix;
		}

		public string GetAlias(Dialect.Dialect dialect, Table table)
		{
			string suffix = table.UniqueInteger.ToString() + StringHelper.Underscore;
			int maxAliasLength = dialect.MaxAliasLength - suffix.Length;
			return GetAlias(maxAliasLength) + suffix;
		}


		/// <summary>
		/// Gets or sets if the column can have null values in it.
		/// </summary>
		/// <value><see langword="true" /> if the column can have a null value in it.</value>
		public bool IsNullable
		{
			get { return _nullable; }
			set { _nullable = value; }
		}

		/// <summary>
		/// Gets or sets the index of the column in the <see cref="IType"/>.
		/// </summary>
		/// <value>
		/// The index of the column in the <see cref="IType"/>.
		/// </value>
		public int TypeIndex
		{
			get { return _typeIndex; }
			set { _typeIndex = value; }
		}

		/// <summary>
		/// Gets or sets if the column contains unique values.
		/// </summary>
		/// <value><see langword="true" /> if the column contains unique values.</value>
		public bool IsUnique
		{
			get { return _unique; }
			set { _unique = value; }
		}

		/// <summary>
		/// Gets the name of the data type for the column.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use to get the valid data types.</param>
		/// <param name="mapping"></param>
		/// <returns>
		/// The name of the data type for the column. 
		/// </returns>
		/// <remarks>
		/// If the mapping file contains a value of the attribute <c>sql-type</c> this will
		/// return the string contained in that attribute.  Otherwise it will use the 
		/// typename from the <see cref="Dialect.Dialect"/> of the <see cref="SqlType"/> object. 
		/// </remarks>
		public string GetSqlType(Dialect.Dialect dialect, IMapping mapping)
		{
			return _sqlType ?? GetDialectTypeName(dialect, mapping);
		}

		private string GetDialectTypeName(Dialect.Dialect dialect, IMapping mapping)
		{
			if (IsCaracteristicsDefined())
			{
				// NH-1070 (the size should be 0 if the precision is defined)
				return dialect.GetTypeName(GetSqlTypeCode(mapping), (!IsPrecisionDefined()) ? Length:0, Precision, Scale);
			}
			else
				return dialect.GetTypeName(GetSqlTypeCode(mapping));
		}

		#region System.Object Members

		/// <summary>
		/// Determines if this instance of <see cref="Column"/> and a specified object, 
		/// which must be a <b>Column</b> can be considered the same.
		/// </summary>
		/// <param name="obj">An <see cref="Object"/> that should be a <see cref="Column"/>.</param>
		/// <returns>
		/// <see langword="true" /> if the name of this Column and the other Column are the same, 
		/// otherwise <see langword="false" />.
		/// </returns>
		public override bool Equals(object obj)
		{
			Column columnObj = obj as Column;
			return columnObj != null && Equals(columnObj);
		}

		/// <summary>
		/// Determines if this instance of <see cref="Column"/> and the specified Column 
		/// can be considered the same.
		/// </summary>
		/// <param name="column">A <see cref="Column"/> to compare to this Column.</param>
		/// <returns>
		/// <see langword="true" /> if the name of this Column and the other Column are the same, 
		/// otherwise <see langword="false" />.
		/// </returns>
		public bool Equals(Column column)
		{
			if (null == column)
				return false;

			if (ReferenceEquals(this, column))
				return true;

			return IsQuoted ? _name.Equals(column._name) : _name.ToLowerInvariant().Equals(column._name.ToLowerInvariant());
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			return IsQuoted ? _name.GetHashCode() : _name.ToLowerInvariant().GetHashCode();
		}

		#endregion

		/// <summary>
		/// Gets or sets the sql data type name of the column.
		/// </summary>
		/// <value>
		/// The sql data type name of the column. 
		/// </value>
		/// <remarks>
		/// This is usually read from the <c>sql-type</c> attribute.
		/// </remarks>
		public string SqlType
		{
			get { return _sqlType; }
			set { _sqlType = value; }
		}

		/// <summary>
		/// Gets or sets if the column needs to be quoted in SQL statements.
		/// </summary>
		/// <value><see langword="true" /> if the column is quoted.</value>
		public bool IsQuoted
		{
			get { return _quoted; }
			set { _quoted = value; }
		}

		/// <summary>
		/// Gets or sets whether the column is unique.
		/// </summary>
		public bool Unique
		{
			get { return _unique; }
			set { _unique = value; }
		}

		/// <summary>
		/// Gets or sets a check constraint on the column
		/// </summary>
		public string CheckConstraint
		{
			get { return _checkConstraint; }
			set { _checkConstraint = value; }
		}

		/// <summary>
		/// Do we have a check constraint?
		/// </summary>
		public bool HasCheckConstraint
		{
			get { return !string.IsNullOrEmpty(_checkConstraint); }
		}

		public string Text
		{
			get { return Name; }
		}

		public string GetText(Dialect.Dialect dialect)
		{
			return GetQuotedName(dialect);
		}

		public bool IsFormula
		{
			get { return false; }
		}

		public int Precision
		{
			get { return _precision.GetValueOrDefault(DefaultPrecision); }
			set { _precision = value; }
		}

		public int Scale
		{
			get { return _scale.GetValueOrDefault(DefaultScale); }
			set { _scale = value; }
		}

		public IValue Value
		{
			get { return _value; }
			set { _value = value; }
		}

		/// <summary> 
		/// The underlying columns SqlType.
		/// </summary>
		/// <remarks>
		/// If null, it is because the sqltype code is unknown.
		/// 
		/// Use <see cref="GetSqlTypeCode(IMapping)"/> to retreive the sqltypecode used
		/// for the columns associated Value/Type.
		/// </remarks>
		public SqlType SqlTypeCode
		{
			get { return _sqlTypeCode; }
			set { _sqlTypeCode = value; }
		}

		public string Comment
		{
			get { return _comment; }
			set { _comment = value; }
		}

		public string DefaultValue
		{
			get { return _defaultValue; }
			set { _defaultValue = value; }
		}

		public string GetTemplate(Dialect.Dialect dialect, SQLFunctionRegistry functionRegistry)
		{
			return GetQuotedName(dialect);
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", GetType().FullName, _name);
		}

		public SqlType GetSqlTypeCode(IMapping mapping)
		{
			IType type = Value.Type;
			try
			{
				SqlType sqltc = type.SqlTypes(mapping)[TypeIndex];
				if (SqlTypeCode != null && !ReferenceEquals(SqlTypeCode, sqltc))
				{
					throw new MappingException(string.Format("SQLType code's does not match. mapped as {0} but is {1}", sqltc, SqlTypeCode));
				}
				return sqltc;
			}
			catch (Exception e)
			{
				throw new MappingException(string.Format("Could not determine type for column {0} of type {1}: {2}", 
					_name, type.GetType().FullName, e.GetType().FullName), e);
			}
		}

		/// <summary>returns quoted name as it would be in the mapping file. </summary>
		public string GetQuotedName()
		{
			return _quoted ? '`' + _name + '`' : _name;
		}

		public bool IsCaracteristicsDefined()
		{
			return IsLengthDefined() || IsPrecisionDefined();
		}

		public bool IsPrecisionDefined()
		{
			return _precision.HasValue || _scale.HasValue;
		}

		public bool IsLengthDefined()
		{
			return _length.HasValue;
		}

		#region ICloneable Members
		/// <summary> Shallow copy, the value is not copied</summary>
		public object Clone()
		{
			Column copy = new Column();
			if (_length.HasValue)
				copy.Length = Length;
			if (_precision.HasValue)
				copy.Precision = Precision;
			if (_scale.HasValue)
				copy.Scale = Scale;
			copy.Value = _value;
			copy.TypeIndex = _typeIndex;
			copy.Name = GetQuotedName();
			copy.IsNullable = _nullable;
			copy.Unique = _unique;
			copy.SqlType = _sqlType;
			copy.SqlTypeCode = _sqlTypeCode;
			copy.UniqueInteger = UniqueInteger; //usually useless
			copy.CheckConstraint = _checkConstraint;
			copy.Comment = _comment;
			copy.DefaultValue = _defaultValue;
			return copy;
		}

		#endregion
	}
}
