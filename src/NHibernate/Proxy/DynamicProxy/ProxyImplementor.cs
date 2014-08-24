#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NHibernate.Proxy.DynamicProxy
{
	internal class ProxyImplementor
	{
		private const MethodAttributes InterceptorMethodsAttributes = MethodAttributes.Public | MethodAttributes.HideBySig |
		                                                  MethodAttributes.SpecialName | MethodAttributes.NewSlot |
		                                                  MethodAttributes.Virtual;

		private FieldBuilder field;

		public FieldBuilder InterceptorField
		{
			get { return field; }
		}

		public void ImplementProxy(TypeBuilder typeBuilder)
		{
			// Implement the IProxy interface
			typeBuilder.AddInterfaceImplementation(typeof (IProxy));

			field = typeBuilder.DefineField("__interceptor", typeof (IInterceptor), FieldAttributes.Private);

			// Implement the getter
			MethodBuilder getterMethod = typeBuilder.DefineMethod("get_Interceptor", InterceptorMethodsAttributes, CallingConventions.HasThis, typeof(IInterceptor), new System.Type[0]);
			getterMethod.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);

			ILGenerator IL = getterMethod.GetILGenerator();

			// This is equivalent to:
			// get { return __interceptor;
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, field);
			IL.Emit(OpCodes.Ret);

			// Implement the setter
			MethodBuilder setterMethod = typeBuilder.DefineMethod("set_Interceptor", InterceptorMethodsAttributes, CallingConventions.HasThis, typeof (void), new[] {typeof (IInterceptor)});

			setterMethod.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);
			IL = setterMethod.GetILGenerator();
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Stfld, field);
			IL.Emit(OpCodes.Ret);

			MethodInfo originalSetter = typeof (IProxy).GetMethod("set_Interceptor");
			MethodInfo originalGetter = typeof (IProxy).GetMethod("get_Interceptor");

			typeBuilder.DefineMethodOverride(setterMethod, originalSetter);
			typeBuilder.DefineMethodOverride(getterMethod, originalGetter);
		}
	}
}