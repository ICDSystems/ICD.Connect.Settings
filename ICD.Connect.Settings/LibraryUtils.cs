using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Comparers;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.IO;
using ICD.Common.Utils.IO.Compression;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Settings.Attributes;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif

namespace ICD.Connect.Settings
{
	/// <summary>
	/// Utility methods for working with reflection.
	/// </summary>
	public static class LibraryUtils
	{
		private const string DLL_EXT = ".dll";

		private static readonly string[] s_ArchiveExtensions =
		{
			".CPZ",
			".CLZ",
			".CPLZ"
		};

		private static readonly string[] s_LibDirectories;

		private static ILoggerService Logger { get { return ServiceProvider.TryGetService<ILoggerService>(); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		static LibraryUtils()
		{
			s_LibDirectories = new[]
			{
				IcdDirectory.GetApplicationDirectory(),
				PathUtils.ProgramLibPath,
				PathUtils.CommonLibPath
			};
		}

		#region Methods

		/// <summary>
		/// Gets assemblies with the KrangPlugin assembly attribute
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Assembly> GetPluginAssemblies()
		{
			UnzipLibAssemblies();

			return GetAssemblyPaths().OrderBy<string, int>(GetDirectoryIndex)
			                         .Distinct(FileNameEqualityComparer.Instance)
			                         .Select<string, Assembly>(SafeLoadAssembly)
			                         .Where(a => a != null && IsKrangPlugin(a))
			                         .OrderBy(a => a.FullName);
		}

		/// <summary>
		/// Given a sequence of paths to plugin archives, returns the minimal
		/// sequence of plugin paths to provide all plugins.
		/// 
		/// E.g. skip over ICD.Common.Utils.clz because it is already available via ICD.Connect.Core.cpz
		/// </summary>
		/// <param name="paths"></param>
		/// <returns></returns>
		public static IEnumerable<string> FilterPluginPaths(IEnumerable<string> paths)
		{
			if (paths == null)
				throw new ArgumentNullException("paths");

			Dictionary<string, IcdHashSet<string>> pluginDlls =
				paths.Distinct()
				     .ToDictionary(p => p, p => GetPluginDllContents(p).ToIcdHashSet());

			Dictionary<string, string> pluginParents =
				pluginDlls.ToDictionary(kvp => kvp.Key,
				                        kvp => pluginDlls.FirstOrDefault(kvpInner => kvpInner.Value.IsProperSupersetOf(kvp.Value)).Key);

			return pluginParents.Where(kvp => kvp.Value == null)
			                    .Select(kvp => kvp.Key);
		}

		/// <summary>
		/// Given a path to a plugin archive yields the names of the contained dll files.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private static IEnumerable<string> GetPluginDllContents(string path)
		{
			if (!IcdFile.Exists(path) || !IsArchive(path))
				throw new ArgumentException("Path is not an archive", "path");

			return
				IcdZip.GetFileNames(path)
				      .Where(f => IcdPath.GetExtension(f).Equals(DLL_EXT, StringComparison.OrdinalIgnoreCase));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Unzips the archive at the given path.
		/// </summary>
		/// <param name="path"></param>
		private static void Unzip(string path)
		{
			string outputDir = PathUtils.GetPathWithoutExtension(path);

			if (IcdDirectory.Exists(path))
				RemoveOldPlugin(path);

			IcdZip.Unzip(path, outputDir);
		}

		private static void RemoveOldPlugin(string path)
		{
			try
			{
				IcdDirectory.Delete(path, true);
				Logger.AddEntry(eSeverity.Informational, "Removed old plugin {0}", path);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException(string.Format("Failed to remove old plugin {0} - {1}", path, e.Message));
			}
		}

		/// <summary>
		/// Loops over the archives in the lib directories and unzips them.
		/// </summary>
		private static void UnzipLibAssemblies()
		{
			IcdHashSet<string> pluginPaths = GetArchivePaths().ToIcdHashSet();
			IcdHashSet<string> filtered = FilterPluginPaths(pluginPaths).ToIcdHashSet();

			foreach (string path in pluginPaths.Order().Where(p => !IsProgramCpz(p)))
			{
				if (filtered.Contains(path))
				{
					try
					{
						Unzip(path);
					}
					catch (Exception e)
					{
						Logger.AddEntry(eSeverity.Warning, "Failed to extract archive {0} - {1}", path, e.Message);
						continue;
					}

					Logger.AddEntry(eSeverity.Informational, "Extracted archive {0}", path);
				}
				else
				{
					string outputDir = PathUtils.GetPathWithoutExtension(path);
					if (IcdDirectory.Exists(outputDir))
						RemoveOldPlugin(outputDir);

					Logger.AddEntry(eSeverity.Warning, "Skipping extracting archive {0} - Already contained in another plugin", path);
				}

				// Delete the archive so we don't waste time extracting on next load
				IcdFile.Delete(path);
			}
		}

		private static bool IsKrangPlugin(Assembly assembly)
		{
			try
			{
				return assembly.GetCustomAttributes<KrangPluginAttribute>().Any();
			}
			catch (FileNotFoundException)
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the paths to the available runtime assemblies.
		/// Assemblies may be located in 3 places, in order of importance:
		///		Program installation directory
		///		Program configuration
		///		Common configuration
		/// 
		/// Further, for the sake of convenience, check if this is running from a build directory
		/// and check sibling build directories for assemblies.
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<string> GetAssemblyPaths()
		{
			return GetBuildAssemblyPaths().Concat(GetLibAssemblyPaths());
		}

		/// <summary>
		/// Gets the paths to the available runtime assemblies.
		/// Assemblies may be located in 3 places, in order of importance:
		///		Program installation directory
		///		Program configuration
		///		Common configuration
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<string> GetLibAssemblyPaths()
		{
			return s_LibDirectories.SelectMany(d => PathUtils.RecurseFilePaths(d)).Where(IsAssembly);
		}

		/// <summary>
		/// If the program is being run from a build directory, finds assemblies from sibling projects
		/// that are part of the same solution.
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<string> GetBuildAssemblyPaths()
		{
#if !SIMPLSHARP
			string path = typeof(LibraryUtils).GetAssembly().GetPath();

			// Find the .sln
			while (path != null)
			{
				path = IcdPath.GetDirectoryName(path);
				bool foundSln = !string.IsNullOrEmpty(path) && IcdDirectory
									.GetFiles(path).Any(p => IcdPath.GetExtension(p).Equals(".sln", StringComparison.OrdinalIgnoreCase));

				if (foundSln)
					return PathUtils.RecurseFilePaths(path)
					                .Where(p => p.Contains("bin"))
					                .Where(IsAssembly)
					                .Where(p =>
					                       {
						                       // Avoid loading any SimplSharp assemblies
						                       string dir = IcdPath.GetDirectoryName(p);
						                       string[] files = IcdDirectory.GetFiles(dir);

						                       return files.All(f => !f.EndsWith("SimplSharpData.dat"));
					                       });
			}
#endif

			return Enumerable.Empty<string>();
		}

		/// <summary>
		/// Gets the paths to archives stored in lib directories.
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<string> GetArchivePaths()
		{
			return s_LibDirectories.SelectMany(d => PathUtils.RecurseFilePaths(d))
								   .Where(IsArchive);
		}

		/// <summary>
		/// Returns true if the given path represents a .CPZ file in a program directory.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private static bool IsProgramCpz(string path)
		{
			return IcdPath.GetExtension(path).Equals(".CPZ", StringComparison.OrdinalIgnoreCase) &&
			       path.Contains(IcdDirectory.GetApplicationDirectory());
		}

		/// <summary>
		/// Returns true if the file at the given path is an assembly.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private static bool IsAssembly(string path)
		{
			return string.Equals(IcdPath.GetExtension(path), DLL_EXT, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Returns true if the file at the given path is an archive.
		/// </summary>
		/// <param name="arg"></param>
		/// <returns></returns>
		private static bool IsArchive(string arg)
		{
			return s_ArchiveExtensions.Any(e => e.Equals(IcdPath.GetExtension(arg), StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Returns the LibDirectories index for the given path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private static int GetDirectoryIndex(string path)
		{
			return s_LibDirectories.FindIndex(p => path.ToLower().StartsWith(p.ToLower()));
		}

		/// <summary>
		/// Attempts to load the assembly at the given path. Returns null if an exception is caught.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		[CanBeNull]
		private static Assembly SafeLoadAssembly(string path)
		{
			try
			{
				return ReflectionUtils.LoadAssemblyFromPath(path);
			}
#if SIMPLSHARP
			// Happens with some crestron libraries
			catch (RestrictionViolationException)
			{
				return null;
			}
#else
			// Happens with the SQLite dll
			catch (BadImageFormatException)
			{
				return null;
			}
#endif
			catch (Exception e)
			{
				Logger.AddEntry(eSeverity.Warning, e, "Failed to load plugin {0} - {1}", path, e.Message);
				return null;
			}
		}

		#endregion
	}
}
