using System;

namespace NHibernate 
{
	/// <summary>
	/// Indicates failure of an assertion: a possible bug in NHibernate
	/// </summary>
	[Serializable]
	public class AssertionFailure : ApplicationException 
	{
		public AssertionFailure(string s) : base(s) 
		{
			log4net.LogManager.GetLogger( typeof(AssertionFailure) ).Error("An AssertionFailure occured - this may indicate a bug in NHibernate", this);
		}

		public AssertionFailure(string s, Exception e) : base(s, e) 
		{
			log4net.LogManager.GetLogger( typeof(AssertionFailure) ).Error("An AssertionFailure occured - this may indicate a bug in NHibernate", e);
		}
	}
}
