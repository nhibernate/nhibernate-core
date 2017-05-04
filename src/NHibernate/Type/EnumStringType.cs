using System;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;

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
	public abstract partial class EnumStringType : AbstractEnumType
	{
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
		protected EnumStringType(System.Type enumClass) : this(enumClass, MaxLengthForEnumString) {}

		/// <summary>
		/// Initializes a new instance of <see cref="EnumStringType"/>.
		/// </summary>
		/// <param name="enumClass">The <see cref="System.Type"/> of the Enum.</param>
		/// <param name="length">The length of the string that can be written to the column.</param>
		protected EnumStringType(System.Type enumClass, int length) : base(SqlTypeFactory.GetString(length), enumClass) {}

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
			get { return "enumstring - " + ReturnedClass.Name; }
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
				return StringToObject(code as string);
			}
			catch (ArgumentException ae)
			{
				throw new HibernateException(string.Format("Can't Parse {0} as {1}", code, ReturnedClass.Name), ae);
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
			return code == null ? string.Empty : Enum.Format(ReturnedClass, code, "G");
		}

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			cmd.Parameters[index].Value = value == null ? DBNull.Value : GetValue(value);
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
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

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Get(rs, rs.GetOrdinal(name), session);
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

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return GetValue(value).ToString();
		}
	}

	[Serializable]
	public class EnumStringType<T> : EnumStringType
	{
		private readonly string typeName;

		public EnumStringType()
			: base(typeof (T))
		{
			System.Type type = GetType();
			typeName = type.FullName + ", " + type.Assembly.GetName().Name;
		}

		public override string Name
		{
			get { return typeName; }
		}
	}
}