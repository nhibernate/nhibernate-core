using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;
using NHibernate.Id.Enhanced;

namespace NHibernate.Id
{
	/// <summary>
	/// Factory methods for <c>IdentifierGenerator</c> framework.
	/// </summary>
	/// <remarks>
	/// <p>The built in strategies for identifier generation in NHibernate are:</p>
	/// <list type="table">
	///		<listheader>
	///			<term>strategy</term>
	///			<description>Implementation of strategy</description>
	///		</listheader>
	///		<item>
	///			<term>assigned</term>
	///			<description><see cref="Assigned"/></description>
	///		</item>
	///		<item>
	///			<term>counter</term>
	///			<description><see cref="CounterGenerator"/></description>
	///		</item>
	///		<item>
	///			<term>foreign</term>
	///			<description><see cref="ForeignGenerator"/></description>
	///		</item>
	///		<item>
	///			<term>guid</term>
	///			<description><see cref="GuidGenerator"/></description>
	///		</item>
	///		<item>
	///			<term>guid.comb</term>
	///			<description><see cref="GuidCombGenerator"/></description>
	///		</item>
	///		<item>
	///			<term>guid.native</term>
	///			<description><see cref="NativeGuidGenerator"/></description>
	///		</item>
	///		<item>
	///			<term>hilo</term>
	///			<description><see cref="TableHiLoGenerator"/></description>
	///		</item>
	///		<item>
	///			<term>identity</term>
	///			<description><see cref="IdentityGenerator"/></description>
	///		</item>
	///		<item>
	///			<term>native</term>
	///			<description>
	///				Chooses between <see cref="IdentityGenerator"/>, <see cref="SequenceGenerator"/>
	///				, and <see cref="TableHiLoGenerator"/> based on the 
	///				<see cref="Dialect.Dialect"/>'s capabilities.
	///			</description>
	///		</item>
	///		<item>
	///			<term>seqhilo</term>
	///			<description><see cref="SequenceHiLoGenerator"/></description>
	///		</item>
	///		<item>
	///			<term>sequence</term>
	///			<description><see cref="SequenceGenerator"/></description>
	///		</item>
	///		<item>
	///			<term>enhanced-sequence</term>
	///			<description><see cref="SequenceStyleGenerator"/></description>
	///		</item>
	///		<item>
	///			<term>uuid.hex</term>
	///			<description><see cref="UUIDHexGenerator"/></description>
	///		</item>
	///		<item>
	///			<term>uuid.string</term>
	///			<description><see cref="UUIDStringGenerator"/></description>
	///		</item>
	/// </list>
	/// </remarks>
	public sealed class IdentifierGeneratorFactory
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof (IdentifierGeneratorFactory));

		/// <summary> Get the generated identifier when using identity columns</summary>
		/// <param name="rs">The <see cref="IDataReader"/> to read the identifier value from.</param>
		/// <param name="type">The <see cref="IIdentifierType"/> the value should be converted to.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> the value is retrieved in.</param>
		/// <returns> The value for the identifier. </returns>
		public static object GetGeneratedIdentity(IDataReader rs, IType type, ISessionImplementor session)
		{
			if (!rs.Read())
			{
				throw new HibernateException("The database returned no natively generated identity value");
			}
			object id = Get(rs, type, session);

			if (log.IsDebugEnabled)
			{
				log.Debug("Natively generated identity: " + id);
			}
			return id;
		}

		/// <summary>
		/// Gets the value of the identifier from the <see cref="IDataReader"/> and
		/// ensures it is the correct <see cref="System.Type"/>.
		/// </summary>
		/// <param name="rs">The <see cref="IDataReader"/> to read the identifier value from.</param>
		/// <param name="type">The <see cref="IIdentifierType"/> the value should be converted to.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> the value is retrieved in.</param>
		/// <returns>
		/// The value for the identifier.
		/// </returns>
		/// <exception cref="IdentifierGenerationException">
		/// Thrown if there is any problem getting the value from the <see cref="IDataReader"/>
		/// or with converting it to the <see cref="System.Type"/>.
		/// </exception>
		public static object Get(IDataReader rs, IType type, ISessionImplementor session)
		{
			// here is an interesting one: 
			// - MsSql's @@identity returns a Decimal
			// - MySql LAST_IDENITY() returns an Int64 			
			try
			{
				return type.NullSafeGet(rs, rs.GetName(0), session, null);
			}
			catch (Exception e)
			{
				throw new IdentifierGenerationException("could not retrieve identifier value", e);
			}
		}

		/// <summary>
		/// An <see cref="Hashtable"/> where the <c>key</c> is the strategy and 
		/// the <c>value</c> is the <see cref="System.Type"/> for the strategy.
		/// </summary>
		private static readonly Dictionary<string, System.Type> idgenerators = new Dictionary<string, System.Type>();

		/// <summary>
		/// When this is returned by <c>Generate()</c> it indicates that the object
		/// has already been saved.
		/// </summary>
		/// <value>
		/// <see cref="string.Empty">String.Empty</see>
		/// </value>
		public static readonly object ShortCircuitIndicator = new object();

		/// <summary>
		/// When this is return
		/// </summary>
		public static readonly object PostInsertIndicator = new object();

		/// <summary>
		/// Initializes the static fields in <see cref="IdentifierGeneratorFactory"/>.
		/// </summary>
		static IdentifierGeneratorFactory()
		{
			idgenerators.Add("uuid.hex", typeof (UUIDHexGenerator));
			idgenerators.Add("uuid.string", typeof (UUIDStringGenerator));
			idgenerators.Add("hilo", typeof (TableHiLoGenerator));
			idgenerators.Add("assigned", typeof (Assigned));
			idgenerators.Add("counter", typeof (CounterGenerator));
			idgenerators.Add("increment", typeof (IncrementGenerator));
			idgenerators.Add("sequence", typeof (SequenceGenerator));
			idgenerators.Add("seqhilo", typeof (SequenceHiLoGenerator));
			idgenerators.Add("vm", typeof (CounterGenerator));
			idgenerators.Add("foreign", typeof (ForeignGenerator));
			idgenerators.Add("guid", typeof (GuidGenerator));
			idgenerators.Add("guid.comb", typeof (GuidCombGenerator));
			idgenerators.Add("guid.native", typeof (NativeGuidGenerator));
			idgenerators.Add("select", typeof (SelectGenerator));
			idgenerators.Add("sequence-identity", typeof (SequenceIdentityGenerator));
			idgenerators.Add("trigger-identity", typeof (TriggerIdentityGenerator));
			idgenerators.Add("enhanced-sequence", typeof(SequenceStyleGenerator));
			idgenerators.Add("enhanced-table", typeof(Enhanced.TableGenerator));
		}

		private IdentifierGeneratorFactory()
		{
			//cannot be instantiated
		}

		/// <summary>
		/// Creates an <see cref="IIdentifierGenerator"/> from the named strategy.
		/// </summary>
		/// <param name="strategy">
		/// The name of the generator to create.  This can be one of the NHibernate abbreviations (ie - <c>native</c>, 
		/// <c>sequence</c>, <c>guid.comb</c>, etc...), a full class name if the Type is in the NHibernate assembly, or
		/// a full type name if the strategy is in an external assembly.
		/// </param>
		/// <param name="type">The <see cref="IType"/> that the retured identifier should be.</param>
		/// <param name="parms">An <see cref="IDictionary"/> of <c>&lt;param&gt;</c> values from the mapping.</param>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with Configuration.</param>
		/// <returns>
		/// An instantiated and configured <see cref="IIdentifierGenerator"/>.
		/// </returns>
		/// <exception cref="MappingException">
		/// Thrown if there are any exceptions while creating the <see cref="IIdentifierGenerator"/>.
		/// </exception>
		public static IIdentifierGenerator Create(string strategy, IType type, IDictionary<string, string> parms,
												  Dialect.Dialect dialect)
		{
			try
			{
				System.Type clazz = GetIdentifierGeneratorClass(strategy, dialect);
				var idgen = (IIdentifierGenerator) Cfg.Environment.BytecodeProvider.ObjectsFactory.CreateInstance(clazz);
				var conf = idgen as IConfigurable;
				if (conf != null)
				{
					conf.Configure(type, parms, dialect);
				}
				return idgen;
			}
			catch (IdentifierGenerationException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new MappingException("could not instantiate id generator: " + strategy, e);
			}
		}

		/// <summary>
		/// Create the correct boxed <see cref="System.Type"/> for the identifier.
		/// </summary>
		/// <param name="value">The value of the new identifier.</param>
		/// <param name="type">The <see cref="IType"/> the identifier should be.</param>
		/// <returns>
		/// The identifier value converted to the <see cref="System.Type"/>.
		/// </returns>
		/// <exception cref="IdentifierGenerationException">
		/// The <c>type</c> parameter must be an <see cref="short"/>, <see cref="int"/>,
		/// or <see cref="long"/>.
		/// </exception>
		public static object CreateNumber(long value, System.Type type)
		{
			// Convert.ChangeType would be better here, but it fails if the value does not fit
			// in the destination type, while we need the value to be truncated in this case.

			if (type == typeof (byte))
			{
				return (byte) value;
			}
			else if (type == typeof (sbyte))
			{
				return (sbyte) value;
			}
			else if (type == typeof (short))
			{
				return (short) value;
			}
			else if (type == typeof (ushort))
			{
				return (ushort) value;
			}
			else if (type == typeof (int))
			{
				return (int) value;
			}
			else if (type == typeof (uint))
			{
				return (uint) value;
			}
			else if (type == typeof (long))
			{
				return value;
			}
			else if (type == typeof (ulong))
			{
				return (ulong) value;
			}
			else if (type == typeof (decimal))
			{
				return (decimal) value;
			}
			else
			{
				try
				{
					return Convert.ChangeType(value, type);
				}
				catch (Exception e)
				{
					throw new IdentifierGenerationException("could not convert generated value to type " + type, e);
				}
			}
		}

		public static System.Type GetIdentifierGeneratorClass(string strategy, Dialect.Dialect dialect)
		{
			System.Type clazz;
			if ("native".Equals(strategy))
			{
				clazz = dialect.NativeIdentifierGeneratorClass;
			}
			else if ("identity".Equals(strategy))
			{
				clazz = dialect.IdentityStyleIdentifierGeneratorClass;
			}
			else
			{
				idgenerators.TryGetValue(strategy, out clazz);
			}
			try
			{
				if (clazz == null)
				{
					clazz = ReflectHelper.ClassForName(strategy);
				}
			}
			catch (Exception)
			{
				throw new IdentifierGenerationException("Could not interpret id generator strategy: " + strategy);
			}
			return clazz;
		}
	}
}