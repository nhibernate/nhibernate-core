namespace NHibernate.Test.NHSpecificTest.NH952
{
	public class Item
	{
		private int uniqueId;

		public virtual int UniqueId
		{
			get { return uniqueId; }
			set { uniqueId = value; }
		}
	}
}