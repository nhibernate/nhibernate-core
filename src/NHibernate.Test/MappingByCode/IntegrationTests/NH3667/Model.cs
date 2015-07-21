using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH3667
{
	public class Entity
	{
		public int A { get; set; }
		public string B { get; set; }
	}

	public class Component
	{
		public int A { get; set; }
		public int B { get; set; }
	}

	public class ClassWithMapEntityElement
	{
		public int Id { get; set; }
		public IDictionary<Entity, string> Map { get; set; }
	}

	public class ClassWithMapElementComponent
	{
		public int Id { get; set; }
		public IDictionary<string, Component> Map { get; set; }
	}

	public class ClassWithMapEntityComponent
	{
		public int Id { get; set; }
		public IDictionary<Entity, Component> Map { get; set; }
	}

	public class ClassWithMapElementEntity
	{
		public int Id { get; set; }
		public IDictionary<string, Entity> Map { get; set; }
	}

	public class ClassWithMapComponentEntity
	{
		public int Id { get; set; }
		public IDictionary<Component, Entity> Map { get; set; }
	}

	public class ClassWithMapComponentElement
	{
		public int Id { get; set; }
		public IDictionary<Component, string> Map { get; set; }
	}

	public class ClassWithMapComponentComponent
	{
		public int Id { get; set; }
		public IDictionary<Component, Component> Map { get; set; }
	}

	public class ClassWithMapEntityEntity
	{
		public int Id { get; set; }
		public IDictionary<Entity, Entity> Map { get; set; }
	}

	public class ClassWithMapElementElement
	{
		public int Id { get; set; }
		public IDictionary<string, string> Map { get; set; }
	}
}
