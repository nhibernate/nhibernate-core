//
// In order to convert some functionality to Visual C#, the Java Language Conversion Assistant
// creates "support classes" that duplicate the original functionality.  
//
// Support classes replicate the functionality of the original code, but in some cases they are 
// substantially different architecturally. Although every effort is made to preserve the 
// original architecture of the application in the converted project, the user should be aware that 
// the primary goal of these support classes is to replicate functionality, and that at times 
// the architecture of the resulting solution may differ somewhat.
//

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;

namespace NHibernate.Tool.hbm2net
{
	/// <summary>
	/// Contains conversion support elements such as classes, interfaces and static methods.
	/// </summary>
	public class SupportClass
	{
		/// <summary>
		/// This class contains different methods to manage Collections.
		/// </summary>
		public class CollectionSupport : CollectionBase
		{
			/// <summary>
			/// Creates an instance of the Collection by using an inherited constructor.
			/// </summary>
			public CollectionSupport() : base()
			{
			}

			/// <summary>
			/// Adds an specified element to the collection.
			/// </summary>
			/// <param name="element">The element to be added.</param>
			/// <returns>Returns true if the element was successfuly added. Otherwise returns false.</returns>
			public virtual bool Add(Object element)
			{
				return (this.List.Add(element) != -1);
			}

			/// <summary>
			/// Adds all the elements contained in the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be added.</param>
			/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
			public virtual bool AddAll(ICollection collection)
			{
				bool result = false;
				if (collection != null)
				{
					IEnumerator tempEnumerator = new ArrayList(collection).GetEnumerator();
					while (tempEnumerator.MoveNext())
					{
						if (tempEnumerator.Current != null)
							result = this.Add(tempEnumerator.Current);
					}
				}
				return result;
			}


			/// <summary>
			/// Adds all the elements contained in the specified support class collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be added.</param>
			/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
			public virtual bool AddAll(CollectionSupport collection)
			{
				return this.AddAll((ICollection) collection);
			}

			/// <summary>
			/// Verifies if the specified element is contained into the collection. 
			/// </summary>
			/// <param name="element"> The element that will be verified.</param>
			/// <returns>Returns true if the element is contained in the collection. Otherwise returns false.</returns>
			public virtual bool Contains(Object element)
			{
				return this.List.Contains(element);
			}

			/// <summary>
			/// Verifies if all the elements of the specified collection are contained into the current collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be verified.</param>
			/// <returns>Returns true if all the elements are contained in the collection. Otherwise returns false.</returns>
			public virtual bool ContainsAll(ICollection collection)
			{
				bool result = false;
				IEnumerator tempEnumerator = new ArrayList(collection).GetEnumerator();
				while (tempEnumerator.MoveNext())
					if (!(result = this.Contains(tempEnumerator.Current)))
						break;
				return result;
			}

			/// <summary>
			/// Verifies if all the elements of the specified collection are contained into the current collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be verified.</param>
			/// <returns>Returns true if all the elements are contained in the collection. Otherwise returns false.</returns>
			public virtual bool ContainsAll(CollectionSupport collection)
			{
				return this.ContainsAll((ICollection) collection);
			}

			/// <summary>
			/// Verifies if the collection is empty.
			/// </summary>
			/// <returns>Returns true if the collection is empty. Otherwise returns false.</returns>
			public virtual bool IsEmpty()
			{
				return (this.Count == 0);
			}

			/// <summary>
			/// Removes an specified element from the collection.
			/// </summary>
			/// <param name="element">The element to be removed.</param>
			/// <returns>Returns true if the element was successfuly removed. Otherwise returns false.</returns>
			public virtual bool Remove(Object element)
			{
				bool result = false;
				if (this.Contains(element))
				{
					this.List.Remove(element);
					result = true;
				}
				return result;
			}

			/// <summary>
			/// Removes all the elements contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be removed.</param>
			/// <returns>Returns true if all the elements were successfuly removed. Otherwise returns false.</returns>
			public virtual bool RemoveAll(ICollection collection)
			{
				bool result = false;
				IEnumerator tempEnumerator = new ArrayList(collection).GetEnumerator();
				while (tempEnumerator.MoveNext())
				{
					if (this.Contains(tempEnumerator.Current))
						result = this.Remove(tempEnumerator.Current);
				}
				return result;
			}

