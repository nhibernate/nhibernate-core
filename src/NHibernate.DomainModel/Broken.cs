using System;

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

		public override bool Equals(object other)
		{
			if (!(other is Broken))
			{
				return false;
			}
			Broken that = (Broken) other;
			return this._id.Equals(that._id) && this._otherId.Equals(that._otherId);
		}

		public override int GetHashCode()
		{
			return 1;
		}
	}
}