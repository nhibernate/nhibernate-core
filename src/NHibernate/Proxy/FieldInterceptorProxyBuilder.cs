using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using NHibernate.Intercept;
using NHibernate.Util;

namespace NHibernate.Proxy
{
	internal static class FieldInterceptorProxyBuilder
	{
		private const MethodAttributes constructorAttributes =
			MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

		private static readonly System.Type FieldInterceptorAccessorType = typeof(IFieldInterceptorAccessor);
		private static readonly PropertyInfo AccessorTypeFieldInterceptorProperty =
			FieldInterceptorAccessorType.GetProperty(nameof(IFieldInterceptorAccessor.FieldInterceptor));
		private static readonly System.Type FieldInterceptorType = typeof(IFieldInterceptor);
		private static readonly MethodInfo FieldInterceptorInterceptMethod = FieldInterceptorType.GetMethod(nameof(IFieldInterceptor.Intercept));
		private static readonly MethodInfo FieldInterceptorMarkDirtyMethod = FieldInterceptorType.GetMethod(nameof(IFieldInterceptor.MarkDirty));
		private static readonly System.Type AbstractFieldInterceptorType = typeof(AbstractFieldInterceptor);
		private static readonly FieldInfo AbstractFieldInterceptorInvokeImplementationField =
			AbstractFieldInterceptorType.GetField(nameof(AbstractFieldInterceptor.InvokeImplementation));

		private static readonly System.Type FieldInterceptorObjectReferenceType = typeof(FieldInterceptorObjectReference);
		private static readonly ConstructorInfo FieldInterceptorObjectReferenceConstructor =
			FieldInterceptorObjectReferenceType.GetConstructor(new[] { typeof(NHibernateProxyFactoryInfo), typeof(IFieldInterceptor) });
		private static readonly MethodInfo FieldInterceptorObjectReferenceGetBaseDataMethod =
			FieldInterceptorObjectReferenceType.GetMethod(nameof(FieldInterceptorObjectReference.GetBaseData));
		private static readonly MethodInfo FieldInterceptorObjectReferenceSetNoAdditionalDataMethod =
			FieldInterceptorObjectReferenceType.GetMethod(nameof(FieldInterceptorObjectReference.SetNoAdditionalData));

		private static readonly ConstructorInfo InvalidOperationWithMessageConstructor =
			typeof(InvalidOperationException).GetConstructor(new[] { typeof(string) });

		public static TypeInfo CreateProxyType(System.Type baseType)
		{
			// Avoid having a suffix ending with "Proxy", for disambiguation with INHibernateProxy proxies
			var typeName = $"{baseType.Name}ProxyForFieldInterceptor";
			var assemblyName = $"{typeName}Assembly";
			var moduleName = $"{typeName}Module";

			var name = new AssemblyName(assemblyName);

			var assemblyBuilder = ProxyBuilderHelper.DefineDynamicAssembly(AppDomain.CurrentDomain, name);
			var moduleBuilder = ProxyBuilderHelper.DefineDynamicModule(assemblyBuilder, moduleName);

			const TypeAttributes typeAttributes = TypeAttributes.AutoClass | TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.BeforeFieldInit;

			var interfaces = new HashSet<System.Type>
			{
				typeof(IFieldInterceptorAccessor),
				typeof(ISerializable)
			};

			// Use the object as the base type
			// since we're not inheriting from any class type
			var parentType = baseType;
			if (baseType.IsInterface)
			{
				throw new ArgumentException(
					$"Field interceptor proxy does not support being build on an interface baseType ({baseType.FullName}).",
					nameof(baseType));
			}

			interfaces.RemoveWhere(i => !i.IsVisible);

			var typeBuilder = moduleBuilder.DefineType(typeName, typeAttributes, parentType, interfaces.ToArray());

			var fieldInterceptorField = typeBuilder.DefineField("__fieldInterceptor", FieldInterceptorType, FieldAttributes.Private);
			var proxyInfoField = typeBuilder.DefineField("__proxyInfo", typeof(NHibernateProxyFactoryInfo), FieldAttributes.Private);

			ImplementConstructor(typeBuilder, parentType, proxyInfoField);

			// Provide a custom implementation of ISerializable instead of redirecting it back to the interceptor
			foreach (var method in ProxyBuilderHelper.GetProxiableMethods(baseType, interfaces.Except(new[] { typeof(ISerializable) })))
			{
				CreateProxiedMethod(typeBuilder, method, fieldInterceptorField);
			}

			ProxyBuilderHelper.MakeProxySerializable(typeBuilder);
			ImplementDeserializationConstructor(typeBuilder, parentType);
			ImplementGetObjectData(typeBuilder, proxyInfoField, fieldInterceptorField, parentType);

			var proxyType = typeBuilder.CreateTypeInfo();

			ProxyBuilderHelper.Save(assemblyBuilder);

			return proxyType;
		}

