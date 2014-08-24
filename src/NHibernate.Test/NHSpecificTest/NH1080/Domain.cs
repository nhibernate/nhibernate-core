using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1080
{
    public class A
    {
        private int id;
        private string value;
        private B b1, b2;
        private C c;

        public virtual C C
        {
            get { return c; }
            set { c = value; }
        }


        public virtual B B1
        {
            get { return b1; }
            set { b1 = value; }
        }

        public virtual B B2
        {
            get { return b2; }
            set { b2 = value; }
        }


        public virtual string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }



        public virtual int ID
        {
            get { return id; }
            set { id = value; }
        }
    }


    public class B
    {
        private int id;
        private string value;

        public virtual string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public virtual int ID
        {
            get { return id; }
            set { id = value; }
        }
    }

    public class C
    {
        private int id;
        private string value;

        public virtual string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public virtual int ID
        {
            get { return id; }
            set { id = value; }
        }
    }
}
