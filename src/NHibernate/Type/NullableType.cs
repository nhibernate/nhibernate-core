using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// Superclass of single-column nullable types.
	/// </summary>
	/// <remarks>
	/// Maps the Property to a single column that is capable of storing nulls in it. If a .net Struct is
	/// used it will be created with its uninitialized value and then on Update the uninitialized value of
	/// the Struct will be written to the column - not <see langword="null" />. 
	/// </remarks>
	[Serializable]
	public abstract partial class NullableType : AbstractType
	{
		private static readonly bool IsDebugEnabled;

		static NullableType()
		{
			//cache this, because it was a significant performance cost
			IsDebugEnabled = NHibernateLogger.For(typeof(IType).Namespace).IsDebugEnabled();
		}

		private INHibernateLogger Log
		{
			get { return NHibernateLogger.For(GetType()); }
		}

		private readonly SqlType _sqlType;

		/// <summary>
		/// Initialize a new instance of the NullableType class using a 
		/// <see cref="NHibernate.SqlTypes.SqlType"/>. 
		/// </summary>
		/// <param name="sqlType">The underlying <see cref="NHibernate.SqlTypes.SqlType"/>.</param>
		/// <remarks>This is used when the Property is mapped to a single column.</remarks>
		protected NullableType(SqlType sqlType)
		{
			_sqlType = sqlType;
		}

		/// <summary>
		/// When implemented by a class, put the value from the mapped 
		/// Property into to the <see cref="DbCommand"/>.
		/// </summary>
		/// <param name="cmd">The <see cref="DbCommand"/> to put the value into.</param>
		/// <param name="value">The object that contains the value.</param>
		/// <param name="index">The index of the <see cref="DbParameter"/> to start writing the values to.</param>
		/// <param name="session">The session for which the operation is done.</param>
		/// <remarks>
		/// Implementors do not need to handle possibility of null values because this will
		/// only be called from <see cref="NullSafeSet(DbCommand, object, int, ISessionImplementor)"/> after 
		/// it has checked for nulls.
		/// </remarks>
		public abstract void Set(DbCommand cmd, object value, int index, ISessionImplementor session);

		/// <summary>
		/// When implemented by a class, gets the object in the 
		/// <see cref="DbDataReader"/> for the Property.
		/// </summary>
		/// <param name="rs">The <see cref="DbDataReader"/> that contains the value.</param>
		/// <param name="index">The index of the field to get the value from.</param>
		/// <param name="session">The session for which the operation is done.</param>
		/// <returns>An object with the value from the database.</returns>
		public abstract object Get(DbDataReader rs, int index, ISessionImplementor session);

		/// <summary>
		/// When implemented by a class, gets the object in the 
		/// <see cref="DbDataReader"/> for the Property.
		/// </summary>
		/// <param name="rs">The <see cref="DbDataReader"/> that contains the value.</param>
		/// <param name="name">The name of the field to get the value from.</param>
		/// <param name="session">The session for which the operation is done.</param>
		/// <returns>An object with the value from the database.</returns>
		/// <remarks>
		/// Most implementors just call the <see cref="Get(DbDataReader, int, ISessionImplementor)"/> 
		/// overload of this method.
		/// </remarks>
		public abstract object Get(DbDataReader rs, string name, ISessionImplementor session);

		/// <summary>
		/// A representation of the value to be embedded in an XML element 
		/// </summary>
		/// <param name="val">The object that contains the values.
		/// </param>
		/// <returns>An Xml formatted string.</returns>
		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version. Override ToLoggableString instead.")]
		public virtual string ToString(object val)
		{
			return val.ToString();
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
		/// Parse the XML representation of an instance
		/// </summary>
		/// <param name="xml">XML string to parse, guaranteed to be non-empty</param>
		/// <returns></returns>
		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public virtual object FromStringValue(string xml)
		{
			throw new NotImplementedException();
		}

		public override void NullSafeSet(DbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
		{
			if (settable[0]) NullSafeSet(st, value, index, session);
		}

		/// <inheritdoc />
		/// <remarks>
		/// <para>
		/// This method has been "sealed" because the Types inheriting from <see cref="NullableType"/>
		/// do not need to and should not override this method.
		/// </para>
		/// <para>
		/// This method checks to see if value is null, if it is then the value of 
		/// <see cref="DBNull"/> is written to the <see cref="DbCommand"/>.
		/// </para>
		/// <para>
		/// If the value is not null, then the method <see cref="Set(DbCommand, object, int, ISessionImplementor)"/> 
		/// is called and that method is responsible for setting the value.
		/// </para>
		/// </remarks>
		public sealed override void NullSafeSet(DbCommand st, object value, int index, ISessionImplementor session)
		{
			if (value == null)
			{
				if (IsDebugEnabled)
				{
					Log.Debug("binding null to parameter: {0}", index);
				}

				//Do we check IsNullable?
				// TODO: find out why a certain Parameter would not take a null value...
				// From reading the .NET SDK the default is to NOT accept a null value. 
				// I definitely need to look into this more...
				st.Parameters[index].Value = DBNull.Value;
			}
			else
			{
				if (IsDebugEnabled)
				{
					Log.Debug("binding '{0}' to parameter: {1}", ToLoggableString(value, session.Factory), index);
				}

				Set(st, value, index, session);
			}
		}

		/// <inheritdoc />
		/// <remarks>
		/// This has been sealed because no other class should override it.  This 
		/// method calls <see cref="NullSafeGet(DbDataReader, String, ISessionImplementor)" /> for a single value.  
		/// It only takes the first name from the string[] names parameter - that is a 
		/// safe thing to do because a Nullable Type only has one field.
		/// </remarks>
		public sealed override object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			return NullSafeGet(rs, names[0], session);
		}

		/// <summary>
		/// Extracts the values of the fields from the DataReader
		/// </summary>
		/// <param name="rs">The DataReader positioned on the correct record</param>
		/// <param name="names">An array of field names.</param>
		/// <param name="session">The session for which the operation is done.</param>
		/// <returns>The value off the field from the DataReader</returns>
		/// <remarks>
		/// In this class this just ends up passing the first name to the NullSafeGet method
		/// that takes a string, not a string[].
		/// 
		/// I don't know why this method is in here - it doesn't look like anybody that inherits
		/// from NullableType overrides this...
		/// 
		/// TODO: determine if this is needed
		/// </remarks>
		public virtual object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session)
		{
			return NullSafeGet(rs, names[0], session);
		}

		/// <summary>
		/// Gets the value of the field from the <see cref="DbDataReader" />.
		/// </summary>
		/// <param name="rs">The <see cref="DbDataReader" /> positioned on the correct record.</param>
		/// <param name="name">The name of the field to get the value from.</param>
		/// <param name="session">The session for which the operation is done.</param>
		/// <returns>The value of the field.</returns>
		/// <remarks>
		/// <para>
		/// This method checks to see if value is null, if it is then the null is returned
		/// from this method.
		/// </para>
		/// <para>
		/// If the value is not null, then the method <see cref="Get(DbDataReader, Int32, ISessionImplementor)"/> 
		/// is called and that method is responsible for retrieving the value.
		/// </para>
		/// </remarks>
		public virtual object NullSafeGet(DbDataReader rs, string name, ISessionImplementor session)
		{
			int index = rs.GetOrdinal(name);

			if (rs.IsDBNull(index))
			{
				if (IsDebugEnabled)
				{
					Log.Debug("returning null as column: {0}", name);
				}
				// TODO: add a method to NullableType.GetNullValue - if we want to
				// use "MAGIC" numbers to indicate null values...
				return null;
			}
			else
			{
				object val;
				try
				{
					val = Get(rs, index, session);
				}
				catch (InvalidCastException ice)
				{
					throw new ADOException(
						string.Format(
							"Could not cast the value in field {0} of type {1} to the Type {2}.  Please check to make sure that the mapping is correct and that your DataProvider supports this Data Type.",
							name, rs[index].GetType().Name, GetType().Name), ice);
				}

				if (IsDebugEnabled)
				{
					Log.Debug("returning '{0}' as column: {1}", ToLoggableString(val, session.Factory), name);
				}

				return val;
			}
		}

		/// <inheritdoc />
		/// <remarks>
		/// <para>
		/// This implementation forwards the call to <see cref="NullSafeGet(DbDataReader, String, ISessionImplementor)" />.
		/// </para>
		/// <para>
		/// It has been "sealed" because the Types inheriting from <see cref="NullableType"/>
		/// do not need to and should not override this method.  All of their implementation
		/// should be in <see cref="NullSafeGet(DbDataReader, String, ISessionImplementor)" />.
		/// </para>
		/// </remarks>
		public sealed override object NullSafeGet(DbDataReader rs, string name, ISessionImplementor session, object owner)
		{
			return NullSafeGet(rs, name, session);
		}

		/// <summary>
		/// Gets the underlying <see cref="NHibernate.SqlTypes.SqlType" /> for 
		/// the column mapped by this <see cref="NullableType" />.
		/// </summary>
		/// <value>The underlying <see cref="NHibernate.SqlTypes.SqlType"/>.</value>
		/// <remarks>
		/// This implementation should be suitable for all subclasses unless they need to
		/// do some special things to get the value.  There are no built in <see cref="NullableType"/>s
		/// that override this Property.
		/// </remarks>
		public virtual SqlType SqlType
		{
			get { return _sqlType; }
		}

		/// <inheritdoc />
		/// <remarks>
		/// <para>
		/// This implementation forwards the call to <see cref="NullableType.SqlType" />.
		/// </para>
		/// <para>
		/// It has been "sealed" because the Types inheriting from <see cref="NullableType"/>
		/// do not need to and should not override this method because they map to a single
		/// column.  All of their implementation should be in <see cref="NullableType.SqlType" />.
		/// </para>
		/// </remarks>
		public sealed override SqlType[] SqlTypes(IMapping mapping)
		{
			return new[] { OverrideSqlType(mapping, SqlType) };
		}

		/// <summary>
		/// Overrides the sql type.
		/// </summary>
		/// <param name="type">The type to override.</param>
		/// <param name="mapping">The mapping for which to override <paramref name="type"/>.</param>
		/// <returns>The refined types.</returns>
		static SqlType OverrideSqlType(IMapping mapping, SqlType type)
		{
			return mapping != null ? mapping.Dialect.OverrideSqlType(type) : type;
		}

		/// <summary>
		/// Returns the number of columns spanned by this <see cref="NullableType"/>
		/// </summary>
		/// <returns>A <see cref="NullableType"/> always returns 1.</returns>
		/// <remarks>
		/// This has the hard coding of 1 in there because, by definition of this class, 
		/// a NullableType can only map to one column in a table.
		/// </remarks>
		public override sealed int GetColumnSpan(IMapping session)
		{
			return 1;
		}

		public override bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			return checkable[0] && IsDirty(old, current, session);
		}

		public override bool[] ToColumnNullness(object value, IMapping mapping)
		{
			return value == null ? ArrayHelper.False : ArrayHelper.True;
		}

		public override bool IsEqual(object x, object y)
		{
			return Equals(x, y);
		}

		#region override of System.Object Members

		/// <summary>
		/// Determines whether the specified <see cref="Object"/> is equal to this
		/// <see cref="NullableType"/>.
		/// </summary>
		/// <param name="obj">The <see cref="Object"/> to compare with this NullableType.</param>
		/// <returns>true if the SqlType and Name properties are the same.</returns>
		public override bool Equals(object obj)
		{
			/*
			 *  Step 1: Perform an == test
			 *  Step 2: Instance of check
			 *  Step 3: Cast argument
			 *  Step 4: For each important field, check to see if they are equal
			 *  Step 5: Go back to equals()'s contract and ask yourself if the equals() 
			 *  method is reflexive, symmetric, and transitive
			 */

			if (this == obj)
				return true;

			NullableType rhsType = obj as NullableType;

			if (rhsType == null)
				return false;

			if (Name.Equals(rhsType.Name) && SqlType.Equals(rhsType.SqlType))
				return true;

			return false;
		}

		/// <summary>
		/// Serves as a hash function for the <see cref="NullableType"/>, 
		/// suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>
		/// A hash code that is based on the <see cref="NullableType.SqlType"/>'s 
		/// hash code and the <see cref="AbstractType.Name"/>'s hash code.</returns>
		public override int GetHashCode()
		{
			return (SqlType.GetHashCode() / 2) + (Name.GetHashCode() / 2);
		}

		#endregion
	}
}
