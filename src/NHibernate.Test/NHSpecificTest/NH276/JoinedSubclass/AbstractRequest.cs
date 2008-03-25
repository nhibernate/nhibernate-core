using System;

namespace NHibernate.Test.NHSpecificTest.NH276.JoinedSubclass
{
	public class AbstractRequest
	{
		private int _requestId;
		private Status _status;
		private Organization _office;

		public int RequestId
		{
			get { return _requestId; }
			set { _requestId = value; }
		}

		public Status Status
		{
			get { return _status; }
			set { _status = value; }
		}

		public Organization Office
		{
			get { return _office; }
			set { _office = value; }
		}
	}
}