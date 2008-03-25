using System;
using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	internal class AuxiliaryDatabaseObjectFactory
	{
		public static IAuxiliaryDatabaseObject Create(HbmDatabaseObject databaseObjectSchema)
		{
			if (databaseObjectSchema.HasDefinition())
				return CreateCustomObject(databaseObjectSchema);

			else
				return CreateSimpleObject(databaseObjectSchema);
		}

		private static IAuxiliaryDatabaseObject CreateSimpleObject(HbmDatabaseObject databaseObjectSchema)
		{
			string createText = databaseObjectSchema.FindCreateText();
			string dropText = databaseObjectSchema.FindDropText();

			IAuxiliaryDatabaseObject simpleObject = new SimpleAuxiliaryDatabaseObject(createText, dropText);

			foreach (string dialectName in databaseObjectSchema.FindDialectScopeNames())
				simpleObject.AddDialectScope(dialectName);

			return simpleObject;
		}

		private static IAuxiliaryDatabaseObject CreateCustomObject(HbmDatabaseObject databaseObjectSchema)
		{
			HbmDefinition definitionSchema = databaseObjectSchema.FindDefinition();
			string customTypeName = definitionSchema.@class;

			try
			{
				System.Type customType = ReflectHelper.ClassForName(customTypeName);

				IAuxiliaryDatabaseObject customObject =
					(IAuxiliaryDatabaseObject) Activator.CreateInstance(customType);

				// Would prefer to change SetParameterValues to take an IDictionary<string, string>.
				Dictionary<string, string> parameters = definitionSchema.FindParameters();
				customObject.SetParameterValues(parameters);

				foreach (string dialectName in databaseObjectSchema.FindDialectScopeNames())
					customObject.AddDialectScope(dialectName);

				return customObject;
			}
			catch (TypeLoadException exception)
			{
				throw new MappingException(string.Format(
					"Could not locate custom database object class [{0}].", customTypeName), exception);
			}
			catch (Exception exception)
			{
				throw new MappingException(string.Format(
					"Could not instantiate custom database object class [{0}].", customTypeName), exception);
			}
		}
	}
}