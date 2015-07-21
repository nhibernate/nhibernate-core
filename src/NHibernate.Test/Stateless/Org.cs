namespace NHibernate.Test.Stateless
{
    public class Org
    {
        private int id;
        private Country country;

        public virtual int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public virtual Country Country
        {
            get
            {
                return country;
            }

            set
            {
                country = value;
            }
        }
    }
}
