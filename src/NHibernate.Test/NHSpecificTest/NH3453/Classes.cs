using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH3453
{
    class Direction
    {

        #region Compisite ID
        
        public virtual Int32 Id1 { get; set; }

        public virtual Int32 Id2 { get; set; }

        #endregion

        public virtual Guid GUID { get; set; }

        public override int GetHashCode()
        {
            return string.Format("{0} - {1}", Id1, Id2).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Id1 == ((Direction)obj).Id1 &&
                   Id2 == ((Direction)obj).Id2;
        }
    }

    class DirectionReferrer
    {
        public virtual Guid GUID { get; set; }
        
        public virtual Direction Direction { get; set; }
    }
}
