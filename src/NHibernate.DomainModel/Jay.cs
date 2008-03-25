using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Jay.
	/// </summary>
	public class Jay
	{
		private long id;
		private Eye eye;

		public Jay()
		{
		}

		public Jay(Eye eye)
		{
			eye.Jays.Add(this);
			this.eye = eye;
		}

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual Eye Eye
		{
			get { return eye; }
			set { eye = value; }
		}
	}
}