		private static void CreateProxiedMethod(TypeBuilder typeBuilder, MethodInfo method, FieldInfo fieldInterceptorField)
		{
			if (method == AccessorTypeFieldInterceptorProperty.GetMethod)
			{
				ImplementGetFieldInterceptor(typeBuilder, method, fieldInterceptorField);
			}
			else if (method == AccessorTypeFieldInterceptorProperty.SetMethod)
			{
				ImplementSetFieldInterceptor(typeBuilder, method, fieldInterceptorField);
			}
			else if (ReflectHelper.IsPropertyGet(method))
			{
				ImplementGet(typeBuilder, method, fieldInterceptorField);
			}
			else if (ReflectHelper.IsPropertySet(method))
			{
				ImplementSet(typeBuilder, method, fieldInterceptorField);
			}
			// else skip
			// The field interceptor proxy stores the entity data in its base implementation (unlike
			// the NHibernateProxy which delegates the entity data to an instance of the entity).
			// As such, it only needs to intercept the properties, and can let the other methods be handled
			// by its base implementation.
		}

		private static void ImplementConstructor(TypeBuilder typeBuilder, System.Type parentType, FieldInfo proxyInfoField)
		{
			var constructor = typeBuilder.DefineConstructor(constructorAttributes, CallingConventions.Standard, new[] { typeof(NHibernateProxyFactoryInfo) });
			constructor.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);

			var IL = constructor.GetILGenerator();

			ProxyBuilderHelper.CallDefaultBaseConstructor(IL, parentType);

			// __proxyInfo == proxyInfo;
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Stfld, proxyInfoField);

