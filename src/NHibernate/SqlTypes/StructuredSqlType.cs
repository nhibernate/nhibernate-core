using System;
using System.Data;

namespace NHibernate.SqlTypes
{
	[Serializable]
	public class StructuredSqlType : SqlType, IEquatable<StructuredSqlType>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StringSqlType"/> class.
		/// </summary>
		public StructuredSqlType(string typeName) : base(DbType.Object)
		{
			TypeName = typeName ?? string.Empty;
		}

		public string TypeName { get; }

		public override bool Equals(object obj)
		{
			return Equals(obj as StructuredSqlType);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (base.GetHashCode() * 397) ^ (TypeName != null ? TypeName.GetHashCode() : 0);
			}
		}

		public bool Equals(StructuredSqlType other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && string.Equals(TypeName, other.TypeName);
		}
	}
}
