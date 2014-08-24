

namespace NHibernate.Test.NHSpecificTest.NH1963
{
    public class DomainClass
    {
        private byte[] byteData;
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public byte[] ByteData
        {
            get { return byteData; }
            set { byteData = value; }
        }
    }
}