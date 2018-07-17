/*
 * Derived from MurmurHash2Simple,
 * http://landman-code.blogspot.com/2009/02/c-superfasthash-and-murmurhash2.html
 */

/***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is HashTableHashing.MurmurHash2.
 *
 * The Initial Developer of the Original Code is
 * Davy Landman.
 * Portions created by the Initial Developer are Copyright (C) 2009
 * the Initial Developer. All Rights Reserved.
 *
 * Contributor(s):
 *
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Text;

namespace NHibernate.Util
{
	/// <summary>A stable hasher using MurmurHash2 algorithm.</summary>
	internal static class Hasher
	{
		internal static string HashToString(string input)
		{
			var hash = Hash(input);
			return hash.ToString("X");
		}

		internal static uint Hash(string input)
		{
			return Hash(Encoding.UTF8.GetBytes(input));
		}

		internal static uint Hash(byte[] data)
		{
			return Hash(data, 0xc58f1a7b);
		}

		private const uint _m = 0x5bd1e995;
		private const int _r = 24;

		internal static uint Hash(byte[] data, uint seed)
		{
			var length = data.Length;
			if (length == 0)
				return 0;
			var h = seed ^ (uint) length;
			var currentIndex = 0;
			while (length >= 4)
			{
				var k = BitConverter.ToUInt32(data, currentIndex);
				k *= _m;
				k ^= k >> _r;
				k *= _m;

				h *= _m;
				h ^= k;
				currentIndex += 4;
				length -= 4;
			}

			switch (length)
			{
				case 3:
					h ^= BitConverter.ToUInt16(data, currentIndex);
					h ^= (uint) data[currentIndex + 2] << 16;
					h *= _m;
					break;
				case 2:
					h ^= BitConverter.ToUInt16(data, currentIndex);
					h *= _m;
					break;
				case 1:
					h ^= data[currentIndex];
					h *= _m;
					break;
			}

			// Do a few final mixes of the hash to ensure the last few
			// bytes are well-incorporated.

			h ^= h >> 13;
			h *= _m;
			h ^= h >> 15;

			return h;
		}
	}
}
