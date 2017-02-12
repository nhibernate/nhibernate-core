using System;
using NUnit.Framework;

namespace NHibernate.Test
{
	public class KnownBugAttribute : IgnoreAttribute
	{
		public string UserMessage { get; set; }

		public KnownBugAttribute(string bug)
			: base("Known bug " + bug)
		{
			UserMessage = "Known bug " + bug;
		}

		public KnownBugAttribute(string bug, System.Type exceptionType)
			: base("Known bug " + bug)
		{
			UserMessage = "Known bug " + bug;
		}

		public KnownBugAttribute(string bug, string exceptionName)
			: base("Known bug " + bug)
		{
			UserMessage = "Known bug " + bug;
		}
	}
}
