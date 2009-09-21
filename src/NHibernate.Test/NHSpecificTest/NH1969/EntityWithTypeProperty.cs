using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1969
{
    /// <summary>
    /// Author : Stephane Verlet
    /// </summary>
    public class EntityWithTypeProperty
    {

        private int _id;
        private System.Type _typeValue;

        public EntityWithTypeProperty()
        {
            _id = 0;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public System.Type TypeValue
        {
            get { return _typeValue; }
            set { _typeValue = value; }
        }

    }
}
