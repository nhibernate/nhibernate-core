using System.IO;
using System.Text;
using NAnt.Core.Attributes;
using NAnt.Core.Tasks;
using NAnt.Core.Types;

namespace NHibernate.Tasks
{
	[TaskName( "hbm2net" )]
	public class Hbm2NetTask : ExternalProgramBase
	{
		private FileSet _set = new FileSet();
		private string _output = null;
		private string _config = null;
		private string _args = null;

		[BuildElement( "fileset", Required=true )]
		public FileSet Hbm2NetFileSet
		{
			get { return _set; }
			set { _set = value; }
		}

		[TaskAttribute( "output" )]
		public string Output
		{
			get { return _output; }
			set { _output = value; }
		}

		[TaskAttribute( "config" )]
		public string Config
		{
			get { return _config; }
			set { _config = value; }
		}

		public override string ExeName
		{
			get
			{
				string asm = this.GetType().Assembly.Location;
				string basename = asm.Substring( 0, asm.LastIndexOf( Path.DirectorySeparatorChar ) + 1 );
				return basename + "NHibernate.Tool.hbm2net.exe";
			}
		}

		public override string ProgramArguments
		{
			get { return _args; }
		}

		protected override void ExecuteTask()
		{
			StringBuilder sb = new StringBuilder();
			if( _output != null )
			{
				sb.Append( "--output=\"" + _output + "\" " );
			}
			if( _config != null )
			{
				sb.Append( "--config=\"" + _config + "\" " );
			}
			foreach( string filename in _set.FileNames )
			{
				sb.Append( "\"" + filename + "\" " );
			}
			_args = sb.ToString();

			base.ExecuteTask();
		}
	}
}