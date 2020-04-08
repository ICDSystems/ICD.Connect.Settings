using ICD.Common.Logging.LoggingContexts;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Utils;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Settings
{
	public static class PluginFactory
	{
		/// <summary>
		/// Maps factory name -> settings type
		/// </summary>
		private static readonly Dictionary<string, Type> s_FactoryNameTypeMap;

		private static readonly ServiceLoggingContext s_Logger;

		/// <summary>
		/// Constructor.
		/// </summary>
		static PluginFactory()
		{
			s_Logger = new ServiceLoggingContext(typeof(PluginFactory));
			s_FactoryNameTypeMap = new Dictionary<string, Type>();

			try
			{
				BuildCache();
			}
			catch (Exception e)
			{
				s_Logger.Log(eSeverity.Error, e, "Failed to cache plugins");
			}
		}

		#region Methods

		/// <summary>
		/// Finds the element in the xml document and instantiates the settings for each child.
		/// Skips and logs any elements that fail to parse.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="elementName"></param>
		/// <returns></returns>
		public static IEnumerable<ISettings> GetSettingsFromXml(string xml, string elementName)
		{
			string child;
			return XmlUtils.TryGetChildElementAsString(xml, elementName, out child)
				       ? GetSettingsFromXml(child)
				       : Enumerable.Empty<ISettings>();
		}

		/// <summary>
		/// Instantiates the settings for each child element in the xml document.
		/// Skips and logs any elements that fail to parse.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static IEnumerable<ISettings> GetSettingsFromXml(string xml)
		{
			foreach (string element in XmlUtils.GetChildElementsAsString(xml))
			{
				ISettings output;

				try
				{
					output = Instantiate(element);
				}
				catch (Exception e)
				{
					string name = XmlUtils.ReadElementName(element);
					string id = XmlUtils.HasAttribute(element, "id") ? XmlUtils.GetAttributeAsString(element, "id") : "NULL";

					s_Logger.Log(eSeverity.Error, e, "Skipping settings element {0} id {1} - {2}", name, id, e.Message);
					continue;
				}

				yield return output;
			}
		}

		/// <summary>
		/// Gets the available factory names.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<string> GetFactoryNames<TSettings>()
			where TSettings : ISettings
		{
			return s_FactoryNameTypeMap.Where(kvp => kvp.Value.IsAssignableTo(typeof(TSettings)))
			                           .Select(kvp => kvp.Key);
		}

		/// <summary>
		/// Gets all available factory names.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<string> GetFactoryNames()
		{
			return s_FactoryNameTypeMap.Keys;
		}

		/// <summary>
		/// Gets the assemblies for the loaded factories.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Assembly> GetFactoryAssemblies()
		{
			return s_FactoryNameTypeMap.Values
			                           .Select(v => v.GetAssembly())
			                           .Distinct();
		}

		/// <summary>
		/// Passes the xml to an available factory method and returns the result.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static ISettings Instantiate(string xml)
		{
			return Instantiate<ISettings>(xml);
		}

		/// <summary>
		/// Calls the default constructor for the class with the given factory name.
		/// </summary>
		/// <returns></returns>
		public static ISettings InstantiateDefault(string factoryName)
		{
			return InstantiateDefault<ISettings>(factoryName);
		}

		/// <summary>
		/// Gets the settings type for the given factory name.
		/// </summary>
		/// <param name="factoryName"></param>
		/// <returns></returns>
		public static Type GetType(string factoryName)
		{
			Type type;
			if (s_FactoryNameTypeMap.TryGetValue(factoryName, out type))
				return type;

			throw new KeyNotFoundException(string.Format("No factory name {0}", factoryName));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Passes the xml to an available factory method and returns the result.
		/// </summary>
		/// <param name="xml"></param>
		/// <typeparam name="TSettings"></typeparam>
		/// <returns></returns>
		private static TSettings Instantiate<TSettings>(string xml)
			where TSettings : ISettings
		{
			string factoryName = XmlUtils.GetAttributeAsString(xml, AbstractSettings.TYPE_ATTRIBUTE);

			try
			{
				TSettings settings = InstantiateDefault<TSettings>(factoryName);
				settings.ParseXml(xml);
				return settings;
			}
			catch (TargetInvocationException e)
			{
				if (e.InnerException != null)
					throw e.InnerException;
				throw;
			}
		}

		/// <summary>
		/// Calls the default constructor for the class with the given factory name.
		/// </summary>
		/// <param name="factoryName"></param>
		/// <typeparam name="TSettings"></typeparam>
		/// <returns></returns>
		private static TSettings InstantiateDefault<TSettings>(string factoryName)
			where TSettings : ISettings
		{
			if (factoryName == null)
				throw new ArgumentNullException("factoryName");

			Type type = GetType(factoryName);

			return (TSettings)ReflectionUtils.CreateInstance(type);
		}

		/// <summary>
		/// Lazy loads the cache.
		/// </summary>
		private static void BuildCache()
		{
			IEnumerable<Assembly> assemblies = LibraryUtils.GetPluginAssemblies();

			foreach (Assembly assembly in assemblies)
				CacheAssembly(assembly);

			foreach (string factoryName in s_FactoryNameTypeMap.Keys.Order())
				s_Logger.Log(eSeverity.Informational, "Loaded type {0}", factoryName);
		}

		private static void CacheAssembly(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");

			Type[] types;

			try
			{
				types = assembly.GetTypes()
#if SIMPLSHARP
				                .Select(t => (Type)t)
#endif
				                .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo<ISettings>())
				                .ToArray();
			}
#if STANDARD
			catch (ReflectionTypeLoadException e)
			{
				foreach (Exception inner in e.LoaderExceptions)
				{
					if (inner is System.IO.FileNotFoundException)
					{
						s_Logger.Log(eSeverity.Error,
						             "Failed to cache assembly {0} - Could not find one or more dependencies by path",
						             assembly.GetName().Name);
						continue;
					}

					s_Logger.Log(eSeverity.Error, inner, "Failed to cache assembly {0}",
					             assembly.GetName().Name);
				}

				return;
			}
#endif
			catch (TypeLoadException e)
			{
#if SIMPLSHARP
				s_Logger.Log(eSeverity.Error, "Failed to cache assembly {0}", assembly.GetName().Name);
#else
				s_Logger.Log(eSeverity.Error, "Failed to cache assembly {0} - could not load type {1}",
							 assembly.GetName().Name, e.TypeName);
#endif
				return;
			}

			foreach (Type type in types)
				CacheType(type);

			s_Logger.Log(eSeverity.Informational, "Loaded plugin {0}", assembly.GetName().Name);
		}

		/// <summary>
		/// Pre-emptively caches the given type for lookup.
		/// </summary>
		/// <param name="type"></param>
		private static void CacheType(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			try
			{
				KrangSettingsAttribute attribute = AttributeUtils.GetClassAttribute<KrangSettingsAttribute>(type, false);
				if (attribute == null)
					return;

				if (s_FactoryNameTypeMap.ContainsKey(attribute.FactoryName))
				{
					s_Logger.Log(eSeverity.Error, "Failed to cache {0} - Duplicate factory name {1}", type.Name, attribute.FactoryName);
					return;
				}

				s_FactoryNameTypeMap.Add(attribute.FactoryName, type);
			}
			// GetMethods for Open Generic Types is not supported.
			catch (NotSupportedException)
			{
			}
			// Not sure why this happens :/
			catch (InvalidProgramException)
			{
			}
		}

		#endregion
	}
}