			/// <summary>
			/// Removes all the elements contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be removed.</param>
			/// <returns>Returns true if all the elements were successfuly removed. Otherwise returns false.</returns>
			public virtual bool RemoveAll(CollectionSupport collection)
			{
				return this.RemoveAll((ICollection) collection);
			}

			/// <summary>
			/// Removes all the elements that aren't contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to verify the elements that will be retained.</param>
			/// <returns>Returns true if all the elements were successfully removed. Otherwise returns false.</returns>
			public virtual bool RetainAll(ICollection collection)
			{
				bool result = false;
				IEnumerator tempEnumerator = this.GetEnumerator();
				CollectionSupport tempCollection = new CollectionSupport();
				tempCollection.AddAll(collection);
				while (tempEnumerator.MoveNext())
					if (!tempCollection.Contains(tempEnumerator.Current))
					{
						result = this.Remove(tempEnumerator.Current);

						if (result == true)
						{
							tempEnumerator = this.GetEnumerator();
						}
					}
				return result;
			}

			/// <summary>
			/// Removes all the elements that aren't contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to verify the elements that will be retained.</param>
			/// <returns>Returns true if all the elements were successfully removed. Otherwise returns false.</returns>
			public virtual bool RetainAll(CollectionSupport collection)
			{
				return this.RetainAll((ICollection) collection);
			}

			/// <summary>
			/// Obtains an array containing all the elements of the collection.
			/// </summary>
			/// <returns>The array containing all the elements of the collection</returns>
			public virtual Object[] ToArray()
			{
				int index = 0;
				Object[] objects = new Object[this.Count];
				IEnumerator tempEnumerator = this.GetEnumerator();
				while (tempEnumerator.MoveNext())
					objects[index++] = tempEnumerator.Current;
				return objects;
			}

			/// <summary>
			/// Obtains an array containing all the elements of the collection.
			/// </summary>
			/// <param name="objects">The array into which the elements of the collection will be stored.</param>
			/// <returns>The array containing all the elements of the collection.</returns>
			public virtual Object[] ToArray(Object[] objects)
			{
				int index = 0;
				IEnumerator tempEnumerator = this.GetEnumerator();
				while (tempEnumerator.MoveNext())
					objects[index++] = tempEnumerator.Current;
				return objects;
			}

			/// <summary>
			/// Creates a CollectionSupport object with the contents specified in array.
			/// </summary>
			/// <param name="array">The array containing the elements used to populate the new CollectionSupport object.</param>
			/// <returns>A CollectionSupport object populated with the contents of array.</returns>
			public static CollectionSupport ToCollectionSupport(Object[] array)
			{
				CollectionSupport tempCollectionSupport = new CollectionSupport();
				tempCollectionSupport.AddAll(array);
				return tempCollectionSupport;
			}
		}

		/*******************************/

		/// <summary>
		/// This class contains different methods to manage list collections.
		/// </summary>
		public class ListCollectionSupport : ArrayList
		{
			/// <summary>
			/// Creates a new instance of the class ListCollectionSupport.
			/// </summary>
			public ListCollectionSupport() : base()
			{
			}

			/// <summary>
			/// Creates a new instance of the class ListCollectionSupport.
			/// </summary>
			/// <param name="collection">The collection to insert into the new object.</param>
			public ListCollectionSupport(ICollection collection) : base(collection)
			{
			}

			/// <summary>
			/// Creates a new instance of the class ListCollectionSupport with the specified capacity.
			/// </summary>
			/// <param name="capacity">The capacity of the new array.</param>
			public ListCollectionSupport(int capacity) : base(capacity)
			{
			}

			/// <summary>
			/// Adds an object to the end of the List.
			/// </summary>          
			/// <param name="valueToInsert">The value to insert in the array list.</param>
			/// <returns>Returns true after adding the value.</returns>
			public new virtual bool Add(Object valueToInsert)
			{
				base.Insert(this.Count, valueToInsert);
				return true;
			}

