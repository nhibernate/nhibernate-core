using NUnit.Framework;

namespace NHibernate.Test
{
	public class KnownBugAttribute : ExpectedExceptionAttribute
	{
		public KnownBugAttribute()
		{
			UserMessage = "Known bug";
		}

		public KnownBugAttribute(System.Type exceptionType) : base(exceptionType)
		{
			UserMessage = "Known bug";
		}

		public KnownBugAttribute(string exceptionName) : base(exceptionName)
		{
			UserMessage = "Known bug";
		}
	}
}