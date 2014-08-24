using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace NHibernate.Test.DynamicProxyTests
{
	// utility class to run PEVerify.exe against a saved-to-disk assembly, similar to:
	// http://stackoverflow.com/questions/7290893/is-there-an-api-for-verifying-the-msil-of-a-dynamic-assembly-at-runtime
	public class PeVerifier
	{
		private string _assemlyLocation;
		private string _peVerifyPath;

		public PeVerifier(string assemblyFileName)
		{
			var assemblyLocation = Path.Combine(Environment.CurrentDirectory, assemblyFileName);

			if (!File.Exists(assemblyLocation))
				throw new ArgumentException(string.Format("Could not locate assembly {0}", assemblyLocation), "assemblyLocation");

			_assemlyLocation = assemblyLocation;

			var dir = Path.GetDirectoryName(_assemlyLocation);

			while (!Directory.Exists(Path.Combine(dir, "Tools/PEVerify")))
			{
				if (Directory.GetParent(dir) == null)
					throw new Exception(string.Format("Could not find Tools/PEVerify directory in ancestor of {0}", _assemlyLocation));

				dir = Directory.GetParent(dir).FullName;
			}

			var versionFolder = "4.0";
			if (Environment.Version.Major == 2)
				versionFolder = "3.5";

			_peVerifyPath = Path.Combine(dir, "Tools/PEVerify/" + versionFolder + "/PEVerify.exe");

			if (!File.Exists(_peVerifyPath))
				throw new Exception(string.Format("Could not find PEVerify.exe at {0}", _peVerifyPath));
		}

		public void AssertIsValid()
		{
			var process = new Process
			{
				StartInfo =
				{
					FileName = _peVerifyPath,
					RedirectStandardOutput = true,
					UseShellExecute = false,
					Arguments = "\"" + _assemlyLocation + "\" /VERBOSE",
					CreateNoWindow = true
				}
			};

			process.Start();
			var processOutput = process.StandardOutput.ReadToEnd();
			process.WaitForExit();

			var result = process.ExitCode + " code ";

			if (process.ExitCode != 0)
				Assert.Fail("PeVerify reported error(s): " + Environment.NewLine + processOutput, result);
		}
	}
}
