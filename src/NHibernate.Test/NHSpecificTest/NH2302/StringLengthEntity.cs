namespace NHibernate.Test.NHSpecificTest.NH2302
{
    public class StringLengthEntity
    {
        private int mID = 0;
        public int ID
        {
            get { return mID; }
        }

        public string StringDefault { get; set; }

        public string StringFixedLength { get; set; }

        public string StringHugeLength { get; set; }

        public string StringSqlType { get; set; }

        public string Blob { get; set; }

        public string BlobLength { get; set; }

        public string BlobSqlType { get; set; }
    }
}
