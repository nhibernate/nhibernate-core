using System;
using System.IO;
using System.Reflection;

namespace NHibernate.Tool.hbm2net.Tests
{
	/// <summary>
	/// Summary description for ResourceHelper.
	/// </summary>
	internal class ResourceHelper
	{
		private static string GetNamespace()
		{
			return MethodBase.GetCurrentMethod().DeclaringType.Namespace;
		}

		public static string GetResource(string name)
		{
			return GetResource(name, GetNamespace());
		}

		public static string GetResource(string name, string ns)
		{
			string output;
			Assembly assembly = Assembly.GetExecutingAssembly();
			name = string.Format("{0}.{1}", ns, name);
			Stream rs = assembly.GetManifestResourceStream(name);
			if (rs == null) throw new ApplicationException(String.Format("Cannot load {0} resource!", name));
			using (StreamReader sr = new StreamReader(rs))
			{
				output = sr.ReadToEnd();
				sr.Close();
			}
			return output;			
		}

		public static FileInfo WriteToFileFromResource(FileInfo file, string resource)
		{
			return WriteToFileFromResource(file, resource, GetNamespace());
		}

		public static FileInfo CreateFileFromResource(string resource)
		{
			FileInfo file = new FileInfo(Path.GetTempFileName());
			return WriteToFileFromResource(file, resource, GetNamespace());
		}

		public static FileInfo WriteToFileFromResource(FileInfo file, string resource, string ns)
		{
			using(FileStream fs = File.OpenWrite(file.FullName))
			{
				StreamWriter s = new StreamWriter(fs);
				s.Write(GetResource(resource, ns));
				s.Flush();
			}
			return file;
		}
	}
}