			/// <summary>
			/// Adds all the elements contained into the specified collection, starting at the specified position.
			/// </summary>
			/// <param name="index">Position at which to add the first element from the specified collection.</param>
			/// <param name="list">The list used to extract the elements that will be added.</param>
			/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
			public virtual bool AddAll(int index, IList list)
			{
				bool result = false;
				if (list != null)
				{
					IEnumerator tempEnumerator = new ArrayList(list).GetEnumerator();
					int tempIndex = index;
					while (tempEnumerator.MoveNext())
					{
						base.Insert(tempIndex++, tempEnumerator.Current);
						result = true;
					}
				}
				return result;
			}

			public virtual bool AddAll(int index, XmlNodeList list)
			{
				bool result = false;
				if (list != null)
				{
					IEnumerator tempEnumerator = list.GetEnumerator();
					int tempIndex = index;
					while (tempEnumerator.MoveNext())
					{
						base.Insert(tempIndex++, tempEnumerator.Current);
						result = true;
					}
				}
				return result;
			}

			/// <summary>
			/// Adds all the elements contained in the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be added.</param>
			/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
			public virtual bool AddAll(IList collection)
			{
				return this.AddAll(this.Count, collection);
			}

			public virtual bool AddAll(XmlNodeList collection)
			{
				return this.AddAll(this.Count, collection);
			}

			/// <summary>
			/// Adds all the elements contained in the specified support class collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be added.</param>
			/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
			public virtual bool AddAll(CollectionSupport collection)
			{
				return this.AddAll(this.Count, collection);
			}

			/// <summary>
			/// Adds all the elements contained into the specified support class collection, starting at the specified position.
			/// </summary>
			/// <param name="index">Position at which to add the first element from the specified collection.</param>
			/// <param name="collection">The list used to extract the elements that will be added.</param>
			/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
			public virtual bool AddAll(int index, CollectionSupport collection)
			{
				return this.AddAll(index, (IList) collection);
			}

			/// <summary>
			/// Creates a copy of the ListCollectionSupport.
			/// </summary>
			/// <returns> A copy of the ListCollectionSupport.</returns>
			public virtual object ListCollectionClone()
			{
				return MemberwiseClone();
			}


			/// <summary>
			/// Returns an iterator of the collection.
			/// </summary>
			/// <returns>An IEnumerator.</returns>
			public virtual IEnumerator ListIterator()
			{
				return base.GetEnumerator();
			}

			/// <summary>
			/// Removes all the elements contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be removed.</param>
			/// <returns>Returns true if all the elements were successfuly removed. Otherwise returns false.</returns>
			public virtual bool RemoveAll(ICollection collection)
			{
				bool result = false;
				IEnumerator tempEnumerator = new ArrayList(collection).GetEnumerator();
				while (tempEnumerator.MoveNext())
				{
					result = true;
					if (base.Contains(tempEnumerator.Current))
						base.Remove(tempEnumerator.Current);
				}
				return result;
			}

			/// <summary>
			/// Removes all the elements contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be removed.</param>
			/// <returns>Returns true if all the elements were successfuly removed. Otherwise returns false.</returns>
			public virtual bool RemoveAll(CollectionSupport collection)
			{
				return this.RemoveAll((ICollection) collection);
			}

			/// <summary>
			/// Removes the value in the specified index from the list.
			/// </summary>          
			/// <param name="index">The index of the value to remove.</param>
			/// <returns>Returns the value removed.</returns>
			public virtual object RemoveElement(int index)
			{
				object objectRemoved = this[index];
				this.RemoveAt(index);
				return objectRemoved;
			}

			/// <summary>
			/// Removes an specified element from the collection.
			/// </summary>
			/// <param name="element">The element to be removed.</param>
			/// <returns>Returns true if the element was successfuly removed. Otherwise returns false.</returns>
			public virtual bool RemoveElement(Object element)
			{
				bool result = false;
				if (this.Contains(element))
				{
					base.Remove(element);
					result = true;
				}
				return result;
			}

