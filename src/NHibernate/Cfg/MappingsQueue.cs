using System.Collections;
using System.Text;
using Iesi.Collections.Generic;
using System.Collections.Generic;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Queues mapping files according to their dependency order.
	/// </summary>
	public class MappingsQueue
	{
		private readonly ISet<string> _processedClassNames = new HashedSet<string>();

		private readonly List<MappingsQueueEntry> _unavailableEntries = new List<MappingsQueueEntry>();
		private readonly Queue _availableEntries = new Queue();

		/// <summary>
		/// Adds the specified document to the queue.
		/// </summary>
		public void AddDocument(NamedXmlDocument document)
		{
			MappingsQueueEntry re = new MappingsQueueEntry(document, ClassExtractor.GetClassEntries(document.Document));
			AddEntry(re);
		}

		/// <summary>
		/// Gets a <see cref="NamedXmlDocument" /> that can now be processed (i.e.
		/// that doesn't depend on classes not yet processed).
		/// </summary>
		/// <returns></returns>
		public NamedXmlDocument GetNextAvailableResource()
		{
			if (_availableEntries.Count == 0)
			{
				return null;
			}

			MappingsQueueEntry entry = (MappingsQueueEntry)_availableEntries.Dequeue();
			AddProcessedClassNames(entry.ContainedClassNames);
			return entry.Document;
		}

		/// <summary>
		/// Checks that no unprocessed documents remain in the queue.
		/// </summary>
		public void CheckNoUnavailableEntries()
		{
			if (_unavailableEntries.Count > 0)
			{
				throw new MappingException(FormatExceptionMessage(_unavailableEntries));
			}
		}

		private void AddProcessedClassNames(ICollection<string> classNames)
		{
			_processedClassNames.AddAll(classNames);
			if (classNames.Count > 0)
			{
				ProcessUnavailableEntries();
			}
		}

		private void AddEntry(MappingsQueueEntry re)
		{
			if (CanProcess(re))
			{
				_availableEntries.Enqueue(re);
			}
			else
			{
				_unavailableEntries.Add(re);
			}
		}

		private void ProcessUnavailableEntries()
		{
			MappingsQueueEntry found;

			while ((found = FindAvailableResourceEntry()) != null)
			{
				_availableEntries.Enqueue(found);
				_unavailableEntries.Remove(found);
			}
		}

		private MappingsQueueEntry FindAvailableResourceEntry()
		{
			foreach (MappingsQueueEntry re in _unavailableEntries)
			{
				if (CanProcess(re))
				{
					return re;
				}
			}
			return null;
		}

		private bool CanProcess(MappingsQueueEntry ce)
		{
			foreach (var c in ce.RequiredClassNames)
			{
				if (!(_processedClassNames.Contains(c.FullClassName) || _processedClassNames.Contains(c.EntityName)))
					return false;
			}
			return true;
		}

		private static string FormatExceptionMessage(ICollection resourceEntries)
		{
			StringBuilder message = new StringBuilder(
				"These classes referenced by 'extends' were not found:");

			foreach (MappingsQueueEntry resourceEntry in resourceEntries)
			{
				foreach (var className in resourceEntry.RequiredClassNames)
				{
					message.Append('\n').Append(className);
				}
			}

			return message.ToString();
		}
	}
}