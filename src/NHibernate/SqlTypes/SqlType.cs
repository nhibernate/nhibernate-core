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
		private readonly DbType dbType;
		private readonly int length;
		private readonly byte precision;
		private readonly byte scale;
		private readonly bool lengthDefined;
		private readonly bool precisionDefined;

		public SqlType(DbType dbType)
		{
			this.dbType = dbType;
		}

		public SqlType(DbType dbType, int length)
		{
			this.dbType = dbType;
			this.length = length;
			lengthDefined = true;
		}

		public SqlType(DbType dbType, byte precision, byte scale)
		{
			this.dbType = dbType;
			this.precision = precision;
			this.scale = scale;
			precisionDefined = true;
		}

		public DbType DbType
		{
			get { return dbType; }
		}

		public int Length
		{
			get { return length; }
		}

		public byte Precision
		{
			get { return precision; }
		}

		public byte Scale
		{
			get { return scale; }
		}

		public bool LengthDefined
		{
			get { return lengthDefined; }
		}

		public bool PrecisionDefined
		{
			get { return precisionDefined; }
		}

		#region System.Object Members

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode;

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
			return obj == this || Equals(obj as SqlType);
		}

		public bool Equals(SqlType rhsSqlType)
		{
			if (rhsSqlType == null)
			{
				return false;
			}

			if (LengthDefined)
			{
				return (DbType.Equals(rhsSqlType.DbType)) && (Length == rhsSqlType.Length);
			}
			if (PrecisionDefined)
			{
				return (DbType.Equals(rhsSqlType.DbType)) && (Precision == rhsSqlType.Precision) && (Scale == rhsSqlType.Scale);
			}
			return (DbType.Equals(rhsSqlType.DbType));
		}

		public override string ToString()
		{
			if (!LengthDefined && !PrecisionDefined)
			{
				// Shortcut
				return DbType.ToString();
			}

			var result = new StringBuilder(DbType.ToString());

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