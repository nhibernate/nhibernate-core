using System;
using System.Reflection;
using NHibernate.Properties;
using NHibernate.Type;

namespace NHibernate.Engine
{
	public sealed class UnsavedValueFactory
	{
		private UnsavedValueFactory()
		{
		}

		private static object[] NoParameters = new object[0];

		private static object Instantiate(ConstructorInfo constructor)
		{
			try
			{
				return constructor.Invoke(NoParameters);
			}
			catch (Exception e)
			{
				throw new InstantiationException("could not instantiate test object: ", e, constructor.DeclaringType);
			}
		}

		/// <summary>
		/// Return an IdentifierValue for the specified unsaved-value. If none is specified,
		/// guess the unsaved value by instantiating a test instance of the class and
		/// reading it's id property, or if that is not possible, using the java default
		/// value for the type
		/// </summary>
		public static IdentifierValue GetUnsavedIdentifierValue(
			string unsavedValue,
			IGetter identifierGetter,
			IType identifierType,
			ConstructorInfo constructor)
		{
			if (unsavedValue == null)
			{
				if (identifierGetter != null && constructor != null)
				{
					// use the id value of a newly instantiated instance as the unsaved-value
					object defaultValue = identifierGetter.Get(Instantiate(constructor));
					return new IdentifierValue(defaultValue);
				}
					// TODO: NH - the branch below is actually never visited, so it's commented out
					/*
				else if( identifierGetter != null && ( identifierType is ValueTypeType ) )
				{
					object defaultValue = ( ( ValueTypeType ) identifierType ).DefaultValue;
					return new Cascades.IdentifierValue( defaultValue );
				}
				*/
				else
				{
					return IdentifierValue.SaveNull;
				}
			}
			else if ("null" == unsavedValue)
			{
				return IdentifierValue.SaveNull;
			}
				// TODO: H3 only, IdentifierValue.IsUnsaved may return true/false/null in H3
				// and SaveUndefined always returns null.
				/*
			else if( "undefined" == unsavedValue )
			{
				return Cascades.IdentifierValue.SaveUndefined;
			}
			*/
			else if ("none" == unsavedValue)
			{
				return IdentifierValue.SaveNone;
			}
			else if ("any" == unsavedValue)
			{
				return IdentifierValue.SaveAny;
			}
			else
			{
				try
				{
					return new IdentifierValue(((IIdentifierType) identifierType).StringToObject(unsavedValue));
				}
				catch (InvalidCastException cce)
				{
					throw new MappingException("Bad identifier type: " + identifierType.Name, cce);
				}
				catch (Exception e)
				{
					throw new MappingException("Could not parse identifier unsaved-value: " + unsavedValue, e);
				}
			}
		}

		public static VersionValue GetUnsavedVersionValue(
			String versionUnsavedValue,
			IGetter versionGetter,
			IVersionType versionType,
			ConstructorInfo constructor)
		{
			if (versionUnsavedValue == null)
			{
				if (constructor != null)
				{
					object defaultValue = versionGetter.Get(Instantiate(constructor));
					if (defaultValue != null && defaultValue.GetType().IsValueType)
						return new VersionValue(defaultValue);
					else
					{
						// if the version of a newly instantiated object is not the same
						// as the version seed value, use that as the unsaved-value
						return
							versionType.IsEqual(versionType.Seed(null), defaultValue)
								? VersionValue.VersionUndefined
								: new VersionValue(defaultValue);
					}
				}
				else
				{
					return VersionValue.VersionUndefined;
				}
			}
			else if ("undefined" == versionUnsavedValue)
			{
				return VersionValue.VersionUndefined;
			}
			else if ("null" == versionUnsavedValue)
			{
				return VersionValue.VersionSaveNull;
			}
			else if ("negative" == versionUnsavedValue)
			{
				return VersionValue.VersionNegative;
			}
			else
			{
				// NHibernate-specific
				try
				{
					return new VersionValue(versionType.FromStringValue(versionUnsavedValue));
				}
				catch (InvalidCastException ice)
				{
					throw new MappingException("Bad version type: " + versionType.Name, ice);
				}
				catch (Exception e)
				{
					throw new MappingException("Could not parse version unsaved-value: " + versionUnsavedValue, e);
				}
			}
		}
	}
}