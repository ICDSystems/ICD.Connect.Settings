using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronXmlLinq;
#else
using System.Xml.Linq;
#endif

namespace ICD.Connect.Settings.Utils
{
	public static class ConfigUtils
	{
		/// <summary>
		/// Gets all of the unique id attribute values from the given xml document.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static IEnumerable<int> GetIdsInDocument(string xml)
		{
			if (xml == null)
				throw new ArgumentNullException("xml");

			return GetIdsInDocument(xml, null);
		}

		/// <summary>
		/// Gets all of the unique id attribute values from elements with the given name in the xml document.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		public static IEnumerable<int> GetIdsInDocument(string xml, string element)
		{
			if (xml == null)
				throw new ArgumentNullException("xml");

			XDocument document = XDocument.Parse(xml);
			XElement root = document.Root;
			if (root == null)
				return Enumerable.Empty<int>();

			return RecursionUtils.BreadthFirstSearch(root, e => e.Elements())
			                     .Where(e => element == null || e.Name == element)
			                     .Select(e => e.Attribute("id"))
			                     .Where(a => a != null)
			                     .Select(a => int.Parse(a.Value))
			                     .Distinct();
		}
	}
}
