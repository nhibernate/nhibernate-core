/// Refly License
/// 
/// Copyright (c) 2004 Jonathan de Halleux, http://www.dotnetwiki.org
///
/// This software is provided 'as-is', without any express or implied warranty. In no event will the authors be held liable for any damages arising from the use of this software.
/// 
/// Permission is granted to anyone to use this software for any purpose, including commercial applications, and to alter it and redistribute it freely, subject to the following restrictions:
///
/// 1. The origin of this software must not be misrepresented; you must not claim that you wrote the original software. If you use this software in a product, an acknowledgment in the product documentation would be appreciated but is not required.
/// 
/// 2. Altered source versions must be plainly marked as such, and must not be misrepresented as being the original software.
///
///3. This notice may not be removed or altered from any source distribution.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.CodeDom;

namespace Refly
{
	using Refly.CodeDom;
	/// <summary>
	/// Summary description for NameConformer.
	/// </summary>
	public class NameConformer
	{
		private Hashtable words = new Hashtable();
		private bool camelize=false;
		private bool capitalize=false;

		private static string[] basicWords = new String[]
			{
				"about",
				"account",
				"across",
				"addition",
				"adjustment",
				"after",
				"air",
				"all",
				"almost",
				"amount",
				"and",
				"angle",
				"angry",
				"animal",
				"answer",
				"ant",
				"any",
				"apparatus",
				"apple",
				"approval",
				"application",
				"arch",
				"argument",
				"arm",
				"army",
				"art",
				"assembly",
				"attack",
				"attempt",
				"attention",
				"attraction",
				"authority",
				"automatic",
				"awake",
				"baby",
				"back",
				"bad",
				"bag",
				"balance",
				"ball",
				"band",
				"base",
				"basin",
				"basket",
				"bath",
				"beautiful",
				"because",
				"bed",
				"bee",
				"before",
				"behaviour",
				"belief",
				"bell",
				"bent",
				"berry",
				"between",
				"bird",
				"birth",
				"bit",
				"bite",
				"bitter",
				"black",
				"blade",
				"blood",
				"blow",
				"blue",
				"board",
				"boat",
				"body",
				"boiling",
				"bone",
				"book",
				"boot",
				"bottle",
				"box",
				"boy",
				"brain",
				"brake",
				"branch",
				"brass",
				"bread",
				"breath",
				"brick",
				"bridge",
				"bright",
				"broken",
				"brother",
				"brown",
				"brush",
				"bucket",
				"building",
				"bulb",
				"burn",
				"burst",
				"business",
				"but",
				"butter",
				"button",
				"by",
				"cake",
				"chapter",
				"camera",
				"callout",
				"canvas",
				"card",
				"care",
				"carriage",
				"cart",
				"cat",
				"cause",
				"certain",
				"cite",
				"chain",
				"chalk",
				"chance",
				"change",
				"cheap",
				"cheese",
				"chemical",
				"chest",
				"chief",
				"chin",
				"church",
				"circle",
				"class",
				"clean",
				"clear",
				"clock",
				"cloth",
				"cloud",
				"conf",
				"coal",
				"coat",
				"cold",
				"collar",
				"colour",
				"comb",
				"come",
				"comfort",
				"committee",
				"common",
				"company",
				"comparison",
				"competition",
				"complete",
				"complex",
				"computer",
				"condition",
				"connection",
				"conscious",
				"constant",
				"constructor",
				"console",
				"contract",
				"control",
				"cook",
				"copper",
				"copy",
				"cord",
				"cork",
				"cotton",
				"cough",
				"country",
				"coverage",
				"cow",
				"crack",
				"credit",
				"crime",
				"cruel",
				"crush",
				"cry",
				"cup",
				"current",
				"curtain",
				"curve",
				"cushion",
				"damage",
				"danger",
				"dark",
				"data",
				"daughter",
				"day",
				"dead",
				"dear",
				"death",
				"debt",
				"decision",
				"deep",
				"default",
				"degree",
				"delicate",
				"dependent",
				"design",
				"desire",
				"destruction",
				"destructor",
				"detail",
				"development",
				"different",
				"digestion",
				"direction",
				"dirty",
				"discovery",
				"discussion",
				"disease",
				"disgust",
				"distance",
				"distribution",
				"division",
				"do",
				"dog",
				"door",
				"doubt",
				"down",
				"drain",
				"drawer",
				"dress",
				"drink",
				"driving",
				"drop",
				"dry",
				"dust",
				"ear",
				"early",
				"earth",
				"east",
				"edge",
				"education",
				"effect",
				"egg",
				"elastic",
				"electric",
				"end",
				"engine",
				"enough",
				"entry",
				"equal",
				"error",
				"even",
				"event",
				"ever",
				"every",
				"example",
				"exchange",
				"existence",
				"expansion",
				"experience",
				"expert",
				"extension",
				"eye",
				"face",
				"fact",
				"failure",
				"fall",
				"false",
				"family",
				"far",
				"farm",
				"fat",
				"father",
				"fear",
				"feather",
				"feeble",
				"feeling",
				"female",
				"fertile",
				"fiction",
				"field",
				"fight",
				"finger",
				"fire",
				"first",
				"fish",
				"fixed",
				"fixture",
				"flag",
				"flame",
				"flat",
				"flight",
				"floor",
				"flower",
				"fly",
				"fold",
				"food",
				"foolish",
				"foot",
				"for",
				"force",
				"fork",
				"form",
				"forward",
				"fowl",
				"frame",
				"free",
				"frequent",
				"friend",
				"from",
				"front",
				"fruit",
				"full",
				"future",
				"garden",
				"general",
				"get",
				"girl",
				"give",
				"glass",
				"glove",
				"go",
				"goat",
				"gold",
				"good",
				"graph",
				"grain",
				"grass",
				"great",
				"green",
				"grey",
				"grip",
				"group",
				"growth",
				"gui",
				"guide",
				"gun",
				"hair",
				"hammer",
				"hand",
				"hanging",
				"happy",
				"harbour",
				"hard",
				"harmony",
				"hat",
				"hate",
				"have",
				"he",
				"head",
				"healthy",
				"hear",
				"hearing",
				"heart",
				"heat",
				"help",
				"high",
				"history",
				"hole",
				"hollow",
				"hook",
				"hope",
				"horn",
				"horse",
				"hospital",
				"hour",
				"house",
				"how",
				"humour",
				"hyper",
				"ice",
				"icon",
				"idea",
				"if",
				"ill",
				"image",
				"important",
				"impulse",
				"in",
				"increase",
				"index",
				"inner",
				"industry",
				"ink",
				"insect",
				"instrument",
				"insurance",
				"int",
				"interest",
				"invention",
				"invoker",
				"iron",
				"island",
				"item",
				"itrace",
				"jelly",
				"jewel",
				"join",
				"journey",
				"judge",
				"jump",
				"keep",
				"kettle",
				"key",
				"kick",
				"kind",
				"kiss",
				"knee",
				"knife",
				"knot",
				"knowledge",
				"land",
				"language",
				"last",
				"late",
				"laugh",
				"law",
				"lead",
				"leaf",
				"learning",
				"leather",
				"left",
				"leg",
				"let",
				"letter",
				"level",
				"library",
				"lift",
				"light",
				"like",
				"limit",
				"line",
				"linen",
				"lip",
				"liquid",
				"list",
				"little",
				"living",
				"locator",
				"lock",
				"long",
				"look",
				"loose",
				"loss",
				"loud",
				"love",
				"low",
				"machine",
				"make",
				"male",
				"man",
				"manager",
				"map",
				"mark",
				"market",
				"married",
				"mass",
				"match",
				"material",
				"may",
				"meal",
				"measure",
				"meat",
				"medical",
				"meeting",
				"memory",
				"metal",
				"method",
				"middle",
				"military",
				"milk",
				"mind",
				"mine",
				"minute",
				"mist",
				"mixed",
				"module",
				"money",
				"monkey",
				"month",
				"moon",
				"morning",
				"mother",
				"motion",
				"mountain",
				"mouth",
				"move",
				"much",
				"muscle",
				"music",
				"nail",
				"name",
				"narrow",
				"nation",
				"natural",
				"near",
				"necessary",
				"neck",
				"need",
				"needle",
				"nerve",
				"net",
				"new",
				"news",
				"night",
				"node",
				"non",
				"noise",
				"normal",
				"north",
				"nose",
				"not",
				"note",
				"now",
				"null",
				"number",
				"nut",
				"observation",
				"of",
				"off",
				"offer",
				"office",
				"oil",
				"old",
				"on",
				"only",
				"open",
				"operation",
				"opinion",
				"opposite",
				"or",
				"orange",
				"order",
				"organization",
				"ornament",
				"other",
				"out",
				"output",
				"oven",
				"over",
				"owner",
				"page",
				"pain",
				"paint",
				"pair",
				"paper",
				"parallel",
				"parcel",
				"part",
				"past",
				"paste",
				"payment",
				"peace",
				"pen",
				"pencil",
				"person",
				"physical",
				"picture",
				"pig",
				"pin",
				"pipe",
				"place",
				"plane",
				"plant",
				"plate",
				"play",
				"please",
				"pleasure",
				"plough",
				"pocket",
				"point",
				"poison",
				"polish",
				"political",
				"poor",
				"port",
				"position",
				"possible",
				"pot",
				"potato",
				"powder",
				"power",
				"present",
				"price",
				"print",
				"prison",
				"private",
				"probable",
				"process",
				"produce",
				"profit",
				"property",
				"prose",
				"protest",
				"public",
				"pull",
				"pump",
				"punishment",
				"purpose",
				"push",
				"put",
				"quality",
				"question",
				"quick",
				"quiet",
				"quite",
				"rail",
				"rain",
				"range",
				"rat",
				"rate",
				"ray",
				"reaction",
				"reading",
				"ready",
				"reason",
				"receipt",
				"record",
				"red",
				"regret",
				"regular",
				"relation",
				"religion",
				"representative",
				"report",
				"request",
				"resource",
				"respect",
				"responsible",
				"rest",
				"result",
				"reward",
				"rhythm",
				"rice",
				"right",
				"ring",
				"river",
				"road",
				"rod",
				"roll",
				"roof",
				"room",
				"root",
				"rough",
				"round",
				"rub",
				"rule",
				"run",
				"sad",
				"safe",
				"sail",
				"salt",
				"same",
				"sand",
				"say",
				"scale",
				"school",
				"science",
				"scissors",
				"screw",
				"sea",
				"seat",
				"second",
				"secret",
				"secretary",
				"see",
				"seed",
				"seem",
				"selection",
				"self",
				"send",
				"sense",
				"separate",
				"serious",
				"servant",
				"set",
				"shade",
				"shake",
				"shame",
				"sharp",
				"sheep",
				"shelf",
				"ship",
				"shirt",
				"shock",
				"shoe",
				"short",
				"shut",
				"side",
				"sign",
				"silk",
				"silver",
				"simple",
				"sister",
				"size",
				"skin",
				"skirt",
				"sky",
				"sleep",
				"slip",
				"slope",
				"slow",
				"small",
				"smash",
				"smell",
				"smile",
				"smoke",
				"smooth",
				"snake",
				"sneeze",
				"snow",
				"soap",
				"society",
				"sock",
				"soft",
				"solid",
				"some",
				"son",
				"song",
				"sort",
				"sound",
				"soup",
				"south",
				"space",
				"spade",
				"special",
				"sponge",
				"spoon",
				"spring",
				"square",
				"stage",
				"stamp",
				"star",
				"start",
				"statement",
				"station",
				"steam",
				"steel",
				"stem",
				"step",
				"stick",
				"sticky",
				"stiff",
				"still",
				"stitch",
				"stocking",
				"stomach",
				"stone",
				"stop",
				"store",
				"story",
				"straight",
				"strange",
				"street",
				"stretch",
				"string",
				"strong",
				"structure",
				"substance",
				"such",
				"sudden",
				"sugar",
				"suggestion",
				"summer",
				"sun",
				"support",
				"surprise",
				"sweet",
				"synopsis",
				"swim",
				"system",
				"table",
				"tail",
				"take",
				"talk",
				"tall",
				"taste",
				"tax",
				"teaching",
				"tendency",
				"test",
				"than",
				"that",
				"the",
				"then",
				"theory",
				"there",
				"thick",
				"thin",
				"thing",
				"this",
				"thought",
				"thread",
				"throat",
				"through",
				"thumb",
				"thunder",
				"ticket",
				"tight",
				"till",
				"time",
				"tin",
				"tired",
				"title",
				"to",
				"toe",
				"together",
				"tomorrow",
				"tongue",
				"tooth",
				"top",
				"touch",
				"town",
				"trade",
				"train",
				"transport",
				"tray",
				"tree",
				"trick",
				"trouble",
				"trousers",
				"true",
				"turn",
				"twist",
				"type",
				"umbrella",
				"under",
				"unit",
				"up",
				"use",
				"value",
				"verse",
				"very",
				"vessel",
				"video",
				"view",
				"violent",
				"visit",
				"voice",
				"waiting",
				"walk",
				"wall",
				"warning",
				"warm",
				"wash",
				"waste",
				"watch",
				"water",
				"wave",
				"wax",
				"way",
				"weather",
				"week",
				"weight",
				"well",
				"west",
				"wet",
				"wheel",
				"when",
				"where",
				"while",
				"whip",
				"whistle",
				"white",
				"wide",
				"will",
				"wind",
				"window",
				"wine",
				"wing",
				"winter",
				"wire",
				"wise",
				"with",
				"woman",
				"wood",
				"wool",
				"word",
				"work",
				"worm",
				"wound",
				"writing",
				"wrong",
				"year",
				"yellow",
				"yesterday",
				"young"
			};

