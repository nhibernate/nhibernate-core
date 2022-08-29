using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3993
{
	public class BaseEntity
	{
		public virtual Guid Id { get; set; }
		private HierarchicalComponent _component;
		public virtual HierarchicalComponent Component
		{
			get => _component;
			set => _component = value;
		}
	}

	public class HierarchicalComponent
	{
		// Covers Parent added to Component Customizer for not visible parents
		private HierarchicalComponent _parent;
		public virtual HierarchicalComponent Parent
		{
			get => _parent;
			set => _parent = value;
		}

		// Element to cover Componenent Element Customiser
		private IDictionary<string, Element> _elements = new Dictionary<string, Element>();
		public IEnumerable<Element> Elements => _elements.Values;
	}

	public class SimpleComponent
	{
		public virtual string SimpleComponentName { get; set; }
	}

	public class Element
	{
		// To cover added Property in Componenent Element Customiser
		private string _name;
		public virtual string Name
		{
			get => _name;
			set => _name = value;
		}

		// To cover added Component Component Element Customiser
		private SimpleComponent _component;

		public virtual SimpleComponent Component
		{
			get => _component;
			set => _component = value;
		}

		// To cover added Parent in Componenent Element Customiser
		private Element _parent;
		public virtual Element Parent
		{
			get => _parent;
			set => _parent = Parent;
		}

		// Used by reflection
#pragma warning disable CS0169 // The field is never used
		private string _description;
#pragma warning restore CS0169 // The field is never used
	}
}
