using NUnit.Framework;

namespace NHibernate.Test
{
	public class KnownBugAttribute : ExpectedExceptionAttribute
	{
		public KnownBugAttribute(string bug)
		{
			UserMessage = "Known bug " + bug;
		}

		public KnownBugAttribute(string bug, System.Type exceptionType) 
			: base(exceptionType)
		{
			UserMessage = "Known bug " + bug;
		}

		public KnownBugAttribute(string bug, string exceptionName)
			: base(exceptionName)
		{
			UserMessage = "Known bug " + bug;
		}
	}
}