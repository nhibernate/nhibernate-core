using System;
using System.Reflection.Emit;

using Apache.Avalon.DynamicProxy;
using Apache.Avalon.DynamicProxy.Builder.CodeGenerators;

namespace NHibernate.Proxy
{
	/// <summary>
	/// This will build a proxy for a <see cref="System.Type"/> that is 
	/// a <c>Class</c> and then add the <see cref="INHibernateProxy"/>
	/// interface to it.
	/// </summary>
	public class AvalonCustomProxyGenerator : ClassProxyGenerator
	{
		public AvalonCustomProxyGenerator(ModuleScope scope) 
			: base(scope)
		{
		}

		public AvalonCustomProxyGenerator(ModuleScope scope, GeneratorContext context) 
			: base(scope, context) 
		{
		}

		protected override TypeBuilder CreateTypeBuilder(System.Type baseType, System.Type[] interfaces)
		{
			bool isImplemented = false;

			// check to see if this implements INHibernateProxy - if not then make it
			// implement the interface
			for( int i=0; i<interfaces.Length; i++) 
			{
				if( interfaces[i].Equals( typeof(INHibernateProxy) ) )
				{
					isImplemented = true;
				}
			}

			if( isImplemented==false ) 
			{
				int length = interfaces.Length;
				System.Type[] newInterfaces = new System.Type[ length + 1 ];
				Array.Copy( interfaces, 0, newInterfaces, 0, length );
				newInterfaces[ length ] = typeof(INHibernateProxy);
				interfaces = newInterfaces;
			}

			
			TypeBuilder builder = base.CreateTypeBuilder(baseType, interfaces);
			
			// the CreateTypeBuilder for a Proxy of a Class ignores the interfaces parameter - 
			// so we need to tell the ProxyGenerator to specifically generate the 
			// implementation of INHibernateProxy : ISerializable
			GenerateInterfaceImplementation( interfaces );

			return builder;
			
		}

	}
}
