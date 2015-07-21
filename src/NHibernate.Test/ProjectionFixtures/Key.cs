namespace NHibernate.Test.ProjectionFixtures
{
    public class Key
    {
        public virtual int Id { get; set; }
        public virtual int Area { get; set; }

        public virtual bool Equals(Key obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.Id == Id && obj.Area == Area;
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
                return (Id*397) ^ Area;
            }
        }
    }
}