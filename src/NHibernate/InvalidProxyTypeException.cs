using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace NHibernate
{
	/// <summary>
	/// Thrown when an invalid type is specified as a proxy for a class.
	/// The exception is also thrown when a class is specified as lazy,
	/// but cannot be used as a proxy for itself.
	/// </summary>
	[Serializable]
	public class InvalidProxyTypeException : MappingException
	{
		private System.Type type;

		public InvalidProxyTypeException( System.Type type, string message )
			: base( message )
		{
			this.type = type;
		}

		public System.Type Type
		{
			get { return type; }
		}

		public override string Message
		{
			get
			{
				return string.Format(
					"Type '{0}' cannot be specified as proxy: {1}",
					type.FullName, base.Message );
			}
		}

		#region Serialization

		public InvalidProxyTypeException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
			this.type = ( System.Type ) info.GetValue( "type", typeof( System.Type ) );
		}

		[SecurityPermissionAttribute(SecurityAction.LinkDemand,
		                             Flags=SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			base.GetObjectData (info, context);
			info.AddValue( "type", type );
		}

		#endregion
	}
}
