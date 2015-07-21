using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Security.Permissions;
using NHibernate.Properties;
using NHibernate.Util;

namespace NHibernate.Bytecode.Lightweight
{
	public class ReflectionOptimizer : IReflectionOptimizer, IInstantiationOptimizer
	{
		private readonly IAccessOptimizer accessOptimizer;
		private readonly CreateInstanceInvoker createInstanceMethod;
		protected readonly System.Type mappedType;
		private readonly System.Type typeOfThis;

		public IAccessOptimizer AccessOptimizer
		{
			get { return accessOptimizer; }
		}

		public IInstantiationOptimizer InstantiationOptimizer
		{
			get { return this; }
		}

		public virtual object CreateInstance()
		{
			return createInstanceMethod != null ? createInstanceMethod() : null;
		}

		/// <summary>
		/// Class constructor.
		/// </summary>
		public ReflectionOptimizer(System.Type mappedType, IGetter[] getters, ISetter[] setters)
		{
			// save off references
			this.mappedType = mappedType;
			typeOfThis = mappedType.IsValueType ? mappedType.MakeByRefType() : mappedType;
			//this.getters = getters;
			//this.setters = setters;

			GetPropertyValuesInvoker getInvoker = GenerateGetPropertyValuesMethod(getters);
			SetPropertyValuesInvoker setInvoker = GenerateSetPropertyValuesMethod(getters, setters);

			accessOptimizer = new AccessOptimizer(getInvoker, setInvoker, getters, setters);

			createInstanceMethod = CreateCreateInstanceMethod(mappedType);
		}

		/// <summary>
		/// Generates a dynamic method which creates a new instance of <paramref name="type" />
		/// when invoked.
		/// </summary>
		protected virtual CreateInstanceInvoker CreateCreateInstanceMethod(System.Type type)
		{
			if (type.IsInterface || type.IsAbstract)
			{
				return null;
			}

			var method = new DynamicMethod(string.Empty, typeof (object), null, type, true);

			ILGenerator il = method.GetILGenerator();

			if (type.IsValueType)
			{
				LocalBuilder tmpLocal = il.DeclareLocal(type);
				il.Emit(OpCodes.Ldloca, tmpLocal);
				il.Emit(OpCodes.Initobj, type);
				il.Emit(OpCodes.Ldloc, tmpLocal);
				il.Emit(OpCodes.Box, type);
				il.Emit(OpCodes.Ret);

				return (CreateInstanceInvoker)method.CreateDelegate(typeof(CreateInstanceInvoker));
			}
			else
			{
				ConstructorInfo constructor = ReflectHelper.GetDefaultConstructor(type);
				if (constructor != null)
				{
					il.Emit(OpCodes.Newobj, constructor);
					il.Emit(OpCodes.Ret);

					return (CreateInstanceInvoker) method.CreateDelegate(typeof (CreateInstanceInvoker));
				}
				else
				{
					ThrowExceptionForNoDefaultCtor(type);
				}
			}
			return null;
		}

		protected virtual void ThrowExceptionForNoDefaultCtor(System.Type type)
		{
			throw new InstantiationException("Object class " + type + " must declare a default (no-argument) constructor", type);
		}

