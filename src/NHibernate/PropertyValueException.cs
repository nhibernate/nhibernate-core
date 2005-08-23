using System;
using System.Runtime.Serialization;

using NHibernate.Util;

namespace NHibernate
{
	public class PropertyValueException : HibernateException
	{
		private readonly System.Type persistentClass;
		private readonly string propertyName;

		public PropertyValueException( string s, System.Type persistentClass, string propertyName )
			: base( s )
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

		public PropertyValueException()
		{
		}

		public override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			base.GetObjectData( info, context );
			info.AddValue( "persistentClass", persistentClass );
			info.AddValue( "propertyName", propertyName );
		}

		protected PropertyValueException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
			persistentClass = ( System.Type ) info.GetValue( "persistentClass", typeof( System.Type ) );
			propertyName = info.GetString( "propertyName" );
		}

		#endregion
	}
}