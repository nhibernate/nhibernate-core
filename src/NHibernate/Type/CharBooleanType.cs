using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type {

	/// <summary>
	/// Maps a <see cref="System.Boolean"/> Property 
	/// to a <see cref="DbType.AnsiStringFixedLength"/> column.
	/// </summary>
	public abstract class CharBooleanType : BooleanType 
	{

		protected abstract string TrueString { get; }
		protected abstract string FalseString { get; }

		internal CharBooleanType(AnsiStringFixedLengthSqlType sqlType) : base(sqlType) {
		}

		public override object Get(IDataReader rs, int index) {
			string code = Convert.ToString(rs[index]);
			if (code==null) {
				return null;
			}
			else {
				return code.ToUpper().Equals(TrueString);
			}
		}

		public override object Get(IDataReader rs, string name) {
			return Get(rs, rs.GetOrdinal(name));
		}

		public override void Set(IDbCommand cmd, Object value, int index) {
			( (IDataParameter) cmd.Parameters[index] ).Value = ( ( (bool)value ) ? TrueString : FalseString );
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