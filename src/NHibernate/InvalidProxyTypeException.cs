using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

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
		private ICollection errors;

		public InvalidProxyTypeException( System.Type type, IList errors )
			: base(FormatMessage(type, errors))
		{
			this.type = type;
			this.errors = errors;
		}

		public System.Type Type
		{
			get { return type; }
		}

		public ICollection Errors
		{
			get { return errors; }
		}

		private static string FormatMessage(System.Type type, IList errors)
		{
			StringBuilder result = new StringBuilder();
			result.AppendFormat("Type '{0}' cannot be specified as proxy:", type.FullName);
			foreach (string error in errors)
			{
				result.Append("\n - ").Append(error);
			}
			return result.ToString();
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
