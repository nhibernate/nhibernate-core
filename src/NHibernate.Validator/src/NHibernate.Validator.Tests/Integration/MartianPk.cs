namespace NHibernate.Validator.Tests.Integration
{
	public class MartianPk
	{
		private string colony;
		private string name;

		public MartianPk()
		{
		}

		public MartianPk(string colony, string name)
		{
			this.colony = colony;
			this.name = name;
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[Length(Max = 4)]
		public string Colony
		{
			get { return colony; }
			set { colony = value; }
		}

		public override bool Equals(object o)
		{
			if (this == o) return true;
			
			if (o == null || GetType() != o.GetType()) return false;

			MartianPk martianPk = (MartianPk) o;

			if (!colony.Equals(martianPk.colony)) return false;
			if (!name.Equals(martianPk.name)) return false;

			return true;
		}

		public override int GetHashCode() 
		{
			int result;
			result = name.GetHashCode();
			result = 29 * result + colony.GetHashCode();
			return result;
		}
	}
}