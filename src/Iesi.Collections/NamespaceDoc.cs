using System;

namespace Iesi.Collections
{
	/// <summary>
	///		<p>
	///			The System.Collections namespace in the .NET Framework provides a number of collection 
	///			types that are extremely useful for manipulating data in memory. However, there is one 
	///			type of collection that is conspicuously missing from <c>System.Collections</c>: the 
	///			<c>Set</c>.
	///		</p>
	///		<p>
	///			A <c>Set</c> is a collection that contains no duplicate elements, and where the order of
	///			the elements may be arbitrary. It is loosely modelled after 
	///			the mathematical concept of a "set." This implementation is based on the Java <c>Set</c> 
	///			interface definition, so if you are also a Java programmer, this may seem familiar. 
	///			This library provides a number of "standard" <c>Set</c> operators that the Java library 
	///			neglected to include.
	///		</p>
	///		<p>
	///			<c>Sets</c> come in handy when an <c>Array</c> or a <c>List</c> won't quite fit the bill. 
	///			Arrays in .NET have a fixed length, making it tedious to add and remove elements. 
	///			Lists allow you add new objects easily, but you can have numerous duplicate 
	///			elements, which is undesirable for some types of problems. 
	///			Searching Arrays or Lists for elements is just plain slow for large data sets, 
	///			requiring a linear search. You could keep the array sorted and use a binary search, 
	///			but that is often more trouble than it is worth (especially since this library, 
	///			and the .NET Framework, provide better ways that are already written for you).
	///		</p>
	///		<p>
	///			With sets, adding elements, removing elements, and checking for the existence 
	///			of an element is fast and simple. You can mix and match the elements in different 
	///			sets using the supported mathematical set operators: union, intersection, 
	///			exclusive-or, and minus. 
	///		</p>
	///		<p>
	///			You will see some interesting side effects with different <c>Set</c> 
	///			implementations in this library, depending on the underlying search algorithm. 
	///			For example, if you choose a sort-based <c>Set</c>, the elements will come out 
	///			in sort order when you iterate using <c>foreach</c>. If you use a hash-based 
	///			<c>Set</c>, the elements will come out in no particular order, but checking 
	///			for inclusion will fastest when dealing with large data sets. If you use a 
	///			list-based <c>Set</c>, elements will come out in the order you put them in 
	///			when you iterate (although the effect of operators on element order in 
	///			<c>Set</c> instances is not well defined by design). Additionally, list-based 
	///			sets are fastest for very small data sets (up to about 10 elements), 
	///			but get slower very quickly as the number of contained elements increases. 
	///			To get the best of both worlds, the library provides a <c>Set</c> type that 
	///			uses lists for small data sets and switches to a hash-based algorithm when 
	///			the data set gets large enough to warrant it.
	///		</p>
	///		<p>
	///			The following sample program demonstrates some of the features of sets:
	///			<code>
	///using System;
	///using Iesi.Collections;
	///namespace RiverDemo
	///{    
	///	class Rivers
	///	{
	///		[STAThread]
	///		static void Main(string[] args)
	///		{
	///			//Use Arrays (which are ICollection objects) to quickly initialize.
	///			Set arizona   
	///				= new SortedSet(new string[] {"Colorado River"});
	///			Set california
	///				= new SortedSet(new string[] {"Colorado River", "Sacramento River"});
	///			Set colorado
	///				= new SortedSet(new string[] {"Arkansas River", "Colorado River", "Green River", "Rio Grande"});
	///			Set kansas
	///				= new SortedSet(new string[] {"Arkansas River", "Missouri River"});
	///			Set nevada
	///				= new SortedSet(new string[] {"Colorado River"});
	///			Set newMexico
	///				= new SortedSet(new string[] {"Rio Grande"});
	///			Set utah
	///				= new SortedSet(new string[] {"Colorado River", "Green River", "San Juan River"});
	///			//Rivers by region.
	///			Set southWest = colorado | newMexico | arizona | utah;
	///			Set midWest = kansas;
	///			Set west = california | nevada;
	///			//All rivers (at least for the demo).
	///			Set all = southWest | midWest | west;
	///			Print("All rivers:", all);
	///			Print("Rivers in the southwest:", southWest);
	///			Print("Rivers in the west:", west);
	///			Print("Rivers in the midwest:", midWest);
	///			Console.WriteLine();
	///
	///			//Use the '-' operator to subtract the rivers in Colorado from 
	///			//the set of all rivers.
	///			Print("Of all rivers, these don't pass through Colorado:", all - colorado);
	///
	///			//Use the '&amp;' operator to find rivers that are in Colorado AND in Utah.
	///			//A river must be present in both states, not just one.
	///			Print("Rivers common to both Colorado and Utah:", colorado &amp; utah);
	///
	///			//use the '^' operator to find rivers that are in Colorado OR Utah,
	///			//but not in both.
	///			Print("Rivers in Colorado and Utah that are not shared by both states:",
	///				colorado ^ utah);
	///
	///			//Use the '&amp;' operator to discover which rivers are present in Arizona, 
	///			// California,Colorado, Nevada, and Utah.  The river must be present in 
	///			// all states to be counted.
	///			Print("Rivers common to Arizona, California, Colorado, Nevada, and Utah:", 
	///				arizona &amp; california &amp; colorado &amp; nevada &amp; utah);
	///			//Just to prove to you that operators always return like types, let's do a
	///			//complex Set operation and look at the type of the result:
	///			Console.WriteLine("The type of this complex operation is: " + 
	///				((southWest ^ colorado &amp; california) | kansas).GetType().FullName);
	///		}
	///		private static void Print(string title, Set elements)
	///		{
	///			Console.WriteLine(title);
	///			foreach(object o in elements)
	///			{
	///				Console.WriteLine("\t" + o);
	///				Console.WriteLine();
	///			}    
	///		}
	///	}
	///			</code>
	///		</p>
	///		<p>
	///			Although there are other kinds of sets available in the library, the example uses 
	///			<c>SortedSet</c> throughout. This is nice for the example, since everything will 
	///			print neatly in alphabetical order. But you may be wondering what kind of <c>Set</c>
	///			is returned when you "union," "intersect," "exclusive-or," or "minus" two <c>Set</c>
	///			instances. The library always returns a <c>Set</c> that is the same type as 
	///			the <c>Set</c> on the left, unless the left operand is null, in which case it 
	///			returns the type of the <c>Set</c> on the right.
	///		</p>
	///		<p>
	///			Here is the output from running the example:
	///			<code>
	///All rivers:
	///Arkansas River
	///Colorado River
	///Green River
	///Missouri River
	///Rio Grande
	///Sacramento River
	///San Juan River
	///
	///Rivers in the southwest:
	///Arkansas River
	///Colorado River
	///Green River
	///Rio Grande
	///San Juan River
	///
	///Rivers in the west:
	///Colorado River
	///Sacramento River
	///
	///Rivers in the midwest:
	///Arkansas River
	///Missouri River
	///
	///Of all rivers, these don't pass through Colorado:
	///Missouri River
	///Sacramento River
	///San Juan River
	///
	///Rivers common to both Colorado and Utah:
	///Colorado River
	///Green River
	///
	///Rivers in Colorado and Utah that are not shared by both states:
	///Arkansas River
	///Rio Grande
	///San Juan River
	///
	///Rivers common to Arizona, California, Colorado, Nevada, and Utah:
	///Colorado River
	///
	///The type of this complex operation is: 
	///Iesi.Collections.SortedSet
	///Press any key to continue
	///			</code>
	///		</p> 
	/// </summary>
	internal sealed class NamespaceDoc
	{
	}
}
