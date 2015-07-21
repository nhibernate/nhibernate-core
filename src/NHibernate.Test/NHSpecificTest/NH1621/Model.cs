namespace NHibernate.Test.NHSpecificTest.NH1621
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
}
