using System;
using System.IO;
using System.Reflection;

namespace NHibernate.Tool.hbm2net.Tests
{
	/// <summary>
	/// Summary description for TestHelper.
	/// </summary>
	internal class TestHelper
	{
		public static DirectoryInfo DefaultOutputDirectory
		{
			get
			{
				return new DirectoryInfo("generated");
			}
		}
	}
}
