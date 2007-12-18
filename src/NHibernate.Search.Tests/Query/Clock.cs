using NHibernate.Search.Attributes;

namespace NHibernate.Search.Tests.Queries
{
	[Indexed]
	public class Clock
	{
		private int id;
		private string brand;
		public Clock()
		{

		}

		public Clock(int id, string brand)
		{
			this.id = id;
			this.brand = brand;
		}

		[DocumentId]
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Field(Index.Tokenized, Store=Attributes.Store.Yes)]
		public virtual string Brand
		{
			get { return brand; }
			set { brand = value; }
		}
	}
}