		public NameConformer()
		{
			foreach(string w in basicWords)
			{
				this.words.Add(w,null);
			}
		}

		public NameConformer(ICollection wordList)
		{
			foreach(string w in wordList)
			{
				this.words.Add(w,null);
			}
		}

		public bool Camelize
        {
            get
            {
                return this.camelize;
            }
            set
            {
                this.camelize=value;
            }
        }

		public bool Capitalize
        {
            get
            {
                return this.capitalize;
            }
            set
            {
                this.capitalize=value;
            }
        }

		public void AddWord(string w)
		{
			if (this.words.Contains(w.ToLower()))
				return;
			this.words.Add(w.ToLower(),null);
		}

		public string ToCamel(string name) {
			if (name==null)
				throw new ArgumentNullException("name");
			if (name.Length<=2)
				return name.ToLower();

			string oname = name.TrimStart('_');
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("_");
			sb.Append((oname.Substring(0,1)).ToLower());
			sb.Append(oname.Substring(1, oname.Length - 1));
			String cname=sb.ToString().Trim();;
            if(!this.camelize)
    			return cname;

			foreach(string s in WordSplit(sb.ToString().Trim()))
			{
				if (cname.Length==1)
					cname += s.ToLower();								
				else
					cname += Cap(s);
			}
			return LowerPlural(cname).Trim();
		}

