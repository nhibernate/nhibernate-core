using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Queues mapping files according to their dependency order.
	/// </summary>
	public class MappingsQueue
	{
		private readonly Queue availableEntries = new Queue();
		private readonly ISet<string> processedClassNames = new HashSet<string>();

		private readonly List<MappingsQueueEntry> unavailableEntries = new List<MappingsQueueEntry>();

		/// <summary>
		/// Adds the specified document to the queue.
		/// </summary>
		public void AddDocument(NamedXmlDocument document)
		{
			var re = new MappingsQueueEntry(document, ClassExtractor.GetClassEntries(document.Document));
			AddEntry(re);
		}

		/// <summary>
		/// Gets a <see cref="NamedXmlDocument" /> that can now be processed (i.e.
		/// that doesn't depend on classes not yet processed).
		/// </summary>
		/// <returns></returns>
		public NamedXmlDocument GetNextAvailableResource()
		{
			if (availableEntries.Count == 0)
			{
				return null;
			}

			var entry = (MappingsQueueEntry) availableEntries.Dequeue();
			AddProcessedClassNames(entry.ContainedClassNames);
			return entry.Document;
		}

		/// <summary>
		/// Checks that no unprocessed documents remain in the queue.
		/// </summary>
		public void CheckNoUnavailableEntries()
		{
			if (unavailableEntries.Count > 0)
			{
				throw new MappingException(FormatExceptionMessage(unavailableEntries));
			}
		}

		private void AddProcessedClassNames(ICollection<string> classNames)
		{
			processedClassNames.UnionWith(classNames);
			if (classNames.Count > 0)
			{
				ProcessUnavailableEntries();
			}
		}

		private void AddEntry(MappingsQueueEntry re)
		{
			if (CanProcess(re))
			{
				availableEntries.Enqueue(re);
			}
			else
			{
				unavailableEntries.Add(re);
			}
		}

		private void ProcessUnavailableEntries()
		{
			MappingsQueueEntry found;

			while ((found = FindAvailableResourceEntry()) != null)
			{
				availableEntries.Enqueue(found);
				unavailableEntries.Remove(found);
			}
		}

		private MappingsQueueEntry FindAvailableResourceEntry()
		{
			return unavailableEntries.FirstOrDefault(CanProcess);
		}

		private bool CanProcess(MappingsQueueEntry ce)
		{
			return
				ce.RequiredClassNames.All(
					c => (processedClassNames.Contains(c.FullClassName) || processedClassNames.Contains(c.EntityName)));
		}

		private static string FormatExceptionMessage(IEnumerable<MappingsQueueEntry> resourceEntries)
		{
			var message = new StringBuilder(500);
			message.Append("These classes referenced by 'extends' were not found:");
			foreach (MappingsQueueEntry.RequiredEntityName className in
				resourceEntries.SelectMany(resourceEntry => resourceEntry.RequiredClassNames))
			{
				message.Append('\n').Append(className);
			}

			return message.ToString();
		}
	}
}