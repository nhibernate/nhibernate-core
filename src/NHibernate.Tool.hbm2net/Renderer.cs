using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace NHibernate.Tool.hbm2net
{
	public interface Renderer
	{
		/// <summary>Called with the optional list of properties from config.xml </summary>
		void configure(DirectoryInfo workingDirectory, NameValueCollection properties);

		/// <summary> </summary>
		/// <param name="savedToPackage">what package is this class placed in
		/// </param>
		/// <param name="savedToClass">what classname does it really get
		/// </param>
		/// <param name="classMapping">what classmapping is this for
		/// </param>
		/// <param name="class2classmap">A complete map from classname to the classmapping
		/// </param>
		/// <param name="writer">where we want the output
		/// @throws Exception
		/// </param>
		void render(string savedToPackage, string savedToClass, ClassMapping classMapping, IDictionary class2classmap,
		            StreamWriter writer);

		/// <summary> Called by the generator to determine the package name of the rendered class.
		/// 
		/// </summary>
		/// <param name="classMapping">The class mapping of the generated class
		/// </param>
		/// <returns> the package name the class should be saved to
		/// </returns>
		string getSaveToPackage(ClassMapping classMapping);

		/// <summary> Called by the generator to determine the class name of the rendered class.
		/// 
		/// </summary>
		/// <param name="classMapping">The class mapping of the generated class
		/// </param>
		/// <returns> the class name the class should be saved to
		/// </returns>
		string getSaveToClassName(ClassMapping classMapping);


		/// <summary>
		///  <para>Gets the working directory for the renderer.</para>
		/// </summary>
		/// <returns>The working directory for the renderer.</returns>
		/// <remarks>
		///  <para>Some renderers require or allow additional supporting files to be specified. When relative
		///  file paths are provide they will be evaluated realtive to the working directory.</para>
		/// </remarks>
		DirectoryInfo WorkingDirectory { get; }
	}
}