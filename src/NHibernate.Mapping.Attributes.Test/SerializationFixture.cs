//
// NHibernate.Mapping.Attributes.Test
// This product is under the terms of the GNU Lesser General Public License.
//
namespace NHibernate.Mapping.Attributes.Test
{
	/// <summary>
	/// Simple tests using serialization to compare the results with reference files
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class Tests
	{
		/// <summary> Set up </summary>
		[NUnit.Framework.TestFixtureSetUp]
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
		[NUnit.Framework.Test]
		public void TestSerialization() 
		{
			// Remove precedent errors
			if(HbmSerializer.Default.Error.Length > 0)
				HbmSerializer.Default.Error.Remove(0, HbmSerializer.Default.Error.Length);

			// Generate (baz & assembly) + Validate
			System.IO.Stream bazStream = null;
			System.IO.Stream assemblyStream = null;
			try
			{
				bazStream = SerializeBaz();
				assemblyStream = SerializeAssembly();

				// Throw if errors
				if(HbmSerializer.Default.Validate && HbmSerializer.Default.Error.Length > 0)
					throw new System.Exception(HbmSerializer.Default.Error.ToString());

				// Compare with references
				Compare(bazStream, "Baz.Reference.hbm.xml");
				Compare(assemblyStream, "DomainModel.Reference.hbm.xml");
			}
			finally
			{
				if(bazStream != null)
					bazStream.Close();
				if(assemblyStream != null)
					assemblyStream.Close();
			}
		}


		/// <summary> Serialize the class 'DomainModel.Baz' and return the generated stream </summary>
		private System.IO.Stream SerializeBaz()
		{
			// Saved in the project directory
			System.IO.Stream stream = new System.IO.FileStream("Baz.hbm.xml", System.IO.FileMode.Create);
			// Note: Baz is decorated with HibernateMappingAttribute which will be used
			HbmSerializer.Default.Serialize(stream, typeof(DomainModel.Baz));

			return stream;
		}


		/// <summary> Serialize the whole assembly and return the generated stream </summary>
		private System.IO.Stream SerializeAssembly()
		{
			// Saved in the project directory
			System.IO.Stream stream = new System.IO.FileStream("DomainModel.hbm.xml", System.IO.FileMode.Create);
			HbmSerializer.Default.HbmAutoImport = true; // Test specifying <hibernate-mapping> attributes
			HbmSerializer.Default.HbmDefaultLazy = true;
			HbmSerializer.Default.Serialize(stream, System.Reflection.Assembly.GetExecutingAssembly());

			return stream;
		}


		private void Compare(System.IO.Stream stream, string refFilePath)
		{
			// if the file is not in the current directory, try in "../../" (should be the project directory)
			if( !System.IO.File.Exists(refFilePath) && System.IO.File.Exists("../../" + refFilePath) )
				refFilePath = "../../" + refFilePath;

			stream.Position = 0;
			using(System.IO.StreamReader reader = new System.IO.StreamReader(stream))
			using(System.IO.StreamReader refReader = new System.IO.StreamReader(new System.IO.FileStream(refFilePath, System.IO.FileMode.Open)))
			{
				int l = 0;
				string line;
				string refLine;
				do // Compare the streams line by line
				{
					l++;
					line = reader.ReadLine();
					refLine = refReader.ReadLine();
					if(l!=2 && line != refLine) // line 2 contain a comment
						throw new System.Exception(string.Format(
							"Difference at line {0}:{3}Src={1}{3}Ref={2}", l, line, refLine, System.Environment.NewLine));
				}
				while(line != null && refLine != null);
			}
		}
	}
}
