using System;

namespace NHibernate.Test.NHSpecificTest.NH372
{
	public class BaseParent
	{
#pragma warning disable 649
		private int _Id;
#pragma warning restore 649

		public virtual int Id
		{
			get { return _Id; }
		}

		private string _Name;

		public virtual string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		private Component _Component = new Component();

		public virtual Component Component
		{
			get { return _Component; }
			set { _Component = value; }
		}
	}

	public class Parent : BaseParent
	{
	}

	public class DynamicParent : BaseParent
	{
	}

	public class Component
	{
		private int _NormalField;

		public virtual int NormalField
		{
			get { return _NormalField; }
			set { _NormalField = value; }
		}

		private int _FieldNotInserted;

		public virtual int FieldNotInserted
		{
			get { return _FieldNotInserted; }
			set { _FieldNotInserted = value; }
		}

		private int _FieldNotUpdated;

		public virtual int FieldNotUpdated
		{
			get { return _FieldNotUpdated; }
			set { _FieldNotUpdated = value; }
		}

		private SubComponent _SubComponent = new SubComponent();

		public virtual SubComponent SubComponent
		{
			get { return _SubComponent; }
			set { _SubComponent = value; }
		}
	}

	public class SubComponent
	{
		private int _NormalField;

		public virtual int NormalField
		{
			get { return _NormalField; }
			set { _NormalField = value; }
		}

		private int _FieldNotInserted;

		public virtual int FieldNotInserted
		{
			get { return _FieldNotInserted; }
			set { _FieldNotInserted = value; }
		}

		private int _FieldNotUpdated;

		public virtual int FieldNotUpdated
		{
			get { return _FieldNotUpdated; }
			set { _FieldNotUpdated = value; }
		}
	}
}