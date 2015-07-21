using System;
using System.Collections.Generic;

namespace NHibernate.Tuple.Component
{
	/// <summary> Centralizes metamodel information about a component. </summary>
	public class ComponentMetamodel
	{
		private readonly string role;
		private readonly bool isKey;
		private readonly int propertySpan;
		private readonly StandardProperty[] properties;
		private readonly Dictionary<string, int> propertyIndexes;
		private readonly ComponentEntityModeToTuplizerMapping tuplizerMapping;

		public ComponentMetamodel(Mapping.Component component)
		{
			role = component.RoleName;
			isKey = component.IsKey;
			propertySpan = component.PropertySpan;
			properties = new StandardProperty[PropertySpan];
			propertyIndexes = new Dictionary<string, int>(propertySpan);
			int i = 0;
			foreach (Mapping.Property property in component.PropertyIterator)
			{
				properties[i] = PropertyFactory.BuildStandardProperty(property, false);
				propertyIndexes[property.Name] = i;
				i++;
			}
			tuplizerMapping = new ComponentEntityModeToTuplizerMapping(component);
		}

		public string Role
		{
			get { return role; }
		}

		public bool IsKey
		{
			get { return isKey; }
		}

		public int PropertySpan
		{
			get { return propertySpan; }
		}

		public StandardProperty[] Properties
		{
			get { return properties; }
		}

		public ComponentEntityModeToTuplizerMapping TuplizerMapping
		{
			get { return tuplizerMapping; }
		}

		public StandardProperty GetProperty(int index)
		{
			if (index < 0 || index >= propertySpan)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("illegal index value for component property access [request={0}, span={1}]", index, propertySpan));
			}
			return properties[index];
		}

		public int GetPropertyIndex(string propertyName)
		{
			int index;
			if (!propertyIndexes.TryGetValue(propertyName, out index))
			{
				throw new HibernateException("component does not contain such a property [" + propertyName + "]");
			}
			return index;
		}

		public StandardProperty GetProperty(string propertyName)
		{
			return properties[GetPropertyIndex(propertyName)];
		}
	}
}
