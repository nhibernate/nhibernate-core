using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

using NHibernate.Util;

namespace NHibernate
{
	[Serializable]
	public class PropertyValueException : HibernateException
	{
		private readonly System.Type persistentClass;
		private readonly string propertyName;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyValueException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="persistentClass">The <see cref="System.Type"/> that NHibernate was trying to access.</param>
		/// <param name="propertyName">The name of the Property that was being get/set.</param>
		public PropertyValueException( string message, System.Type persistentClass, string propertyName )
			: base( message )
		{
			this.persistentClass = persistentClass;
			this.propertyName = propertyName;
		}

		public System.Type PersistentClass
		{
			get { return persistentClass; }
		}

		public string PropertyName
		{
			get { return propertyName; }
		}

		public override string Message
		{
			get
			{
				return base.Message +
					StringHelper.Qualify( persistentClass.FullName, propertyName );
			}
		}

		#region Serialization

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyValueException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected PropertyValueException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
			persistentClass = info.GetValue( "persistentClass", typeof( System.Type ) ) as System.Type;
			propertyName = info.GetString( "propertyName" );
		}

		/// <summary>
		/// Sets the serialization info for <see cref="PropertyValueException"/> after 
		/// getting the info from the base Exception.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		[SecurityPermissionAttribute(SecurityAction.LinkDemand,
		                             Flags=SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			base.GetObjectData( info, context );
			info.AddValue( "persistentClass", persistentClass );
			info.AddValue( "propertyName", propertyName );
		}

		#endregion
	}
}
