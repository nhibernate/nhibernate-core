using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

using NHibernate.Cfg.MappingSchema;

using NUnit.Framework;

namespace NHibernate.Test.CfgTest
{
	[TestFixture]
	public class MappingDocumentAggregatorTests
	{
		private readonly Assembly domainModelAssembly = typeof (DomainModel.A).Assembly;

		[Test]
		public void CanAddDomainModelAssembly()
		{
			MappingDocumentAggregator aggregator = new MappingDocumentAggregator();
			aggregator.Add(domainModelAssembly);
			IList<HbmMapping> results = aggregator.List();
			Assert.IsTrue(results.Count > 0); // 54
		}

		[Test]
		public void CanSerializeAndDeserializeDomainModelAssembly()
		{
			MappingDocumentAggregator aggregator = new MappingDocumentAggregator();
			aggregator.Add(domainModelAssembly);
			IList<HbmMapping> originalList = aggregator.List();

			using (MemoryStream memory = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(memory, originalList);

				memory.Position = 0;
				IList<HbmMapping> newList = (IList<HbmMapping>) formatter.Deserialize(memory);

				Assert.IsTrue(newList.Count == originalList.Count);
			}
		}

		[Test]
		public void CompareDeserializationTimes()
		{
			// Interesting... On the first pass, the XML time is much greater than disk and memory, but on
			// subsequent iterations, the xml time is actually better!

			// About a quarter of that time is in instantiating the MappingDocumentParser. Uncomment th
			// following line and observe how the first XML pass time changed. It went down 75% for me.
			// The XmlSerializer must walk the class schema the first time only and then cache the results.

			// new MappingDocumentParser();

			// Unfortunately, there's not much we can do about the one-time initial performance hit if we use
			// the XmlSerializer. The alternative would be to parse the Xml by walking the XML DOM manually
			// but there's no guarantee this will be faster.

			// I think a better idea would be to implement a caching mechanism that uses the binary
			// serialization to disk and then reloads that serialized data on subsequent application runs.

			for (int i = 0; i < 5; i++)
			{
				FileInfo tempFile = new FileInfo(Guid.NewGuid() + ".hbm.binary");

				// Load the embedded xml mappings (and time it)
				Stopwatch stopwatch1 = Stopwatch.StartNew();
				MappingDocumentAggregator aggregator = new MappingDocumentAggregator();
				aggregator.Add(domainModelAssembly);
				stopwatch1.Stop();
				Console.WriteLine("XML:    " + stopwatch1.ElapsedMilliseconds + " ms");

				// write those same mappings to disk
				IList<HbmMapping> mappings = aggregator.List();
				BinaryFormatter formatter = new BinaryFormatter();
				using (FileStream stream = tempFile.OpenWrite())
					formatter.Serialize(stream, mappings);

				// Load the serialized mappings from disk (and time it)
				Stopwatch stopwatch2 = Stopwatch.StartNew();
				using (FileStream stream = tempFile.OpenRead())
				{
					IList<HbmMapping> mappings2 = (IList<HbmMapping>) formatter.Deserialize(stream);
				}
				stopwatch2.Stop();
				Console.WriteLine("Disk:   " + stopwatch2.ElapsedMilliseconds + " ms");

				using (MemoryStream memoryStream = new MemoryStream())
				{
					formatter.Serialize(memoryStream, mappings);
					memoryStream.Position = 0;

					Stopwatch stopwatch3 = Stopwatch.StartNew();
					IList<HbmMapping> mappings3 = (IList<HbmMapping>) formatter.Deserialize(memoryStream);
					stopwatch3.Stop();
					Console.WriteLine("Memory: " + stopwatch3.ElapsedMilliseconds + " ms");
				}

				tempFile.Delete();
				Console.WriteLine();
			}
		}
	}
}