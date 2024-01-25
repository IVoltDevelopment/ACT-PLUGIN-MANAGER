// Author           : Mark Alicz
// Created          : 05-16-2022
// Modified			  : 01-18-2024
// ***********************************************************************
// <copyright file="CurrentCore.cs" company="IVolt LLC" year="2024"/>
// <summary>
// Primary PLUGIN Factory Like Implemention
// TODO Implement Relational Plugin Matching
// </summary>
// ***********************************************************************
using ACT.Core.PluginManager.Extensions;
using System.Diagnostics;
using System.Reflection;
using ACT.PluginManager.Interfaces;
using ACT.Core.PluginManager.JSON_Objects;
using ACT.Core.PluginManager.LocalPlugins;

namespace ACT.Core.PluginManager
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Plugin Management Class. </summary>
	///
	/// <remarks>
	/// 1. Handles Creation Of Interface Plugin
	/// 2. Stores Plugin Assemblies and Details In Caches  
	/// 3. Allows Dynamic Compilation.
	/// </remarks>
	///
	/// <typeparam name="T">	Interface To Focus On. </typeparam>
	///
	/// <example>
	/// 1. GETS THE CURRENT HIGHESt priority Plugin or Internal Implementation
	/// 	I_Cached_Assembly = CurrentCore&lt;I_Cached_Assembly&gt;.GetCurrent();
	/// </example>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public static class CurrentCore<T>
	{



		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets local plugins. </summary>
		///
		/// <remarks>	Mark Alicz, 1/22/2024. </remarks>
		///
		/// <returns>	The local plugins. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal static T GetLocalPlugin()
		{
			if (__._local_cached_assemblies == null || __._local_cached_assemblies.Count() == 0)
			{
				LoadLocalPlugins(__.ResourceDirectory.EnsureDirectoryFormat() + "LocalPlugins\\");
			}

			if (__._local_cached_assemblies.First(x => x.Assembly_Types.Contains(typeof(T))) == null)
			{
				if (typeof(T) == typeof(I_Cached_Assembly))
				{
					return (T)__.LocalCachedAssemblies.First(x => x.Assembly_Types.Contains(typeof(T)));
				}
				else if (typeof(T) == typeof(I_Assembly_Loader))
				{
					return (T)__.LocalCachedAssemblies.First(x => x.Assembly_Types.Contains(typeof(T)));
				}
			}

			return default(T);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Gets the Current Default Interface Implementation as Defined in the Plugins Section of the
		/// Configuration File.
		/// </summary>
		///
		/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
		///
		/// <exception cref="TypeLoadException">	. </exception>
		///
		/// <returns>	T. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static T GetCurrent()
		{
			// Check For Cached Class
			if (__._CachedClasses.ContainsKey(typeof(T)))
			{
				if (__._CachedClasses[typeof(T)] == null) { __._CachedClasses.Remove(typeof(T)); }
				else { return (T)__._CachedClasses[typeof(T)]; }
			}

			global::System.Reflection.Assembly _A;
			object _TmpClass;

			Dictionary<I_Cached_Assembly, List<Type>> AvailablePlugins = __.GetAllPlugins<T>();


			if (AvailablePlugins != null)
			{
				foreach (var plugin in AvailablePlugins)
				{
					if (__.CachedAssemblies.Any(x => x.Full_PluginDLL_Path == plugin.Key.Full_PluginDLL_Path))
					{
						var _tmpAssCach = __.CachedAssemblies.First(x => x..Full_PluginDLL_Path == plugin.Key.Full_PluginDLL_Path);
						foreach(var _tmpType.
						if (__.CachedAssemblies.First(x => x.Full_PluginDLL_Path == plugin.Key.Full_PluginDLL_Path).Loaded_Assembly == null)
						{
							__.CachedAssemblies.First(x => x.Full_PluginDLL_Path == plugin.Key.Full_PluginDLL_Path).Loaded_Assembly = Assembly.LoadFile(plugin.Key.Full_PluginDLL_Path);
						}
					}
				}


				if (__.CachedAssemblies.Any(x => AvailablePlugins.Keys.SelectMany(x => x.Full_PluginDLL_Path == x.Full_PluginDLL_Path)    // Cached_Assemblies.First(x => x.Assembly_CacheID == _CachedAssemblyID).Loaded_Assembly;
				_TmpClass = _A.CreateInstance(_PluginInfo.FullClassName, true,
					global::System.Reflection.BindingFlags.CreateInstance, null, _PluginInfo.Arguments.ToArray(),
					global::System.Globalization.CultureInfo.CurrentCulture, null);
			}
			else
			{
				if (_PluginInfo.DllFullPath != "")
				{
					Assembly_Loader _AH = new Assembly_Loader();
					var _CA = _AH.Load_From_FilePath(_PluginInfo.DllFullPath);
					_CA.Assembly_CacheID = Guid.NewGuid();

					try
					{
						_TmpClass = _A.CreateInstance(_PluginInfo.FullClassName, true, global::System.Reflection.BindingFlags.CreateInstance, null, _PluginInfo.Arguments.ToArray(), global::System.Globalization.CultureInfo.CurrentCulture, null);
					}
					catch (Exception ex)
					{
						__.LogFatalError("CurrentCore<>.GetCurrent() + Error Loading Class " + _PluginInfo.FullClassName + " -- In Dll: " + _PluginInfo.DLLName, ex);
						throw new TypeLoadException("Error Locating Class " + _PluginInfo.FullClassName + " -- In Dll: " + typeof(T).FullName, ex);
					}

					if (AddCachedAssembly(new Cached_Assembly(_PluginInfo.DllFullPath, _PluginInfo.DLLName, typeof(T).FullName)))
					{
						var _CacheID = FindCachedAssembly(typeof(T));
						_A = Cached_Assemblies.First(x => x.Assembly_CacheID == _CacheID).Loaded_Assembly;
					}
					else
					{
						__.LogBasicInfo("CurrentCore<>.GetCurrent() + Error locating DLL: " + _PluginInfo.DLLName);
						throw new TypeLoadException("Error Locating " + typeof(T).FullName);
					}


				}
				else
				{
					try
					{
						_TmpClass = global::System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(_PluginInfo.FullClassName);
					}
					catch (Exception ex)
					{
						__.LogFatalError("CurrentCore<>.GetCurrent() + Error Loading Class " + _PluginInfo.FullClassName + " -- In Dll: " + _PluginInfo.DLLName, ex);
						throw new TypeLoadException("Error Locating " + _PluginInfo.FullClassName + " -- In Dll: " + typeof(T).FullName, ex);
					}
				}
			}

			if (_PluginInfo.StoreOnce) { _CachedClasses.Add(typeof(T), _TmpClass); }
			return (T)_TmpClass;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets a specific. </summary>
		///
		/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
		///
		/// <exception cref="TypeLoadException">	Thrown when a Type Load error condition occurs. </exception>
		///
		/// <param name="PluginInfo">	Information describing the plugin. </param>
		///
		/// <returns>	The specific. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static T GetSpecific(Plugin_Arguments PluginInfo)
		{

			if (PluginInfo.DllFullPath.NullOrEmpty())
			{
				__.LogBasicInfo("Using GetSpecific Without Specifying the DLL Path!");
			}

			// Check For Cached Clas
			if (_CachedClasses.ContainsKey(typeof(T)))
			{
				if (_CachedClasses[typeof(T)] == null) { _CachedClasses.Remove(typeof(T)); }
				else { return (T)_CachedClasses[typeof(T)]; }
			}

			var _PluginInfo = PluginInfo;

			global::System.Reflection.Assembly _A;
			object _TmpClass;

			Guid? _CachedAssemblyID = FindCachedAssembly(typeof(T), _PluginInfo.DllFullPath);

			if (_CachedAssemblyID != null)
			{
				_A = Cached_Assemblies.First(x => x.Assembly_CacheID == _CachedAssemblyID).Loaded_Assembly;
				_TmpClass = _A.CreateInstance(_PluginInfo.FullClassName, true,
					global::System.Reflection.BindingFlags.CreateInstance, null, _PluginInfo.Arguments.ToArray(),
					global::System.Globalization.CultureInfo.CurrentCulture, null);
			}
			else
			{
				if (_PluginInfo.DLLName != "")
				{
					if (AddCachedAssembly(_PluginInfo.DLLName, typeof(T).FullName))
					{
						var _CacheID = FindCachedAssembly(typeof(T));
						_A = Cached_Assemblies.First(x => x.Assembly_CacheID == _CacheID).Loaded_Assembly;
					}
					else
					{
						__.LogBasicInfo("CurrentCore<>.GetCurrent() + Error locating DLL: " + _PluginInfo.DLLName);
						throw new TypeLoadException("Error Locating " + typeof(T).FullName);
					}

					try
					{
						_TmpClass = _A.CreateInstance(_PluginInfo.FullClassName, true,
							global::System.Reflection.BindingFlags.CreateInstance, null, _PluginInfo.Arguments.ToArray(),
							global::System.Globalization.CultureInfo.CurrentCulture, null);
					}
					catch (Exception ex)
					{
						__.LogFatalError("CurrentCore<>.GetCurrent() + Error Loading Class " + _PluginInfo.FullClassName + " -- In Dll: " + _PluginInfo.DLLName, ex);
						throw new TypeLoadException("Error Locating Class " + _PluginInfo.FullClassName + " -- In Dll: " + typeof(T).FullName, ex);
					}
				}
				else
				{
					try
					{
						_TmpClass = global::System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(_PluginInfo.FullClassName);
					}
					catch (Exception ex)
					{
						__.LogFatalError("CurrentCore<>.GetCurrent() + Error Loading Class " + _PluginInfo.FullClassName + " -- In Dll: " + _PluginInfo.DLLName, ex);
						throw new TypeLoadException("Error Locating " + _PluginInfo.FullClassName + " -- In Dll: " + typeof(T).FullName, ex);
					}
				}
			}

			if (_PluginInfo.StoreOnce) { _CachedClasses.Add(typeof(T), _TmpClass); }
			return (T)_TmpClass;
		}

#endregion

		#region Private Methods



		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Loads all known paths. </summary>
		///
		/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private static void LoadAllKnownPaths()
		{
			LoadKnownPluginLocations();
			LoadSystemConfigurationPluginLocations();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Loads known plugin locations. </summary>
		///
		/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private static void LoadKnownPluginLocations()
		{
			List<string> _BadPaths = new List<string>();
			string _tmp = __.BaseDirectory + "resources\\plugins\\";
			_PluginDirectories.Add(_tmp);
			_tmp = __.BaseDirectory + "plugins\\";
			_PluginDirectories.Add(_tmp);
			_tmp = __.BaseDirectory + "bin\\plugins\\";
			_PluginDirectories.Add(_tmp);
			_tmp = __.BaseDirectory + "bin\\resources\\plugins\\";
			_PluginDirectories.Add(_tmp);
			//_tmp = ACT_Status.LoadResults.Active_Installation_Directory_FullFilePath + "plugins\\";
			//_PluginDirectories.Add(_tmp);
			//_tmp = ACT_Status.LoadResults.Active_Installation_Directory_FullFilePath + "resources\\plugins\\";
			//_PluginDirectories.Add(_tmp);
			//_tmp = ACT_Status.LoadResults.Active_Installation_Directory_FullFilePath + "bin\\resources\\plugins\\";
			//_PluginDirectories.Add(_tmp);
			//_tmp = ACT_Status.LoadResults.Active_Installation_Directory_FullFilePath + "bin\\plugins\\";
			//_PluginDirectories.Add(_tmp);

			_PluginDirectories.ForEach(x => { if (x.Ex.DirectoryExists() == false) { _BadPaths.Add(x); } });
			_BadPaths.ForEach(x => { _PluginDirectories.Remove(x); });
			_BadPaths.Clear();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Loads system configuration plugin locations. </summary>
		///
		/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private static void LoadSystemConfigurationPluginLocations()
		{
			var _AllPluginPathsFound = ACT_Status.LoadResults.Running_SystemConfiguration.Plugins.Where(x => x.FullPath.DirectoryExists()).Select(x => x.FullPath.GetDirectoryFromFileLocation().EnsureDirectoryFormat()).Distinct();

			_PluginDirectories.AddRange(_AllPluginPathsFound.ToArray());
			var _DistinctPluginPaths = _PluginDirectories.Distinct();
			_PluginDirectories.Clear();
			_PluginDirectories.AddRange(_DistinctPluginPaths);

		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Locates act built in plugins. </summary>
		///
		/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private static void Locate_ACTBuiltInPlugins()
		{
			string _FoundBasePluginDLL = "";

			string _tmpPath = "";
			foreach (string Path in _PluginDirectories)
			{
				_tmpPath = Path.EnsureDirectoryFormat() + "act_plugins.dll";
				if (_tmpPath.FileExists())
				{
					if (_FoundBasePluginDLL.NullOrEmpty()) { _FoundBasePluginDLL = _tmpPath; }
					else
					{
						FileInfo _FI1 = new FileInfo(_FoundBasePluginDLL);
						FileInfo _FI2 = new FileInfo(_tmpPath);

						if (_FI1.LastWriteTimeUtc < _FI2.LastWriteTimeUtc) { _FoundBasePluginDLL = _tmpPath; }
						else if (_FI1.LastWriteTimeUtc == _FI2.LastWriteTimeUtc) { continue; }
					}
				}
			}

			if (_FoundBasePluginDLL.NullOrEmpty())
			{
				ACTPluginsFullPath = null;
				__.LogFatalError("ACT Builtin Plugins are Missing", new FileNotFoundException("Missing ACT_Plugins.dll file. err|883467648"));
			}
			else { ACTPluginsFullPath = _FoundBasePluginDLL; }
		}

		#endregion
	}
}


/* OLD CODE PRESERVATION */
/*
 // ***********************************************************************
// Assembly         : ACT_Core
// Author           : Mark Alicz
// Created          : 05-16-2022
//
// Last Modified By : Mark Alicz
// Last Modified On : 10-28-2022
// ***********************************************************************
// <copyright file="CurrentCore.cs" company="IVolt LLC">
//     Copyright ©  2022
// </copyright>
// <summary>
// Primary PLUGIN Factory Like Implemention
// TODO Implement Relational Plugin Matching
// </summary>
// ***********************************************************************
using ACT.Core.Extensions;
using System.Diagnostics;
using System.Reflection;

namespace ACT.Core
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Holds the Plugin Assembly and a Single Type After Loading. </summary>
	///
	/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public class Cached_Assembly
	{
		public Cached_Assembly(string PluginDLL_Path, string Plugin_Name, global::System.Reflection.Assembly AssemblyToAdd)
		{


			Full_PluginDLL_Path = PluginDLL_Path;
			PluginName = Plugin_Name;
			Loaded_Assembly = AssemblyToAdd;
		}

		public string GetVersion()
		{
			//using(var z = new FileInfo(Full_PluginDLL_Path).Attributes[
			// Get the AssemblyFileVersionAttribute
			var fileVersionAttribute = Loaded_Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
			string fileVersion = fileVersionAttribute?.Version;

			// Get the AssemblyVersionAttribute
			var assemblyVersionAttribute = Loaded_Assembly.GetCustomAttribute<AssemblyVersionAttribute>();
			string assemblyVersion = assemblyVersionAttribute?.Version;

			string _ReturnVersion = fileVersion.Combine(assemblyVersion, true, true);
		}

		/// <summary>	Identifier for the assembly cache. </summary>
		private Guid _Assembly_CacheID = Guid.NewGuid();

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the identifier of the assembly cache. </summary>
		///
		/// <value>	The identifier of the assembly cache. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public Guid Assembly_CacheID { get { return _Assembly_CacheID; } }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the full pathname of the full plugin DLL file. </summary>
		///
		/// <value>	The full pathname of the full plugin DLL file. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public string Full_PluginDLL_Path { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the type of the assembly. </summary>
		///
		/// <value>	The type of the assembly. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public List<Type> Assembly_Types { get; set; } = new List<Type>();

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the name of the plugin. </summary>
		///
		/// <value>	The name of the plugin. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public string PluginName { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the loaded assembly. </summary>
		///
		/// <value>	The loaded assembly. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public global::System.Reflection.Assembly Loaded_Assembly { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the Date/Time of the date loaded. </summary>
		///
		/// <value>	The date loaded. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public DateTime DateLoaded { get; set; } = DateTime.Now;

	}

	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	A current core. </summary>
	///
	/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
	///
	/// <typeparam name="T">	Generic type parameter. </typeparam>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public static class CurrentCore<T>
	{
		/// <summary>	The available interfaces. </summary>
		public static Dictionary<string, List<Type>> Available_ApplicationDefined_Interfaces = new Dictionary<string, List<Type>>();

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Loads all plugins. </summary>
		///
		/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
		///
		/// <param name="PluginPathBaseDirectory">	Full pathname that contains all of the plugin files (Recursive). </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void LoadAllPlugins(string PluginPathBaseDirectory)
		{
			var _AllPlugins = Directory.GetFiles(PluginPathBaseDirectory, "*.dll");

			foreach (string PluginPath in _AllPlugins.Where(x => File.Exists(x) == true).Select(x => x))
			{
				try
				{
					Assembly _WorkingAssembly = Assembly.Load(PluginPath);
					List<Type> _Interfaces = _WorkingAssembly.GetExportedTypes().Where(x => x.IsInterface).ToList();
					Available_ApplicationDefined_Interfaces.Add(PluginPath, _Interfaces);
					FileInfo _FI = new FileInfo(PluginPath);
					Cached_Assembly _CA = new Cached_Assembly(PluginPath, _FI.Name, _WorkingAssembly);
					AddCachedAssembly(_CA);
				}
				catch
				{

				}
			}
		}

		#region Internal Private Properties

		/// <summary>	The cached assemblies. </summary>
		static List<Cached_Assembly> Cached_Assemblies = new List<Cached_Assembly>();

		/// <summary>	The cached classes TODO. </summary>
		static Dictionary<Type, object> _CachedClasses = new Dictionary<Type, object>();

		/// <summary>	Private List of Plugin Directories Defined. </summary>
		static List<string> _PluginDirectories = new List<string>();

		#endregion

		#region Public Properties

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the cached assemblies. </summary>
		///
		/// <value>	The cached assemblies. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static List<Cached_Assembly> CachedAssemblies { get { return Cached_Assemblies; } }


		/// <summary>	List of types of the cached strings. </summary>
		public static Dictionary<string, (string, string)> CachedStringTypes = new Dictionary<string, (string, string)>();



		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	List of Plugin Directories Defined. </summary>
		///
		/// <value>	The get plugin directories. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static List<string> GetPluginDirectories
		{
			get
			{
				// IF NO Plugin Directories Are Defined and Saved -- Load Known Plugins
				if (!_PluginDirectories.Any()) { LoadAllKnownPaths(); }

				return _PluginDirectories;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Full file Path to ACT Build In Plugins. </summary>
		///
		/// <value>	The full pathname of the act plugins full file. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static string ACTPluginsFullPath { get; set; } = null;
		#endregion

		#region Public Methods

		/// <summary>
		/// Adds the cached assembly.
		/// </summary>
		/// <param name="AssemblyTooAdd">The assembly too add.</param>
		/// <returns>Null if AssemblyToAdd is Null, True if Added, False if Not Added.</returns>
		public static bool AddCachedAssembly(Cached_Assembly AssemblyTooAdd)
		{
			if (AssemblyTooAdd == null) { throw new ArgumentNullException("Parameter Must Not Be Null"); }
			if (Cached_Assemblies.Exists(x => x.PluginName == AssemblyTooAdd.PluginName && x.Assembly_Types.Count == x.Assembly_Types.Count)) { return true; }

			return false;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		///// <summary>	Adds the cached assembly If DLL if Found and Loaded Properly. </summary>
		/////
		///// <remarks>	Mark Alicz, 1/11/2024. </remarks>
		/////
		///// <param name="DLL">					The DLL PluginName or Full DLL Path. </param>
		///// <param name="TypeStringName">	The type. </param>
		/////
		///// <returns>	<c>true</c> if XXXX, <c>false</c> otherwise. </returns>
		//////////////////////////////////////////////////////////////////////////////////////////////////////

		//public static bool AddCachedAssembly(string DLL, string TypeStringName)
		//{
		//	if (DLL.NullOrEmpty() || TypeStringName.NullOrEmpty())
		//	{
		//		__.LogFatalError("Error Parameters are Empty or Null", null);
		//		return false;
		//	}

		//	string _FullDLLPath = "";
		//	global::System.Reflection.Assembly _A = null;
		//	Type _T;

		//	// Try and Load The Type
		//	try { _T = Type.GetType(TypeStringName); }
		//	catch
		//	{
		//		__.LogFatalError("Invalid Type String: " + TypeStringName, null);
		//		return false;
		//	}

		//	// Type Must NOT Be Null
		//	if (_T == null) { return false; }

		//	// If DLL is Not Full Path Then Loop and Find the DLL Full PAth
		//	if (DLL.FileExists()) { _FullDLLPath = DLL; }
		//	else
		//	{
		//		foreach (string Path in _PluginDirectories)
		//		{
		//			if ((Path + DLL).FileExists() == false) { }
		//			else
		//			{
		//				_FullDLLPath = Path + DLL;
		//				break;
		//			}
		//		}
		//	}

		//	//Try and Load the Assembly Found
		//	try { _A = global::System.Reflection.Assembly.LoadFile(_FullDLLPath); }
		//	catch (Exception ex) { _.LogFatalError("Error Loading Plugin Assembly: " + _FullDLLPath + " -- With Message " + ex.Message, ex); }

		//	// Ensure the Assembly was Loaded
		//	if (_A == null) { return false; }

		//	// Cache The Assembly IF IT Hasnt Been Cached
		//	if (Cached_Assemblies.Exists(x => (x.PluginName == _T.FullName) && x.Assembly_Types == _T && _FullDLLPath == x.Full_PluginDLL_Path) == false)
		//	{
		//		try { Cached_Assemblies.Add(new CachedAssembly() { PluginName = _T.FullName, Assembly_Types = _T, Loaded_Assembly = _A, Full_PluginDLL_Path = _A.Location }); }
		//		catch (Exception ex)
		//		{
		//			__.LogFatalError("Error Caching The Assembly: " + ex.Message, ex);
		//			return false;
		//		}
		//	}

		//	return true;
		//}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Checks and Finds Cached Type. </summary>
		///
		/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
		///
		/// <param name="DllType">			Type of Cached DLL. </param>
		/// <param name="DllFullPath">	(Optional) Full Path to DLL or Blank if Not Needed. </param>
		///
		/// <returns>	CachedTypeID or Null. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static Guid? FindCachedAssembly(Type DllType, string DllFullPath = "")
		{
			if (Cached_Assemblies.Any() == false) { return null; }

			var _AllFoundTypes = Cached_Assemblies.Where(x => x.Assembly_Types.Contains(DllType)).ToList();
			

				if (Cached_Assemblies.Select(x =>
										 x.Assembly_Types == DllType &&
										 (x.Full_PluginDLL_Path == DllFullPath || DllFullPath.NullOrEmpty()))
									 .Count() > 1)
				{
					__.LogBasicInfo("Cached Assemblies Contain Multiple Type Defined Plugins: " + DllType.FullName);
				}

				// Returns the First One.  Logs When there are more than one tyoe.
				return Cached_Assemblies.Where(x => x.Assembly_Types == DllType && (x.Full_PluginDLL_Path == DllFullPath || DllFullPath.NullOrEmpty())).OrderByDescending(y => y.DateLoaded).First().CacheID;
			}

			return null;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Gets the Current Default Interface Implementation as Defined in the Plugins Section of the
		/// Configuration File.
		/// </summary>
		///
		/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
		///
		/// <exception cref="TypeLoadException">	. </exception>
		///
		/// <returns>	T. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static T GetCurrent()
		{
			// Check For Cached Clas
			if (_CachedClasses.ContainsKey(typeof(T)))
			{
				if (_CachedClasses[typeof(T)] == null) { _CachedClasses.Remove(typeof(T)); }
				else { return (T)_CachedClasses[typeof(T)]; }
			}

			var _PluginInfo = new Plugin_Arguments(typeof(T).FullName);

			global::System.Reflection.Assembly _A;
			object _TmpClass;

			Guid? _CachedAssemblyID = FindCachedAssembly(typeof(T));

			if (_CachedAssemblyID != null)
			{
				_A = Cached_Assemblies.First(x => x.Assembly_CacheID == _CachedAssemblyID).Loaded_Assembly;
				_TmpClass = _A.CreateInstance(_PluginInfo.FullClassName, true,
					global::System.Reflection.BindingFlags.CreateInstance, null, _PluginInfo.Arguments.ToArray(),
					global::System.Globalization.CultureInfo.CurrentCulture, null);
			}
			else
			{
				if (_PluginInfo.DLLName != "")
				{
					if (AddCachedAssembly(_PluginInfo.DLLName, typeof(T).FullName))
					{
						var _CacheID = FindCachedAssembly(typeof(T));
						_A = Cached_Assemblies.First(x => x.Assembly_CacheID == _CacheID).Loaded_Assembly;
					}
					else
					{
						__.LogBasicInfo("CurrentCore<>.GetCurrent() + Error locating DLL: " + _PluginInfo.DLLName);
						throw new TypeLoadException("Error Locating " + typeof(T).FullName);
					}

					try
					{
						_TmpClass = _A.CreateInstance(_PluginInfo.FullClassName, true, global::System.Reflection.BindingFlags.CreateInstance, null, _PluginInfo.Arguments.ToArray(), global::System.Globalization.CultureInfo.CurrentCulture, null);
					}
					catch (Exception ex)
					{
						__.LogFatalError("CurrentCore<>.GetCurrent() + Error Loading Class " + _PluginInfo.FullClassName + " -- In Dll: " + _PluginInfo.DLLName, ex);
						throw new TypeLoadException("Error Locating Class " + _PluginInfo.FullClassName + " -- In Dll: " + typeof(T).FullName, ex);
					}
				}
				else
				{
					try
					{
						_TmpClass = global::System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(_PluginInfo.FullClassName);
					}
					catch (Exception ex)
					{
						__.LogFatalError("CurrentCore<>.GetCurrent() + Error Loading Class " + _PluginInfo.FullClassName + " -- In Dll: " + _PluginInfo.DLLName, ex);
						throw new TypeLoadException("Error Locating " + _PluginInfo.FullClassName + " -- In Dll: " + typeof(T).FullName, ex);
					}
				}
			}

			if (_PluginInfo.StoreOnce) { _CachedClasses.Add(typeof(T), _TmpClass); }
			return (T)_TmpClass;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets a specific. </summary>
		///
		/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
		///
		/// <exception cref="TypeLoadException">	Thrown when a Type Load error condition occurs. </exception>
		///
		/// <param name="PluginInfo">	Information describing the plugin. </param>
		///
		/// <returns>	The specific. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static T GetSpecific(Plugin_Arguments PluginInfo)
		{

			if (PluginInfo.DllFullPath.NullOrEmpty())
			{
				__.LogBasicInfo("Using GetSpecific Without Specifying the DLL Path!");
			}

			// Check For Cached Clas
			if (_CachedClasses.ContainsKey(typeof(T)))
			{
				if (_CachedClasses[typeof(T)] == null) { _CachedClasses.Remove(typeof(T)); }
				else { return (T)_CachedClasses[typeof(T)]; }
			}

			var _PluginInfo = PluginInfo;

			global::System.Reflection.Assembly _A;
			object _TmpClass;

			Guid? _CachedAssemblyID = FindCachedAssembly(typeof(T), _PluginInfo.DllFullPath);

			if (_CachedAssemblyID != null)
			{
				_A = Cached_Assemblies.First(x => x.Assembly_CacheID == _CachedAssemblyID).Loaded_Assembly;
				_TmpClass = _A.CreateInstance(_PluginInfo.FullClassName, true,
					global::System.Reflection.BindingFlags.CreateInstance, null, _PluginInfo.Arguments.ToArray(),
					global::System.Globalization.CultureInfo.CurrentCulture, null);
			}
			else
			{
				if (_PluginInfo.DLLName != "")
				{
					if (AddCachedAssembly(_PluginInfo.DLLName, typeof(T).FullName))
					{
						var _CacheID = FindCachedAssembly(typeof(T));
						_A = Cached_Assemblies.First(x => x.Assembly_CacheID == _CacheID).Loaded_Assembly;
					}
					else
					{
						__.LogBasicInfo("CurrentCore<>.GetCurrent() + Error locating DLL: " + _PluginInfo.DLLName);
						throw new TypeLoadException("Error Locating " + typeof(T).FullName);
					}

					try
					{
						_TmpClass = _A.CreateInstance(_PluginInfo.FullClassName, true,
							global::System.Reflection.BindingFlags.CreateInstance, null, _PluginInfo.Arguments.ToArray(),
							global::System.Globalization.CultureInfo.CurrentCulture, null);
					}
					catch (Exception ex)
					{
						__.LogFatalError("CurrentCore<>.GetCurrent() + Error Loading Class " + _PluginInfo.FullClassName + " -- In Dll: " + _PluginInfo.DLLName, ex);
						throw new TypeLoadException("Error Locating Class " + _PluginInfo.FullClassName + " -- In Dll: " + typeof(T).FullName, ex);
					}
				}
				else
				{
					try
					{
						_TmpClass = global::System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(_PluginInfo.FullClassName);
					}
					catch (Exception ex)
					{
						__.LogFatalError("CurrentCore<>.GetCurrent() + Error Loading Class " + _PluginInfo.FullClassName + " -- In Dll: " + _PluginInfo.DLLName, ex);
						throw new TypeLoadException("Error Locating " + _PluginInfo.FullClassName + " -- In Dll: " + typeof(T).FullName, ex);
					}
				}
			}

			if (_PluginInfo.StoreOnce) { _CachedClasses.Add(typeof(T), _TmpClass); }
			return (T)_TmpClass;
		}

		#endregion

		#region Private Methods

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Loads all known paths. </summary>
		///
		/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private static void LoadAllKnownPaths()
		{
			LoadKnownPluginLocations();
			LoadSystemConfigurationPluginLocations();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Loads known plugin locations. </summary>
		///
		/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private static void LoadKnownPluginLocations()
		{
			List<string> _BadPaths = new List<string>();
			string _tmp = __.BaseDirectory + "resources\\plugins\\";
			_PluginDirectories.Add(_tmp);
			_tmp = __.BaseDirectory + "plugins\\";
			_PluginDirectories.Add(_tmp);
			_tmp = __.BaseDirectory + "bin\\plugins\\";
			_PluginDirectories.Add(_tmp);
			_tmp = __.BaseDirectory + "bin\\resources\\plugins\\";
			_PluginDirectories.Add(_tmp);
			_tmp = ACT_Status.LoadResults.Active_Installation_Directory_FullFilePath + "plugins\\";
			_PluginDirectories.Add(_tmp);
			_tmp = ACT_Status.LoadResults.Active_Installation_Directory_FullFilePath + "resources\\plugins\\";
			_PluginDirectories.Add(_tmp);
			_tmp = ACT_Status.LoadResults.Active_Installation_Directory_FullFilePath + "bin\\resources\\plugins\\";
			_PluginDirectories.Add(_tmp);
			_tmp = ACT_Status.LoadResults.Active_Installation_Directory_FullFilePath + "bin\\plugins\\";
			_PluginDirectories.Add(_tmp);

			_PluginDirectories.ForEach(x => { if (x.DirectoryExists() == false) { _BadPaths.Add(x); } });
			_BadPaths.ForEach(x => { _PluginDirectories.Remove(x); });
			_BadPaths.Clear();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Loads system configuration plugin locations. </summary>
		///
		/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private static void LoadSystemConfigurationPluginLocations()
		{
			var _AllPluginPathsFound = ACT_Status.LoadResults.Running_SystemConfiguration.Plugins.Where(x => x.FullPath.DirectoryExists()).Select(x => x.FullPath.GetDirectoryFromFileLocation().EnsureDirectoryFormat()).Distinct();

			_PluginDirectories.AddRange(_AllPluginPathsFound.ToArray());
			var _DistinctPluginPaths = _PluginDirectories.Distinct();
			_PluginDirectories.Clear();
			_PluginDirectories.AddRange(_DistinctPluginPaths);

		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Locates act built in plugins. </summary>
		///
		/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private static void Locate_ACTBuiltInPlugins()
		{
			string _FoundBasePluginDLL = "";

			string _tmpPath = "";
			foreach (string Path in _PluginDirectories)
			{
				_tmpPath = Path.EnsureDirectoryFormat() + "act_plugins.dll";
				if (_tmpPath.FileExists())
				{
					if (_FoundBasePluginDLL.NullOrEmpty()) { _FoundBasePluginDLL = _tmpPath; }
					else
					{
						FileInfo _FI1 = new FileInfo(_FoundBasePluginDLL);
						FileInfo _FI2 = new FileInfo(_tmpPath);

						if (_FI1.LastWriteTimeUtc < _FI2.LastWriteTimeUtc) { _FoundBasePluginDLL = _tmpPath; }
						else if (_FI1.LastWriteTimeUtc == _FI2.LastWriteTimeUtc) { continue; }
					}
				}
			}

			if (_FoundBasePluginDLL.NullOrEmpty())
			{
				ACTPluginsFullPath = null;
				__.LogFatalError("ACT Builtin Plugins are Missing", new FileNotFoundException("Missing ACT_Plugins.dll file. err|883467648"));
			}
			else { ACTPluginsFullPath = _FoundBasePluginDLL; }
		}

		#endregion
	}
}

/* OLD CODE PRESERVATION */

/*

/// < summary >
/// Gets the Current Default Interface Implementation as Defined in the Plugins Section of the Configuration File
/// </summary>
/// <param name="SettingsInstance">The settings instance.</param>
/// <returns>T.</returns>
/// <exception cref="System.TypeLoadException">Error Locating " + typeof(T).FullName</exception>
public static T GetCurrent(SystemSettingsInstance SettingsInstance)
{
	Plugin_Arguments _PluginInfo;

	if (_CachedClasses.ContainsKey(typeof(T)))
	{
		if (_CachedClasses[typeof(T)] == null)
		{
			_CachedClasses.Remove(typeof(T));
		}
		else
		{
			return (T)_CachedClasses[typeof(T)];
		}
	}

	_PluginInfo = new Plugin_Arguments(typeof(T).FullName, SettingsInstance);

	System.Reflection.Assembly _A;
	object _TmpClass;

	if (Cached_Assemblies.ContainsKey((typeof(T), typeof(T).FullName)))
	{
		_A = Cached_Assemblies[(typeof(T), typeof(T).FullName)];
		_TmpClass = _A.CreateInstance(_PluginInfo.FullClassName, true, System.Reflection.BindingFlags.CreateInstance, null, _PluginInfo.Arguments.ToArray(), System.Globalization.CultureInfo.CurrentCulture, null);
	}
	else
	{
		if (_PluginInfo.DLLName != "")
		{
			try
			{
				try
				{
					_A = System.Reflection.Assembly.LoadFile(__.BaseDirectory + _PluginInfo.DLLName);
				}
				catch
				{
					_A = System.Reflection.Assembly.LoadFile(__.BaseDirectory + "bin\\" + _PluginInfo.DLLName);
				}

				Cached_Assemblies.Add((typeof(T), typeof(T).FullName), _A);
				_TmpClass = _A.CreateInstance(_PluginInfo.FullClassName, true, System.Reflection.BindingFlags.CreateInstance, null, _PluginInfo.Arguments.ToArray(), System.Globalization.CultureInfo.CurrentCulture, null);

			}
			catch (Exception ex)
			{
				throw new System.TypeLoadException("Error Locating " + typeof(T).FullName, ex);
			}

		}
		else
		{
			_TmpClass = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(_PluginInfo.FullClassName);
		}
	}

	if (_PluginInfo.StoreOnce == true)
	{
		_CachedClasses.Add(typeof(T), _TmpClass);
	}

	return (T)_TmpClass;
}

*/


/*
		 OLD CODE OLDER 
/// < summary >
/// Gets the Current Default Interface Implementation as Defined in the Plugins Section of the Configuration File
/// </summary>
/// <param name="SettingsInstance">The settings instance.</param>
/// <returns>T.</returns>
/// <exception cref="System.TypeLoadException">Error Locating " + typeof(T).FullName</exception>
public static T GetCurrent(SystemSettingsInstance SettingsInstance)
{
	Plugin_Arguments _PluginInfo;

	if (_CachedClasses.ContainsKey(typeof(T)))
	{
		if (_CachedClasses[typeof(T)] == null)
		{
			_CachedClasses.Remove(typeof(T));
		}
		else
		{
			return (T)_CachedClasses[typeof(T)];
		}
	}

	_PluginInfo = new Plugin_Arguments(typeof(T).FullName, SettingsInstance);

	System.Reflection.Assembly _A;
	object _TmpClass;

	if (Cached_Assemblies.ContainsKey((typeof(T), typeof(T).FullName)))
	{
		_A = Cached_Assemblies[(typeof(T), typeof(T).FullName)];
		_TmpClass = _A.CreateInstance(_PluginInfo.FullClassName, true, System.Reflection.BindingFlags.CreateInstance, null, _PluginInfo.Arguments.ToArray(), System.Globalization.CultureInfo.CurrentCulture, null);
	}
	else
	{
		if (_PluginInfo.DLLName != "")
		{
			try
			{
				try
				{
					_A = System.Reflection.Assembly.LoadFile(__.BaseDirectory + _PluginInfo.DLLName);
				}
				catch
				{
					_A = System.Reflection.Assembly.LoadFile(__.BaseDirectory + "bin\\" + _PluginInfo.DLLName);
				}

				Cached_Assemblies.Add((typeof(T), typeof(T).FullName), _A);
				_TmpClass = _A.CreateInstance(_PluginInfo.FullClassName, true, System.Reflection.BindingFlags.CreateInstance, null, _PluginInfo.Arguments.ToArray(), System.Globalization.CultureInfo.CurrentCulture, null);

			}
			catch (Exception ex)
			{
				throw new System.TypeLoadException("Error Locating " + typeof(T).FullName, ex);
			}

		}
		else
		{
			_TmpClass = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(_PluginInfo.FullClassName);
		}
	}

	if (_PluginInfo.StoreOnce == true)
	{
		_CachedClasses.Add(typeof(T), _TmpClass);
	}

	return (T)_TmpClass;
}

//////////////////////////////////////////////////////////////////////////////////////////////////////
		///// <summary>	Adds the cached assembly If DLL if Found and Loaded Properly. </summary>
		/////
		///// <remarks>	Mark Alicz, 1/11/2024. </remarks>
		/////
		///// <param name="DLL">					The DLL PluginName or Full DLL Path. </param>
		///// <param name="TypeStringName">	The type. </param>
		/////
		///// <returns>	<c>true</c> if XXXX, <c>false</c> otherwise. </returns>
		//////////////////////////////////////////////////////////////////////////////////////////////////////

		//public static bool AddCachedAssembly(string DLL, string TypeStringName)
		//{
		//	if (DLL.NullOrEmpty() || TypeStringName.NullOrEmpty())
		//	{
		//		__.LogFatalError("Error Parameters are Empty or Null", null);
		//		return false;
		//	}

		//	string _FullDLLPath = "";
		//	global::System.Reflection.Assembly _A = null;
		//	Type _T;

		//	// Try and Load The Type
		//	try { _T = Type.GetType(TypeStringName); }
		//	catch
		//	{
		//		__.LogFatalError("Invalid Type String: " + TypeStringName, null);
		//		return false;
		//	}

		//	// Type Must NOT Be Null
		//	if (_T == null) { return false; }

		//	// If DLL is Not Full Path Then Loop and Find the DLL Full PAth
		//	if (DLL.FileExists()) { _FullDLLPath = DLL; }
		//	else
		//	{
		//		foreach (string Path in _PluginDirectories)
		//		{
		//			if ((Path + DLL).FileExists() == false) { }
		//			else
		//			{
		//				_FullDLLPath = Path + DLL;
		//				break;
		//			}
		//		}
		//	}

		//	//Try and Load the Assembly Found
		//	try { _A = global::System.Reflection.Assembly.LoadFile(_FullDLLPath); }
		//	catch (Exception ex) { _.LogFatalError("Error Loading Plugin Assembly: " + _FullDLLPath + " -- With Message " + ex.Message, ex); }

		//	// Ensure the Assembly was Loaded
		//	if (_A == null) { return false; }

		//	// Cache The Assembly IF IT Hasnt Been Cached
		//	if (Cached_Assemblies.Exists(x => (x.PluginName == _T.FullName) && x.Assembly_Types == _T && _FullDLLPath == x.Full_PluginDLL_Path) == false)
		//	{
		//		try { Cached_Assemblies.Add(new CachedAssembly() { PluginName = _T.FullName, Assembly_Types = _T, Loaded_Assembly = _A, Full_PluginDLL_Path = _A.Location }); }
		//		catch (Exception ex)
		//		{
		//			__.LogFatalError("Error Caching The Assembly: " + ex.Message, ex);
		//			return false;
		//		}
		//	}

		//	return true;
		//}
*/