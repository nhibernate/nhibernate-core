using System;
using System.Runtime.Serialization;

namespace NHibernate 
{
	/// <summary>
	/// Indicates failure of an assertion: a possible bug in NHibernate
	/// </summary>
	[Serializable]
	public class AssertionFailure : ApplicationException 
	{
		public AssertionFailure() : base(String.Empty)
		{
			log4net.LogManager.GetLogger( typeof(AssertionFailure) ).Error("An AssertionFailure occured - this may indicate a bug in NHibernate");
		}

		public AssertionFailure(string message) : base(message) 
		{
			log4net.LogManager.GetLogger( typeof(AssertionFailure) ).Error("An AssertionFailure occured - this may indicate a bug in NHibernate", this);
		}

		public AssertionFailure(string message, Exception e) : base(message, e) 
		{
			log4net.LogManager.GetLogger( typeof(AssertionFailure) ).Error("An AssertionFailure occured - this may indicate a bug in NHibernate", e);
		}

		protected AssertionFailure(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
