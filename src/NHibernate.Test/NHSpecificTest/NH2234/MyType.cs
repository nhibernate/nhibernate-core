using System.Collections.Generic;
using System.Data.Common;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.NH2234
{
  public class MyUsertype
  {
    public MyUsertype(int id, string value)
    {
      Id = id;
      Value = value;
    }

    public int Id { get; set; }
    public string Value { get; set; }

    public override int GetHashCode()
    {
      return Id.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      var mut = obj as MyUsertype;
      return mut != null && mut.Id == Id;
    }

    public static bool operator ==(MyUsertype left, MyUsertype right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(MyUsertype left, MyUsertype right)
    {
      return !Equals(left, right);
    }
  }

  public static class MyUserTypes
  {
    public static readonly List<MyUsertype> _values = new List<MyUsertype>()
                                                        {new MyUsertype(1, "Value 1"), new MyUsertype(2, "Value 2")};


    public static MyUsertype Value1
    {
      get { return _values[0]; }
    }

    public static MyUsertype Value2
    {
      get { return _values[1]; }
    }

    public static MyUsertype Find(int id)
    {
      return _values.Find(item => item.Id == id);
    }
  }

	public class SimpleCustomType : IUserType
	{
		private static readonly SqlType[] ReturnSqlTypes = { SqlTypeFactory.Int32 };


		#region IUserType Members

		public new bool Equals(object x, object y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(object x)
		{
			return (x == null) ? 0 : x.GetHashCode();
		}

		public SqlType[] SqlTypes
		{
			get { return ReturnSqlTypes; }
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public void NullSafeSet(DbCommand cmd, object value, int index)
		{
			if (value == null)
			  NHibernateUtil.Int32.NullSafeSet(cmd, null, index, null);
			else
        NHibernateUtil.Int32.NullSafeSet(cmd, ((MyUsertype)value).Id, index, null);
		}

		public System.Type ReturnedType
		{
			get { return typeof(MyUsertype); }
		}

		public object NullSafeGet(DbDataReader rs, string[] names, object owner)
		{
			int value = (int)NHibernateUtil.Int32.NullSafeGet(rs, names[0], null, owner);
		  return MyUserTypes.Find(value);
		}

		public bool IsMutable
		{
			get { return false; }
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public object Assemble(object cached, object owner)
		{
			return cached;
		}

		public object Disassemble(object value)
		{
			return value;
		}

		#endregion
	}

}
