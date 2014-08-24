using System.Collections.Generic;
using NHibernate.Classic;

namespace NHibernate.Test.Classic
{
	public class EntityWithLifecycle : ILifecycle
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual double Heigth { get; set; }
		public virtual double Width { get; set; }
		public EntityWithLifecycle() {}
		public EntityWithLifecycle(string name, double heigth, double width)
		{
			Name = name;
			Heigth = heigth;
			Width = width;
		}

		public virtual LifecycleVeto OnSave(ISession s)
		{
			return IsValid() ? LifecycleVeto.NoVeto : LifecycleVeto.Veto;
		}

		public virtual LifecycleVeto OnUpdate(ISession s)
		{
			return IsValid() ? LifecycleVeto.NoVeto : LifecycleVeto.Veto;
		}

		public virtual LifecycleVeto OnDelete(ISession s)
		{
			return IsValid() ? LifecycleVeto.NoVeto : LifecycleVeto.Veto;
		}

		public virtual void OnLoad(ISession s, object id)
		{
			// nothing to do
		}

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
		private bool IsValid()
		{
			IList<string> br = GetBrokenRules();
			return br == null || br.Count == 0;
		}
	}
}