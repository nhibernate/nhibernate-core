using System;
using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class TypeBinder: Binder
	{
		private readonly SimpleValue value;

		public TypeBinder(SimpleValue value, Mappings mappings)
			: base(mappings)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.value = value;
		}

		public void Bind(string typeName)
		{
			if(string.IsNullOrEmpty(typeName))
			{
				return;
			}
			Bind(new HbmType { name= typeName });
		}

		public void Bind(HbmType typeMapping)
		{
			if (typeMapping == null)
			{
				// will find the type through reflection
				return;
			}
			string originalTypeName = typeMapping.name;
			if(originalTypeName == null)
			{
				return;
			}
			var parameters = new Dictionary<string, string>();
			if(typeMapping.param != null)
			{
				System.Array.ForEach(typeMapping.param, p => parameters[p.name] = p.Text.LinesToString());
			}

			BindThroughTypeDefOrType(originalTypeName, parameters);
		}

		private void BindThroughTypeDefOrType(string originalTypeName, IDictionary<string, string> parameters)
		{
			string typeName = null;
			TypeDef typeDef = Mappings.GetTypeDef(originalTypeName);
			if (typeDef != null)
			{
				typeName = FullQualifiedClassName(typeDef.TypeClass, Mappings);
				// parameters on the property mapping should
				// override parameters in the typedef
				parameters = new Dictionary<string, string>(typeDef.Parameters).AddOrOverride(parameters);
			}
			else if (NeedQualifiedClassName(originalTypeName)
			         && (typeDef = Mappings.GetTypeDef(FullQualifiedClassName(originalTypeName, Mappings))) != null)
			{
				// NH: allow className completing it with assembly+namespace of the mapping doc.
				typeName = typeDef.TypeClass;
				// parameters on the property mapping should
				// override parameters in the typedef
				parameters = new Dictionary<string, string>(typeDef.Parameters).AddOrOverride(parameters);
			}

			if (parameters.Count != 0)
			{
				value.TypeParameters = parameters;
			}
			var typeToMap = typeName ?? originalTypeName;
			if (NeedQualifiedClassName(typeToMap))
			{
				typeToMap = FullQualifiedClassName(typeToMap, Mappings);
			}
			value.TypeName = typeToMap;
		}
	}
}