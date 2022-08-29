using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3793
{
	[Serializable]
	public class ParentEntityKey : IEquatable<ParentEntityKey>
	{
		public ParentEntityKey()
		{
		}

		public virtual int Number { get; set; }
		public virtual string StringKey { get; set; }

		public bool Equals(ParentEntityKey other)
		{
			if (other == null)
			{
				return false;
			}
			return Number.Equals(other.Number) &&
				(StringKey ?? String.Empty).Equals(other.StringKey ?? String.Empty);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ParentEntityKey);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Number * 19) + (StringKey ?? String.Empty).GetHashCode();
			}
		}
	}

	[Serializable]
	public class ParentEntity
	{
		public ParentEntity()
		{
		}

		public virtual ParentEntityKey ParentEntityKey { get; set; }
		public virtual int Number { get; set; }
		public virtual string StringKey { get; set; }
		public virtual string OtherInformationOne { get; set; }
		public virtual string OtherInformationTwo { get; set; }
		public virtual ISet<ChildEntity> Children { get; set; }
	}

	[Serializable]
	public class ChildEntityKey : IEquatable<ChildEntityKey>
	{
		public ChildEntityKey()
		{
		}

		public virtual ParentEntity ParentEntity { get; set; }
		public virtual string ChildIdentifier { get; set; }

		public virtual bool Equals(ChildEntityKey other)
		{
			if (other == null)
			{
				return false;
			}
			if (ParentEntity == null && other.ParentEntity == null)
			{
				return ChildIdentifier == other.ChildIdentifier;
			}
			if (ParentEntity == null || other.ParentEntity == null)
			{
				return false;
			}
			if (ParentEntity.ParentEntityKey == null && other.ParentEntity.ParentEntityKey == null)
			{
				return ChildIdentifier == other.ChildIdentifier;
			}
			if (ParentEntity.ParentEntityKey == null || other.ParentEntity.ParentEntityKey == null)
			{
				return false;
			}
			return ParentEntity.ParentEntityKey.Equals(other.ParentEntity.ParentEntityKey) &&
				ChildIdentifier == other.ChildIdentifier;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			var other = obj as ChildEntityKey;
			if (other != null)
			{
				return Equals(other);
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			if (ChildIdentifier == null)
			{
				return Int32.MinValue;
			}
			return ChildIdentifier.GetHashCode();
		}
	}

	[Serializable]
	public class ChildEntity
	{
		public ChildEntity()
		{
		}

		public virtual ChildEntityKey ChildEntityKey { get; set; }
		public virtual string ChildIdentifier { get; set; }
		public virtual string SomeOtherProperty { get; set; }
	}

	[Serializable]
	public class ObjectWithBothEntityNames
	{
		public ObjectWithBothEntityNames()
		{
		}

		public virtual int ObjectWithBothEntityNamesId { get; set; }

		public virtual ISet<ParentEntity> ParentEntityOneSet { get; set; }
		public virtual ISet<ParentEntity> ParentEntityTwoSet { get; set; }
	}
}
