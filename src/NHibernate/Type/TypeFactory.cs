using System;
using System.Collections;
using System.Globalization;

using NHibernate.Type;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Type {

	/// <summary>
	/// Used internally to obtain instances of IType.
	/// Applications should use static methods and constants on NHibernate.NHibernate.
	/// </summary>
	public class TypeFactory {

		private static readonly Hashtable basicTypes;

		static TypeFactory() {
			basicTypes = new Hashtable(41);  //TODO: Fixup to prime number size when decommented all add.

			basicTypes.Add(typeof(bool).Name, NHibernate.Boolean);
			basicTypes.Add(typeof(long).Name, NHibernate.Long);
			basicTypes.Add(typeof(short).Name, NHibernate.Short);
			basicTypes.Add(typeof(int).Name, NHibernate.Integer);
			basicTypes.Add(typeof(byte).Name, NHibernate.Byte);
			basicTypes.Add(typeof(float).Name, NHibernate.Float);
			basicTypes.Add(typeof(double).Name, NHibernate.Double);
			basicTypes.Add(typeof(char).Name, NHibernate.Character);
			basicTypes.Add(typeof(string).Name, NHibernate.String);
			basicTypes.Add(typeof(decimal).Name, NHibernate.Decimal);
			basicTypes.Add(NHibernate.Character.Name, NHibernate.Character);
			basicTypes.Add(NHibernate.Integer.Name, NHibernate.Integer);
			basicTypes.Add(NHibernate.Decimal.Name, NHibernate.Decimal);
			basicTypes.Add(NHibernate.Long.Name, NHibernate.Long);
			basicTypes.Add(NHibernate.Short.Name, NHibernate.Short);
			basicTypes.Add(NHibernate.String.Name, NHibernate.String);
			basicTypes.Add(NHibernate.Date.Name, NHibernate.Date);
			//basicTypes.Add(NHibernate.Time.Name, NHibernate.Time);
			basicTypes.Add(typeof(CultureInfo).Name, NHibernate.CultureInfo);
			basicTypes.Add(NHibernate.CultureInfo.Name, NHibernate.CultureInfo);
			basicTypes.Add(NHibernate.Timestamp.Name, NHibernate.Timestamp);
			//basicTypes.Add(NHibernate.Calendar.Name, NHibernate.Calendar);
			//basicTypes.Add(NHibernate.CalendarDate.Name, NHibernate.CalendarDate);
			//basicTypes.Add(NHibernate.Currency.Name, NHibernate.Currency);
			//basicTypes.Add(NHibernate.Timezone.Name, NHibernate.Timezone);
			basicTypes.Add(NHibernate.TrueFalse.Name, NHibernate.TrueFalse);
			basicTypes.Add(NHibernate.YesNo.Name, NHibernate.YesNo);
			basicTypes.Add(NHibernate.Binary.Name, NHibernate.Binary);
			//basicTypes.Add(NHibernate.Blob.Name, NHibernate.Blob);
			//basicTypes.Add(NHibernate.Clob.Name, NHibernate.Clob);
			//basicTypes.Add(NHibernate.BigDecimal.Name, NHibernate.BigDecimal);
			//basicTypes.Add(NHibernate.Serializable.Name, NHibernate.Serializable);
			basicTypes.Add(NHibernate.Object.Name, NHibernate.Object);
			basicTypes.Add(NHibernate.Boolean.Name, NHibernate.Boolean);
			basicTypes.Add(typeof(DateTime).Name, NHibernate.Timestamp);  
			//basicTypes.Add(typeof(Calendar).Name, NHibernate.Calendar); 
			// if ( CurrencyType.CURRENCY_CLASS!=null) basicTypes.put( CurrencyType.CURRENCY_CLASS.getName(), Hibernate.CURRENCY);
			// basicTypes.put( TimeZone.class.getName(), Hibernate.TIMEZONE);
			basicTypes.Add(typeof(object).Name, NHibernate.Object);
			basicTypes.Add(typeof(System.Type).Name, NHibernate.Class);
			basicTypes.Add(NHibernate.Class.Name, NHibernate.Class);
			basicTypes.Add(typeof(byte[]).Name, NHibernate.Binary);
			//basicTypes.put( Blob.class.getName(), Hibernate.BLOB);
			//basicTypes.put( Clob.class.getName(), Hibernate.CLOB);
			//basicTypes.put( Serializable.class.getName(), Hibernate.SERIALIZABLE);
		}
	

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

		/// <summary>
		/// Uses hueristics to deduce a Hibernate type given a string naming the type or
		/// Java class. Return an instance of <c>NHibernate.Type.IType</c>
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static IType HueristicType(string typeName) {
			IType type = TypeFactory.Basic(typeName);
			if (type==null) {
				System.Type typeClass;
				try {
					typeClass = ReflectHelper.ClassForName(typeName);
				}
				catch (Exception) {
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
					else if ( typeof(IUserType).IsAssignableFrom(typeClass) ) {
						type = new CustomType(typeClass);
					}
					else if ( typeof(ILifecycle).IsAssignableFrom(typeClass) ) {
						type = NHibernate.Association(typeClass);
					}
					else if ( typeof(IPersistentEnum).IsAssignableFrom(typeClass) ) {
						type = NHibernate.Enum(typeClass);
					}
					else if ( typeClass.IsSerializable ) {
						type = NHibernate.GetSerializable(typeClass);
					}
				}
			}
			return type;
		}

		// Collection Types:
	
		
		public static PersistentCollectionType Array(string role, System.Type elementClass) {
			return new ArrayType(role, elementClass);
		}
		
		public static PersistentCollectionType List(string role) {
			return new ListType(role);
		}
		/*
		public static PersistentCollectionType Bag(string role) {
			return new BagType(role);
		}
		*/
		public static PersistentCollectionType Map(string role) {
			return new MapType(role);
		}
		/*
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
				System.Array.Copy(results, 0, trimmed, 0, count);
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