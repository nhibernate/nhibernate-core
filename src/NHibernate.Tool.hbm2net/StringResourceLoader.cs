/*
* Created on 12-10-2003
*
* To change the template for this generated file go to
* Window - Preferences - Java - Code Generation - Code and Comments
*/
using System;
using System.IO;
using System.Text;

using Commons.Collections;

using NVelocity.Runtime.Resource;
using NVelocity.Runtime.Resource.Loader;

using StringInputStream = System.IO.StringReader;

namespace NHibernate.Tool.hbm2net
{
	/// <author>  MAX
	/// 
	/// To change the template for this generated type comment go to
	/// Window - Preferences - Java - Code Generation - Code and Comments
	/// </author>
	public class StringResourceLoader : ResourceLoader
	{
		/* (non-Javadoc)
		* @see org.apache.velocity.runtime.resource.loader.ResourceLoader#init(org.apache.commons.collections.ExtendedProperties)
		*/

		public override void init(ExtendedProperties configuration)
		{
			// TODO Auto-generated method stub
		}

		/* (non-Javadoc)
		* @see org.apache.velocity.runtime.resource.loader.ResourceLoader#getResourceStream(java.lang.String)
		*/

		public override Stream getResourceStream(string source)
		{
			return new MemoryStream(Encoding.ASCII.GetBytes(source));
		}

		/* (non-Javadoc)
		* @see org.apache.velocity.runtime.resource.loader.ResourceLoader#isSourceModified(org.apache.velocity.runtime.resource.Resource)
		*/

		public override bool isSourceModified(Resource resource)
		{
			return false;
		}

		/* (non-Javadoc)
		* @see org.apache.velocity.runtime.resource.loader.ResourceLoader#getLastModified(org.apache.velocity.runtime.resource.Resource)
		*/

		public override long getLastModified(Resource resource)
		{
			return (DateTime.Now.Ticks - 621355968000000000) / 10000;
		}
	}
}