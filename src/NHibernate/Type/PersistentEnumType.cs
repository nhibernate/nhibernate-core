using System;
using System.Collections;
using System.Data;

using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type {

	/// <summary>
	/// PersistentEnumType
	/// </summary>
	public class PersistentEnumType : ImmutableType, ILiteralType {

		private readonly System.Type enumClass;
		
		public PersistentEnumType(System.Type enumClass) : base(GetUnderlyingSqlType(enumClass)) {
			if(enumClass.IsEnum)
				this.enumClass = enumClass;
			else
				throw new MappingException(enumClass.Name + " did not inherit from System.Enum" );
		}

		/// <summary>
		/// Determines what the NHibernate SqlType should be based on the 
		/// values contain in the Enum
		/// </summary>
		/// <param name="enumClass">The Enumeration class to get the values from.</param>
		/// <returns>The SqlType for this EnumClass</returns>
		public static SqlType GetUnderlyingSqlType (System.Type enumClass) { 
			switch( Enum.GetUnderlyingType(enumClass).FullName ) {
				case "System.Byte":
					return SqlTypeFactory.GetByte();// DbType.Byte;
				case "System.Int16":
					return SqlTypeFactory.GetInt16(); // DbType.Int16;
				case "System.Int32":
					return SqlTypeFactory.GetInt32(); //DbType.Int32;
				case "System.Int64":
					return SqlTypeFactory.GetInt64(); //DbType.Int64;
				case "System.SByte":
					return SqlTypeFactory.GetByte(); //DbType.SByte;
				case "System.UInt16":
					return SqlTypeFactory.GetInt16(); //DbType.UInt16;
				case "System.UInt32":
					return SqlTypeFactory.GetInt32(); //DbType.UInt32;
				case "System.UInt64":
					return SqlTypeFactory.GetInt64(); //DbType.UInt64;
				default:
					throw new HibernateException( "Unknown UnderlyingDbType for Enum"); //Impossible exception
			}
		
		}

//		protected DbType UnderlyingDbType { //Not overridable. These are the DbTypes.
//			get {
//				switch( Enum.GetUnderlyingType(enumClass).FullName ) {
//					case "System.Byte":
//						return DbType.Byte;
//					case "System.Int16":
//						return DbType.Int16;
//					case "System.Int32":
//						return DbType.Int32;
//					case "System.Int64":
//						return DbType.Int64;
//					case "System.SByte":
//						return DbType.SByte;
//					case "System.UInt16":
//						return DbType.UInt16;
//					case "System.UInt32":
//						return DbType.UInt32;
//					case "System.UInt64":
//						return DbType.UInt64;
//					default:
//						throw new HibernateException( "Unknown UnderlyingDbType for Enum"); //Impossible exception
//				}
//			}
//		}

		public virtual object GetInstance(object code) {
			try {
				return Enum.ToObject(enumClass, code);
			}
			catch (ArgumentException ae) {
				throw new HibernateException( "ArgumentException occurred inside Enum.ToObject()", ae );
			}
		}

		public virtual object GetValue(object code) {
			//code is an enum instance.
			//An explicit cast is needed to convert from enum type to an integral type.

			switch( Enum.GetUnderlyingType(enumClass).FullName ) {
				case "System.Byte":
					return (byte)code;
				case "System.Int16":
					return (short)code;
				case "System.Int32":
					return (int)code;
				case "System.Int64":
					return (long)code;
				case "System.SByte":
					return (sbyte)code;
				case "System.UInt16":
					return (ushort)code;
				case "System.UInt32":
					return (uint)code;
				case "System.UInt64":
					return (ulong)code;
				default:
					throw new HibernateException( "Unknown UnderlyingType for Enum"); //Impossible exception
			}
		}
		
		public override bool Equals(object x, object y) {
			return (x==y) || ( x!=null && y!=null && x.GetType()==y.GetType() && Enum.ToObject(enumClass, x)==Enum.ToObject(enumClass, y) );
		}
	
		public override System.Type ReturnedClass {
			get { return enumClass.GetType(); }
		}
	
		public override void Set(IDbCommand cmd, object value, int index) {
			IDataParameter par = (IDataParameter) cmd.Parameters[index];
			if (value==null) {
				par.Value = DBNull.Value;
			}
			else {
				par.Value = value;
			}
		}

		public override object Get(IDataReader rs, int index) {
			object code = rs[index];
			if ( code==DBNull.Value || code==null) {
				return null;
			}
			else {
				return GetInstance(code);
			}
		}

		public override object Get(IDataReader rs, string name) {
			return Get(rs, rs.GetOrdinal(name));
		}

		public override string Name {
			get { return enumClass.Name; }
		}
	
		public override string ToXML(object value) {
			return (value==null) ? null : GetValue(value).ToString();
		}

		public override object Assemble(object cached, ISessionImplementor session, object owner) {
			if (cached==null) {
				return null;
			}
			else {
				return GetInstance(cached);;
			}
		}
	
		public override object Disassemble(object value, ISessionImplementor session) {
			return (value==null) ? null : GetValue(value);
		}
	
		public string ObjectToSQLString(object value) {
			return GetValue(value).ToString();
		}
	}
}