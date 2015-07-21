using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2112
{
    [Serializable]
    public abstract class BaseEntity<TEntity, TKey> : IEquatable<TEntity>
        where TEntity : BaseEntity<TEntity, TKey>
        where TKey : struct
    {
        public virtual TKey? Id { get; set; }

        public virtual bool Equals(TEntity other)
        {
            if (other == null) return false;
            if (object.ReferenceEquals(this, other)) return true;
            if (Id != null && other.Id == null) return false;
            if (Id == null && other.Id != null) return false;
            if (Id == null && other.Id == null)
                return object.ReferenceEquals(this, other);
            else
                return Id.Value.Equals(other.Id.Value);
        }
        public override bool Equals(object obj)
        {

            return this.Equals(obj as TEntity);
        }
        private int? mHashCode;
        public override int GetHashCode()
        {
            if (mHashCode == null) // le hashcode retourné doit être le meme tout le long de la vie de l'objet
            {
                if (this.Id != null)
                    mHashCode = this.Id.GetHashCode();
                else mHashCode = base.GetHashCode();
            }
            return mHashCode.Value;
        }
        public virtual bool IsNew
        {
            get
            {
                return (Id == null);
            }
        }
    }

    public class A : BaseEntity<A, int>
    {
        public A()
        {
            Map = new Dictionary<B, string>();
        }

    	private int version;
    	public virtual int Version
    	{
    		get { return version; }
    		set { version = value; }
    	}

    	public virtual string Name { get; set; }
        public virtual IDictionary<B, string> Map { get; set; }

    }

    public class B : BaseEntity<B, int>
    {
        public virtual string Name { get; set; }
    }
}
