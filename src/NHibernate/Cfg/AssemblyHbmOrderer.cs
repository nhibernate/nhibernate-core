using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;

using Iesi.Collections;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Analyzes the contents of the <c>hbm.xml</c> files embedded in the 
	/// <see cref="Assembly"/> for their dependency order.
	/// </summary>
	/// <remarks>
	/// This solves the problem caused when you have embedded <c>hbm.xml</c> files
	/// that contain subclasses/joined-subclasses that make use of the <c>extends</c>
	/// attribute.  This ensures that those subclasses/joined-subclasses will not be
	/// processed until after the class they extend is processed.
	/// </remarks>
	public class AssemblyHbmOrderer
	{
		private readonly IList _unorderedResourceEntries = new ArrayList();

		private readonly Queue _orderedResources = new Queue();

		private readonly ISet _processedClassNames = new HashedSet();

		/// <summary>
		/// Creates a new instance of AssemblyHbmOrderer with the specified <paramref name="resourceNames" />
		/// added.
		/// </summary>
		public static AssemblyHbmOrderer CreateWithResources(Assembly assembly, IEnumerable resourceNames)
		{
			AssemblyHbmOrderer result = new AssemblyHbmOrderer();
			result.AddResources(assembly, resourceNames);
			return result;
		}

		public void AddResources(Assembly assembly, IEnumerable resourceNames)
		{
			foreach (string resource in resourceNames)
			{
				AddResource(assembly, resource);
			}
		}

		/// <summary>
		/// Adds the specified resource to the resources being ordered.
		/// </summary>
		/// <param name="assembly">Assembly containing the resource.</param>
		/// <param name="resourceName">Name of the embedded resource.</param>
		public void AddResource(Assembly assembly, string resourceName)
		{
			EmbeddedResource resource = new EmbeddedResource(assembly, resourceName);
			using (Stream xmlInputStream = resource.OpenStream())
			{
				ResourceEntry re = new ResourceEntry(
					resource,
					ClassExtractor.GetClassEntries(xmlInputStream));
				AddResourceEntry(re);
			}
		}

		private void AddResourceEntry(ResourceEntry re)
		{
			if (CanProcess(re))
			{
				EnqueueResource(re);
				ProcessUnorderedResources();
			}
			else
			{
				_unorderedResourceEntries.Add(re);
			}
		}

		/// <summary>
		/// Gets an <see cref="EmbeddedResource" /> that can now be processed (i.e.
		/// that doesn't depend on resources not yet processed).
		/// </summary>
		/// <returns></returns>
		public EmbeddedResource GetNextAvailableResource()
		{
			if (_orderedResources.Count == 0)
			{
				CheckNoUnorderedResources();
				return null;
			}

			return (EmbeddedResource) _orderedResources.Dequeue();
		}

		private void ProcessUnorderedResources()
		{
			ResourceEntry found;

			while ((found = FindAvailableResourceEntry()) != null)
			{
				EnqueueResource(found);
				_unorderedResourceEntries.Remove(found);
			}
		}

		private ResourceEntry FindAvailableResourceEntry()
		{
			foreach (ResourceEntry re in _unorderedResourceEntries)
			{
				if (CanProcess(re))
				{
					return re;
				}
			}
			return null;
		}

		private void EnqueueResource(ResourceEntry resourceEntry)
		{
			_processedClassNames.AddAll(resourceEntry.ContainedClassNames);
			_orderedResources.Enqueue(resourceEntry.Resource);
		}

		private bool CanProcess(ResourceEntry ce)
		{
			return _processedClassNames.ContainsAll(ce.RequiredClassNames);
		}

		private void CheckNoUnorderedResources()
		{
			if (_unorderedResourceEntries.Count > 0)
			{
				throw new MappingException(FormatExceptionMessage(_unorderedResourceEntries));
			}
		}

		private static string FormatExceptionMessage(ICollection resourceEntries)
		{
			StringBuilder message = new StringBuilder(
				"These resources contain classes that extend unmapped classes:");

			foreach (ResourceEntry resourceEntry in resourceEntries)
			{
				message.Append('\n')
					.Append(resourceEntry.Resource);
			}

			return message.ToString();
		}
	}
}