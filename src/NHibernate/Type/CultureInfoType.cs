using System;
using System.Data;
using System.Globalization;

namespace NHibernate.Type {

	/// <summary>
	/// CultureInfoType.
	/// </summary>
	public class CultureInfoType : ImmutableType, ILiteralType {

		public override object Get(IDataReader rs, string name) {
			string str = (string) NHibernate.String.Get(rs, name);
			if (str == null) {
				return null;
			}
			else {
				return new CultureInfo(str);
			}
		}

		public override void Set(IDbCommand cmd, object value, int index) {
			NHibernate.String.Set(cmd, value.ToString(), index);
		}
	
		public override DbType SqlType {
			get { return NHibernate.String.SqlType; }
		}
	
		public override string ToXML(object value) {
			return value.ToString();
		}
	
		public override System.Type ReturnedClass {
			get { return typeof(CultureInfo); }
		}
	
		public override bool Equals(object x, object y) {
			return (x==y); //???
		}
	
		public override string Name {
			get { return "CultureInfo"; }
		}

		public string ObjectToSQLString(object value) {
			return ( (ILiteralType) NHibernate.String ).ObjectToSQLString( value.ToString() );
		}
	}
}