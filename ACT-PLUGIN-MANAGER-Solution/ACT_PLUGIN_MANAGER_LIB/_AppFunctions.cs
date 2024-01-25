using ACT.Core.PluginManager.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ACT.Core.PluginManager.JSON_Objects;
using ACT.PluginManager.Interfaces;
using ACT.Core.PluginManager.LocalPlugins;

namespace ACT.Core.PluginManager
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Primary Data Holding Core Class </summary>
	///
	/// <remarks>	Mark Alicz, 1/17/2024. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static class __
	{
		#region Local Variables
		/// <summary>	The cached classes Dictionary. </summary>
		internal static Dictionary<Type, object> _CachedClasses = new Dictionary<Type, object>();
		/// <summary>	The local cached assemblies. </summary>
		internal static List<I_Cached_Assembly> _local_cached_assemblies;
		/// <summary>	Cached assemblies for the application. </summary>
		internal static List<I_Cached_Assembly> _cached_assemblies;
		/// <summary>	The application plugins. </summary>
		internal static Application_Plugins _Local_Plugins = null;
		/// <summary>	The supported interfaces. </summary>
		internal static Supported_Interfaces _Local_Supported_Interfaces = null;
		/// <summary>	True to ignore local plugins. </summary>
		internal static bool IgnoreLocalPlugins = true;
		/// <summary>	The application plugins. </summary>
		internal static Application_Plugins _Application_Plugins = null;
		/// <summary>	The supported interfaces. </summary>
		internal static Supported_Interfaces _Supported_Interfaces = null;
		#endregion

		#region Initialize The Class Data

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets a value indicating whether the initilized. </summary>
		///
		/// <value>	True if initilized, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static bool System_Initialized { get; internal set; } = false;

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Static constructor. </summary>
		///
		/// <remarks>	Mark Alicz, 1/24/2024. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		static __()
		{
			Initialize();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Initializes this object. </summary>
		///
		/// <remarks>	Mark Alicz, 1/24/2024. </remarks>
		///
		/// <exception cref="Exception">	Thrown when an exception error condition occurs. </exception>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal static void Initialize()
		{
			#region Load JSON Data For Plugins And Interfaces

			#region Local Plugins and Interfaces

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

			#region Application Plugins and Interfaces

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

			#endregion

			#region Load Plugin and Assembly Data

			LoadAllPlugins(LocalPluginsDirectory, true);

			LoadAllPlugins(LocalPluginsDirectory, false);

			#endregion

			System_Initialized = true;
		}

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
		internal static int LoadAllPlugins(string PluginPathBaseDirectory, bool LocalPlugins)
		{
			int _TmpReturn = 0;
			if (LocalPlugins)
			{
				if (__.HasLoaded_LocalInterfaces == false) { return 0; }
				if (__.HasLoaded_LocalPlugins == false) { return 0; }

				if (LocalPluginsDirectory.DirectoryExists() == false) { throw new DirectoryNotFoundException("Unable To Locate:" + LocalPluginsDirectory); }

				var _AllFiles = Directory.GetFiles(LocalPluginsDirectory);
				var _AllPlugins = _AllFiles.Where(x => x.ToLower().EndsWith(".dll")).ToList();

				foreach (var _Plugin in __._Local_Plugins.InterfacePluginDlls)
				{
					try
					{
						if (__._Local_Supported_Interfaces.Has_Plugin_For_Interface(_Plugin.Interface, false))
						{
							Assembly_Loader _AH = new Assembly_Loader();
							var _CA = _AH.Load_From_FilePath(_Plugin.SpecificPath);
							if (_CA.Assembly_Types.Count() == 0) { continue; }
							AddCachedAssembly(_CA, true);
							_TmpReturn++;
						}
					}
					catch (Exception ex)
					{
						__._ExceptionsCaught.Add(Guid.NewGuid().ToString() + " - Error Adding Local Plugins From LoadAllPlugins(string, bool) ", ex);
						//__.LogFatalError("CurrentCore<" + typeof(T).ToString() + ">()...LoadAllPlugins...	On::" + _Plugin.SpecificPath, ex); 
					}
				}
			}
			else
			{
				if (__.HasLoadedInterfaces == false) { return 0; }
				if (__.HasLoadedPlugins == false) { return 0; }

				if (PluginPathBaseDirectory.DirectoryExists() == false) { throw new DirectoryNotFoundException("Unable To Locate:" + PluginPathBaseDirectory); }

				var _AllFiles = Directory.GetFiles(PluginPathBaseDirectory);
				var _AllPlugins = _AllFiles.Where(x => x.ToLower().EndsWith(".dll")).ToList();

				foreach (var _Plugin in __._Application_Plugins.InterfacePluginDlls)
				{
					try
					{
						if (__._Supported_Interfaces.Has_Plugin_For_Interface(_Plugin.Interface, false))
						{
							Assembly_Loader _AH = new Assembly_Loader();
							var _CA = _AH.Load_From_FilePath(_Plugin.SpecificPath);
							if (_CA.Assembly_Types.Count() == 0) { continue; }
							AddCachedAssembly(_CA, false);
							_TmpReturn++;
						}
					}
					catch (Exception ex)
					{
						__._ExceptionsCaught.Add(Guid.NewGuid().ToString() + " - Error Adding Application Plugins From LoadAllPlugins(string, bool) ", ex);
					}
				}
			}
			return _TmpReturn;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Loads local plugins. </summary>
		///
		/// <remarks>	Mark Alicz, 1/23/2024. </remarks>
		///
		/// <param name="PluginPath">	Full pathname of the plugin file. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal static void LoadLocalPlugins(string PluginPath)
		{
			//__._local_cached_assemblies
		}

		#endregion

		#region NewLine Character Logic

		/// <summary>	The windows new line. </summary>
		private static string _WindowsNewLine = "\r\n";

		/// <summary>	The linux new line. </summary>
		private static string _LinuxNewLine = "\n";

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the newline. </summary>
		///
		/// <value>	The newline. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static string NL
		{
			get
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) { return _WindowsNewLine; }
				else { return _LinuxNewLine; }
			}
		}

		#endregion

		#region CACHED DATA

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
				__._ExceptionsCaught.Add("Error in private AddCachedAsembly(I_Cached_Assembly, bool)", ex);
				return false;
			}

			return true;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the cached assemblies. </summary>
		///
		/// <value>	The cached assemblies. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static List<I_Cached_Assembly> CachedAssemblies { get { return __._cached_assemblies; } set { __._cached_assemblies = value; } }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the local - internal - cached assemblies. </summary>
		///
		/// <value>	The cached assemblies. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static List<I_Cached_Assembly> LocalCachedAssemblies { get { return __._local_cached_assemblies; } set { __._local_cached_assemblies = value; } }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets all plugins. </summary>
		///
		/// <remarks>	Mark Alicz, 1/25/2024. </remarks>
		///
		/// <typeparam name="T">	Generic type parameter. </typeparam>
		///
		/// <returns>	all plugins. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static Dictionary<I_Cached_Assembly, List<Type>> GetAllPlugins<T>()
		{
			Dictionary<I_Cached_Assembly, List<Type>> _tmpReturn = new Dictionary<I_Cached_Assembly, List<Type>>();
			bool _Found = false;
			foreach (var ca in CachedAssemblies)
			{
				List<Type> _TypesFound = ca.Assembly_Types.Where(x => x == typeof(T)).ToList();

				if (_tmpReturn.Keys.Any(x => x.Full_PluginDLL_Path == ca.Full_PluginDLL_Path) == false)
				{
					_tmpReturn.Add(ca, new List<Type>());
				}

				I_Cached_Assembly _cat = _tmpReturn.Keys.First(x => x.Full_PluginDLL_Path == ca.Full_PluginDLL_Path);

				foreach (Type t in _TypesFound)
				{
					if (_cat.Assembly_Types.Contains(t) == false) { _tmpReturn[_cat].Add(t); _Found = true; }
				}
			}

			if (_Found) { return _tmpReturn; }
			else { return null; }
		}
		#endregion

		#region Application Level Plugins and Interfaces

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets a value indicating whether this object has valid plugins. </summary>
		///
		/// <value>	True if this object has valid plugins, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal static bool HasValidPlugins { get { if (_Application_Plugins.InterfacePluginDlls.Any(x => x.ValidPlugin == true)) { return true; } return false; } }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets a value indicating whether this object has loaded interfaces. </summary>
		///
		/// <value>	True if this object has loaded interfaces, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal static bool HasLoadedPlugins { get { if (_Application_Plugins == null || _Application_Plugins.InterfacePluginDlls == null || _Application_Plugins.InterfacePluginDlls.Count < 1) { return false; } return true; } }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets a value indicating whether this object has loaded interfaces. </summary>
		///
		/// <value>	True if this object has loaded interfaces, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal static bool HasLoadedInterfaces { get { if (_Supported_Interfaces == null || _Supported_Interfaces.InterfaceSources == null || _Supported_Interfaces.InterfaceSources.Count < 1) { return false; } return true; } }

		#endregion

		#region Local Level Plugins and Interfaces

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets a value indicating whether this object has valid local plugins. </summary>
		///
		/// <value>	True if this object has valid local plugins, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal static bool HasValid_LocalPlugins { get { if (_Local_Plugins.InterfacePluginDlls.Any(x => x.ValidPlugin == true)) { return true; } return false; } }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets a value indicating whether this object has loaded interfaces. </summary>
		///
		/// <value>	True if this object has loaded interfaces, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal static bool HasLoaded_LocalPlugins { get { if (_Local_Plugins == null || _Local_Plugins.InterfacePluginDlls == null || _Local_Plugins.InterfacePluginDlls.Count < 1) { return false; } return true; } }


		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Gets a value indicating whether this object has loaded local interfaces.
		/// </summary>
		///
		/// <value>	True if this object has loaded local interfaces, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal static bool HasLoaded_LocalInterfaces { get { if (_Local_Supported_Interfaces == null || _Local_Supported_Interfaces.InterfaceSources == null || _Local_Supported_Interfaces.InterfaceSources.Count < 1) { return false; } return true; } }

		#endregion

		#region Directory Definitions

		/// <summary>	Pathname of the base directory. </summary>
		private static string _BaseDirectory = System.Reflection.Assembly.GetCallingAssembly().FullName;

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the pathname of the base directory. </summary>
		///
		/// <value>	The pathname of the base directory. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static string BaseDirectory { get { return _BaseDirectory.Substring(0, BaseDirectory.LastIndexOf("\\")); } }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the pathname of the resource directory. </summary>
		///
		/// <value>	The pathname of the resource directory. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static string ResourceDirectory { get { return BaseDirectory.EnsureDirectoryFormat() + "Resources\\"; } }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the pathname of the plugins directory. </summary>
		///
		/// <value>	The pathname of the plugins directory. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static string PluginsDirectory { get { return ResourceDirectory.EnsureDirectoryFormat() + "Plugins\\"; } }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the pathname of the plugins directory. </summary>
		///
		/// <value>	The pathname of the plugins directory. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static string LocalPluginsDirectory { get { return ResourceDirectory.EnsureDirectoryFormat() + "LocalPlugins\\"; } }

		#endregion

		#region Error Logging

		/// <summary>	The exceptions caught. </summary>
		internal static Dictionary<string, Exception> _ExceptionsCaught = new Dictionary<string, Exception>();

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets a value indicating whether this object has caught exceptions. </summary>
		///
		/// <value>	True if this object has caught exceptions, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static bool HasCaughtExceptions { get { return __._ExceptionsCaught.Any(); } }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		///  <summary>	Caught exceptions. </summary>
		///
		/// <remarks>	Mark Alicz, 1/23/2024. </remarks>
		///
		/// <param name="clearExceptions">	True to clear exceptions. </param>
		///
		/// <returns>	True if it succeeds, false if it fails. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static Dictionary<string, Exception> GetAllCaughtExceptions(bool clearExceptions)
		{

			var _TmpReturn = new Dictionary<string, Exception>(__._ExceptionsCaught);

			if (clearExceptions) { __._ExceptionsCaught.Clear(); }

			return _TmpReturn;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Logs fatal error. </summary>
		///
		/// <remarks>	Mark Alicz, 1/17/2024. </remarks>
		///
		/// <param name="messageToLog">	The message to log. </param>
		/// <param name="ex">			 	The exception. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void LogFatalError(string messageToLog, Exception ex) { }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Logs basic information. </summary>
		///
		/// <remarks>	Mark Alicz, 1/17/2024. </remarks>
		///
		/// <param name="messageToLog">	The message to log. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void LogBasicInfo(string messageToLog) { }

		#endregion
	}
}