		public string ToCapitalized(string name) 
		{
			if (name==null)
				throw new ArgumentNullException("name");
			if (name.Length<=1)
				return name.ToUpper();
			if (name.Length==2)
				return char.ToUpper(name[0]).ToString() + char.ToLower(name[1]).ToString();

			string trimmedName = name.TrimStart('_');
            string cname = trimmedName;
			if (this.capitalize)
			{
				string cname2=cname;
				cname="";
				foreach(string s in WordSplit(cname2))
				{
					cname += Cap(s);
				}
			}
			else
			{
				cname=Char.ToUpper(trimmedName[0])+trimmedName.Substring(1,trimmedName.Length-1);
			}
			return LowerPlural(cname).Trim();
		}


		public string NormalizeMember(string name, MemberAttributes attributes)
		{
			if (attributes==MemberAttributes.Public)
				return this.ToCapitalized(name);
			else
				return this.ToCamel(name);
		}

		public string ToSingular(string text)
		{
			if (text.Substring(text.Length-3,3).ToLower() == "ies")
			{
				return text.Substring(0,text.Length-3) + "y";
			} 
			else 
			{
				if (text.Substring(text.Length-1,1).ToLower() == "s")
				{
					return text.Substring(0,text.Length-1);
				} 
				else 
				{
					return text;
				}
			}
		}
/*
		private StringCollection WordSplit(string str)
		{
            if (str == null)
                throw new ArgumentNullException("str");
			StringCollection list = new StringCollection();
			// avoid short names
			if (str.Length<=2)
			{
				list.Add(str.ToLower());
				return list;
			}

			// split words
			int previous = 0;
			for (int i = 2;i<str.Length;++i)
			{
				for (int j=i+1;j<str.Length;++j)
				{
					String sub = str.Substring(previous,j-previous);
					if (this.words.Contains(sub))
					{
						// add to list
						list.Add(sub);
						previous = j;
						i=previous+1;
						break;
					}
				}
			}
			
			if (previous<str.Length)
				list.Add(str.Substring(previous,str.Length-previous));
			return list;
		}
*/

