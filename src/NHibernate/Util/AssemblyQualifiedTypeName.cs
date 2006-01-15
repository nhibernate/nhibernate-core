using System;
using System.Text;

namespace NHibernate.Util
{
	public class AssemblyQualifiedTypeName
	{
		private string type;
		private string assembly;

		public AssemblyQualifiedTypeName( string type, string assembly )
		{
			if( type == null )
			{
				throw new ArgumentNullException( "type" );
			}
			this.type = type;
			this.assembly = assembly;
		}

		public string Type
		{
			get { return type; }
		}

		public string Assembly
		{
			get { return assembly; }
		}

		public override bool Equals( object obj )
		{
			AssemblyQualifiedTypeName other = obj as AssemblyQualifiedTypeName;

			if( other == null ) return false;

			return string.Equals( type, other.type )
				&& string.Equals( assembly, other.assembly );
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = 0;
				if( type != null )
				{
					hashCode += type.GetHashCode();
				}

				if( assembly != null )
				{
					hashCode += assembly.GetHashCode();
				}

				return hashCode;
			}
		}

		public override string ToString()
		{
			if( assembly == null )
			{
				return type;
			}

			return string.Concat( type, ", ", assembly );
		}

		private static string GetAssemblyName( string text, int start )
		{
			StringBuilder result = new StringBuilder( text.Length - start );

			int i = start;
			while( i < text.Length && text[ i ] == ' ' )
			{
				i++;
			}

			while( i < text.Length )
			{
				if( text[ i ] == '\\' )
				{
					// Append the backslash and the character after it
					result.Append( text[ i ] );
					i++;
				}
				result.Append( text[ i ] );
				i++;
			}
			return result.ToString();
		}

		public static AssemblyQualifiedTypeName Parse( string text )
		{
			return Parse( text, null, null );
		}

		public static AssemblyQualifiedTypeName Parse( string text, string defaultAssembly )
		{
			return Parse( text, null, defaultAssembly );
		}

		public static AssemblyQualifiedTypeName Parse( string text, string defaultNamespace, string defaultAssembly )
		{
			text = text.Trim();

			StringBuilder type = new StringBuilder( text.Length );
			string assembly = defaultAssembly;

			int i = 0;

			try
			{
				bool needNamespace = true;
				while( i < text.Length )
				{
					if( text[ i ] == '.' )
					{
						needNamespace = false;
					}

					if( text[ i ] == ',' )
					{
						assembly = GetAssemblyName( text, i + 1 );
						break;
					}
					else if( text[ i ] == '\\' )
					{
						// Append the backslash and the character after it
						type.Append( text[ i ] );
						i++;
					}
					type.Append( text[ i ] );
					i++;
				}

				if( needNamespace && defaultNamespace != null )
				{
					type.Append( '.' ).Append( defaultNamespace );
				}
				return new AssemblyQualifiedTypeName( type.ToString(), assembly );
			}
			catch( IndexOutOfRangeException )
			{
				throw new ArgumentException( "Invalid fully-qualified type name: " + text, "text" );
			}
		}
	}
}