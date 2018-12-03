namespace NHibernate.Test.NHSpecificTest.EmbIdBidirectional
{
	public class BiEmbIdRefEdEntity
	{
		public virtual EmbId Id { get; set; }
		public virtual string Data { get; set; }
		public virtual BiEmbIdRefIngEntity Referencing { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as BiEmbIdRefEdEntity;
			if (casted == null)
				return false;
			return Id.Equals(casted.Id) && Data.Equals(casted.Data);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Data.GetHashCode();
		}
	}
}
