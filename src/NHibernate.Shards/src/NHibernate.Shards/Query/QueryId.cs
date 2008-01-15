namespace NHibernate.Shards.Query
{
	public class QueryId
	{
		private readonly int id;

		public QueryId(int id)
		{
			this.id = id;
		}

		public int Id
		{
			get { return id; }
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (!(o is QueryId))
			{
				return false;
			}

			QueryId queryId = (QueryId) o;

			if (id != queryId.id)
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			return id;
		}
	}
}