using System;
using System.Data.Common;
using System.Xml.Linq;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	[Serializable]
	public class XDocType : MutableType
	{
		public XDocType()
			: base(new XmlSqlType())
		{
		}

		public XDocType(SqlType sqlType)
			: base(sqlType)
		{
		}

		public override string Name
		{
			get { return "XDoc"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof (XDocument); }
		}

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			cmd.Parameters[index].Value = ((XDocument) value).ToString(SaveOptions.DisableFormatting);
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			// according to documentation, GetValue should return a string, at list for MsSQL
			// hopefully all DataProvider has the same behaviour
			string xmlString = Convert.ToString(rs.GetValue(index));
			return FromStringValue(xmlString);
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Get(rs, rs.GetOrdinal(name), session);
		}

		public override string ToString(object val)
		{
			return val == null ? null : val.ToString();
		}

		public override object FromStringValue(string xml)
		{
			if (xml != null)
			{
				return XDocument.Parse(xml);
			}

			return null;
		}

		public override object DeepCopyNotNull(object value)
		{
			var original = (XDocument) value;
			var copy = new XDocument(original);
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

			return XNode.DeepEquals((XDocument) x, (XDocument) y);
		}
	}
}