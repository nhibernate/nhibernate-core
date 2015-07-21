using System;

namespace NHibernate.Test.NHSpecificTest.NH958
{
    public class Female : Person
    {
        public Female()
        {
        }

        public Female(string name)
            : base (name) {}
    }
}
