using System;
using System.Collections;
using System.Data;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that maps an <see cref="Array"/> collection
	/// to the database.
	/// </summary>
	public class ArrayType : PersistentCollectionType
	{
		private readonly System.Type elementClass;
		private readonly System.Type arrayClass;

		/// <summary>
		/// Initializes a new instance of a <see cref="ArrayType"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="elementClass">The <see cref="System.Type"/> of the element contained in the array.</param>
		/// <remarks>
		/// This creates a bag that is non-generic.
		/// </remarks>
		public ArrayType( string role, string propertyRef, System.Type elementClass )
			: base( role, propertyRef )
		{
			this.elementClass = elementClass;
			arrayClass = System.Array.CreateInstance( elementClass, 0 ).GetType();
		}

		/// <summary>
		/// The <see cref="System.Array"/> for the element.
		/// </summary>
		public override System.Type ReturnedClass
		{
			get { return arrayClass; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="persister"></param>
		/// <returns></returns>
		public override IPersistentCollection Instantiate( ISessionImplementor session, ICollectionPersister persister )
		{
			return new ArrayHolder( session, persister );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		/// <param name="session"></param>
		public override void NullSafeSet( IDbCommand st, object value, int index, ISessionImplementor session )
		{
			base.NullSafeSet( st, session.GetArrayHolder( value ), index, session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override ICollection GetElementsCollection( object collection )
		{
			return ( Array ) collection;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override object Disassemble( object value, ISessionImplementor session )
		{
			if( value == null )
			{
				return null;
			}
			return session.GetLoadedCollectionKey( session.GetArrayHolder( value ) );
		}

		/// <summary>
		/// Wraps a <see cref="System.Array"/> in a <see cref="ArrayHolder"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="array">The unwrapped array.</param>
		/// <returns>
		/// An <see cref="ArrayHolder"/> that wraps the non NHibernate <see cref="System.Array"/>.
		/// </returns>
		public override IPersistentCollection Wrap( ISessionImplementor session, object array )
		{
			return new ArrayHolder( session, array );
		}

		/// <summary></summary>
		public override bool IsArrayType
		{
			get { return true; }
		}

		// Not ported - ToString( object value, ISessionFactoryImplementor factory )
		// - PesistentCollectionType implementation is able to handle arrays too in .NET

		/// <summary>
		/// 
		/// </summary>
		/// <param name="original"></param>
		/// <param name="target"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <param name="copiedAlready"></param>
		/// <returns></returns>
		public override object Copy( object original, object target, ISessionImplementor session, object owner, IDictionary copiedAlready )
		{
			if ( original == null ) 
			{
				return null;
			}
			if ( original == target )
			{
				return target;
			}
			
			System.Array orig = ( System.Array ) original;
			int length = orig.Length;
			System.Array result = System.Array.CreateInstance( elementClass, length );

			IType elemType = GetElementType( session.Factory );
			for ( int i = 0; i < length; i++ )
			{
				result.SetValue(
					elemType.Copy( orig.GetValue( i ), null, session, owner, copiedAlready ),
					i );
			}
			return result;
		}
	}
}