using System;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Test the ability of GetSetHelperFactory to generate code that can set
    /// a value type from a null.
	/// </summary>
    public class GetSetHelper
    {
        public enum TestEnum
        {
            One, Two
        }

        private int _id;

        private int _a = 1;
        private TimeSpan _b = TimeSpan.FromSeconds(100);
        private bool _c = true;
        private DateTime _d = new DateTime(2005, 10, 5);
        private short _e = 2;
        private byte _f = 3;
        private float _g = 4.5F;
        private double _h = 5.5;
        private decimal _i = 77;
        private string _m = "Dummy";
        private TestEnum _l = TestEnum.Two;

        #region Property

        public string M
        {
            get { return _m; }
            set { _m = value; }
        }

        public TestEnum L
        {
            get { return _l; }
            set { _l = value; }
        }

        public decimal I
        {
            get { return _i; }
            set { _i = value; }
        }

        public TimeSpan B
        {
            get { return _b; }
            set { _b = value; }
        }

        public double H
        {
            get { return _h; }
            set { _h = value; }
        }

        public float G
        {
            get { return _g; }
            set { _g = value; }
        }

        public byte F
        {
            get { return _f; }
            set { _f = value; }
        }

        public short E
        {
            get { return _e; }
            set { _e = value; }
        }

        public DateTime D
        {
            get { return _d; }
            set { _d = value; }
        }

        public bool C
        {
            get { return _c; }
            set { _c = value; }
        }

        public int A
        {
            get { return _a; }
            set { _a = value; }
        }

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        } 
        #endregion

	}
}
