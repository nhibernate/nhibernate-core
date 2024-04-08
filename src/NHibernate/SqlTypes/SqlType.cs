using System;
using System.Data;
using System.Data.Common;
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
	/// information to create an <see cref="DbParameter"/>.  
	/// </p>
	/// <p>
	/// The <see cref="Dialect.Dialect"/> use the SqlType to convert the <see cref="DbType"/>
	/// to the appropriate sql type for SchemaExport.
	/// </p>
	/// </remarks>
	[Serializable]
	public class SqlType : IEquatable<SqlType>
	{
		public SqlType(DbType dbType)
		{
			DbType = dbType;
		}

		public SqlType(DbType dbType, int length)
		{
			DbType = dbType;
			Length = length;
			LengthDefined = true;
		}

		public SqlType(DbType dbType, byte precision, byte scale)
		{
			DbType = dbType;
			Precision = precision;
			Scale = scale;
			ScaleDefined = true;
			PrecisionDefined = true;
		}

		public SqlType(DbType dbType, byte scale)
		{
			DbType = dbType;
			Scale = scale;
			ScaleDefined = true;
		}

		public DbType DbType { get; }

		public int Length { get; }

		public byte Precision { get; }

		public byte Scale { get; }

		public bool LengthDefined { get; }

		public bool PrecisionDefined { get; }
		

		public bool ScaleDefined { get; }

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
				else if (ScaleDefined)
				{
					hashCode = DbType.GetHashCode() / 3 + Scale.GetHashCode() / 3;
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
			if (ReferenceEquals(this, rhsSqlType))
				return true;

			if (rhsSqlType == null)
				return false;

			if (DbType != rhsSqlType.DbType)
				return false;

			if (LengthDefined != rhsSqlType.LengthDefined)
				return false;

			if (PrecisionDefined != rhsSqlType.PrecisionDefined)
				return false;

			if (ScaleDefined != rhsSqlType.ScaleDefined)
				return false;

			if (Length != rhsSqlType.Length)
				return false;

			if (Precision != rhsSqlType.Precision)
				return false;

			if (Scale != rhsSqlType.Scale)
				return false;

			return true;
		}

		public override string ToString()
		{
			if (!LengthDefined && !PrecisionDefined && !ScaleDefined)
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
			else if (ScaleDefined)
			{
				result.Append("Scale=").Append(Scale).Append(')');
			}

			return result.ToString();
		}

		#endregion
	}
}
