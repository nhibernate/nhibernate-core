using System;

namespace NHibernate.Test.NHSpecificTest.NH276.JoinedSubclass
{
	/// <summary>
	/// Summary description for Request.
	/// </summary>
	public class Request : AbstractRequest
	{
		private string _extra;

		public string Extra
		{
			get { return _extra; }
			set { _extra = value; }
		}
	}
}
