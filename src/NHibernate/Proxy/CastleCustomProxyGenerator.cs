using System;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Builder.CodeBuilder;
using Castle.DynamicProxy.Builder.CodeGenerators;

namespace NHibernate.Proxy
{
	/// <summary>
	/// This will build a proxy for a <see cref="System.Type"/> that is 
	/// a <c>Class</c> and then add the <see cref="INHibernateProxy"/>
	/// interface to it.
	/// </summary>
	[CLSCompliant( false )]
	public class CastleCustomProxyGenerator : ClassProxyGenerator
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="scope"></param>
		public CastleCustomProxyGenerator( ModuleScope scope )
			: base( scope )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="context"></param>
		public CastleCustomProxyGenerator( ModuleScope scope, GeneratorContext context )
			: base( scope, context )
		{
		}

		/// <summary>
		/// Override of the <see cref="BaseCodeGenerator.CreateTypeBuilder(System.Type, System.Type[])"/> implementation.
		/// </summary>
		/// <param name="baseType">The base <see cref="System.Type"/> for the Proxy.</param>
		/// <param name="interfaces">The extra <see cref="System.Type"/> interfaces for the Proxy to implement.</param>
		/// <returns>An <see cref="EasyType"/> with the required interfaces added.</returns>
		protected override EasyType CreateTypeBuilder( System.Type baseType, System.Type[ ] interfaces )
		{
			bool isImplemented = false;

			// check to see if this implements INHibernateProxy - if not then make it
			// implement the interface
			for( int i = 0; i < interfaces.Length; i++ )
			{
				if( interfaces[ i ].Equals( typeof( INHibernateProxy ) ) )
				{
					isImplemented = true;
				}
			}

			if( isImplemented == false )
			{
				int length = interfaces.Length;
				System.Type[ ] newInterfaces = new System.Type[length + 1];
				Array.Copy( interfaces, 0, newInterfaces, 0, length );
				newInterfaces[ length ] = typeof( INHibernateProxy );
				interfaces = newInterfaces;
			}

			return base.CreateTypeBuilder( baseType, interfaces );
		}

	}
}