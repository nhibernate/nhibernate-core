using System;
using NHibernate.SqlTypes;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Extension to the Parameter class that supports Parameters with
	/// a Length.
	/// </summary>
	/// <remarks>
	/// This should only be used when the property needs to be mapped with
	/// a <c>type="String(200)"</c> because for some reason the default parameter
	/// generation of <c>nvarchar(4000)</c> (MsSql specific) is not good enough.
	/// </remarks>
	[Serializable]
	public sealed class ParameterLength : Parameter
	{
		private int _length;

		/// <summary>
		/// Initializes a new instance of <see cref="ParameterLength"/> class.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="sqlType">
		/// The <see cref="SqlType"/> that contains a Length 
		/// to create the parameter for.
		/// </param>
		public ParameterLength(string name, SqlType sqlType)
			: this( name, null, sqlType )
		{	
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ParameterLength"/> class.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="tableAlias">The Alias to use for the table.</param>
		/// <param name="sqlType">
		/// The <see cref="SqlType"/> that contains a Length 
		/// to create the parameter for.
		/// </param>
		public ParameterLength(string name, string tableAlias, SqlType sqlType) 
			: base( name, tableAlias, sqlType )
		{
			_length = sqlType.Length;
		}

		/// <summary>
		/// Gets the length of data the IDbDataParameter should hold.
		/// </summary>
		/// <value>The length of data the IDbDataParameter should hold.</value>
		public int Length
		{
			get { return _length; }
		}

		#region System.Object Members

		/// <summary>
		/// Determines wether this instance and the specified object 
		/// are the same Type and have the same values.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns>
		/// <c>true</c> if the object is a <see cref="ParameterLength" /> and has the
		/// same values.
		/// </returns>
		public override bool Equals( object obj )
		{
			ParameterLength rhs = null;

			// Step1: Perform an equals test
			if( obj == this )
			{
				return true;
			}

			// Step	2: Instance of check - don't need to worry about subclasses
			// getting in here because this class is sealed.
			rhs = obj as ParameterLength;
			if( rhs == null )
			{
				return false;
			}
		
			//Step 3: Check each important field

			// isTypeSensitive is false because we want to check the values
			// of the fields - we know the rhs is a subclass of Parameter.
			if( this.Equals( rhs, false )==false )
			{
				return false;
			}
			

			return this.Length==rhs.Length;
		}

		/// <summary>
		/// Gets a hash code based on the SqlType, Name, and TableAlias properties.
		/// </summary>
		/// <returns>
		/// An <see cref="Int32"/> value for the hash code.
		/// </returns>
		/// <remarks>
		/// This just uses the <see cref="Parameter"/>'s <c>GetHashCode()</c> method.  The
		/// compiler complains if Equals is implemented without a GetHashCode.
		/// </remarks>
		public override int GetHashCode()
		{
			// base should be sufficient enough because SqlType uses the 
			// precision and scale to get the hash code
			return base.GetHashCode();
		}

		#endregion
	}
}