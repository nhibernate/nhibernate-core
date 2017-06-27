using System;
using NHibernate.Linq;

namespace NHibernate.Test.Linq
{
	static class ExtensionMethods
	{
		[LinqExtensionMethod("Replace")]
		public static string ReplaceExtension(this string subject, string search, string replaceWith)
		{
			throw new InvalidOperationException("To be translated to SQL only");
		}

		[LinqExtensionMethod]
		public static string Replace(this string subject, string search, string replaceWith)
		{
			throw new InvalidOperationException("To be translated to SQL only");
		}

		// NH4 default behavior
		[LinqExtensionMethod("Replace", LinqExtensionPreEvaluation.AllowPreEvaluation)]
		public static string ReplaceWithEvaluation(this string subject, string search, string replaceWith)
		{
			if (subject == null)
				throw new ArgumentNullException(nameof(subject));
			return subject.Replace(search, replaceWith) + " (done in-memory)";
		}

		// Just for checking weird combination
		[LinqExtensionMethod("Replace", LinqExtensionPreEvaluation.AllowPreEvaluation), NoPreEvaluation]
		public static string ReplaceWithEvaluation2(this string subject, string search, string replaceWith)
		{
			if (subject == null)
				throw new ArgumentNullException(nameof(subject));
			return subject.Replace(search, replaceWith) + " (done in-memory)";
		}
	}
}