			IL.Emit(OpCodes.Ret);
		}

		private static void ImplementDeserializationConstructor(TypeBuilder typeBuilder, System.Type parentType)
		{
			var parameterTypes = new[] { typeof (SerializationInfo), typeof (StreamingContext) };
			var constructor = typeBuilder.DefineConstructor(constructorAttributes, CallingConventions.Standard, parameterTypes);
			constructor.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);

			var IL = constructor.GetILGenerator();

			if (typeof(ISerializable).IsAssignableFrom(parentType))
			{
				var baseConstructor = parentType.GetConstructor(
					BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
					null,
					parameterTypes,
					null);
				if (baseConstructor == null)
				{
					// Throw new InvalidOperationException(<message>);
					IL.Emit(
						OpCodes.Ldstr,
						$"Proxy for class {parentType.FullName} cannot be deserialized because the class implements " +
						$"{nameof(ISerializable)} without having a deserialization constructor (see CA2229).");
					IL.Emit(OpCodes.Newobj, InvalidOperationWithMessageConstructor);
					IL.Emit(OpCodes.Throw);
				}
				else
				{
					IL.Emit(OpCodes.Ldarg_0);
					IL.Emit(OpCodes.Ldarg_1);
					IL.Emit(OpCodes.Ldarg_2);
					IL.Emit(OpCodes.Call, baseConstructor);
				}
			}
			else
				ProxyBuilderHelper.CallDefaultBaseConstructor(IL, parentType);

			// Everything else is done in FieldInterceptorObjectReference.
			IL.Emit(OpCodes.Ret);
		}

		private static void ImplementGetObjectData(TypeBuilder typeBuilder, FieldInfo proxyInfoField, FieldInfo fieldInterceptorField, System.Type parentType)
		{
			var methodBuilder = ProxyBuilderHelper.GetObjectDataMethodBuilder(typeBuilder);

			var IL = methodBuilder.GetILGenerator();
			IL.DeclareLocal(FieldInterceptorObjectReferenceType);
			
			// info.SetType(<FieldInterceptorObjectReferenceType>);
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldtoken, FieldInterceptorObjectReferenceType);
			IL.Emit(OpCodes.Call, ReflectionCache.TypeMethods.GetTypeFromHandle);
			IL.Emit(OpCodes.Callvirt, ProxyBuilderHelper.SerializationInfoSetTypeMethod);

			// var objectReference = new FieldInterceptorObjectReference(this.__proxyInfo, this.__fieldInterceptor));
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, proxyInfoField);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, fieldInterceptorField);
			IL.Emit(OpCodes.Newobj, FieldInterceptorObjectReferenceConstructor);
			IL.Emit(OpCodes.Stloc_0);

			// objectReference.GetObjectData(info, context);
			IL.Emit(OpCodes.Ldloc_0);
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldarg_2);
			IL.Emit(OpCodes.Callvirt, ProxyBuilderHelper.SerializableGetObjectDataMethod);

			if (typeof(ISerializable).IsAssignableFrom(parentType))
			{
				var parentGetObjectData = parentType.GetMethod(
					nameof(ISerializable.GetObjectData),
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
					null,
					new[] { typeof (SerializationInfo), typeof (StreamingContext) },
					null);
				if (parentGetObjectData == null)
				{
					// Throw new InvalidOperationException(<message>);
					IL.Emit(
						OpCodes.Ldstr,
						$"Proxy for class {parentType.FullName} cannot be serialized because the class implements " +
						$"{nameof(ISerializable)} without having a public or protected GetObjectData.");
					IL.Emit(OpCodes.Newobj, InvalidOperationWithMessageConstructor);
					IL.Emit(OpCodes.Throw);
					typeBuilder.DefineMethodOverride(methodBuilder, ProxyBuilderHelper.SerializableGetObjectDataMethod);
					return;
				}

				// base.GetObjectData(info, context);
				IL.Emit(OpCodes.Ldarg_0);
				IL.Emit(OpCodes.Ldarg_1);
				IL.Emit(OpCodes.Ldarg_2);
				IL.Emit(OpCodes.Call, parentGetObjectData);

				// objectReference.SetNoAdditionalData(info);
				IL.Emit(OpCodes.Ldloc_0);
				IL.Emit(OpCodes.Ldarg_1);
				IL.Emit(OpCodes.Callvirt, FieldInterceptorObjectReferenceSetNoAdditionalDataMethod);
			}
			else
			{
				// objectReference.SerializeBaseData(info, context, this, <parentType>);
				IL.Emit(OpCodes.Ldloc_0);
				IL.Emit(OpCodes.Ldarg_1);
				IL.Emit(OpCodes.Ldarg_2);
				IL.Emit(OpCodes.Ldarg_0);
				IL.Emit(OpCodes.Ldtoken, parentType);
				IL.Emit(OpCodes.Call, ReflectionCache.TypeMethods.GetTypeFromHandle);
				IL.Emit(OpCodes.Callvirt, FieldInterceptorObjectReferenceGetBaseDataMethod);
			}

			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(methodBuilder, ProxyBuilderHelper.SerializableGetObjectDataMethod);
		}

		private static void ImplementGetFieldInterceptor(TypeBuilder typeBuilder, MethodInfo method, FieldInfo fieldInterceptorField)
		{
			// get { return this.__fieldInterceptor; }

			const MethodAttributes attributes =
				MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig |
				MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual;
			
			var getMethod = typeBuilder.DefineMethod(
				$"{FieldInterceptorAccessorType.FullName}.get_{nameof(INHibernateProxy.HibernateLazyInitializer)}",
				attributes, CallingConventions.HasThis, FieldInterceptorType, System.Type.EmptyTypes);
			getMethod.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);

			var IL = getMethod.GetILGenerator();

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, fieldInterceptorField);
			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(getMethod, method);
		}

		private static void ImplementSetFieldInterceptor(TypeBuilder typeBuilder, MethodInfo method, FieldInfo fieldInterceptorField)
		{
			// set { this.__fieldInterceptor = value; }

			const MethodAttributes attributes =
				MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig |
				MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual;
			
			var setMethod = typeBuilder.DefineMethod(
				$"{FieldInterceptorAccessorType.FullName}.set_{nameof(INHibernateProxy.HibernateLazyInitializer)}",
				attributes, CallingConventions.HasThis, null, new[] {FieldInterceptorType});
			setMethod.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);

			var IL = setMethod.GetILGenerator();

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Stfld, fieldInterceptorField);
			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(setMethod, method);
		}

		private static void ImplementGet(TypeBuilder typeBuilder, MethodInfo getter, FieldInfo fieldInterceptorField)
		{
			/*
				var propValue = base.<getter>();
				if (this.__fieldInterceptor != null)
				{
					var result = this.__fieldInterceptor.Intercept(this, <ReflectHelper.GetPropertyName(getter)>, propValue);

					if (result != AbstractFieldInterceptor.InvokeImplementation)
					{
						return (<getter.ReturnType>)result;
					}
				}
				return propValue;
			 */
			var methodOverride = ProxyBuilderHelper.GenerateMethodSignature(getter.Name, getter, typeBuilder);

			var IL = methodOverride.GetILGenerator();
			IL.DeclareLocal(getter.ReturnType); // propValue
			IL.DeclareLocal(typeof(object)); // result

			// var propValue = base.<getter>();
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Call, getter);
			IL.Emit(OpCodes.Stloc_0);

			// if (this.__fieldInterceptor != null)
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, fieldInterceptorField);
			IL.Emit(OpCodes.Ldnull);
			var skipInterceptor = IL.DefineLabel();
			IL.Emit(OpCodes.Beq, skipInterceptor);

			// var result = this.__fieldInterceptor.Intercept(this, <ReflectHelper.GetPropertyName(getter)>, propValue);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, fieldInterceptorField);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldstr, ReflectHelper.GetPropertyName(getter));
			IL.Emit(OpCodes.Ldloc_0);
			if (getter.ReturnType.IsValueType)
				IL.Emit(OpCodes.Box, getter.ReturnType);
			IL.Emit(OpCodes.Callvirt, FieldInterceptorInterceptMethod);
			IL.Emit(OpCodes.Stloc_1);

			// if (result != AbstractFieldInterceptor.InvokeImplementation)
			IL.Emit(OpCodes.Ldloc_1);
			IL.Emit(OpCodes.Ldsfld, AbstractFieldInterceptorInvokeImplementationField);
			var skipInterceptorResult = IL.DefineLabel();
			IL.Emit(OpCodes.Beq, skipInterceptorResult);

			// return (<getter.ReturnType>)result;
			IL.Emit(OpCodes.Ldloc_1);
			IL.Emit(OpCodes.Unbox_Any, getter.ReturnType);
			IL.Emit(OpCodes.Ret);

			// end if (result != AbstractFieldInterceptor.InvokeImplementation)
			IL.MarkLabel(skipInterceptorResult);

			// end if (this.__fieldInterceptor != null)
			IL.MarkLabel(skipInterceptor);

			// return propValue;
			IL.Emit(OpCodes.Ldloc_0);
			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(methodOverride, getter);
		}

		private static void ImplementSet(TypeBuilder typeBuilder, MethodInfo setter, FieldInfo fieldInterceptorField)
		{
			/*
				if (this.__fieldInterceptor != null)
				{
					this.__fieldInterceptor.MarkDirty();
					this.__fieldInterceptor.Intercept(this, <ReflectHelper.GetPropertyName(setter)>, value);
				}
				base.<setter>(value);
			 */
			var methodOverride = ProxyBuilderHelper.GenerateMethodSignature(setter.Name, setter, typeBuilder);

			var IL = methodOverride.GetILGenerator();

			// if (this.__fieldInterceptor != null)
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, fieldInterceptorField);
			IL.Emit(OpCodes.Ldnull);
			var skipInterceptor = IL.DefineLabel();
			IL.Emit(OpCodes.Beq, skipInterceptor);

			// this.__fieldInterceptor.MarkDirty();
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, fieldInterceptorField);
			IL.Emit(OpCodes.Callvirt, FieldInterceptorMarkDirtyMethod);

			// this.__fieldInterceptor.Intercept(this, <ReflectHelper.GetPropertyName(setter)>, propValue);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, fieldInterceptorField);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldstr, ReflectHelper.GetPropertyName(setter));
			IL.Emit(OpCodes.Ldarg_1);
			var propertyType = setter.GetParameters()[0].ParameterType;
			if (propertyType.IsValueType)
				IL.Emit(OpCodes.Box, propertyType);
			IL.Emit(OpCodes.Callvirt, FieldInterceptorInterceptMethod);
			IL.Emit(OpCodes.Pop);

			// end if (this.__fieldInterceptor != null)
			IL.MarkLabel(skipInterceptor);

			// base.<setter>(value);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Call, setter);

			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(methodOverride, setter);
		}
	}
}
