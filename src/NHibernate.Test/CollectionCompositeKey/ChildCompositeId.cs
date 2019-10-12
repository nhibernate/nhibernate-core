
namespace NHibernate.Test.CollectionCompositeKey
{
	public class ChildCompositeId
	{
		public virtual int Number { get; set; }

		public virtual string ParentCode { get; set; }

		public virtual int ParentNumber { get; set; }

		public override bool Equals(object obj)
		{
			if (!(obj is ChildCompositeId key))
			{
				return false;
			}

			return Number.Equals(key.Number) && ParentCode.Equals(key.ParentCode) && ParentNumber.Equals(key.ParentNumber);
		}

		public override int GetHashCode()
		{
			return Number.GetHashCode() ^ ParentCode.GetHashCode() ^ ParentNumber.GetHashCode();
		}
	}
}
