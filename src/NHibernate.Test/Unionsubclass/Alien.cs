using System.Collections.Generic;

namespace NHibernate.Test.Unionsubclass
{
	public class Alien : Being
	{
		private string species;
		private Hive hive;
		private IList<Alien> hivemates = new List<Alien>();

		public override string Species
		{
			get { return species; }
			set { species = value; }
		}

		public virtual Hive Hive
		{
			get { return hive; }
			set { hive = value; }
		}

		public virtual IList<Alien> Hivemates
		{
			get { return hivemates; }
			set { hivemates = value; }
		}
	}
}