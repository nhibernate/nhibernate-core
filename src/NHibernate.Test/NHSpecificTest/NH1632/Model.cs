namespace NHibernate.Test.NHSpecificTest.NH1632
{
	public class Nums
	{
		public virtual int ID { get; set; }

		public virtual int NumA { get; set; }
		public virtual int NumB { get; set; }
		public virtual int Sum
		{
			get
			{
				return NumA + NumB;
			}
		}
	}

	public class Person
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}
}
