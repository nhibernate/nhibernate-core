using System;
using System.Data;

using NHibernate.Sql;

namespace NHibernate.Type {

	/// <summary>
	/// Alternative boolean types.
	/// </summary>
	public abstract class CharBooleanType : BooleanType {

		protected abstract string TrueString { get; }
		protected abstract string FalseString { get; }

		public override object Get(IDataReader rs, string name) {
            string code = rs[name].ToString();
			if (code==null) {
				return null;
			}
			else {
				return code.ToUpper().Equals(TrueString);
			}
		}

		public override void Set(IDbCommand cmd, Object value, int index) {
			( (IDataParameter) cmd.Parameters[index] ).Value = ( ( (bool)value ) ? TrueString : FalseString );
		}

		public override DbType SqlType {
			get { return DbType.AnsiStringFixedLength; }
		}
		
		public override string ObjectToSQLString(object value) {
            return "'" + ( ( (bool) value ) ? TrueString : FalseString ) + "'";
		}

		public Object stringToObject(String xml) {
            if ( TrueString.Equals(xml) ) {
				return true;
			}
			else if ( FalseString.Equals(xml) ) {
				return false;
			}
			else {
				throw new HibernateException("Could not interpret: " + xml);
			}
		}
	}
}