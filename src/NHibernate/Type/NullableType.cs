using System;
using System.Data;

using NHibernate.SqlTypes;
using NHibernate.Engine;
using NHibernate.Ps;
using NHibernate.Util;


namespace NHibernate.Type
{
	/// <summary>
	/// Superclass of single-column nullable types.
	/// </summary>
	/// <remarks>Maps the Property to a single column that is capable of storing nulls.</remarks>
	public abstract class NullableType : AbstractType {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(NullableType));

		/// <summary>
		/// The underlying <see cref="SqlType"/> used to retrieve and store the value.
		/// </summary>
		/// <value>The underlying <see cref="SqlType"/>.</value>
		protected SqlType sqlType;

		/// <summary>
		/// Initialize a new instance of the NullableType class using a 
		/// <see cref="SqlType"/>. 
		/// </summary>
		/// <param name="sqlType">The underlying <see cref="SqlType"/>.</param>
		/// <remarks>This is used when the Property is mapped to a single column.</remarks>
		protected NullableType(SqlType sqlType) 
		{
			this.sqlType = sqlType;
		}

		/// <summary>
		/// When implemented by a class, put the value from the mapped 
		/// Property into to the <see cref="IDbCommand"/>.
		/// </summary>
		/// <param name="cmd">The <see cref="IDbCommand"/> to put the value into.</param>
		/// <param name="value">The object that contains the value.</param>
		/// <param name="index">The index of the <see cref="IDbDataParameter"/> to start writing the values to.</param>
		/// <remarks>
		/// Implementors do not need to handle possibility of null values because this will
		/// only be called from <see cref="NullSafeSet(IDbCommand, object, int)"/> after 
		/// it has checked for nulls.
		/// </remarks>
		public abstract void Set(IDbCommand cmd, object value, int index);

		/// <summary>
		/// When implemented by a class, gets the object in the 
		/// <see cref="IDataReader"/> for the Property.
		/// </summary>
		/// <param name="rs">The <see cref="IDataReader"/> that contains the value.</param>
		/// <param name="index">The index of the field to get the value from.</param>
		/// <returns>An object with the value from the database.</returns>
		public abstract object Get(IDataReader rs, int index);
		
		/// <summary>
		/// When implemented by a class, gets the object in the 
		/// <see cref="IDataReader"/> for the Property.
		/// </summary>
		/// <param name="rs">The <see cref="IDataReader"/> that contains the value.</param>
		/// <param name="name">The name of the field to get the value from.</param>
		/// <returns>An object with the value from the database.</returns>
		/// <remarks>
		/// Most implementors just call the <see cref="Get(IDataReader, Int32)"/> 
		/// overload of this method.
		/// </remarks>
		public abstract object Get(IDataReader rs, string name);

		
		/// <summary>
		/// A representation of the value to be embedded in an XML element 
		/// </summary>
		/// <param name="val">The object that contains the values.
		/// </param>
		/// <returns>An Xml formatted string.</returns>
		public abstract string ToXML(object val);

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.ToXML"]/*'
		/// /> 
		/// <remarks>
		/// <para>
		/// This implemenation forwards the call to <see cref="ToXML(Object)"/> if the parameter 
		/// value is not null.
		/// </para>
		/// <para>
		/// It has been "sealed" because the Types inheriting from <see cref="NullableType"/>
		/// do not need and should not override this method.  All of their implementation
		/// should be in <see cref="ToXML(Object)"/>.
		/// </para>
		/// </remarks>
		public override sealed string ToXML(object value, ISessionFactoryImplementor pc) 
		{
			return (value==null) ? null : ToXML(value);
		}
		
		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeSet"]/*'
		/// /> 
		/// <remarks>
		/// <para>
		/// This implemenation forwards the call to <see cref="NullSafeSet(IDbCommand, Object, Int32)" />.
		/// </para>
		/// <para>
		/// It has been "sealed" because the Types inheriting from <see cref="NullableType"/>
		/// do not need to and should not override this method.  All of their implementation
		/// should be in <see cref="NullSafeSet(IDbCommand, Object, Int32)" />.
		/// </para>
		/// </remarks>
		public override sealed void NullSafeSet(IDbCommand cmd, object value, int index, ISessionImplementor session) 
		{
			NullSafeSet(cmd, value, index);
		}

