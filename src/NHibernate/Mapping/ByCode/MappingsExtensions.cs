using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode
{
	/// <summary>
	/// Util extensions to use in your test or where you need to see the XML mappings
	/// </summary>
	public static class MappingsExtensions
	{
		public static void WriteAllXmlMapping(this IEnumerable<HbmMapping> mappings)
		{
			if (mappings == null)
			{
				throw new ArgumentNullException("mappings");
			}
			string mappingsFolderPath = ArrangeMappingsFolderPath();
			foreach (HbmMapping hbmMapping in mappings)
			{
				string fileName = GetFileName(hbmMapping);
				string document = Serialize(hbmMapping);
				File.WriteAllText(Path.Combine(mappingsFolderPath, fileName), document);
			}
		}

		public static string AsString(this HbmMapping mappings)
		{
			if (mappings == null)
			{
				throw new ArgumentNullException("mappings");
			}
			return Serialize(mappings);
		}

		private static string ArrangeMappingsFolderPath()
		{
			string baseDir = AppDomain.CurrentDomain.BaseDirectory;
			string relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
			string binPath = relativeSearchPath != null ? Path.Combine(baseDir, relativeSearchPath): baseDir;
			string mappingsFolderPath = Path.Combine(binPath, "Mappings");

			if (!Directory.Exists(mappingsFolderPath))
			{
				Directory.CreateDirectory(mappingsFolderPath);
			}
			else
			{
				System.Array.ForEach(Directory.GetFiles(mappingsFolderPath), File.Delete);
			}
			return mappingsFolderPath;
		}

		private static string GetFileName(HbmMapping hbmMapping)
		{
			string name = "MyMapping";
			HbmClass rc = hbmMapping.RootClasses.FirstOrDefault();
			if (rc != null)
			{
				name = rc.Name;
			}
			HbmSubclass sc = hbmMapping.SubClasses.FirstOrDefault();
			if (sc != null)
			{
				name = sc.Name;
			}
			HbmJoinedSubclass jc = hbmMapping.JoinedSubclasses.FirstOrDefault();
			if (jc != null)
			{
				name = jc.Name;
			}
			HbmUnionSubclass uc = hbmMapping.UnionSubclasses.FirstOrDefault();
			if (uc != null)
			{
				name = uc.Name;
			}
			return name + ".hbm.xml";
		}

		private static string Serialize(HbmMapping hbmElement)
		{
			string result;
			var setting = new XmlWriterSettings { Indent = true };
			var serializer = new XmlSerializer(typeof(HbmMapping));
			using (var memStream = new MemoryStream(2048))
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(memStream, setting))
				{
					serializer.Serialize(xmlWriter, hbmElement);
				}
				memStream.Position = 0;
				using (var sr = new StreamReader(memStream))
				{
					result = sr.ReadToEnd();
				}
			}
			return result;
		}
	}
}