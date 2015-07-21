namespace NHibernate.Test.NHSpecificTest.NH1584
{
	/// <summary>
	/// This class assumes that all male cats have a tabby coat pattern (which is not true).
	/// </summary>
	public class Male : Cat
	{
		private Tabby _coat;

		public virtual Tabby Coat
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