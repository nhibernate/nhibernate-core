using System;
using System.Data.Common;
using System.Xml;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	[Serializable]
	public partial class XmlDocType : MutableType
	{
		public XmlDocType()
			: base(new XmlSqlType())
		{
		}

		public XmlDocType(SqlType sqlType) : base(sqlType)
		{
		}

		public override string Name
		{
			get { return "XmlDoc"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof (XmlDocument); }
		}

		/// <inheritdoc />
		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			cmd.Parameters[index].Value = GetStringRepresentation(value);
		}

		/// <inheritdoc />
		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			// according to documentation, GetValue should return a string, at least for MsSQL
			// hopefully all DataProvider has the same behaviour
			string xmlString = Convert.ToString(rs.GetValue(index));
			// 6.0 TODO: inline the call.
#pragma warning disable 618
			return FromStringValue(xmlString);
#pragma warning restore 618
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Get(rs, rs.GetOrdinal(name), session);
		}

		/// <inheritdoc />
		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			return (value == null) ? null :
				// 6.0 TODO: inline this call.
#pragma warning disable 618
				ToString(value);
#pragma warning restore 618
		}

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version. Override ToLoggableString instead.")]
		public override string ToString(object val)
		{
			return GetStringRepresentation(val);
		}

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public override object FromStringValue(string xml)
		{
			if (xml != null)
			{
				var xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(xml);
				return xmlDocument;
			}
			return null;
		}

		public override object DeepCopyNotNull(object value)
		{
			var original = (XmlDocument) value;
			var copy = new XmlDocument();
			copy.LoadXml(original.OuterXml);
			return copy;
		}

		public override bool IsEqual(object x, object y)
		{
			if (x == null && y == null)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			return ((XmlDocument) x).OuterXml == ((XmlDocument) y).OuterXml;
		}

		/// <inheritdoc />
		public override object Assemble(object cached, ISessionImplementor session, object owner)
		{
			return FromStringValue(cached as string);
		}

		/// <inheritdoc />
		public override object Disassemble(object value, ISessionImplementor session, object owner)
		{
			return GetStringRepresentation(value);
		}

		private string GetStringRepresentation(object value)
		{
			return ((XmlDocument) value)?.OuterXml;
		}
	}
}
