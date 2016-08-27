using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;

namespace NHibernate.Test.Linq
{
	public class ObjectDumper
	{
		private readonly int _depth;
		private readonly TextWriter _writer;
		private int _level;
		private int _pos;

		private ObjectDumper(int depth)
		{
			_writer = new StringWriter(new StringBuilder());
			_depth = depth;
		}

		public static string Write(object o)
		{
			return Write(o, 0);
		}

		public static string Write(object o, int depth)
		{
			var dumper = new ObjectDumper(depth);
			dumper.WriteObject(null, o);
			return dumper._writer.ToString();
		}

		private void Write(string s)
		{
			if (s != null)
			{
				_writer.Write(s);
				_pos += s.Length;
			}
		}

		private void WriteIndent()
		{
			for (int i = 0; i < _level; i++) _writer.Write("  ");
		}

		private void WriteLine()
		{
			_writer.WriteLine();
			_pos = 0;
		}

		private void WriteTab()
		{
			Write("  ");
			while (_pos%8 != 0) Write(" ");
		}

		private void WriteObject(string prefix, object o)
		{
			if (o == null || o is ValueType || o is string)
			{
				WriteIndent();
				Write(prefix);
				WriteValue(o);
				WriteLine();
			}
			else if (o is IEnumerable)
			{
				foreach (object element in (IEnumerable) o)
				{
					if (element is IEnumerable && !(element is string))
					{
						WriteIndent();
						Write(prefix);
						Write("...");
						WriteLine();
						if (_level < _depth)
						{
							_level++;
							WriteObject(prefix, element);
							_level--;
						}
					}
					else
					{
						WriteObject(prefix, element);
					}
				}
			}
			else
			{
				MemberInfo[] members = o.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
				WriteIndent();
				Write(prefix);
				bool propWritten = false;
				foreach (MemberInfo m in members)
				{
					var f = m as FieldInfo;
					var p = m as PropertyInfo;
					if (f != null || p != null)
					{
						if (propWritten)
						{
							WriteTab();
						}
						else
						{
							propWritten = true;
						}
						Write(m.Name);
						Write("=");
						System.Type t = f != null ? f.FieldType : p.PropertyType;
						if (t.IsValueType || t == typeof (string))
						{
							WriteValue(f != null ? f.GetValue(o) : p.GetValue(o, null));
						}
						else
						{
							if (typeof (IEnumerable).IsAssignableFrom(t))
							{
								Write("...");
							}
							else
							{
								Write("{ }");
							}
						}
					}
				}
				if (propWritten) WriteLine();
				if (_level < _depth)
				{
					foreach (MemberInfo m in members)
					{
						var f = m as FieldInfo;
						var p = m as PropertyInfo;
						if (f != null || p != null)
						{
							System.Type t = f != null ? f.FieldType : p.PropertyType;
							if (!(t.IsValueType || t == typeof (string)))
							{
								object value = f != null ? f.GetValue(o) : p.GetValue(o, null);
								if (value != null)
								{
									_level++;
									WriteObject(m.Name + ": ", value);
									_level--;
								}
							}
						}
					}
				}
			}
		}

		private void WriteValue(object o)
		{
			if (o == null)
			{
				Write("null");
			}
			else if (o is DateTime)
			{
				Write(((DateTime) o).ToShortDateString());
			}
			else if (o is ValueType || o is string)
			{
				Write(o.ToString());
			}
			else if (o is IEnumerable)
			{
				Write("...");
			}
			else
			{
				Write("{ }");
			}
		}
	}
}