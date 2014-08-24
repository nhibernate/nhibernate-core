using System;

namespace NHibernate.Test.NHSpecificTest.NH965
{
    public class CompositeElement
    {
        private string prop;

        public virtual string Prop
        {
            get { return prop; }
            set { prop = value; }
        }
    }
}
