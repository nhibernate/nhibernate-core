//
// NHibernate.Mapping.Attributes.Test
// This product is under the terms of the GNU Lesser General Public License.
//
namespace NHibernate.Mapping.Attributes.Test
{
	/// <summary>
	/// Contains the main entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary> Instanciation forbidden </summary>
		private Program()
		{
			throw new System.NotSupportedException();
		}


		/// <summary> The main entry point for the application. </summary>
		[System.STAThread]
		static void Main()
		{
			try
			{
				System.Console.Out.WriteLine("Run the tests (TestSerialization: Serialize Baz and the assembly)...\n");
				Tests tests = new Tests();
				tests.TestFixtureSetUp();
				tests.TestSerialization();

				System.Console.Out.WriteLine("Tests succeeded !\n");
			}
			catch(System.Exception ex)
			{
				System.Console.Error.WriteLine(ex.ToString());
			}
			catch
			{
				System.Console.Error.WriteLine("Unexpected non-CLSCompliant Exception");
			}
		}
	}
}