			/// <summary>
			/// Removes the first value from an array list.
			/// </summary>          
			/// <returns>Returns the value removed.</returns>
			public virtual object RemoveFirst()
			{
				object objectRemoved = this[0];
				this.RemoveAt(0);
				return objectRemoved;
			}

			/// <summary>
			/// Removes the last value from an array list.
			/// </summary>
			/// <returns>Returns the value removed.</returns>
			public virtual object RemoveLast()
			{
				object objectRemoved = this[this.Count - 1];
				base.RemoveAt(this.Count - 1);
				return objectRemoved;
			}

			/// <summary>
			/// Removes all the elements that aren't contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to verify the elements that will be retained.</param>
			/// <returns>Returns true if all the elements were successfully removed. Otherwise returns false.</returns>
			public virtual bool RetainAll(ICollection collection)
			{
				bool result = false;
				IEnumerator tempEnumerator = this.GetEnumerator();
				ListCollectionSupport tempCollection = new ListCollectionSupport(collection);
				while (tempEnumerator.MoveNext())
					if (!tempCollection.Contains(tempEnumerator.Current))
					{
						result = this.RemoveElement(tempEnumerator.Current);

						if (result == true)
						{
							tempEnumerator = this.GetEnumerator();
						}
					}
				return result;
			}

			/// <summary>
			/// Removes all the elements that aren't contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to verify the elements that will be retained.</param>
			/// <returns>Returns true if all the elements were successfully removed. Otherwise returns false.</returns>
			public virtual bool RetainAll(CollectionSupport collection)
			{
				return this.RetainAll((ICollection) collection);
			}

			/// <summary>
			/// Verifies if all the elements of the specified collection are contained into the current collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be verified.</param>
			/// <returns>Returns true if all the elements are contained in the collection. Otherwise returns false.</returns>
			public virtual bool ContainsAll(ICollection collection)
			{
				bool result = false;
				IEnumerator tempEnumerator = new ArrayList(collection).GetEnumerator();
				while (tempEnumerator.MoveNext())
					if (!(result = this.Contains(tempEnumerator.Current)))
						break;
				return result;
			}

			/// <summary>
			/// Verifies if all the elements of the specified collection are contained into the current collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be verified.</param>
			/// <returns>Returns true if all the elements are contained in the collection. Otherwise returns false.</returns>
			public virtual bool ContainsAll(CollectionSupport collection)
			{
				return this.ContainsAll((ICollection) collection);
			}

			/// <summary>
			/// Returns a new list containing a portion of the current list between a specified range. 
			/// </summary>
			/// <param name="startIndex">The start index of the range.</param>
			/// <param name="endIndex">The end index of the range.</param>
			/// <returns>A ListCollectionSupport instance containing the specified elements.</returns>
			public virtual ListCollectionSupport SubList(int startIndex, int endIndex)
			{
				int index = 0;
				IEnumerator tempEnumerator = this.GetEnumerator();
				ListCollectionSupport result = new ListCollectionSupport();
				for (index = startIndex; index < endIndex; index++)
					result.Add(this[index]);
				return result;
			}

			/// <summary>
			/// Obtains an array containing all the elements of the collection.
			/// </summary>
			/// <param name="objects">The array into which the elements of the collection will be stored.</param>
			/// <returns>The array containing all the elements of the collection.</returns>
			public virtual Object[] ToArray(Object[] objects)
			{
				if (objects.Length < this.Count)
					objects = new Object[this.Count];
				int index = 0;
				IEnumerator tempEnumerator = this.GetEnumerator();
				while (tempEnumerator.MoveNext())
					objects[index++] = tempEnumerator.Current;
				return objects;
			}

			/// <summary>
			/// Returns an iterator of the collection starting at the specified position.
			/// </summary>
			/// <param name="index">The position to set the iterator.</param>
			/// <returns>An IEnumerator at the specified position.</returns>
			public virtual IEnumerator ListIterator(int index)
			{
				if ((index < 0) || (index > this.Count)) throw new IndexOutOfRangeException();
				IEnumerator tempEnumerator = this.GetEnumerator();
				if (index > 0)
				{
					int i = 0;
					while ((tempEnumerator.MoveNext()) && (i < index - 1))
						i++;
				}
				return tempEnumerator;
			}

