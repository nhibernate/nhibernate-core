using System;
using NHibernate.SqlTypes;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// An immutable Parameter that supports a Precision and a Scale
	/// </summary>
	/// <remarks>
	/// This should only be used when the property needs to be mapped with
	/// a <c>type="Decimal(20,4)"</c> because for some reason the default parameter
	/// generation of <c>decimal(19,5)</c> (MsSql specific) is not good enough.
	/// </remarks>
	[Serializable]
	public sealed class ParameterPrecisionScale : Parameter
	{
		private byte _precision;
		private byte _scale;
		
		/// <summary>
		/// Initializes a new instance of <see cref="ParameterPrecisionScale"/> class.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="sqlType">
		/// The <see cref="SqlType"/> that contains a Precision &amp; Scale 
		/// to create the parameter for.
		/// </param>
		public ParameterPrecisionScale(string name, SqlType sqlType)
			: this( name, null, sqlType )
		{		
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ParameterPrecisionScale"/> class.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="tableAlias">The Alias to use for the table.</param>
		/// <param name="sqlType">
		/// The <see cref="SqlType"/> that contains a Precision &amp; Scale 
		/// to create the parameter for.
		/// </param>
		public ParameterPrecisionScale(string name, string tableAlias, SqlType sqlType ) 
			: base( name, tableAlias, sqlType )
		{
			_precision = sqlType.Precision;
			_scale = sqlType.Scale;
		}

		/// <summary>
		/// Gets the precision of data the IDbDataParameter should hold.
		/// </summary>
		/// <value>The precision of data the IDbDataParameter should hold.</value>
		public byte Precision
		{
			get { return _precision; }
		}

		/// <summary>
		/// Gets the scale of data the IDbDataParameter should hold.
		/// </summary>
		/// <value>The scale of data the IDbDataParameter should hold.</value>
		public byte Scale
		{
			get { return _scale; }
		}

		#region System.Object Members

		/// <summary>
		/// Determines wether this instance and the specified object 
		/// are the same Type and have the same values.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns>
		/// <c>true</c> if the object is a <see cref="ParameterPrecisionScale" /> and has the
		/// same values.
		/// </returns>
		public override bool Equals( object obj )
		{
			ParameterPrecisionScale rhs = null;

			// Step1: Perform an equals test
			if( obj == this )
			{
				return true;
			}

			// Step	2: Instance of check - don't need to worry about subclasses
			// getting in here because this class is sealed.
			rhs = obj as ParameterPrecisionScale;
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
			
			return this.Precision == rhs.Precision
				&& this.Scale == rhs.Scale;
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
			// base should be sufficient enough because SqlType factors 
			// in the Precision and Scale to the hash code.
			return base.GetHashCode();
		}

		#endregion
	}
}