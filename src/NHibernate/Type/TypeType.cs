using System;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps the Assembly Qualified Name of a <see cref="System.Type"/> to a 
	/// <see cref="DbType.String" /> column.
	/// </summary>
	[Serializable]
	public partial class TypeType : ImmutableType
	{
		/// <summary></summary>
		internal TypeType()
			: base(new StringSqlType()) {}

		/// <summary>
		/// Initialize a new instance of the TypeType class using a 
		/// <see cref="SqlTypes.SqlType"/>. 
		/// </summary>
		/// <param name="sqlType">The underlying <see cref="SqlTypes.SqlType"/>.</param>
		internal TypeType(StringSqlType sqlType)
			: base(sqlType) {}

		public override SqlType SqlType
		{
			get { return NHibernateUtil.String.SqlType; }
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> in the <see cref="DbDataReader"/> for the Property.
		/// </summary>
		/// <param name="rs">The <see cref="DbDataReader"/> that contains the value.</param>
		/// <param name="index">The index of the field to get the value from.</param>
		/// <param name="session">The session for which the operation is done.</param>
		/// <returns>The <see cref="System.Type"/> from the database.</returns>
		/// <exception cref="TypeLoadException">
		/// Thrown when the value in the database can not be loaded as a <see cref="System.Type"/>
		/// </exception>
		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			return ParseStringRepresentation(NHibernateUtil.String.Get(rs, index, session));
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> in the <see cref="DbDataReader"/> for the Property.
		/// </summary>
		/// <param name="rs">The <see cref="DbDataReader"/> that contains the value.</param>
		/// <param name="name">The name of the field to get the value from.</param>
		/// <param name="session">The session for which the operation is done.</param>
		/// <returns>The <see cref="System.Type"/> from the database.</returns>
		/// <remarks>
		/// This just calls gets the index of the name in the DbDataReader
		/// and calls the overloaded version <see cref="Get(DbDataReader, Int32, ISessionImplementor)"/>
		/// (DbDataReader, Int32). 
		/// </remarks>
		/// <exception cref="TypeLoadException">
		/// Thrown when the value in the database can not be loaded as a <see cref="System.Type"/>
		/// </exception>
		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Get(rs, rs.GetOrdinal(name), session);
		}

		/// <summary>
		/// Puts the Assembly Qualified Name of the <see cref="System.Type"/> 
		/// Property into to the <see cref="DbCommand"/>.
		/// </summary>
		/// <param name="cmd">The <see cref="DbCommand"/> to put the value into.</param>
		/// <param name="value">The <see cref="System.Type"/> that contains the value.</param>
		/// <param name="index">The index of the <see cref="DbParameter"/> to start writing the value to.</param>
		/// <param name="session">The session for which the operation is done.</param>
		/// <remarks>
		/// This uses the <see cref="NullableType.Set(DbCommand, Object, Int32, ISessionImplementor)"/> method of the 
		/// <see cref="NHibernateUtil.String"/> object to do the work.
		/// </remarks>
		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			NHibernateUtil.String.Set(cmd, GetStringRepresentation(value), index, session);
		}

		/// <inheritdoc />
		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			return (value == null) ? null :
				// 6.0 TODO: inline this call.
#pragma warning disable 618
				ToString(value);
#pragma warning restore 618
		}

		/// <summary>
		/// A representation of the value to be embedded in an XML element 
		/// </summary>
		/// <param name="value">The <see cref="System.Type"/> that contains the values.
		/// </param>
		/// <returns>An Xml formatted string that contains the Assembly Qualified Name.</returns>
		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version. Override ToLoggableString instead.")]
		public override string ToString(object value)
		{
			return GetStringRepresentation(value);
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> that will be returned 
		/// by the <c>NullSafeGet()</c> methods.
		/// </summary>
		/// <value>
		/// A <see cref="System.Type"/> from the .NET framework.
		/// </value>
		public override System.Type ReturnedClass
		{
			get { return typeof(System.Type); }
		}

		/// <inheritdoc />
		public override string Name
		{
			get { return "Type"; }
		}

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public override object FromStringValue(string xml)
		{
			return ParseStringRepresentation(xml);
		}

		/// <inheritdoc />
		public override object Assemble(object cached, ISessionImplementor session, object owner)
		{
			return ParseStringRepresentation(cached);
		}

		/// <inheritdoc />
		public override object Disassemble(object value, ISessionImplementor session, object owner)
		{
			return GetStringRepresentation(value);
		}

		private static string GetStringRepresentation(object value)
		{
			return ((System.Type) value)?.AssemblyQualifiedName;
		}

		private static object ParseStringRepresentation(object value)
		{
			var str = value as string;
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}

			try
			{
				return ReflectHelper.ClassForFullName(str);
			}
			catch (Exception cnfe)
			{
				throw new HibernateException("Class not found: " + str, cnfe);
			}
		}
	}
}
