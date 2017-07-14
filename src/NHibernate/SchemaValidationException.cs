using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate
{
	public class SchemaValidationException : HibernateException
	{
		public ReadOnlyCollection<string> ValidationErrors { get; }

		public SchemaValidationException(string msg, IList<string> validationErrors) : base(msg)
		{
			ValidationErrors = new ReadOnlyCollection<string>(validationErrors);
		}
	}
}
