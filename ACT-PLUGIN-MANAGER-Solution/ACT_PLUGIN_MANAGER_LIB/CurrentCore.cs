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
		#region Private Variables - Properties


		/// <summary>	The local cached assemblies. </summary>
		static List<I_Cached_Assembly> _local_cached_assemblies;
		/// <summary>	Cached assemblies for the application. </summary>
		static List<I_Cached_Assembly> _cached_assemblies;

		/// <summary>	True to loaded application plugin data. </summary>
		static bool _loaded_Application_Plugin_Data;
		/// <summary>	True to supported interfaces. </summary>
		static bool _loaded_Supported_Interfaces_Data;

		

		/// <summary>	Private List of Plugin Directories Defined. </summary>
		static List<string> _PluginDirectories = new List<string>();

		/// <summary>	The exceptions caught. </summary>
		static Dictionary<string, Exception> _ExceptionsCaught = new Dictionary<string, Exception>();

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets a value indicating whether the initilized. </summary>
		///
		/// <value>	True if initilized, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static bool INITILIZED { get; internal set; } = false;

		/// <summary>	True to ignore local plugins. </summary>
		internal static bool IgnoreLocalPlugins = true;
		#endregion

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Initalizes this object. </summary>
		///
		/// <remarks>	Mark Alicz, 1/24/2024. </remarks>
		///
		/// <exception cref="Exception">	Thrown when an exception error condition occurs. </exception>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void Initalize() 
		{
			#region Local Plugins

			// LOAD THE LOCAL SUPPORTED INTERFACES	  		
			try
			{
				string _SuportedInterfacesPath = (__.LocalPluginsDirectory.EnsureDirectoryFormat() + "Local_Suported_Interfaces.json");
				if (_SuportedInterfacesPath.FileExists()) { __._Local_Supported_Interfaces = Supported_Interfaces.FromJson(System.IO.File.ReadAllText(_SuportedInterfacesPath)); }

				string _PluginsPath = (__.LocalPluginsDirectory.EnsureDirectoryFormat() + "Local_Plugins.json");
				if (_PluginsPath.FileExists()) { __._Local_Plugins = Application_Plugins.FromJson(System.IO.File.ReadAllText(_PluginsPath)); }

				if (__._Local_Supported_Interfaces == null || __._Local_Supported_Interfaces.InterfaceSources.Count > 0)
				{
					if (__._Local_Plugins == null || __._Local_Plugins.InterfacePluginDlls.Count > 0)
					{
						foreach (var plug in __._Local_Plugins.InterfacePluginDlls)
						{
							if (__._Local_Supported_Interfaces.Has_Plugin_For_Interface(plug.Interface, true)) { plug.SpecificPath = __.PluginsDirectory.EnsureDirectoryFormat() + plug.DllName; IgnoreLocalPlugins = false; }
						}
					}
				}
			}
			catch { }

			#endregion

			#region Application Plugins

			// LOAD THE APPLICATION INTERFACES AND PLUGINS	  		
			try
			{
				string _SuportedInterfacesPath = (__.PluginsDirectory.EnsureDirectoryFormat() + "Suported_Interfaces.json");
				if (_SuportedInterfacesPath.FileExists()) { __._Local_Supported_Interfaces = Supported_Interfaces.FromJson(System.IO.File.ReadAllText(_SuportedInterfacesPath)); }
				else { throw new Exception("Missing Supported Interfaces File: " + _SuportedInterfacesPath); }

				string _PluginsPath = (__.PluginsDirectory.EnsureDirectoryFormat() + "Application_Plugins.json");
				if (_PluginsPath.FileExists()) { __._Local_Plugins = Application_Plugins.FromJson(System.IO.File.ReadAllText(_PluginsPath)); }
				else { throw new Exception("Missing Application Plugins File: " + _PluginsPath); }

				if (__._Supported_Interfaces == null || __._Supported_Interfaces.InterfaceSources.Count > 0)
				{
					if (__._Application_Plugins == null || __._Application_Plugins.InterfacePluginDlls.Count > 0)
					{
						foreach (var plug in __._Application_Plugins.InterfacePluginDlls)
						{
							if (__._Supported_Interfaces.Has_Plugin_For_Interface(plug.Interface, true)) { plug.SpecificPath = __.PluginsDirectory.EnsureDirectoryFormat() + plug.DllName; plug.ValidPlugin = true; }
						}
					}
				}
			}
			catch { }

			#endregion
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the cached assemblies. </summary>
		///
		/// <value>	The cached assemblies. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static List<I_Cached_Assembly> CachedAssemblies { get { return _cached_assemblies; } set { _cached_assemblies = value; } }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the local - internal - cached assemblies. </summary>
		///
		/// <value>	The cached assemblies. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		internal static List<I_Cached_Assembly> LocalCachedAssemblies { get { return _local_cached_assemblies; } set { _local_cached_assemblies = value; } }





		#region Public Exception Tracking Methods

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets a value indicating whether this object has caught exceptions. </summary>
		///
		/// <value>	True if this object has caught exceptions, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static bool HasCaughtExceptions { get { return _ExceptionsCaught.Any(); } }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Caught exceptions. </summary>
		///
		/// <remarks>	Mark Alicz, 1/23/2024. </remarks>
		///
		/// <param name="clearExceptions">	True to clear exceptions. </param>
		///
		/// <returns>	True if it succeeds, false if it fails. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static Dictionary<string, Exception> GetAllCaughtExceptions(bool clearExceptions)
		{

			var _TmpReturn = new Dictionary<string, Exception>(_ExceptionsCaught);

			if (clearExceptions) { _ExceptionsCaught.Clear(); }

			return _TmpReturn;
		}
		#endregion

		#region Loading Plugins - Local and Normal

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Loads all plugins. </summary>
		///
		/// <remarks>	Mark Alicz, 1/24/2024. </remarks>
		///
		/// <exception cref="DirectoryNotFoundException">	Thrown when the requested directory is not
		/// 																present. </exception>
		///
		/// <param name="PluginPathBaseDirectory">	Pathname of the plugin path base directory. </param>
		/// <param name="LocalPlugins">					True to local plugins. </param>
		///
		/// <returns>	all plugins. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private int LoadAllPlugins(string PluginPathBaseDirectory, bool LocalPlugins)
		{
			if (PluginPathBaseDirectory.DirectoryExists() == false) { throw new DirectoryNotFoundException("Unable To Locate:" + PluginPathBaseDirectory); }
		
			var _AllFiles = Directory.GetFiles(PluginPathBaseDirectory);
			var _AllPlugins = _AllFiles.Where(x => x.ToLower().EndsWith(".dll")).ToList();

			foreach (var _Plugin in  __._Application_Plugins.InterfacePluginDlls)//_AllPlugins.Where(x => File.Exists(x) == true).Select(x => x))
			{
				try
				{

					if (__._Supported_Interfaces.Has_Plugin_For_Interface(_Plugin.Interface, false))
					{
						Assembly_Loader _AH = new Assembly_Loader();
						var _CA = _AH.Load_From_FilePath(_Plugin.SpecificPath);
						if (_CA.Assembly_Types.Count() == 0) { continue; }
						if (LocalPlugins)
						{
							AddCachedAssembly(_CA, true);
						}
						else
						{
							AddCachedAssembly(_CA);
						}
					}
					

					
				}
				catch (Exception ex) { _ExceptionsCaught.Add(Guid.NewGuid().ToString() + " - Error Adding Plugins From " + PluginPath, ex); __.LogFatalError("CurrentCore<" + typeof(T).ToString() + ">()...LoadAllPlugins...	On::" + PluginPath, ex); }
			}
		}

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

		#endregion

		#region Local Plugins

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Loads local plugins. </summary>
		///
		/// <remarks>	Mark Alicz, 1/23/2024. </remarks>
		///
		/// <param name="PluginPath">	Full pathname of the plugin file. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		internal static void LoadLocalPlugins(string PluginPath)
		{
			//_local_cached_assemblies
		}

		
		#endregion Local Plugin Methods

		#region Public Methods

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Adds Cached Assembly.  Can be added post Load. calls <seealso cref="AddCachedAssembly(I_Cached_Assembly, bool)"/>
		/// </summary>
		///
		/// <remarks>	Mark Alicz, 1/19/2024. </remarks>
		///
		/// <param name="AssemblyTooAdd">	The assembly too add. </param>
		///
		/// <returns>	Null if AssemblyToAdd is Null, True if Added, False if Not Added. </returns>
		///
		/// ### <exception cref="ArgumentNullException">	Thrown when one or more required arguments are
		/// 																null. </exception>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static bool AddCachedAssembly(I_Cached_Assembly AssemblyTooAdd)
		{
			return AddCachedAssembly(AssemblyTooAdd, false);
		}

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
			if (CachedAssemblies.Any() == false) { return null; }

			var _AllFoundTypes = CachedAssemblies.Where(x => x.Assembly_Types.Contains(DllType)).ToList();


			if (CachedAssemblies.Select(x =>
									 x.Assembly_Types.Contains(DllType) &&
									 (x.Full_PluginDLL_Path == DllFullPath || DllFullPath.NullOrEmpty()))
								 .Count() > 1)
			{
				__.LogBasicInfo("Cached Assemblies Contain Multiple Type Defined Plugins: " + DllType.FullName);
			}

			// Returns the First One.  Logs When there are more than one tyoe.
			return CachedAssemblies.Where(x => x.Assembly_Types.Contains(DllType) && (x.Full_PluginDLL_Path == DllFullPath || DllFullPath.NullOrEmpty())).OrderByDescending(y => y.DateLoaded).First().Assembly_CacheID;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets local plugins. </summary>
		///
		/// <remarks>	Mark Alicz, 1/22/2024. </remarks>
		///
		/// <returns>	The local plugins. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		internal static T GetLocalPlugin()
		{
			if (_local_cached_assemblies == null || _local_cached_assemblies.Count() == 0)
			{
				LoadLocalPlugins(__.ResourceDirectory.EnsureDirectoryFormat() + "LocalPlugins\\");
			}

			if (_local_cached_assemblies.First(x => x.Assembly_Types.Contains(typeof(T))) == null)
			{
				if (typeof(T) == typeof(I_Cached_Assembly))
				{
					return (T)LocalCachedAssemblies.First(x => x.Assembly_Types.Contains(typeof(T)));
				}
				else if (typeof(T) == typeof(I_Cached_Assembly))
				{

				}
			}
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

					if (AddCachedAssembly(new Cached_Assembly(_PluginInfo.DllFullPath, _PluginInfo.DLLName, typeof(T).FullName))
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
		/// <summary>	Adds the cached assembly. </summary>
		///
		/// <remarks>	Mark Alicz, 1/19/2024. </remarks>
		///
		/// <exception cref="ArgumentNullException">	Thrown when one or more required arguments are null. </exception>
		///
		/// <param name="AssemblyTooAdd">	The assembly too add. </param>
		/// <param name="localPlugin">		True to local plugin. </param>
		///
		/// <returns>	Null if AssemblyToAdd is Null, True if Added, False if Not Added. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private static bool AddCachedAssembly(I_Cached_Assembly AssemblyTooAdd, bool localPlugin)
		{
			try
			{
				if (AssemblyTooAdd == null) { throw new ArgumentNullException("Parameter Must Not Be Null"); }
				if (localPlugin == false)
				{
					if (CachedAssemblies.Exists(x => x.PluginName == AssemblyTooAdd.PluginName && x.Assembly_Types.Count == x.Assembly_Types.Count)) { return true; }
				}
				else
				{
					if (LocalCachedAssemblies.Exists(x => x.PluginName == AssemblyTooAdd.PluginName && x.Assembly_Types.Count == x.Assembly_Types.Count)) { return true; }
				}
			}
			catch (Exception ex)
			{
				_ExceptionsCaught.Add("Error in private AddCachedAsembly(I_Cached_Assembly, bool)", ex);
				return false;
			}

			return true;
		}

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