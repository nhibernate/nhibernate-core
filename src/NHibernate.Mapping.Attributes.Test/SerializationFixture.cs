//
// NHibernate.Mapping.Attributes.Test
// This product is under the terms of the GNU Lesser General Public License.
//
using System;
using System.IO;
using System.Reflection;

using NHibernate.Mapping.Attributes.Test.DomainModel;

using NUnit.Framework;

namespace NHibernate.Mapping.Attributes.Test
{
	/// <summary>
	/// Simple tests using serialization to compare the results with reference files
	/// </summary>
	[TestFixture]
	public class Tests
	{
		/// <summary> Set up </summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			// Enable validation of generated XML files
			HbmSerializer.Default.Validate = true;
			HbmSerializer.Default.HbmWriter.Patterns.Add(@"X\+(\S+), NHibernate.Mapping.Attributes.Test", "X+$1, NHMA.Test");
		}


		/// <summary>
		/// Test how this assembly is serialized:
		/// Compare the output with a reference (hand-verified) version.
		/// </summary>
		[Test]
		public void TestSerialization()
		{
			// Remove precedent errors
			if (HbmSerializer.Default.Error.Length > 0)
				HbmSerializer.Default.Error.Remove(0, HbmSerializer.Default.Error.Length);

			// Generate (baz & assembly) + Validate
			Stream bazStream = null;
			Stream assemblyStream = null;
			try
			{
				bazStream = SerializeBaz();
				assemblyStream = SerializeAssembly();

				// Throw if errors
				if (HbmSerializer.Default.Validate && HbmSerializer.Default.Error.Length > 0)
					throw new Exception(HbmSerializer.Default.Error.ToString());

				// Compare with references
				Compare(bazStream, "Baz.Reference.hbm.xml");
				Compare(assemblyStream, "DomainModel.Reference.hbm.xml");
			}
			finally
			{
				if (bazStream != null)
					bazStream.Close();
				if (assemblyStream != null)
					assemblyStream.Close();
			}
		}


		/// <summary> Serialize the class 'DomainModel.Baz' and return the generated stream </summary>
		private Stream SerializeBaz()
		{
			// Saved in the project directory
			Stream stream = new FileStream("Baz.hbm.xml", FileMode.Create);
			// Note: Baz is decorated with HibernateMappingAttribute which will be used
			HbmSerializer.Default.Serialize(stream, typeof(Baz));

			return stream;
		}


		/// <summary> Serialize the whole assembly and return the generated stream </summary>
		private Stream SerializeAssembly()
		{
			// Saved in the project directory
			Stream stream = new FileStream("DomainModel.hbm.xml", FileMode.Create);
			HbmSerializer.Default.HbmAutoImport = true; // Test specifying <hibernate-mapping> attributes
			HbmSerializer.Default.HbmDefaultLazy = true;
			HbmSerializer.Default.Serialize(stream, Assembly.GetExecutingAssembly());

			return stream;
		}


		private void Compare(Stream stream, string refFilePath)
		{
			// if the file is not in the current directory, try in "../../" (should be the project directory)
			if (!File.Exists(refFilePath) && File.Exists("../../" + refFilePath))
				refFilePath = "../../" + refFilePath;

			stream.Position = 0;
			using (StreamReader reader = new StreamReader(stream))
			using (StreamReader refReader = new StreamReader(new FileStream(refFilePath, FileMode.Open)))
			{
				int l = 0;
				string line;
				string refLine;
				do // Compare the streams line by line
				{
					l++;
					line = reader.ReadLine();
					refLine = refReader.ReadLine();
					if (l != 2 && line != refLine) // line 2 contain a comment
						throw new Exception(string.Format(
						                    	"Difference at line {0}:{3}Src={1}{3}Ref={2}", l, line, refLine, Environment.NewLine));
				} while (line != null && refLine != null);
			}
		}
	}
}