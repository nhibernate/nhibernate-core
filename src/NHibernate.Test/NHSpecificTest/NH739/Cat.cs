using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH739 {
    public class Cat {
        private int id;
        private IList children = new ArrayList();
        private Cat mother;

        public int Id {
            get { return id; }
            set { id = value; }
        }

        public Cat Mother {
            get { return mother; }
            set { mother = value; }
        }

        public IList Children {
            get { return children; }
            set { children = value; }
        }
    }
}