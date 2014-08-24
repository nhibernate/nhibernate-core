using System;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary> 
	/// A value which is "typed" by reference to some other value 
	/// (for example, a foreign key is typed by the referenced primary key). 
	/// </summary>
	[Serializable]
	public class DependantValue : SimpleValue
	{
		private readonly IKeyValue wrappedValue;
		private bool isNullable = true;
		private bool isUpdateable = true;

		public DependantValue(Table table, IKeyValue prototype)
			: base(table)
		{
			wrappedValue = prototype;
		}

		public override IType Type
		{
			get { return wrappedValue.Type; }
		}

		public void SetTypeUsingReflection(string className, string propertyName) { }

		public override bool IsNullable
		{
			get { return isNullable; }
		}

		public void SetNullable(bool nullable)
		{
			isNullable = nullable;
		}

		public override bool IsUpdateable
		{
			get { return isUpdateable; }
		}

		public virtual void SetUpdateable(bool updateable)
		{
			isUpdateable = updateable;
		}
	}
}