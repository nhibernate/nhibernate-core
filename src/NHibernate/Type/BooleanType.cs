using System;
using System.Data;

using NHibernate.Sql;
using NHibernate.SqlTypes;

namespace NHibernate.Type {

	/// <summary>
	/// Maps a <see cref="System.Boolean"/> Property 
	/// to a <see cref="DbType.Boolean"/> column.
	/// </summary>

	//Had to use setShort / getShort instead of setBoolean / getBoolean
	//to work around a HypersonicSQL driver bug

	public class BooleanType : PrimitiveType, IDiscriminatorType {

		private static readonly string TRUE = "1";
		private static readonly string FALSE = "0";

		/// <summary>
		/// Initialize a new instance of the BooleanType class using a 
		/// <see cref="BooleanSqlType"/>. 
		/// </summary>
		/// <param name="sqlType">The underlying <see cref="SqlType"/>.</param>
		/// <remarks>This is used when the Property is mapped to a native boolean type.</remarks>
		internal BooleanType(BooleanSqlType sqlType) : base(sqlType) {
		}

		/// <summary>
		/// Initialize a new instance of the BooleanType class using a
		/// <see cref="AnsiStringFixedLengthSqlType"/>.
		/// </summary>
		/// <param name="sqlType">The underlying <see cref="SqlType"/>.</param>
		/// <remarks>
		/// This is used when the Property is mapped to a string column
		/// that stores true or false as a string.
		/// </remarks>
		internal BooleanType(AnsiStringFixedLengthSqlType sqlType) : base(sqlType) {
		}

		public override object Get(IDataReader rs, int index) {
			//TODO: either fix the mapping of a boolean to byte or fix this
			// to read the GetByte instead of the GetBoolean...
			return Convert.ToBoolean(rs.GetByte(index));
		}

		public override object Get(IDataReader rs, string name) {
			return Get(rs, rs.GetOrdinal(name));
		}

		public override System.Type PrimitiveClass {
			get { return typeof(bool); }
		}
	
		public override System.Type ReturnedClass {
			get { return typeof(bool); }
		}

		public override void Set(IDbCommand cmd, object value, int index) {
			( (IDataParameter)cmd.Parameters[index] ).Value = (bool) value;
		}
	
		public override string Name {
			get { return "Boolean"; }
		}
	
		public override string ObjectToSQLString(object value) {
			return ( (bool)value ) ? TRUE : FALSE;
		}
	
		public virtual object StringToObject(string xml) {
			return bool.Parse(xml);
		}
	}
}