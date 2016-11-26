

using System;
using System.Collections.Generic;
namespace NHibernate.Test.NHSpecificTest.NH3898
{
    public class Employee
    {
        private Int32 id;
        public virtual Int32 Id
        {
            get { return id; }
            set { id = value; }
        }

        private String name;
        public virtual String Name
        {
            get { return name; }
            set { name = value; }
        }

        private Int32 promotionCount;

        public virtual Int32 PromotionCount
        {
            get { return promotionCount; }
            set { promotionCount = value; }
        }
    }
}