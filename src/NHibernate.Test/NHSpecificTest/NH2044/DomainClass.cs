

namespace NHibernate.Test.NHSpecificTest.NH2044
{
    public class DomainClass
    {
        private char symbol;
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public char Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }
    }
}