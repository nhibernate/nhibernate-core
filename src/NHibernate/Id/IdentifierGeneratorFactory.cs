using System;
using System.Data;
using System.Collections;

using NHibernate.Type;


namespace NHibernate.Id 
{
	/// <summary>
	/// Factory methods for <c>IdentifierGenerator</c> framework.
	/// </summary>
	public sealed class IdentifierGeneratorFactory {
		public static object Get(IDataReader rs, System.Type clazz) {
			
			// here is an interesting one - MsSql's @@identity returns a
			// numeric - which translates to a C# decimal type.  I don't know
			// if this is specific to the SqlServer provider or other providers
			
			decimal identityValue = rs.GetDecimal(0);

			if (clazz==typeof(long)) 
			{
				return (long)identityValue;
				//return (long)rs[0];
			} 
			else if (clazz==typeof(int)) 
			{
				return (int)identityValue;
				//return (int)rs.[0];
			} 
			else if (clazz==typeof(short)) 
			{
				return (short)identityValue;
				//return (short)rs[0];
			} 
			else 
			{
				throw new IdentifierGenerationException("this id generator generates Int64, Int32, Int16");
			}
		}

		private static readonly Hashtable idgenerators = new Hashtable();
		public static readonly string ShortCircuitIndicator = string.Empty;

		static IdentifierGeneratorFactory() 
		{
			idgenerators.Add("uuid.hex", typeof(UUIDHexGenerator));
			idgenerators.Add("uuid.string", typeof(UUIDStringGenerator));
			idgenerators.Add("hilo", typeof(TableHiLoGenerator));
			idgenerators.Add("assigned", typeof(Assigned));
			idgenerators.Add("identity", typeof(IdentityGenerator));
			idgenerators.Add("sequence", typeof(SequenceGenerator));
			idgenerators.Add("seqhilo", typeof(SequenceHiLoGenerator));
			idgenerators.Add("vm", typeof(CounterGenerator));
			idgenerators.Add("foreign", typeof(ForeignGenerator));
			idgenerators.Add("guid", typeof(GuidGenerator));
		}

		private IdentifierGeneratorFactory() {} //cannot be instantiated

		public static IIdentifierGenerator Create(string strategy, IType type, IDictionary parms, Dialect.Dialect dialect) 
		{
			try 
			{
				System.Type clazz = (System.Type) idgenerators[strategy];
				if ( "native".Equals(strategy) ) 
				{
					if ( dialect.SupportsIdentityColumns ) 
					{
						clazz = typeof(IdentityGenerator);
					}
					else if ( dialect.SupportsSequences ) 
					{
						clazz = typeof(SequenceGenerator);
					}
					else 
					{
						clazz = typeof(TableHiLoGenerator);
					}
				}
				if (clazz==null) clazz = System.Type.GetType(strategy);
				IIdentifierGenerator idgen = (IIdentifierGenerator) Activator.CreateInstance(clazz);
				if (idgen is IConfigurable) ((IConfigurable) idgen).Configure(type, parms, dialect);
				return idgen;
			} 
			catch (Exception e) 
			{
				throw new MappingException("could not instantiate id generator", e);
			}
		}

		internal static object CreateNumber(long value, System.Type type) {
			if (type==typeof(long)) 
			{
				return value;
			} 
			else if ( type==typeof(int)) 
			{
				return (int) value;
			} 
			else if ( type==typeof(short)) 
			{
				return (short) value;
			} 
			else 
			{
				throw new IdentifierGenerationException("this id generator generates Int64, Int32, Int16");
			}
		}
	}
}