		protected DynamicMethod CreateDynamicMethod(System.Type returnType, System.Type[] argumentTypes)
		{
			System.Type owner = mappedType.IsInterface ? typeof (object) : mappedType;
			bool canSkipChecks = SecurityManager.IsGranted(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
			return new DynamicMethod(string.Empty, returnType, argumentTypes, owner, canSkipChecks);
		}

		private static void EmitCastToReference(ILGenerator il, System.Type type)
		{
			if (type.IsValueType)
			{
				il.Emit(OpCodes.Unbox, type);
			}
			else
			{
				il.Emit(OpCodes.Castclass, type);
			}
		}

		/// <summary>
		/// Generates a dynamic method on the given type.
		/// </summary>
		private GetPropertyValuesInvoker GenerateGetPropertyValuesMethod(IGetter[] getters)
		{
			var methodArguments = new[] {typeof (object), typeof (GetterCallback)};
			DynamicMethod method = CreateDynamicMethod(typeof (object[]), methodArguments);

			ILGenerator il = method.GetILGenerator();

			LocalBuilder thisLocal = il.DeclareLocal(typeOfThis);
			LocalBuilder dataLocal = il.DeclareLocal(typeof (object[]));

			// Cast the 'this' pointer to the appropriate type and store it in a local variable
			il.Emit(OpCodes.Ldarg_0);
			EmitCastToReference(il, mappedType);
			il.Emit(OpCodes.Stloc, thisLocal);

			// Allocate the values array and store it in a local variable
			il.Emit(OpCodes.Ldc_I4, getters.Length);
			il.Emit(OpCodes.Newarr, typeof (object));
			il.Emit(OpCodes.Stloc, dataLocal);

			//get all the data from the object into the data array to be returned
			for (int i = 0; i < getters.Length; i++)
			{
				// get the member accessors
				IGetter getter = getters[i];

				// queue up the array storage location for the value
				il.Emit(OpCodes.Ldloc, dataLocal);
				il.Emit(OpCodes.Ldc_I4, i);

				// get the value...
				var optimizableGetter = getter as IOptimizableGetter;
				if (optimizableGetter != null)
				{
					// using the getter's emitted IL code
					il.Emit(OpCodes.Ldloc, thisLocal);
					optimizableGetter.Emit(il);
					EmitUtil.EmitBoxIfNeeded(il, getter.ReturnType);
				}
				else
				{
					// using the getter itself via a callback
					MethodInfo invokeMethod = typeof (GetterCallback).GetMethod("Invoke",
					                                                            new[] {typeof (object), typeof (int)});
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldc_I4, i);
					il.Emit(OpCodes.Callvirt, invokeMethod);
				}

				//store the value
				il.Emit(OpCodes.Stelem_Ref);
			}

			// Return the data array
			il.Emit(OpCodes.Ldloc, dataLocal.LocalIndex);
			il.Emit(OpCodes.Ret);

			return (GetPropertyValuesInvoker) method.CreateDelegate(typeof (GetPropertyValuesInvoker));
		}

		/// <summary>
		/// Generates a dynamic method on the given type.
		/// </summary>
		/// <returns></returns>
		private SetPropertyValuesInvoker GenerateSetPropertyValuesMethod(IGetter[] getters, ISetter[] setters)
		{
			var methodArguments = new[] {typeof (object), typeof (object[]), typeof (SetterCallback)};
			DynamicMethod method = CreateDynamicMethod(null, methodArguments);

			ILGenerator il = method.GetILGenerator();

			// Declare a local variable used to store the object reference (typed)
			LocalBuilder thisLocal = il.DeclareLocal(typeOfThis);
			il.Emit(OpCodes.Ldarg_0);
			EmitCastToReference(il, mappedType);
			il.Emit(OpCodes.Stloc, thisLocal.LocalIndex);

			for (int i = 0; i < setters.Length; i++)
			{
				// get the member accessor
				ISetter setter = setters[i];
				System.Type valueType = getters[i].ReturnType;

				var optimizableSetter = setter as IOptimizableSetter;

				if (optimizableSetter != null)
				{
					// load 'this'
					il.Emit(OpCodes.Ldloc, thisLocal);

					// load the value from the data array
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldc_I4, i);
					il.Emit(OpCodes.Ldelem_Ref);

					EmitUtil.PreparePropertyForSet(il, valueType);

					// using the setter's emitted IL
					optimizableSetter.Emit(il);
				}
				else
				{
					// using the setter itself via a callback
					MethodInfo invokeMethod = typeof (SetterCallback).GetMethod("Invoke",
					                                                            new[] {typeof (object), typeof (int), typeof (object)});
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldc_I4, i);

					// load the value from the data array
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldc_I4, i);
					il.Emit(OpCodes.Ldelem_Ref);

					il.Emit(OpCodes.Callvirt, invokeMethod);
				}
			}

			// Setup the return
			il.Emit(OpCodes.Ret);

			return (SetPropertyValuesInvoker) method.CreateDelegate(typeof (SetPropertyValuesInvoker));
		}
	}
}