		/// <summary>
		/// Puts the value from the mapped class into the <see cref="IDbCommand"/>.
		/// </summary>
		/// <param name="cmd">The <see cref="IDbCommand"/> to put the values into.</param>
		/// <param name="value">The object that contains the values.</param>
		/// <param name="index">The index of the <see cref="IDbDataParameter"/> to write the value to.</param>
		/// <remarks>
		/// <para>
		/// This method checks to see if value is null, if it is then the value of 
		/// <see cref="DBNull"/> is written to the <see cref="IDbCommand"/>.
		/// </para>
		/// <para>
		/// If the value is not null, then the method <see cref="Set(IDbCommand, Object, Int32)"/> 
		/// is called and that method is responsible for setting the value.
		/// </para>
		/// </remarks>
		public void NullSafeSet(IDbCommand cmd, object value, int index) {
			if (value==null) {
				if ( log.IsDebugEnabled )
					log.Debug("binding null to parameter: " + index.ToString());
				
				//Do we check IsNullable?
				// TODO: find out why a certain Parameter would not take a null value...
				// From reading the .NET SDK the default is to NOT accept a null value. 
                // I definitely need to look into this more...
				( (IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
			}
			else {
				if ( log.IsDebugEnabled )
					log.Debug("binding '" + ToXML(value) + "' to parameter: " + index);
				
				Set(cmd, value, index);
			}
		}
		
		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeGet(IDataReader, String[], ISessionImplementor, Object)"]/*'
		/// /> 
		/// <remarks>
		/// This has been sealed because no other class should override it.  This 
		/// method calss <see cref="NullSafeGet(IDataReader, String)" /> for a single value.  
		/// It only takes the first name from the string[] names parameter - that is a 
		/// safe thing to do because a Nullable Type only has one field.
		/// </remarks>
		public override sealed object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, object owner) 
		{
			return NullSafeGet(rs, names[0]);
		}

		/// <summary>
		/// Extracts the values of the fields from the DataReader
		/// </summary>
		/// <param name="rs">The DataReader positioned on the correct record</param>
		/// <param name="names">An array of field names.</param>
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
		public virtual object NullSafeGet(IDataReader rs, string[] names) {
			return NullSafeGet(rs, names[0]);
		}

		/// <summary>
		/// Gets the value of the field from the <see cref="IDataReader" />.
		/// </summary>
		/// <param name="rs">The <see cref="IDataReader" /> positioned on the correct record.</param>
		/// <param name="name">The name of the field to get the value from.</param>
		/// <returns>The value of the field.</returns>
		/// <remarks>
		/// <para>
		/// This method checks to see if value is null, if it is then the null is returned
		/// from this method.
		/// </para>
		/// <para>
		/// If the value is not null, then the method <see cref="Get(IDataReader, Int32)"/> 
		/// is called and that method is responsible for retreiving the value.
		/// </para>
		/// </remarks>
		public virtual object NullSafeGet(IDataReader rs, string name) 
		{
			
			int index = rs.GetOrdinal(name);

			if(rs.IsDBNull(index)) {
				if ( log.IsDebugEnabled )
					log.Debug("returning null as column: " + name);
				// TODO: add a method to NullableType.GetNullValue - if we want to
				// use "MAGIC" numbers to indicate null values...
				return null;
			}
			else {
				
				object val = null;
				try 
				{
					val = Get(rs, index);
				}
				catch(System.InvalidCastException ice) 
				{
					throw new ADOException(
						"Could not cast the value in field " + name + " to the Type " + this.GetType().Name + 
						".  Please check to make sure that the mapping is correct and that your DataProvider supports this Data Type.", ice);
				}
				
				if ( log.IsDebugEnabled )
					log.Debug("returning '" + ToXML(val) + "' as column: " + name);
				
				return val;
			}

		}

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.NullSafeGet(IDataReader, String, ISessionImplementor, Object)"]/*'
		/// /> 
		/// <remarks>
		/// <para>
		/// This implemenation forwards the call to <see cref="NullSafeGet(IDataReader, String)" />.
		/// </para>
		/// <para>
		/// It has been "sealed" because the Types inheriting from <see cref="NullableType"/>
		/// do not need to and should not override this method.  All of their implementation
		/// should be in <see cref="NullSafeGet(IDataReader, String)" />.
		/// </para>
		/// </remarks>
		public override sealed object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, object owner) 
		{
			return NullSafeGet(rs, name);
		}
	
