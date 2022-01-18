using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Security;

namespace NHibernate
{
	[Serializable]
	public class SchemaValidationException : HibernateException
	{
		public ReadOnlyCollection<string> ValidationErrors { get; }

		public SchemaValidationException(string msg, IList<string> validationErrors) : base(msg)
		{
			ValidationErrors = new ReadOnlyCollection<string>(validationErrors);
		}

		protected SchemaValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			ValidationErrors =
				(ReadOnlyCollection<string>) info.GetValue("ValidationErrors", typeof(ReadOnlyCollection<string>));
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ValidationErrors", ValidationErrors);
		}

		public override string Message
		{
			get
			{
				var message = base.Message;
				if (ValidationErrors == null || ValidationErrors.Count == 0)
					return message;
				var errors = "ValidationErrors:" + string.Join(Environment.NewLine + "- ", ValidationErrors);
				return message + Environment.NewLine + errors;
			}
		}
	}
}
