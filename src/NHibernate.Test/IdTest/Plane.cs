namespace NHibernate.Test.IdTest
{
	public class Plane
	{
		private long id;
		private int nbrOfSeats;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual int NbrOfSeats
		{
			get { return nbrOfSeats; }
			set { nbrOfSeats = value; }
		}
	}
}