using System;
using System.Data;

using NHibernate.Sql;

namespace NHibernate.Type {

	/// <summary>
	/// Summary description for CharacterType.
	/// </summary>
	public class CharacterType : PrimitiveType , IDiscriminatorType	{

		public override object Get(IDataReader rs, string name) {
            string str = rs[name].ToString();
			if (str==null) {
				return null;
			}
			else {
				return str[0];
			}	
		}
	
		public override System.Type PrimitiveClass {
			get { return typeof(char); }
		}
	
		public override System.Type ReturnedClass {
			get { return typeof(char); }
		}
	
		public override void Set(IDbCommand cmd, object value, int index) {
			( (IDataParameter) cmd.Parameters[index] ).Value = (char) value;
		}
	
		public override Types SqlType {
			get { return Types.Char; }
		}

		public override string Name {
			get { return "character"; }
		}
	
		public override string ObjectToSQLString(object value) {
			return '\'' + value.ToString() + '\'';
		}
	
		public virtual object StringToObject(string xml) {
			if ( xml.Length != 1 ) throw new MappingException("multiple or zero characters found parsing string");
			return xml[0];
		}
	}
}