			/// <summary>
			/// Gets the last value from a list.
			/// </summary>
			/// <returns>Returns the last element of the list.</returns>
			public virtual object GetLast()
			{
				if (this.Count == 0) throw new ArgumentOutOfRangeException();
				else
				{
					return this[this.Count - 1];
				}
			}

			/// <summary>
			/// Return whether this list is empty.
			/// </summary>
			/// <returns>True if the list is empty, false if it isn't.</returns>
			public virtual bool IsEmpty()
			{
				return (this.Count == 0);
			}

			/// <summary>
			/// Replaces the element at the specified position in this list with the specified element.
			/// </summary>
			/// <param name="index">Index of element to replace.</param>
			/// <param name="element">Element to be stored at the specified position.</param>
			/// <returns>The element previously at the specified position.</returns>
			public virtual object Set(int index, Object element)
			{
				object result = this[index];
				this[index] = element;
				return result;
			}

			/// <summary>
			/// Returns the element at the specified position in the list.
			/// </summary>
			/// <param name="index">Index of element to return.</param>
			/// <returns>The element at the specified position in the list.</returns>
			public virtual object Get(int index)
			{
				return this[index];
			}
		}

		/*******************************/

		/// <summary>
		/// This class manages a set of elements.
		/// </summary>
		public class SetSupport : ArrayList
		{
			/// <summary>
			/// Creates a new set.
			/// </summary>
			public SetSupport() : base()
			{
			}

			/// <summary>
			/// Creates a new set initialized with System.Collections.ICollection object
			/// </summary>
			/// <param name="collection">System.Collections.ICollection object to initialize the set object</param>
			public SetSupport(ICollection collection) : base(collection)
			{
			}

			/// <summary>
			/// Creates a new set initialized with a specific capacity.
			/// </summary>
			/// <param name="capacity">value to set the capacity of the set object</param>
			public SetSupport(int capacity) : base(capacity)
			{
			}

			/// <summary>
			/// Adds an element to the set.
			/// </summary>
			/// <param name="objectToAdd">The object to be added.</param>
			/// <returns>True if the object was added, false otherwise.</returns>
			new public virtual bool Add(object objectToAdd)
			{
				if (this.Contains(objectToAdd))
					return false;
				else
				{
					base.Add(objectToAdd);
					return true;
				}
			}

			/// <summary>
			/// Adds all the elements contained in the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be added.</param>
			/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
			public virtual bool AddAll(ICollection collection)
			{
				bool result = false;
				if (collection != null)
				{
					IEnumerator tempEnumerator = new ArrayList(collection).GetEnumerator();
					while (tempEnumerator.MoveNext())
					{
						if (tempEnumerator.Current != null)
							result = this.Add(tempEnumerator.Current);
					}
				}
				return result;
			}

			/// <summary>
			/// Adds all the elements contained in the specified support class collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be added.</param>
			/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
			public virtual bool AddAll(CollectionSupport collection)
			{
				return this.AddAll((ICollection) collection);
			}

			/// <summary>
			/// Verifies that all the elements of the specified collection are contained into the current collection. 
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be verified.</param>
			/// <returns>True if the collection contains all the given elements.</returns>
			public virtual bool ContainsAll(ICollection collection)
			{
				bool result = false;
				IEnumerator tempEnumerator = collection.GetEnumerator();
				while (tempEnumerator.MoveNext())
					if (!(result = this.Contains(tempEnumerator.Current)))
						break;
				return result;
			}

			/// <summary>
			/// Verifies if all the elements of the specified collection are contained into the current collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be verified.</param>
			/// <returns>Returns true if all the elements are contained in the collection. Otherwise returns false.</returns>
			public virtual bool ContainsAll(CollectionSupport collection)
			{
				return this.ContainsAll((ICollection) collection);
			}

