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
		private ICollection errors;

		public InvalidProxyTypeException(ICollection errors)
			: base(FormatMessage(errors))
		{
			this.errors = errors;
		}

		public ICollection Errors
		{
			get { return errors; }
		}

		private static string FormatMessage(ICollection errors)
		{
			StringBuilder result = new StringBuilder("The following types may not be used as proxies:");
			foreach (string error in errors)
			{
				result.Append('\n').Append(error);
			}
			return result.ToString();
		}

		#region Serialization

		public InvalidProxyTypeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.errors = (ICollection) info.GetValue("errors", typeof(ICollection));
		}

		[SecurityPermission(SecurityAction.LinkDemand,
			Flags=SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errors", errors, typeof(ICollection));
		}

		#endregion
	}
}