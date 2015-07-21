using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.ExpressionTest.Projection
{
    public class ProjectionTestClass
    {
        private int _id;
        private double _pay;

        public ProjectionTestClass() { }
        public ProjectionTestClass(double pay)
        {
            Pay = pay;
        }

        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        public double Pay
        {
            get { return _pay; }
            set { _pay = value; }
        }
    }
}
