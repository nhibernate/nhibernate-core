using System;

namespace NHibernate.Test.NHSpecificTest.NH958
{
    public class Hobby
    {
        private int _id;
        private string _name = null;
        private Person _person = null;

        public Hobby()
        {
        }

        public Hobby(string name)
            : this()
        {
            _name = name;
        }

        public virtual int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual Person Person
        {
            get { return _person; }
            set { _person = value; }
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            else if ((obj == null) || (obj.GetType() != this.GetType()))
            {
                return false;
            }
            else
            {
                Hobby cast = this as Hobby;

                return _id == cast.Id;
            }
        }
    }
}
