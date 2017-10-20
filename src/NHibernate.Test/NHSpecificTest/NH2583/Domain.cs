using System;

namespace NHibernate.Test.NHSpecificTest.NH2583
{
    public partial class MyRef1
    {
        private static int _idCt = 1000;
        private int _id;

        public MyRef1()
        {
            _id = ++_idCt;
        }

        public virtual int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual int? I1 { get; set; }
        public virtual int I2 { get; set; }
        public virtual int I3 { get; set; }

        public virtual MyRef2 BO2 { get; set; }
        public virtual MyRef3 BO3 { get; set; }

        public virtual MyRef2 GetOrCreateBO2(ISession s)
        {
            if (BO2 == null)
            {
                BO2 = new MyRef2();
                s.Save(BO2);
            }
            return BO2;
        }

        public virtual MyRef3 GetOrCreateBO3(ISession s)
        {
            if (BO3 == null)
            {
                BO3 = new MyRef3();
                s.Save(BO3);
            }
            return BO3;
        }
    }

    public class MyRef2
    {
        private static int _idCt = 1000;
        private int _id;

        public MyRef2()
        {
            _id = ++_idCt;
        }

        public virtual int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual int? J1 { get; set; }
        public virtual int J2 { get; set; }
        public virtual int J3 { get; set; }
    }

    public class MyRef3
    {
        private static int _idCt = 3000;
        private int _id;

        public MyRef3()
        {
            _id = ++_idCt;
        }

        public virtual int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual int L1 { get; set; }
    }

    public enum Ignore { Ignore }
    public enum TK { ValueNull, Zero, One }
    public enum TBO1_I { Null, ValueNull, Zero, One }
    public enum TBO2_J { Null, ValueNull, Zero, One }
    public enum TBO1_BO2_J { Null, BO1, ValueNull, Zero, One }
    public enum TBO1_BO3_L { Null, BO1, ValueNull, Zero, One }

    public partial class MyBO
    {
        private static int _idCt = 0;
        private int _id;

        public MyBO()
        {
            _id = ++_idCt;
        }

        public virtual int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual string Name { get; set; }
        public virtual MyBO LeftSon { get; set; }
        public virtual MyBO RightSon { get; set; }
        public virtual MyRef1 BO1 { get; set; }
        public virtual MyRef1 OtherBO1 { get; set; }
        public virtual MyRef2 BO2 { get; set; }
        public virtual int? K1 { get; set; }
        public virtual int K2 { get; set; }
        public virtual int K3 { get; set; }

        private MyRef1 GetOrCreateBO1(ISession s)
        {
            if (BO1 == null)
            {
                BO1 = new MyRef1();
                s.Save(BO1);
            }
            return BO1;
        }

        private MyRef2 GetOrCreateBO2(ISession s)
        {
            if (BO2 == null)
            {
                BO2 = new MyRef2();
                s.Save(BO2);
            }
            return BO2;
        }

        public static void SetK1(MyBO bo, ISession s, TK value)
        {
            switch (value)
            {
                case TK.ValueNull:
                    bo.K1 = null;
                    break;
                case TK.Zero:
                    bo.K1 = 0;
                    break;
                case TK.One:
                    bo.K1 = 1;
                    break;
                default:
                    throw new Exception("Value " + value + " not handled in code");
            }
        }

        public static void SetK2(MyBO bo, ISession s, TK value)
        {
            bo.K2 = value == TK.One ? 1 : 0;
        }

        public static void SetK3(MyBO bo, ISession s, TK value)
        {
            bo.K3 = value == TK.One ? 1 : 0;
        }

        private static void SetBO1_I(MyBO bo, ISession s, TBO1_I value, Action<MyRef1, int?> set)
        {
            switch (value)
            {
                case TBO1_I.Null:
                    bo.BO1 = null;
                    break;
                case TBO1_I.ValueNull:
                    set(bo.GetOrCreateBO1(s), null);
                    break;
                case TBO1_I.Zero:
                    set(bo.GetOrCreateBO1(s), 0);
                    break;
                case TBO1_I.One:
                    set(bo.GetOrCreateBO1(s), 1);
                    break;
                default:
                    throw new Exception("Value " + value + " not handled in code");
            }
        }

