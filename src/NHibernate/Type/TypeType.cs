using System;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps the Assembly Qualified Name of a <see cref="System.Type"/> to a 
	/// <see cref="DbType.String" /> column.
	/// </summary>
	[Serializable]
	public class TypeType : ImmutableType
	{
		/// <summary></summary>
		internal TypeType()
			: base(new StringSqlType()) {}

		/// <summary>
		/// Initialize a new instance of the TypeType class using a 
		/// <see cref="SqlType"/>. 
		/// </summary>
		/// <param name="sqlType">The underlying <see cref="SqlType"/>.</param>
		internal TypeType(StringSqlType sqlType)
			: base(sqlType) {}

		public override SqlType SqlType
		{
			get { return NHibernateUtil.String.SqlType; }
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> in the <see cref="IDataReader"/> for the Property.
		/// </summary>
		/// <param name="rs">The <see cref="IDataReader"/> that contains the value.</param>
		/// <param name="index">The index of the field to get the value from.</param>
		/// <returns>The <see cref="System.Type"/> from the database.</returns>
		/// <exception cref="TypeLoadException">
		/// Thrown when the value in the database can not be loaded as a <see cref="System.Type"/>
		/// </exception>
		public override object Get(IDataReader rs, int index)
		{
			string str = (string)NHibernateUtil.String.Get(rs, index);
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}
			else
			{
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

		/// <summary>
		/// Gets the <see cref="System.Type"/> in the <see cref="IDataReader"/> for the Property.
		/// </summary>
		/// <param name="rs">The <see cref="IDataReader"/> that contains the value.</param>
		/// <param name="name">The name of the field to get the value from.</param>
		/// <returns>The <see cref="System.Type"/> from the database.</returns>
		/// <remarks>
		/// This just calls gets the index of the name in the IDataReader
		/// and calls the overloaded version <see cref="Get(IDataReader, Int32)"/>
		/// (IDataReader, Int32). 
		/// </remarks>
		/// <exception cref="TypeLoadException">
		/// Thrown when the value in the database can not be loaded as a <see cref="System.Type"/>
		/// </exception>
		public override object Get(IDataReader rs, string name)
		{
			return Get(rs, rs.GetOrdinal(name));
		}

		/// <summary>
		/// Puts the Assembly Qualified Name of the <see cref="System.Type"/> 
		/// Property into to the <see cref="IDbCommand"/>.
		/// </summary>
		/// <param name="cmd">The <see cref="IDbCommand"/> to put the value into.</param>
		/// <param name="value">The <see cref="System.Type"/> that contains the value.</param>
		/// <param name="index">The index of the <see cref="IDbDataParameter"/> to start writing the value to.</param>
		/// <remarks>
		/// This uses the <see cref="NullableType.Set(IDbCommand, Object,Int32)"/> method of the 
		/// <see cref="NHibernateUtil.String"/> object to do the work.
		/// </remarks>
		public override void Set(IDbCommand cmd, object value, int index)
		{
			NHibernateUtil.String.Set(cmd, ((System.Type)value).AssemblyQualifiedName, index);
		}

		/// <summary>
		/// A representation of the value to be embedded in an XML element 
		/// </summary>
		/// <param name="value">The <see cref="System.Type"/> that contains the values.
		/// </param>
		/// <returns>An Xml formatted string that contains the Assembly Qualified Name.</returns>
		public override string ToString(object value)
		{
			return ((System.Type)value).AssemblyQualifiedName;
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

		/// <summary></summary>
		public override string Name
		{
			get { return "Type"; }
		}

		public override object FromStringValue(string xml)
		{
			try
			{
				return ReflectHelper.ClassForFullName(xml);
			}
			catch (Exception tle)
			{
				throw new HibernateException("could not parse xml:" + xml, tle);
			}
		}
	}
}
