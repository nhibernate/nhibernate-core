using System;
using System.Collections;

using NHibernate.Type;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Type {

	/// <summary>
	/// Used internally to obtain instances of IType.
	/// Applications should use static methods and constants on NHibernate.NHibernate.
	/// </summary>
	public class TypeFactory {

		private static readonly Hashtable basicTypes;

		private TypeFactory() { throw new NotSupportedException(); }
		
		/// <summary>
		/// A one-to-one association type for the given class and cascade style.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="foreignKeyType"></param>
		/// <returns></returns>
		public static IType OneToOne(System.Type persistentClass, ForeignKeyType foreignKeyType) {
			return new OneToOneType(persistentClass, foreignKeyType);
		}

		/// <summary>
		/// A many-to-one association type for the given class and cascade style.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public static IType ManyToOne(System.Type persistentClass) {
			return new ManyToOneType(persistentClass);
		}
		
		/// <summary>
		/// Given the name of a Hibernate basic type, return an instance
		/// of NHibernate.Type.IType
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static IType Basic(string name) {
			return (IType) basicTypes[name];
		}

		public static IType HueristicType(string typeName) {
			IType type = TypeFactory.Basic(typeName);
			if (type==null) {
				System.Type typeClass;
				try {
					typeClass = null; //HACK: need ReflectHelper implementation
					//TODO: typeClass = ReflectHelper.classForName(typeName);
				}
				catch (TypeLoadException tle) {
					typeClass = null;
				}
				if (typeClass!=null) {
					if ( typeof(IType).IsAssignableFrom(typeClass) ) {
						try {
							type = (IType) Activator.CreateInstance(typeClass);
						}
						catch (Exception e) {
							throw new MappingException("Could not instantiate IType " + typeClass.Name + ": " + e);
						}
					}
					/*
					else if ( typeof(UserType).IsAssignableFrom(typeClass) ) {
						type = new CustomType(typeClass);
					}
					*/
					else if ( typeof(ILifecycle).IsAssignableFrom(typeClass) ) {
						type = NHibernate.Association(typeClass);
					}

					/* Do we use standar enum instead of this?
					 * 
					else if ( typeof(PersistentEnum).IsAssignableFrom(typeClass) ) {
						type = NHibernate.Enum(typeClass);
					}
					*/
					
					//TODO: Check Serializable Attribute????
					/* 
					else if  ( Serializable.class.isAssignableFrom(typeClass) ) {
						type = NHibernate.Serializable(typeClass);
					}
					*/
				}
			}
			return type;
		}

		// Collection Types:
	
		/*
		public static PersistentCollectionType Array(string role, Class elementClass) {
			return new ArrayType(role, elementClass);
		}
		public static PersistentCollectionType List(string role) {
			return new ListType(role);
		}
		public static PersistentCollectionType Bag(string role) {
			return new BagType(role);
		}
		public static PersistentCollectionType Map(string role) {
			return new MapType(role);
		}
		public static PersistentCollectionType Set(string role) {
			return new SetType(role);
		}
		public static PersistentCollectionType SortedMap(string role, Comparator comparator) {
			return new SortedMapType(role, comparator);
		}
		public static PersistentCollectionType SortedSet(string role, Comparator comparator) {
			return new SortedSetType(role, comparator);
		}
		*/

		/// <summary>
		/// Deep copy values in the first array into the second
		/// </summary>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <param name="copy"></param>
		/// <param name="target"></param>
		public static void DeepCopy(object[] values, IType[] types, bool[] copy, object[] target) {
			for ( int i=0; i<types.Length; i++ ) {
				if ( copy[i] ) target[i] = types[i].DeepCopy( values[i] );
			}
		}
	
		/// <summary>
		/// Determine if any of the given field values are dirty,
		/// returning an array containing indexes of
		/// the dirty fields or null if no fields are dirty.
		/// </summary>
		/// <param name="types"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="check"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public static int[] FindDirty(IType[] types, object[] x, object[] y, bool[] check, ISessionImplementor session) {
			int[] results = null;
			int count = 0;
			for (int i=0; i<types.Length; i++) {
				if ( check[i] && types[i].IsDirty( x[i], y[i], session ) ) {
					if (results==null) results = new int[ types.Length ];
					results[count++]=i;
				}
			}
			if (count==0) {
				return null;
			}
			else {
				int[] trimmed = new int[count];
				Array.Copy(results, 0, trimmed, 0, count);
				return trimmed;
			}
		}
	
		/*
		/// <summary>
		/// Return <tt>-1</tt> if non-dirty, or the index of the first dirty value otherwise
		/// </summary>
		/// <param name="types"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="owner"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public static int FindDirty(IType[] types, object[] x, object[] y, object owner, ISessionFactoryImplementor factory) {
			for (int i=0; i<types.Length; i++) {
				if ( types[i].IsDirty( x[i], y[i], owner, factory ) ) {
					return i;
				}
			}
			return -1;
		}
		*/
	}
}