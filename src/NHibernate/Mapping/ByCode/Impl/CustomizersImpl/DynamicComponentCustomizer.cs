using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class DynamicComponentCustomizer<TComponent> : PropertyContainerCustomizer<TComponent>, IDynamicComponentMapper<TComponent>
	{
		public DynamicComponentCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ICustomizersHolder customizersHolder, PropertyPath propertyPath)
			: base(explicitDeclarationsHolder, customizersHolder, propertyPath)
		{
			if (propertyPath == null)
			{
				throw new ArgumentNullException("propertyPath");
			}
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			explicitDeclarationsHolder.AddAsDynamicComponent(propertyPath.LocalMember, typeof(TComponent));
		}

		public DynamicComponentCustomizer(IDictionary<string, System.Type> template, IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ICustomizersHolder customizersHolder, PropertyPath propertyPath)
			: base(explicitDeclarationsHolder, customizersHolder, propertyPath)
		{
			if (propertyPath == null)
			{
				throw new ArgumentNullException("propertyPath");
			}
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}

			var componentType = this.CreateType(template);

			explicitDeclarationsHolder.AddAsDynamicComponent(propertyPath.LocalMember, componentType);
		}

		private System.Type CreateType(IDictionary<string, System.Type> properties)
		{
			var assemblyName = new AssemblyName("MyDynamicAssembly");
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
			var typeBuilder = moduleBuilder.DefineType("MyDynamicType", TypeAttributes.Public | TypeAttributes.Serializable);

			foreach (var property in properties)
			{
				var propertyBuilder = typeBuilder.DefineProperty(property.Key, PropertyAttributes.HasDefault, property.Value, null);
				var getMethodBuilder = typeBuilder.DefineMethod("get_" + property.Key, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, property.Value, System.Type.EmptyTypes);
				var setMethodBuilder = typeBuilder.DefineMethod("set_" + property.Key, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(void), new System.Type[] { property.Value });

				var getILGenerator = getMethodBuilder.GetILGenerator();
				getILGenerator.Emit(OpCodes.Ldarg_0);
				getILGenerator.Emit(OpCodes.Nop);
				getILGenerator.Emit(OpCodes.Ret);

				var setILGenerator = setMethodBuilder.GetILGenerator();
				setILGenerator.Emit(OpCodes.Ldarg_0);
				setILGenerator.Emit(OpCodes.Ldarg_1);
				setILGenerator.Emit(OpCodes.Ret);

				propertyBuilder.SetGetMethod(getMethodBuilder);
				propertyBuilder.SetSetMethod(setMethodBuilder);
			}

			var type = typeBuilder.CreateType();

			return type;
		}

		#region IDynamicComponentMapper<TComponent> Members

		public void Access(Accessor accessor)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IDynamicComponentAttributesMapper m) => m.Access(accessor));
		}

		public void Access(System.Type accessorType)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IDynamicComponentAttributesMapper m) => m.Access(accessorType));
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IDynamicComponentAttributesMapper m) => m.OptimisticLock(takeInConsiderationForOptimisticLock));
		}

		public void Update(bool consideredInUpdateQuery)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IDynamicComponentAttributesMapper m) => m.Update(consideredInUpdateQuery));
		}

		public void Insert(bool consideredInInsertQuery)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IDynamicComponentAttributesMapper m) => m.Insert(consideredInInsertQuery));
		}

		public void Unique(bool unique)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IDynamicComponentAttributesMapper m) => m.Unique(unique));
		}

		#endregion
	}
}