		/// <summary>
		/// Gets the underlying <see cref="SqlType" /> for 
		/// the column mapped by this <see cref="NullableType" />.
		/// </summary>
		/// <value>The underlying <see cref="SqlType"/>.</value>
		/// <remarks>
		/// This implementation should be suitable for all subclasses unless they need to
		/// do some special things to get the value.  There are no built in <see cref="NullableType"/>s
		/// that override this Property.
		/// </remarks>
		public virtual SqlType SqlType 
		{
			get {return this.sqlType;}
		}

		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.SqlTypes"]/*'
		/// /> 
		/// <remarks>
		/// <para>
		/// This implemenation forwards the call to <see cref="NullableType.SqlType" />.
		/// </para>
		/// <para>
		/// It has been "sealed" because the Types inheriting from <see cref="NullableType"/>
		/// do not need to and should not override this method because they map to a single
		/// column.  All of their implementation should be in <see cref="NullableType.SqlType" />.
		/// </para>
		/// </remarks>
		public override sealed SqlType[] SqlTypes(IMapping session) 
		{
			return new SqlType[] {SqlType};
		}

		/// <summary>
		/// Returns the number of columns spanned by this <see cref="NullableType"/>
		/// </summary>
		/// <returns>A <see cref="NullableType"/> always returns 1.</returns>
		/// <remarks>
		/// This has the hard coding of 1 in there because, by definition of this class, 
		/// a NullableType can only map to one column in a table.
		/// </remarks>
		public override sealed int GetColumnSpan(IMapping session) {
			return 1;
		}

		
		/// <summary>
		/// When implemented by a class, returns a deep copy of the persistent state.
		/// </summary>
		/// <param name="val">The value to deep copy.</param>
		/// <returns>A deep copy of the object.</returns>
		/// <remarks>
		/// Most of the built in <see cref="NullableTypes"/> will just return the same object
		/// passed into it.
		/// </remarks>
		public abstract object DeepCopyNotNull(object val);
	
		/// <include file='IType.cs.xmldoc' 
		///		path='//members[@type="IType"]/member[@name="M:IType.DeepCopy"]/*'
		/// /> 
		/// <remarks>
		/// <para>
		/// This implemenation forwards the call to <see cref="DeepCopyNotNull(Object)"/> if the parameter 
		/// value is not null.
		/// </para>
		/// <para>
		/// It has been "sealed" because the Types inheriting from <see cref="NullableType"/>
		/// do not need and should not override this method.  All of their implementation
		/// should be in <see cref="DeepCopyNotNull(Object)"/>.
		/// </para>
		/// </remarks>
		public override sealed object DeepCopy(object val) 
		{
			return (val==null) ? null : DeepCopyNotNull(val);
		}


		#region override of System.Object Members

		/// <summary>
		/// Determines whether the specified <see cref="Object"/> is equal to this
		/// <see cref="NullableType"/>.
		/// </summary>
		/// <param name="obj">The <see cref="Object"/> to compare with this NullableType.</param>
		/// <returns>true if the SqlType and Name properties are the same.</returns>
		public override bool Equals(object obj) {
			/*
			 *  Step 1: Perform an == test
			 *  Step 2: Instance of check
			 *  Step 3: Cast argument
			 *  Step 4: For each important field, check to see if they are equal
			 *  Step 5: Go back to equals()'s contract and ask yourself if the equals() 
			 *  method is reflexive, symmetric, and transitive
			 */ 

			if(this==obj) return true;

			NullableType rhsType = obj as NullableType;

			if(rhsType==null) return false;

			if(this.Name.Equals(rhsType.Name)
				&& this.SqlType.Equals(rhsType.SqlType)) return true;

			return false;
		}
	
		/// <summary>
		/// Serves as a hash function for the <see cref="NullableType"/>, 
		/// suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>
		/// A hash code that is based on the <see cref="NullableType.SqlType"/>'s 
		/// hash code and the <see cref="NullableType.Name"/>'s hash code.</returns>
		public override int GetHashCode() {
			
			return (SqlType.GetHashCode() / 2) + (Name.GetHashCode() / 2);

		}

		#endregion

	}
}