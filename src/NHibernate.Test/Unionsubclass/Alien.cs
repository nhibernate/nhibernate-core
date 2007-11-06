using System.Collections;

namespace NHibernate.Test.Unionsubclass
{
	public class Alien : Being
	{
		private string species;
		private Hive hive;
		private IList hivemates = new ArrayList();

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

		public virtual IList Hivemates
		{
			get { return hivemates; }
			set { hivemates = value; }
		}
	}
}