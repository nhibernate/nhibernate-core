namespace NHibernate.Test.NHSpecificTest.NH1584
{
	/// <summary>
	/// This class assumes that all female cats have a calico coat (which is not actually true).
	/// </summary>
	public class Female : Cat
	{
		private Calico _coat;

		public virtual Calico Coat
		{
			get { return _coat; }
			set
			{
				if (value != null)
				{
					_coat = value;
					_coat.Cat = this;
				}
			}
		}
	}
}