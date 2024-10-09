﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// A collection of name value pairs of the form 'name=value'.
	/// </summary>
	public class NameValuePairCollection : CollectionBase
	{		
		/// <summary>
		/// Generates a collection of name value pairs from a string of the form
		/// 'name1=value1;name2=value2'
		/// </summary>
		public NameValuePairCollection(string list)
		{
			Parse(list);
		}
		
		public NameValuePairCollection()
		{
		}
		
		/// <summary>
		/// Gets the value for the specified name.
		/// </summary>
		/// <returns>An empty string if the name cannot be found.</returns>
		public string GetValue(string name)
		{
			foreach (NameValuePair pair in List) {
				if (name == pair.Name) {
					return pair.Value;
				}
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Gets the name value pair at the specified index.
		/// </summary>
		public NameValuePair this[int index] {
			get {
				return (NameValuePair)List[index];
			}
		}
		
		/// <summary>
		/// Returns a list of name value pairs separated by a semi colon.
		/// </summary>
		public string GetList()
		{
			string[] nameValuePairs = new string[List.Count];
			for (int i = 0; i < nameValuePairs.Length; ++i) {
				NameValuePair pair = this[i];
				nameValuePairs[i] = pair.ToString();
			}
			return String.Join(";", nameValuePairs);
		}
		
		/// <summary>
		/// Adds a name value pair to the collection.
		/// </summary>
		public void Add(NameValuePair pair)
		{
			List.Add(pair);
		}
		
		/// <summary>
		/// Parses a list of name value pairs of the form
		/// 'name1=value1;name2=value2' and adds them to the NameValuePairCollection
		/// class.
		/// </summary>
		void Parse(string list)
		{
			if (list.Length == 0) {
				return;
			}

			string [] pairs = list.Split(';');
			foreach (string pair in pairs) {
				string[] nameValue = pair.Split(new char[] {'='}, 2);
				if (nameValue.Length > 1) {
					List.Add(new NameValuePair(nameValue[0].Trim(), nameValue[1].Trim()));
				} else {
					List.Add(new NameValuePair(pair));
				}
			}
		}
	}
}
