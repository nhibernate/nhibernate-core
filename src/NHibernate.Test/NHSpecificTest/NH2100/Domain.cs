using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.Test.NHSpecificTest.NH2100
{
	public abstract class DomainObject
	{
		public virtual int ID { get; set; }
		public virtual int EntityVersion { get; set; }
	}

	public class Class1 : DomainObject
	{
		private readonly IList<Class2> _class2List = new List<Class2>();

		public virtual void AddClass2(Class2 toAdd)
		{
			if (false == _class2List.Contains(toAdd))
			{
				_class2List.Add(toAdd);
				toAdd.AddClass1(this);
			}
		}

		public virtual ReadOnlyCollection<Class2> Class2List
		{
			get { return new ReadOnlyCollection<Class2>(_class2List); }
		}
	}

	public class Class1DTO : DTO
	{
		public Class2DTO[] Class2Ary { get; set; }
	}

	/// <summary>
	/// RG
	/// </summary>
	public class Class2 : DomainObject
	{
		private readonly IList<Class1> _class1List = new List<Class1>();

		public virtual void AddClass1(Class1 toAdd)
		{
			if (false == _class1List.Contains(toAdd))
			{
				_class1List.Add(toAdd);
				toAdd.AddClass2(this);
			}
		}
	}

	public class Class2DTO : DTO { }

	public abstract class DTO
	{
		public int ID;
		public int EntityVersion;
	}
}