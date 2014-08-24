namespace NHibernate.Test.NHSpecificTest.NH2113
{
	public class Loan
	{
        public virtual Broker Broker{ get; set;}
        public virtual Group Group { get; set; }
        public virtual string Name { get; set; }

	    public virtual bool Equals(Loan other)
	    {
	        if (ReferenceEquals(null, other)) return false;
	        if (ReferenceEquals(this, other)) return true;
	        return Equals(other.Broker, Broker) && Equals(other.Group, Group);
	    }

	    public override bool Equals(object obj)
	    {
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != typeof (Loan)) return false;
	        return Equals((Loan) obj);
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            return ((Broker != null ? Broker.Key.GetHashCode() : 0)*397) ^ (Group != null ? Group.GetHashCode() : 0);
	        }
	    }
	} 

    public class Group
    {
        public virtual int Id { get; set; }
    }
    public class Broker
    {
        public virtual Key Key { get; set; }

        public virtual  bool Equals(Broker other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Key, Key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Broker)) return false;
            return Equals((Broker) obj);
        }

        public override int GetHashCode()
        {
            return (Key != null ? Key.GetHashCode() : 0);
        }
    }
    public class Key
    {
        public int Id { get; set; }
        public int BankId { get; set; }

        public virtual bool Equals(Key other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id == Id && other.BankId == BankId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Key)) return false;
            return Equals((Key) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id*397) ^ BankId;
            }
        }
    }
}
