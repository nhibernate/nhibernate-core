using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1264
{
	public class Passenger
	{
		private int id;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		private Name name;

		public Name Name
		{
			get { return name; }
			set { name = value; }
		}

		private string frequentFlyerNumber;

		public string FrequentFlyerNumber
		{
			get { return frequentFlyerNumber; }
			set { frequentFlyerNumber = value; }
		}

		private Reservation reservation;

		public Reservation Reservation
		{
			get { return reservation; }
			set { reservation = value; }
		}
	}
}
