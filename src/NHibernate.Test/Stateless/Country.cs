namespace NHibernate.Test.Stateless
{
    public class Country
    {
        private int id;

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
    }
}
