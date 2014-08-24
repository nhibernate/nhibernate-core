namespace NHibernate.Test.NHSpecificTest.NH1253
{
	public class Car
	{
		private int _Id;
		private string _Make;
		private string _Model;

		public virtual int Id
		{
			get { return _Id; }
			set { _Id = value; }
		}

		public virtual string Model
		{
			get { return _Model; }
			set { _Model = value; }
		}

		public virtual string Make
		{
			get { return _Make; }
			set { _Make = value; }
		}
	}
}