using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Classic;

namespace NHibernate.Test.Classic
{
	public class Video: IValidatable
	{
		private int id;
		private string name;
		private double heigth;
		private double width;

		public Video() {}

		public Video(string name, double heigth, double width)
		{
			this.name = name;
			this.heigth = heigth;
			this.width = width;
		}

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

		public virtual double Heigth
		{
			get { return heigth; }
			set { heigth = value; }
		}

		public virtual double Width
		{
			get { return width; }
			set { width = value; }
		}

		#region IValidatable Members
		public virtual IList<string> GetBrokenRules()
		{
			IList<string> result = new List<string>(3);
			if (string.IsNullOrEmpty(Name) || Name.Trim().Length < 2)
				result.Add("The Name must have more than one char.");
			if (Heigth <= 0)
				result.Add("Heigth must be great than 0");
			if (Width <= 0)
				result.Add("Width must be great than 0.");
			return result;
		}

		/// <summary>
		/// Validate the state of the object before persisting it. If a violation occurs,
		/// throw a <see cref="ValidationFailure" />. This method must not change the state of the object
		/// by side-effect.
		/// </summary>
		public virtual void Validate()
		{
			IList<string> br = GetBrokenRules();
			if (br != null && br.Count > 0)
				throw new ValidationFailure(BrokenRulesFormat(typeof(Video), br));
		}

		private static string BrokenRulesFormat(System.Type entity, IList<string> brokenRulesDescriptions)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			if (brokenRulesDescriptions == null)
				throw new ArgumentNullException("brokenRulesDescriptions");

			StringBuilder sb = new StringBuilder(50 + brokenRulesDescriptions.Count * 50)
				.AppendLine(string.Format("Entity:{0}", entity));
			foreach (string message in brokenRulesDescriptions)
				sb.AppendLine(message);

			return sb.ToString();
		}

		#endregion
	}
}
