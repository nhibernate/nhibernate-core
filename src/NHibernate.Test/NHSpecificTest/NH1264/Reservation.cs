using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1264
{
	public class Reservation
	{
		private IList<Passenger> passengers;

		private int id;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		private string confirmationNumber;

		public string ConfirmationNumber
		{
			get { return confirmationNumber; }
			set { confirmationNumber = value; }
		}

		public IList<Passenger> Passengers
		{
			get
			{
				if (passengers == null)
				{
					passengers = new List<Passenger>();
				}
				return passengers;
			}

			private set
			{
				passengers = value;
			}
		}
	}
}
