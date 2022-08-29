using System;

namespace NHibernate.Test.LinqBulkManipulation.Domain
{
	public class Mammal : Animal
	{
		private bool pregnant;
		private DateTime? birthdate;

		public virtual bool Pregnant
		{
			get { return pregnant; }
			set { pregnant = value; }
		}

		public virtual DateTime? Birthdate
		{
			get { return birthdate; }
			set { birthdate = value; }
		}
	}
}
