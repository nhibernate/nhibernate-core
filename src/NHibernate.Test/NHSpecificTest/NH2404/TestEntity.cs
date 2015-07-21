namespace NHibernate.Test.NHSpecificTest.NH2404
{
    public class TestEntity
    {
    	private string name;
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
