using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Type
{
	[Serializable]
	public class StructuredType : MutableType, IParameterizedType, IEquatable<StructuredType>
	{
		public const string TypeNameParameter = "TypeName";

		public string TypeName { get; private set; }

		public StructuredType()
			: this(string.Empty)
		{
		}

		public StructuredType(string typeName) : base(SqlTypeFactory.Structured(typeName))
		{
			TypeName = typeName ?? string.Empty;
		}

		public override object DeepCopyNotNull(object value)
		{
			return ((DataTable) value).Clone();
		}

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			cmd.Parameters[index].Value = value;
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			return (DataTable) rs[index];
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return (DataTable) rs[name];
		}

		[Obsolete("This method has no more usages and will be removed in a future version. Override ToLoggableString instead.")]
		public override string ToString(object val)
		{
			var builder = new StringBuilder();
			using (var stream = new StringWriter(builder))
			{
				((DataTable) val).WriteXml(stream);
				return builder.ToString();
			}
		}

		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public override object FromStringValue(string xml)
		{
			using (var stream = new StringReader(xml))
			{
				var table = new DataTable();
				table.ReadXml(stream);
				return table;
			}
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as StructuredType);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (base.GetHashCode() * 397) ^ (TypeName != null ? TypeName.GetHashCode() : 0);
			}
		}

		public override string Name => "Structured";

		public override System.Type ReturnedClass => typeof(DataTable);

		public void SetParameterValues(System.Collections.Generic.IDictionary<string, string> parameters)
		{
			if (parameters.TryGetValue(TypeNameParameter, out var typeName))
			{
				TypeName = typeName;
			}
		}

		public bool Equals(StructuredType other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && string.Equals(TypeName, other.TypeName);
		}
	}
}
