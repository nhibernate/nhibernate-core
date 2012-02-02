namespace NHibernate.Test.NHSpecificTest.NH2941
{
    public class Child
    {
        private string name;
        private int id;
        private int version;
        private Parent parent;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int Version
        {
            get { return version; }
            set { version = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Parent Parent
        {
            get { return parent; }
            set { parent = value; }
        }
    }
}