using System.Collections;
using System.Xml;

namespace NHibernate
{
	/// <summary>
	/// Provides XML marshalling for classes registered with a <c>SessionFactory</c>
	/// </summary>
	/// <remarks>
	/// <para>
	/// Hibernate defines a generic XML format that may be used to represent any class
	/// (<c>hibernate-generic.dtd</c>). The user configures an XSLT stylesheet for marshalling
	/// data from this generic format to an application and/or user readable format. By default,
	/// Hibernate will use <c>hibernate-default.xslt</c> which maps data to a useful human-
	/// readable format.
	/// </para>
	/// <para>
	/// The property <c>xml.output_stylesheet</c> specifies a user-written stylesheet.
	/// Hibernate will attempt to load the stylesheet from the classpath first and if not found,
	/// will attempt to load it as a file
	/// </para>
	/// <para>
	/// It is not intended that implementors be threadsafe
	/// </para>
	/// </remarks>
	public interface IDatabinder
	{
		/// <summary>
		/// Add an object to the output document.
		/// </summary>
		/// <param name="obj">A transient or persistent instance</param>
		/// <returns>Databinder</returns>
		IDatabinder Bind(object obj);

		/// <summary>
		/// Add a collection of objects to the output document
		/// </summary>
		/// <param name="objs">A collection of transient or persistent instance</param>
		/// <returns>Databinder</returns>
		IDatabinder BindAll(ICollection objs);

		/// <summary>
		/// Output the generic XML representation of the bound objects
		/// </summary>
		/// <returns>Generic Xml representation</returns>
		string ToGenericXml();

		/// <summary>
		/// Output the generic XML Representation of the bound objects
		/// to a <c>XmlDocument</c>
		/// </summary>
		/// <returns>A generic Xml tree</returns>
		XmlDocument ToGenericXmlDocument();

		/// <summary>
		/// Output the custom XML representation of the bound objects
		/// </summary>
		/// <returns>Custom Xml representation</returns>
		string ToXML();

		/// <summary>
		/// Output the custom XML representation of the bound objects as
		/// an <c>XmlDocument</c>
		/// </summary>
		/// <returns>A custom Xml Tree</returns>
		XmlDocument ToXmlDocument();

		/// <summary>
		/// Controls whether bound objects (and their associated objects) that are lazily instanciated
		/// are explicityl initialized or left as they are
		/// </summary>
		/// <value>True to explicitly initilize lazy objects, false to leave them in the state they are in</value>
		bool InitializeLazy { get; set; }
	}
}