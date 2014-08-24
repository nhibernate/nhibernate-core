namespace NHibernate.Test.Unionsubclass
{
	public class Human: Being
	{
		private char sex;

		public virtual char Sex
		{
			get { return sex; }
			set { sex = value; }
		}

		public override string Species
		{
			get { return "human"; }
		}
	}
}