			/// <summary>
			/// Verifies if the collection is empty.
			/// </summary>
			/// <returns>True if the collection is empty, false otherwise.</returns>
			public virtual bool IsEmpty()
			{
				return (this.Count == 0);
			}

			/// <summary>
			/// Removes an element from the set.
			/// </summary>
			/// <param name="elementToRemove">The element to be removed.</param>
			/// <returns>True if the element was removed.</returns>
			new public virtual bool Remove(object elementToRemove)
			{
				bool result = false;
				if (this.Contains(elementToRemove))
					result = true;
				base.Remove(elementToRemove);
				return result;
			}

			/// <summary>
			/// Removes all the elements contained in the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be removed.</param>
			/// <returns>True if all the elements were successfuly removed, false otherwise.</returns>
			public virtual bool RemoveAll(ICollection collection)
			{
				bool result = false;
				IEnumerator tempEnumerator = collection.GetEnumerator();
				while (tempEnumerator.MoveNext())
				{
					if ((result == false) && (this.Contains(tempEnumerator.Current)))
						result = true;
					this.Remove(tempEnumerator.Current);
				}
				return result;
			}

			/// <summary>
			/// Removes all the elements contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to extract the elements that will be removed.</param>
			/// <returns>Returns true if all the elements were successfuly removed. Otherwise returns false.</returns>
			public virtual bool RemoveAll(CollectionSupport collection)
			{
				return this.RemoveAll((ICollection) collection);
			}

			/// <summary>
			/// Removes all the elements that aren't contained in the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to verify the elements that will be retained.</param>
			/// <returns>True if all the elements were successfully removed, false otherwise.</returns>
			public virtual bool RetainAll(ICollection collection)
			{
				bool result = false;
				IEnumerator tempEnumerator = collection.GetEnumerator();
				SetSupport tempSet = (SetSupport) collection;
				while (tempEnumerator.MoveNext())
					if (!tempSet.Contains(tempEnumerator.Current))
					{
						result = this.Remove(tempEnumerator.Current);
						tempEnumerator = this.GetEnumerator();
					}
				return result;
			}

			/// <summary>
			/// Removes all the elements that aren't contained into the specified collection.
			/// </summary>
			/// <param name="collection">The collection used to verify the elements that will be retained.</param>
			/// <returns>Returns true if all the elements were successfully removed. Otherwise returns false.</returns>
			public virtual bool RetainAll(CollectionSupport collection)
			{
				return this.RetainAll((ICollection) collection);
			}

			/// <summary>
			/// Obtains an array containing all the elements of the collection.
			/// </summary>
			/// <returns>The array containing all the elements of the collection.</returns>
			new public virtual object[] ToArray()
			{
				int index = 0;
				object[] tempObject = new object[this.Count];
				IEnumerator tempEnumerator = this.GetEnumerator();
				while (tempEnumerator.MoveNext())
					tempObject[index++] = tempEnumerator.Current;
				return tempObject;
			}

			/// <summary>
			/// Obtains an array containing all the elements in the collection.
			/// </summary>
			/// <param name="objects">The array into which the elements of the collection will be stored.</param>
			/// <returns>The array containing all the elements of the collection.</returns>
			public virtual object[] ToArray(object[] objects)
			{
				int index = 0;
				IEnumerator tempEnumerator = this.GetEnumerator();
				while (tempEnumerator.MoveNext())
					objects[index++] = tempEnumerator.Current;
				return objects;
			}
		}

		/*******************************/

		/// <summary>
		/// This class manages a tree set collection of sorted elements.
		/// </summary>
		public class TreeSetSupport : SortedSetSupport
		{
			/// <summary>
			/// Creates a new TreeSetSupport.
			/// </summary>
			public TreeSetSupport()
			{
			}

			/// <summary>
			/// Create a new TreeSetSupport with a specific collection.
			/// </summary>
			/// <param name="collection">The collection used to iniciatilize the TreeSetSupport</param>
			public TreeSetSupport(ICollection collection) : base(collection)
			{
			}

