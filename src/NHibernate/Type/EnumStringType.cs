using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Enum"/> to a 
	/// <see cref="DbType.String">DbType.String</see>.
	/// </summary>
	/// <remarks>
	/// If your database should store the <see cref="System.Enum"/>
	/// using the named values in the enum instead of the underlying values
	/// then subclass this <see cref="IType"/>.
	/// 
	/// <para>
	/// All that needs to be done is to provide a default constructor that
	/// NHibernate can use to create the specific type.  For example, if 
	/// you had an enum defined as.
	/// </para>
	/// 
	/// <code>
	/// public enum MyEnum 
	/// {
	///		On,
	///		Off,
	///		Dimmed
	/// }
	/// </code>
	/// 
	/// <para>
	/// all that needs to be written for your enum string type is:
	/// </para>
	/// 
	/// <code>
	/// public class MyEnumStringType : NHibernate.Type.EnumStringType
	/// {
	///		public MyEnumStringType()
	///			: base( typeof( MyEnum ) )
	///		{
	///		}
	/// }
	/// </code>
	/// 
	/// <para>
	/// The mapping would look like:
	/// </para>
	/// 
	/// <code>
	/// ...
	///		&lt;property name="Status" type="MyEnumStringType, AssemblyContaining" /&gt;
	///	...
	/// </code>
	/// 
	/// <para>
	/// The TestFixture that shows the working code can be seen
	/// in <c>NHibernate.Test.TypesTest.EnumStringTypeFixture.cs</c>
	/// , <c>NHibernate.Test.TypesTest.EnumStringClass.cs</c>
	/// , and <c>NHibernate.Test.TypesTest.EnumStringClass.hbm.xml</c>
	/// </para>
	/// </remarks>
	[Serializable]
	public abstract class EnumStringType : ImmutableType, IDiscriminatorType
	{
		private readonly System.Type enumClass;

		/// <summary>
		/// Hardcoding of <c>255</c> for the maximum length
		/// of the Enum name that will be saved to the db.
		/// </summary>
		/// <value>
		/// <c>255</c> because that matches the default length that hbm2ddl will
		/// use to create the column.
		/// </value>
		public const int MaxLengthForEnumString = 255;

		/// <summary>
		/// Initializes a new instance of <see cref="EnumStringType"/>.
		/// </summary>
		/// <param name="enumClass">The <see cref="System.Type"/> of the Enum.</param>
		protected EnumStringType(System.Type enumClass)
			: this(enumClass, MaxLengthForEnumString)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="EnumStringType"/>.
		/// </summary>
		/// <param name="enumClass">The <see cref="System.Type"/> of the Enum.</param>
		/// <param name="length">The length of the string that can be written to the column.</param>
		protected EnumStringType(System.Type enumClass, int length)
			: base(SqlTypeFactory.GetString(length))
		{
			if (enumClass.IsEnum)
			{
				this.enumClass = enumClass;
			}
			else
			{
				throw new MappingException(enumClass.Name + " did not inherit from System.Enum");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public virtual object GetInstance(object code)
		{
			//code is an named constants defined for the enumeration.
			try
			{
				return Enum.Parse(enumClass, code as string, true);
			}
			catch (ArgumentException ae)
			{
				throw new HibernateException(string.Format("Can't Parse {0} as {1}", code, enumClass.Name), ae);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public virtual object GetValue(object code)
		{
			//code is an enum instance.
			return code == null ? string.Empty : code.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		public override System.Type ReturnedClass
		{
			get { return enumClass; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set(IDbCommand cmd, object value, int index)
		{
			IDataParameter par = (IDataParameter) cmd.Parameters[index];
			if (value == null)
			{
				par.Value = DBNull.Value;
			}
			else
			{
				par.Value = Enum.Format(this.enumClass, value, "G");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, int index)
		{
			object code = rs[index];
			if (code == DBNull.Value || code == null)
			{
				return null;
			}
			else
			{
				return GetInstance(code);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, string name)
		{
			return Get(rs, rs.GetOrdinal(name));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// This appends <c>enumstring - </c> to the beginning of the underlying
		/// enums name so that <see cref="System.Enum"/> could still be stored
		/// using the underlying value through the <see cref="PersistentEnumType"/>
		/// also.
		/// </remarks>
		public override string Name
		{
			get { return "enumstring - " + enumClass.Name; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ToString(object value)
		{
			return (value == null) ? null : GetValue(value).ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cached"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object Assemble(object cached, ISessionImplementor session, object owner)
		{
			if (cached == null)
			{
				return null;
			}
			else
			{
				return GetInstance(cached);
			}
		}

		public override object Disassemble(object value, ISessionImplementor session, object owner)
		{
			return (value == null) ? null : GetValue(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public object StringToObject(string xml)
		{
			return (string.IsNullOrEmpty(xml)) ? null : FromStringValue(xml);
		}

		public override object FromStringValue(string xml)
		{
			return GetInstance(xml);
		}

		public string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return GetValue(value).ToString();
		}
	}
}