using System;
using System.Collections;
using System.Data;

using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type {

	/// <summary>
	/// Persistent EnumType underlying using string store.
	/// </summary>
	public class EnumType : ImmutableType, IDiscriminatorType 
	{
		private readonly System.Type enumClass;
		/// <summary></summary>
		public const int MaxLengthForEnumString = 100;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="enumClass"></param>
		public EnumType(System.Type enumClass) : base(SqlTypeFactory.GetStringFixedLength(MaxLengthForEnumString)) {
			if(enumClass.IsEnum)
				this.enumClass = enumClass;
			else
				throw new MappingException(enumClass.Name + " did not inherit from System.Enum" );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public virtual object GetInstance(object code) {
			//code is an named constants defined for the enumeration.
			try {
				return Enum.Parse(enumClass, code as string, true);
			}
			catch (ArgumentException ae) {
				throw new HibernateException( string.Format( "Can't Parse {0} as {1}", code, enumClass.Name), ae );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public virtual object GetValue(object code) {
			//code is an enum instance.
			return code == null ? string.Empty : code.ToString();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override bool Equals(object x, object y) {
			//return (x==y) || ( x!=null && y!=null && x.GetType()==y.GetType() && x.ToString()==y.ToString() );
			return (x==y) || ( x!=null && y!=null && x.Equals(y) ) ;
		}
	
		/// <summary>
		/// 
		/// </summary>
		public override System.Type ReturnedClass {
			get { return enumClass; }
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set(IDbCommand cmd, object value, int index) {
			IDataParameter par = (IDataParameter) cmd.Parameters[index];
			if (value == null) {
				par.Value = DBNull.Value;
			}
			else {
				par.Value = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, int index) {
			object code = rs[index];
			if ( code == DBNull.Value || code==null) {
				return null;
			}
			else {
				return GetInstance(code);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, string name) {
			return Get(rs, rs.GetOrdinal(name));
		}

		/// <summary>
		/// 
		/// </summary>
		public override string Name {
			get { return enumClass.Name; }
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ToXML(object value) {
			return (value==null) ? null : GetValue(value).ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cached"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object Assemble(object cached, ISessionImplementor session, object owner) {
			if (cached==null) {
				return null;
			}
			else {
				return GetInstance(cached);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override object Disassemble(object value, ISessionImplementor session) {
			return (value==null) ? null : GetValue(value);
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public string ObjectToSQLString(object value) {
			return GetValue(value).ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public object StringToObject( string xml )
		{
			return GetInstance( xml );
		}
	}
}