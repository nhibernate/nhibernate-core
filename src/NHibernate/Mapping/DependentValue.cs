using NHibernate.Type;
using System;

namespace NHibernate.Mapping
{
	public class DependentValue : SimpleValue
	{
		private IKeyValue wrappedValue;
		private bool isNullable;
		private bool isUpdateable;

		public DependentValue(Table table, IKeyValue prototype)
			: base(table)
		{
			this.wrappedValue = prototype;
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

		// SimpleValue does not have a setter for IsNullable. We cannot add
		// a setter here with the "override" keyword on the IsNullable property.
		// Therefore, we need to create this method to set IsNullable.
		// This is a limitation on .NET 2.0 and before.
		public void SetNullable(bool nullable)
		{
			isNullable = nullable;
		}

		public virtual bool IsUpdateable
		{
			get { return isUpdateable; }
			set { isUpdateable = value; }
		}

	}
}