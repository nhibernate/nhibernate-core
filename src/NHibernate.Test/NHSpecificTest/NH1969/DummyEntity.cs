using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1969
{
    /// <summary>
    /// Author : Stephane Verlet
    /// </summary>
    public class DummyEntity
    {

        private int _id;


        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

    }
}
