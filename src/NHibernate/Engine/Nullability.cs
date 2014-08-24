using System.Collections;
using System.Text;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary> 
	/// Implements the algorithm for validating property values
	/// for illegal null values
	/// </summary>
	public sealed class Nullability
	{
		private ISessionImplementor session;

		public Nullability(ISessionImplementor session)
		{
			this.session = session;
		}

		/// <summary> 
		/// Check nullability of the class persister properties
		/// </summary>
		/// <param name="values">entity properties </param>
		/// <param name="persister">class persister </param>
		/// <param name="isUpdate">wether it is intended to be updated or saved </param>
		public void CheckNullability(object[] values, IEntityPersister persister, bool isUpdate)
		{
			/*
			* Algorithm
			* Check for any level one nullability breaks
			* Look at non null components to
			*   recursively check next level of nullability breaks
			* Look at Collections contraining component to
			*   recursively check next level of nullability breaks
			*
			*
			* In the previous implementation, not-null stuffs where checked
			* filtering by level one only updateable
			* or insertable columns. So setting a sub component as update="false"
			* has no effect on not-null check if the main component had good checkeability
			* In this implementation, we keep this feature.
			* However, I never see any documentation mentioning that, but it's for
			* sure a limitation.
			*/

			bool[] nullability = persister.PropertyNullability;
			bool[] checkability = isUpdate ? persister.PropertyUpdateability : persister.PropertyInsertability;
			IType[] propertyTypes = persister.PropertyTypes;

			for (int i = 0; i < values.Length; i++)
			{
				if (checkability[i])
				{
					object value = values[i];
					if (!nullability[i] && value == null)
					{
						//check basic level one nullablilty
						throw new PropertyValueException("not-null property references a null or transient value", persister.EntityName, persister.PropertyNames[i]);
					}
					else if (value != null)
					{
						//values is not null and is checkable, we'll look deeper
						string breakProperties = CheckSubElementsNullability(propertyTypes[i], value);
						if (breakProperties != null)
						{
							throw new PropertyValueException("not-null property references a null or transient value", persister.EntityName, BuildPropertyPath(persister.PropertyNames[i], breakProperties));
						}
					}
				}
			}
		}

		/// <summary> 
		/// Check sub elements-nullability. Returns property path that break
		/// nullability or null if none 
		/// </summary>
		/// <param name="propertyType">type to check </param>
		/// <param name="value">value to check </param>
		/// <returns> property path </returns>
		private string CheckSubElementsNullability(IType propertyType, object value)
		{
			//for non null args, check for components and elements containing components
			if (propertyType.IsComponentType)
			{
				return CheckComponentNullability(value, (IAbstractComponentType)propertyType);
			}
			else if (propertyType.IsCollectionType)
			{
				//persistent collections may have components
				CollectionType collectionType = (CollectionType)propertyType;
				IType collectionElementType = collectionType.GetElementType(session.Factory);
				if (collectionElementType.IsComponentType)
				{
					//check for all components values in the collection
					IAbstractComponentType componentType = (IAbstractComponentType)collectionElementType;
					IEnumerable ec = CascadingAction.GetLoadedElementsIterator(session, collectionType, value);
					foreach (object compValue in ec)
					{
						if (compValue != null)
							return CheckComponentNullability(compValue, componentType);
					}
				}
			}
			return null;
		}

		/// <summary> 
		/// Check component nullability. Returns property path that break
		/// nullability or null if none 
		/// </summary>
		/// <param name="value">component properties </param>
		/// <param name="compType">component not-nullable type </param>
		/// <returns> property path </returns>
		private string CheckComponentNullability(object value, IAbstractComponentType compType)
		{
			// will check current level if some of them are not null or sublevels if they exist
			bool[] nullability = compType.PropertyNullability;
			if (nullability != null)
			{
				//do the test
				object[] values = compType.GetPropertyValues(value, session.EntityMode);
				IType[] propertyTypes = compType.Subtypes;
				for (int i = 0; i < values.Length; i++)
				{
					object subvalue = values[i];
					if (!nullability[i] && subvalue == null)
					{
						return compType.PropertyNames[i];
					}
					else if (subvalue != null)
					{
						string breakProperties = CheckSubElementsNullability(propertyTypes[i], subvalue);
						if (breakProperties != null)
						{
							return BuildPropertyPath(compType.PropertyNames[i], breakProperties);
						}
					}
				}
			}
			return null;
		}

		/// <summary> 
		/// Return a well formed property path.
		/// Basicaly, it will return parent.child 
		/// </summary>
		/// <param name="parent">parent in path </param>
		/// <param name="child">child in path </param>
		/// <returns> parent-child path</returns>
		private static string BuildPropertyPath(string parent, string child)
		{
			return new StringBuilder(parent.Length + child.Length + 1)
				.Append(parent)
				.Append('.')
				.Append(child)
				.ToString();
		}
	}
}
