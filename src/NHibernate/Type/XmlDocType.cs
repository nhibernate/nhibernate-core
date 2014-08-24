using System;
using System.Data;
using System.Xml;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	[Serializable]
	public class XmlDocType : MutableType
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

		public override void Set(IDbCommand cmd, object value, int index)
		{
			((IDataParameter) cmd.Parameters[index]).Value = ((XmlDocument) value).OuterXml;
		}

		public override object Get(IDataReader rs, int index)
		{
			// according to documentation, GetValue should return a string, at list for MsSQL
			// hopefully all DataProvider has the same behaviour
			string xmlString = Convert.ToString(rs.GetValue(index));
			return FromStringValue(xmlString);
		}

		public override object Get(IDataReader rs, string name)
		{
			return Get(rs, rs.GetOrdinal(name));
		}

		public override string ToString(object val)
		{
			return val == null ? null : ((XmlDocument) val).OuterXml;
		}

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
	}
}