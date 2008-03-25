using System;
using System.Data;
using System.Text;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// This is the base class that adds information to the <see cref="DbType" /> 
	/// for the <see cref="Driver.IDriver"/> and <see cref="Dialect.Dialect"/>
	/// to use.
	/// </summary>
	/// <remarks>
	/// <p>
	/// The <see cref="Driver.IDriver"/> uses the SqlType to get enough
	/// information to create an <see cref="IDbDataParameter"/>.  
	/// </p>
	/// <p>
	/// The <see cref="Dialect.Dialect"/> use the SqlType to convert the <see cref="DbType"/>
	/// to the appropriate sql type for SchemaExport.
	/// </p>
	/// </remarks>
	[Serializable]
	public class SqlType
	{
		private DbType _dbType;
		private int _length;
		private byte _precision;
		private byte _scale;

		// false by default
		private bool _lengthDefined;
		// false by default
		private bool _precisionDefined;

		public SqlType(DbType dbType)
		{
			_dbType = dbType;
		}

		public SqlType(DbType dbType, int length)
		{
			_dbType = dbType;
			_length = length;
			_lengthDefined = true;
		}

		public SqlType(DbType dbType, byte precision, byte scale)
		{
			_dbType = dbType;
			_precision = precision;
			_scale = scale;
			_precisionDefined = true;
		}

		public DbType DbType
		{
			get { return _dbType; }
		}

		public int Length
		{
			get { return _length; }
		}

		public byte Precision
		{
			get { return _precision; }
		}

		public byte Scale
		{
			get { return _scale; }
		}

		public bool LengthDefined
		{
			get { return _lengthDefined; }
		}

		public bool PrecisionDefined
		{
			get { return _precisionDefined; }
		}

		#region System.Object Members

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = 0;

				if (LengthDefined)
				{
					hashCode = (DbType.GetHashCode() / 2) + (Length.GetHashCode() / 2);
				}
				else if (PrecisionDefined)
				{
					hashCode = (DbType.GetHashCode() / 3) + (Precision.GetHashCode() / 3) + (Scale.GetHashCode() / 3);
				}
				else
				{
					hashCode = DbType.GetHashCode();
				}

				return hashCode;
			}
		}

		public override bool Equals(object obj)
		{
			SqlType rhsSqlType;

			// Step1: Perform an equals test
			if (obj == this)
			{
				return true;
			}

			// Step	2: Instance of check
			rhsSqlType = obj as SqlType;
			if (rhsSqlType == null)
			{
				return false;
			}

			//Step 3: Check each important field
			bool equals = false;
			if (LengthDefined)
			{
				equals = (DbType.Equals(rhsSqlType.DbType))
				         && (Length == rhsSqlType.Length);
			}
			else if (PrecisionDefined)
			{
				equals = (DbType.Equals(rhsSqlType.DbType))
				         && (Precision == rhsSqlType.Precision)
				         && (Scale == rhsSqlType.Scale);
			}
			else
			{
				equals = (DbType.Equals(rhsSqlType.DbType));
			}

			return equals;
		}

		public override string ToString()
		{
			if (!LengthDefined && !PrecisionDefined)
			{
				// Shortcut
				return DbType.ToString();
			}

			StringBuilder result = new StringBuilder(DbType.ToString());

			if (LengthDefined)
			{
				result.Append("(Length=").Append(Length).Append(')');
			}

			if (PrecisionDefined)
			{
				result.Append("(Precision=").Append(Precision).Append(", ").Append("Scale=").Append(Scale).Append(')');
			}

			return result.ToString();
		}

		#endregion
	}
}