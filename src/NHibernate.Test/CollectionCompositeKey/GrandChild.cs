
namespace NHibernate.Test.CollectionCompositeKey
{
	public class GrandChild
	{
		protected GrandChild()
		{
		}

		public GrandChild(int number, string name, Parent grandParent)
		{
			Number = number;
			GrandParent = grandParent;
			Name = name;
		}

		public virtual int Number { get; set; }

		public virtual Parent GrandParent { get; set; }

		public virtual string Name { get; set; }

		public override bool Equals(object obj)
		{
			if (!(obj is GrandChild key))
			{
				return false;
			}

			return Number.Equals(key.Number) && GrandParent.Equals(key.GrandParent);
		}

		public override int GetHashCode()
		{
			return Number.GetHashCode() ^ GrandParent.GetHashCode();
		}
	}
}
