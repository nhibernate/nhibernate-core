using System;

namespace NHibernate.Test.Hql.Ast
{
	public class KeyManyToOneEntity
	{
		private Id id;
		private string name;

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public class Id
		{
			private KeyManyToOneKeyEntity key1;
			private string key2;
			private int? requestedHash;

			protected Id() {}

			public Id(KeyManyToOneKeyEntity key1, string key2)
			{
				if (key1 == null)
				{
					throw new ArgumentNullException("key1");
				}
				if (key2 == null)
				{
					throw new ArgumentNullException("key2");
				}

				this.key1 = key1;
				this.key2 = key2;

			}

			public virtual KeyManyToOneKeyEntity Key1
			{
				get { return key1; }
				set { key1 = value; }
			}

			public virtual string Key2
			{
				get { return key2; }
				set { key2 = value; }
			}

			public override bool Equals(object obj)
			{
				return Equals(obj as Id);
			}

			public bool Equals(Id other)
			{
				if (ReferenceEquals(null, other))
				{
					return false;
				}
				if (ReferenceEquals(this, other))
				{
					return true;
				}
				return Equals(other.key1, key1) && Equals(other.key2, key2);
			}

			public override int GetHashCode()
			{
				if (!requestedHash.HasValue)
				{
					unchecked
					{
						requestedHash = ((key1 != null ? key1.GetHashCode() : 0) * 397) ^ (key2 != null ? key2.GetHashCode() : 0);
					}
				}
				return requestedHash.Value;
			}
		}
	}
}