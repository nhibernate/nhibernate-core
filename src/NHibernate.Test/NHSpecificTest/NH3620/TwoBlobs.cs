using System;

namespace NHibernate.Test.NHSpecificTest.NH3620 {
    public class TwoBlobs {
        public virtual int Id { get; set; }
        public virtual byte[] Blob1 { get; set; }
        public virtual byte[] Blob2 { get; set; }
        public virtual DateTime TheDate { get; set; }

    }
}
