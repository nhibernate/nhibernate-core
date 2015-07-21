namespace NHibernate.Test.NHSpecificTest.NH1869
{
	public abstract class Entity<T, TKey> where T : Entity<T, TKey>
	{
		private int? m_oldHashCode;

		public override bool Equals(object obj)
		{
			T other = obj as T;

			if (other == null)
			{
				return false;
			}

			bool otherIsTransient = Equals(other.Id, default(TKey));
			bool thisIsTransient = Equals(Id, default(TKey));

			if (otherIsTransient && thisIsTransient)
			{
				return ReferenceEquals(other, this);
			}

			return other.Id.Equals(Id);
		}

		public override int GetHashCode()
		{
			if (m_oldHashCode.HasValue)
			{
				return m_oldHashCode.Value;
			}
			bool thisIsTransient = Equals(Id, default(TKey));

			if (thisIsTransient)
			{
				m_oldHashCode = base.GetHashCode();
				return m_oldHashCode.Value;
			}

			return Id.GetHashCode();
		}

		public abstract TKey Id { get; set; }

		public static bool operator ==(Entity<T, TKey> x, Entity<T, TKey> y)
		{
			return Equals(x, y);
		}

		public static bool operator !=(Entity<T, TKey> x, Entity<T, TKey> y)
		{
			return !(x == y);
		}
	}

	public class Keyword : Entity<Keyword, int>
	{
		public override int Id { get; set; }
	}

	public class NodeKeyword
	{
		public virtual int NodeId { get; set; }
		public virtual Keyword Keyword { get; set; }

		public override bool Equals(object obj)
		{
			NodeKeyword other = (NodeKeyword)obj;
			return NodeId == other.NodeId && Keyword == other.Keyword;
		}

		public override int GetHashCode()
		{
			return NodeId.GetHashCode() + Keyword.GetHashCode();
		}
	}
}