        public static void SetBO1_I1(MyBO bo, ISession s, TBO1_I value)
        {
            SetBO1_I(bo, s, value, (b, i) => b.I1 = i);
        }

        public static void SetBO1_I2(MyBO bo, ISession s, TBO1_I value)
        {
            SetBO1_I(bo, s, value, (b, i) => b.I2 = i ?? 0);
        }

        public static void SetBO1_I3(MyBO bo, ISession s, TBO1_I value)
        {
            SetBO1_I(bo, s, value, (b, i) => b.I3 = i ?? 0);
        }

        private static void SetBO2_J(MyBO bo, ISession s, TBO2_J value, Action<MyRef2, int?> set)
        {
            switch (value)
            {
                case TBO2_J.Null:
                    bo.BO2 = null;
                    break;
                case TBO2_J.ValueNull:
                    set(bo.GetOrCreateBO2(s), null);
                    break;
                case TBO2_J.Zero:
                    set(bo.GetOrCreateBO2(s), 0);
                    break;
                case TBO2_J.One:
                    set(bo.GetOrCreateBO2(s), 1);
                    break;
                default:
                    throw new Exception("Value " + value + " not handled in code");
            }
        }

        public static void SetBO2_J1(MyBO bo, ISession s, TBO2_J value)
        {
            SetBO2_J(bo, s, value, (b, i) => b.J1 = i);
        }

        public static void SetBO2_J2(MyBO bo, ISession s, TBO2_J value)
        {
            SetBO2_J(bo, s, value, (b, i) => b.J2 = i ?? 0);
        }

        public static void SetBO2_J3(MyBO bo, ISession s, TBO2_J value)
        {
            SetBO2_J(bo, s, value, (b, i) => b.J3 = i ?? 0);
        }

        private static void SetBO1_BO2_J(MyBO bo, ISession s, TBO1_BO2_J value, Action<MyRef2, int?> set)
        {
            switch (value)
            {
                case TBO1_BO2_J.Null:
                    bo.BO1 = null;
                    break;
                case TBO1_BO2_J.BO1:
                    bo.GetOrCreateBO1(s).BO2 = null;
                    break;
                case TBO1_BO2_J.ValueNull:
                    set(bo.GetOrCreateBO1(s).GetOrCreateBO2(s), null);
                    break;
                case TBO1_BO2_J.Zero:
                    set(bo.GetOrCreateBO1(s).GetOrCreateBO2(s), 0);
                    break;
                case TBO1_BO2_J.One:
                    set(bo.GetOrCreateBO1(s).GetOrCreateBO2(s), 1);
                    break;
                default:
                    throw new Exception("Value " + value + " not handled in code");
            }
        }

        public static void SetBO1_BO2_J1(MyBO bo, ISession s, TBO1_BO2_J value)
        {
            SetBO1_BO2_J(bo, s, value, (b, i) => b.J1 = i);
        }

        public static void Set_BO1_BO2_J2(MyBO bo, ISession s, TBO1_BO2_J value)
        {
            SetBO1_BO2_J(bo, s, value, (b, i) => b.J2 = i ?? 0);
        }

        public static void SetBO1_BO3_L1(MyBO bo, ISession s, TBO1_BO3_L value)
        {
            switch (value)
            {
                case TBO1_BO3_L.Null:
                    bo.BO1 = null;
                    break;
                case TBO1_BO3_L.BO1:
                    bo.GetOrCreateBO1(s).BO3 = null;
                    break;
                case TBO1_BO3_L.ValueNull:
                    bo.GetOrCreateBO1(s).GetOrCreateBO3(s).L1 = 0; // L1 is int, not int?
                    break;
                case TBO1_BO3_L.Zero:
                    bo.GetOrCreateBO1(s).GetOrCreateBO3(s).L1 = 0;
                    break;
                case TBO1_BO3_L.One:
                    bo.GetOrCreateBO1(s).GetOrCreateBO3(s).L1 = 1;
                    break;
                default:
                    throw new Exception("Value " + value + " not handled in code");
            }
        }
    }
}