		private StringCollection WordSplit(string str) // KPixel - Brand new algo :)
		{
            if (str == null)
                throw new ArgumentNullException("str");

			str = str.ToLower();
			StringCollection list = new StringCollection();
			// avoid short names or check if present
			if (str.Length<=2 || this.words.Contains(str))
			{
				list.Add(str);
				return list;
			}


			// special case : Words precede by a letter 
			if(this.words.Contains(str.Substring(1)))
			{
				list.Add(str[0].ToString());
				list.Add(str.Substring(1));
				return list;
			}

			// split words
			for(int start=0; start<str.Length; start++)
			{
				while(char.IsDigit(str[start]))
				{
					list.Add(str[start].ToString());
					start++; // Don't take numbers
					if(start>=str.Length)
						break;
				}
				int end;
				for(end=str.Length; end>start; end--)
				{
					string word = str.Substring(start, end-start);
					if (this.words.Contains(word))
					{
						list.Add(word);
						break;
					}
				}

				if(end == start) // => not found
				{
					if(start < str.Length)
					{
System.Console.WriteLine( "\""  + str.Substring(start, str.Length-start) + "\"," ); // Note : Output the unknow word (to add it to the dictionnary)
						list.Add( str.Substring(start, str.Length-start) ); // Add this last word and stop
					}
					break;
				}
				else start = end - 1; // found, continue at the end of the precedent word
			}

			return list;
		}








		private string Cap(string str)
		{
			return string.Format("{0}{1}",
				Char.ToUpper(str[0]), 
				str.Substring(1,str.Length-1).ToLower()
				);
		}

		private string LowerPlural(string str)
		{
			if (str.ToLower().EndsWith("ies"))
			{
				return str.Substring(0, str.Length-3) + str.Substring(str.Length-3,3);
			}
			if (str.ToLower().EndsWith("s"))
			{
				if (str.Length==1)
					return str.ToLower();
				return str.Substring(0,str.Length-1) + Char.ToLower(str[str.Length-1]);
			}
			return str;
		}
	}
}
