using System;
using Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1025
{
    public class Entity
    {
        public Entity()
        {
            Components = new HashedSet<Component>();
        }

        public virtual int Id { get; set; }

        public virtual ISet<Component> Components { get; set; }
    }

    public class Component : IEquatable<Component>
    {
        public Component()
        {
            
        }

        public Component(int? valueA, int? valueB, int? valueC)
        {
            ValueA = valueA;
            ValueB = valueB;
            ValueC = valueC;
        }

        public virtual Entity Parent { get; set; }

        public virtual int? ValueA { get; set; }
        public virtual int? ValueB { get; set; }
        public virtual int? ValueC { get; set; }

				public override bool Equals(object obj)
				{
					if (ReferenceEquals(null, obj))
					{
						return false;
					}
					if (ReferenceEquals(this, obj))
					{
						return true;
					}
					if (obj.GetType() != typeof (Component))
					{
						return false;
					}
					return Equals((Component) obj);
				}

    	public bool Equals(Component other)
    	{
    		if (ReferenceEquals(null, other))
    		{
    			return false;
    		}
    		if (ReferenceEquals(this, other))
    		{
    			return true;
    		}
    		return other.ValueA.Equals(ValueA) && other.ValueB.Equals(ValueB) && other.ValueC.Equals(ValueC);
    	}

    	public override int GetHashCode()
    	{
    		unchecked
    		{
    			int result = (ValueA.HasValue ? ValueA.Value : 0);
    			result = (result * 397) ^ (ValueB.HasValue ? ValueB.Value : 0);
    			result = (result * 397) ^ (ValueC.HasValue ? ValueC.Value : 0);
    			return result;
    		}
    	}
    }
}