			/// <summary>
			/// Creates a copy of the TreeSetSupport.
			/// </summary>
			/// <returns>A copy of the TreeSetSupport.</returns>
			public virtual object TreeSetClone()
			{
				TreeSetSupport internalClone = new TreeSetSupport();
				internalClone.AddAll(this);
				return internalClone;
			}

			/// <summary>
			/// Retrieves the number of elements contained in the set.
			/// </summary>
			/// <returns>An interger value that represent the number of element in the set.</returns>
			public virtual int Size()
			{
				return this.Count;
			}
		}

		/*******************************/

		/// <summary> 
		/// This class contains methods to manage a sorted collection.
		/// </summary>
		public class SortedSetSupport : SetSupport
		{
			/// <summary>
			/// Creates a new SortedSetSupport.
			/// </summary>
			public SortedSetSupport() : base()
			{
			}

			/// <summary>
			/// Create a new SortedSetSupport with a specific collection.
			/// </summary>
			/// <param name="collection">The collection used to iniciatilize the SortedSetSupport</param>
			public SortedSetSupport(ICollection collection) : base(collection)
			{
			}

			/// <summary>
			/// Returns the first element from the set.
			/// </summary>
			/// <returns>Returns the first element from the set.</returns>
			public virtual object First()
			{
				IEnumerator tempEnumerator = this.GetEnumerator();
				tempEnumerator.MoveNext();
				return tempEnumerator.Current;
			}

			/// <summary>
			/// Returns a view of elements until the specified element.
			/// </summary>
			/// <returns>Returns a sorted set of elements that are strictly less than the specified element.</returns>
			public virtual SortedSetSupport HeadSet(Object toElement)
			{
				SortedSetSupport tempSortedSet = new SortedSetSupport();
				IEnumerator tempEnumerator = this.GetEnumerator();
				while ((tempEnumerator.MoveNext() && ((tempEnumerator.Current.ToString().CompareTo(toElement.ToString())) < 0)))
					tempSortedSet.Add(tempEnumerator.Current);
				return tempSortedSet;
			}

			/// <summary>
			/// Returns the last element of the set.
			/// </summary>
			/// <returns>Returns the last element from the set.</returns>
			public virtual object Last()
			{
				IEnumerator tempEnumerator = this.GetEnumerator();
				object element = null;
				while (tempEnumerator.MoveNext())
					if (tempEnumerator.Current != null)
						element = tempEnumerator.Current;
				return element;
			}

			/// <summary>
			/// Returns a view of elements from the specified element.
			/// </summary>
			/// <returns>Returns a sorted set of elements that are greater or equal to the specified element.</returns>
			public virtual SortedSetSupport TailSet(Object fromElement)
			{
				SortedSetSupport tempSortedSet = new SortedSetSupport();
				IEnumerator tempEnumerator = this.GetEnumerator();
				while ((tempEnumerator.MoveNext() && (!((Int32) tempEnumerator.Current >= (Int32) fromElement))))
					tempSortedSet.Add(tempEnumerator.Current);
				return tempSortedSet;
			}

			/// <summary>
			/// Returns a view of elements between the specified elements.
			/// </summary>
			/// <returns>Returns a sorted set of elements from the first specified element to the second specified element.</returns>
			public virtual SortedSetSupport SubSet(Object fromElement, Object toElement)
			{
				SortedSetSupport tempSortedSet = new SortedSetSupport();
				IEnumerator tempEnumerator = this.GetEnumerator();
				while ((tempEnumerator.MoveNext() && ((!((Int32) tempEnumerator.Current >= (Int32) fromElement))) && (!((Int32) tempEnumerator.Current < (Int32) toElement))))
					tempSortedSet.Add(tempEnumerator.Current);
				return tempSortedSet;
			}
		}

		/*******************************/

		/// <summary>
		/// This class manages different operation with collections.
		/// </summary>
		public class AbstractSetSupport : SetSupport
		{
			/// <summary>
			/// The constructor with no parameters to create an abstract set.
			/// </summary>
			public AbstractSetSupport()
			{
			}
		}


		/*******************************/

		/// <summary> 
		/// This class manages a hash set of elements.
		/// </summary>
		public class HashSetSupport : AbstractSetSupport
		{
			/// <summary>
			/// Creates a new hash set collection.
			/// </summary>
			public HashSetSupport()
			{
			}

