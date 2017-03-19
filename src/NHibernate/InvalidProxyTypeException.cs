using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;

#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
using System.Security;
#endif

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
		public InvalidProxyTypeException(ICollection<string> errors)
			: base(FormatMessage(errors))
		{
			Errors = errors;
		}

		public ICollection<string> Errors { get; private set; }

		private static string FormatMessage(IEnumerable<string> errors)
		{
			var result = new StringBuilder("The following types may not be used as proxies:");
			foreach (string error in errors)
			{
				result.Append('\n').Append(error);
			}
			return result.ToString();
		}

#if FEATURE_SERIALIZATION
		#region Serialization

		public InvalidProxyTypeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			Errors = (ICollection<string>)info.GetValue("errors", typeof(ICollection));
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errors", Errors, typeof (ICollection));
		}

		#endregion
#endif
	}
}
