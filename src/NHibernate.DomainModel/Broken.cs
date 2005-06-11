using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class Broken
	{
		private long _id;
		private string _otherId;
		private DateTime _timestamp;

		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string OtherId
		{
			get { return _otherId; }
			set { _otherId = value; }
		}

		public DateTime Timestamp
		{
			get { return _timestamp; }
			set { _timestamp = value; }
		}
	}
}
