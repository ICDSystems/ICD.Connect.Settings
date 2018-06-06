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
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Settings
{
	public static class PluginFactory
	{
		/// <summary>
		/// Maps factory name -> settings type
		/// </summary>
		private static readonly Dictionary<string, Type> s_FactoryNameTypeMap;

		/// <summary>
		/// Maps settings type -> factory name
		/// </summary>
		private static readonly Dictionary<Type, string> s_FactoryNameMapInverse;

		private static ILoggerService Logger { get { return ServiceProvider.TryGetService<ILoggerService>(); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		static PluginFactory()
		{
			s_FactoryNameTypeMap = new Dictionary<string, Type>();
			s_FactoryNameMapInverse = new Dictionary<Type, string>();

			try
			{
				BuildCache();
			}
			catch (Exception e)
			{
				Logger.AddEntry(eSeverity.Error, e, "{0} failed to cache plugins", typeof(PluginFactory).Name);
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

					Logger.AddEntry(eSeverity.Error, "Skipping settings element {0} id {1} - {2}", name, id, e.Message);
					continue;
				}

				yield return output;
			}
		}

		/// <summary>
		/// Gets the factory name for the given settings type.
		/// </summary>
		/// <typeparam name="TSettings"></typeparam>
		/// <returns></returns>
		public static string GetFactoryName<TSettings>()
			where TSettings : ISettings
		{
			Type type = typeof(TSettings);

			if (!s_FactoryNameMapInverse.ContainsKey(type))
				throw new KeyNotFoundException(string.Format("Unable to find factory name for {0}", type.Name));
			return s_FactoryNameMapInverse[type];
		}

		/// <summary>
		/// Gets the available factory names.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<string> GetFactoryNames<TSettings>()
			where TSettings : ISettings
		{
			return s_FactoryNameMapInverse.Where(kvp => kvp.Key.IsAssignableTo(typeof(TSettings)))
			                               .Select(kvp => kvp.Value);
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
			                           .Select(v =>
#if SIMPLSHARP
			                                   ((CType)v)
#else
			                                   v.GetTypeInfo()
#endif
				                                   .Assembly)
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
				throw e.InnerException ?? e;
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
			{
				try
				{
					if (AttributeUtils.CacheAssembly(assembly))
						Logger.AddEntry(eSeverity.Informational, "Loaded plugin {0}", assembly.GetName().Name);
				}
				catch (Exception e)
				{
					Logger.AddEntry(eSeverity.Error, e, "{0} failed to load plugin {1}", typeof(PluginFactory).Name,
					                assembly.GetName().Name);
				}
			}

			foreach (
				KrangSettingsAttribute attribute in
					AttributeUtils.GetClassAttributes<KrangSettingsAttribute>().OrderBy(a => a.FactoryName))
			{
				Logger.AddEntry(eSeverity.Informational, "Loaded type {0}", attribute.FactoryName);

				Type type = AttributeUtils.GetClass(attribute);

				s_FactoryNameTypeMap.Add(attribute.FactoryName, type);
				s_FactoryNameMapInverse[type] = attribute.FactoryName;
			}
		}

		#endregion
	}
}
