namespace NHibernate.Shards.Criteria
{
	/// <summary>
	/// Uniquely identifies a {@link ShardedCriteria}
	/// </summary>
	public class CriteriaId
	{
		private readonly int id;

		/// <summary>
		/// Construct the criteria Id
		/// </summary>
		/// <param name="id">the int representation of the id</param>
		public CriteriaId(int id)
		{
			this.id = id;
		}

		/// <summary>
		/// return the int representation
		/// </summary>
		public int Id
		{
			get { return id; }
		}

		public override bool Equals(object o)
		{
			if (this == o)
				return true;
			
			if (!(o is CriteriaId))
				return false;
			
			CriteriaId criteriaId = (CriteriaId) o;

			if (id != criteriaId.id)
				return false;
			
			return true;
		}

		public override int GetHashCode() 
		{
			return id;
		}
	}
}