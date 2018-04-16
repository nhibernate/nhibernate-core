﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.Tools.hbm2ddl.SchemaValidator
{
	public class Version
	{
		private int id;
		private string name;
		private string description;
		private string title;
		private Version previous;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}

		public virtual Version Previous
		{
			get { return previous; }
			set { previous = value; }
		}
	}
}