			/// <summary>
			/// Creates a new hash set collection.
			/// </summary>
			/// <param name="collection">The collection to initialize the hash set with.</param>
			public HashSetSupport(ICollection collection)
			{
				this.AddRange(collection);
			}

			/// <summary>
			/// Creates a new hash set with the given capacity.
			/// </summary>
			/// <param name="capacity">The initial capacity of the hash set.</param>
			public HashSetSupport(int capacity)
			{
				this.Capacity = capacity;
			}

			/// <summary>
			/// Creates a new hash set with the given capacity.
			/// </summary>
			/// <param name="capacity">The initial capacity of the hash set.</param>
			/// <param name="loadFactor">The load factor of the hash set.</param>
			public HashSetSupport(int capacity, float loadFactor)
			{
				this.Capacity = capacity;
			}

			/// <summary>
			/// Creates a copy of the HashSetSupport.
			/// </summary>
			/// <returns> A copy of the HashSetSupport.</returns>
			public virtual object HashSetClone()
			{
				return MemberwiseClone();
			}
		}

		/*******************************/

		/// <summary>
		/// Creates an instance of a received Type.
		/// </summary>
		/// <param name="classType">The Type of the new class instance to return.</param>
		/// <returns>An Object containing the new instance.</returns>
		public static object CreateNewInstance(System.Type classType)
		{
			if (classType == null) throw new Exception("Class not found");
			object instance = null;
			System.Type[] constructor = new System.Type[] {};
			ConstructorInfo[] constructors = null;

			constructors = classType.GetConstructors();

			if (constructors.Length == 0)
				throw new UnauthorizedAccessException();
			else
			{
				for (int i = 0; i < constructors.Length; i++)
				{
					ParameterInfo[] parameters = constructors[i].GetParameters();

					if (parameters.Length == 0)
					{
						instance = classType.GetConstructor(constructor).Invoke(new Object[] {});
						break;
					}
					else if (i == constructors.Length - 1)
						throw new MethodAccessException();
				}
			}
			return instance;
		}


		/*******************************/

		/// <summary>
		/// Writes the exception stack trace to the received stream
		/// </summary>
		/// <param name="throwable">Exception to obtain information from</param>
		/// <param name="stream">Output sream used to write to</param>
		public static void WriteStackTrace(Exception throwable, TextWriter stream)
		{
			stream.Write(throwable.ToString());
			stream.Flush();
		}

		/*******************************/

		/// <summary>
		/// Adds a new key-and-value pair into the hash table
		/// </summary>
		/// <param name="collection">The collection to work with</param>
		/// <param name="key">Key used to obtain the value</param>
		/// <param name="newValue">Value asociated with the key</param>
		/// <returns>The old element associated with the key</returns>
		public static object PutElement(IDictionary collection, Object key, Object newValue)
		{
			object element = collection[key];
			collection[key] = newValue;
			return element;
		}

		/*******************************/

		/// <summary>
		/// Copies all of the elements from the source Dictionary to target Dictionary. These elements will replace any elements that 
		/// target Dictionary had for any of the elements currently in the source dictionary.
		/// </summary>
		/// <param name="target">Target Dictionary.</param>
		/// <param name="source">Source Dictionary.</param>
		public static void PutAll(IDictionary target, IDictionary source)
		{
			ICollection tempCollection1 = source.Keys;
			ICollection tempCollection2 = source.Values;

			Array tempArray1 = Array.CreateInstance(typeof (Object), tempCollection1.Count);
			Array tempArray2 = Array.CreateInstance(typeof (Object), tempCollection2.Count);

			Int32 tempInt1 = new Int32();
			Int32 tempInt2 = new Int32();

			tempCollection1.CopyTo(tempArray1, tempInt1);
			tempCollection2.CopyTo(tempArray2, tempInt2);

			for (long index = 0; index < tempCollection1.Count; index++)
				target[tempArray1.GetValue(index)] = tempArray2.GetValue(index);
		}

	}
}