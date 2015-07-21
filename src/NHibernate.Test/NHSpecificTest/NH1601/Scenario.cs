using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1601
{

    public class Scenario
    {

        protected int _id;


        public virtual int id
        {
            get { return _id; }
            set { _id = value; }
        }


        public Scenario( )
        {
        }

    }

}
