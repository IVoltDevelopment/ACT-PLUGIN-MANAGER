


using ACT.Core.PluginManager.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ACT.Core.PluginManager.JSON_Objects;

namespace ACT.Core.PluginManager
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	A . </summary>
	///
	/// <remarks>	Mark Alicz, 1/17/2024. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public static class __
	{

		/// <summary>	The windows new line. </summary>
		private static string _WindowsNewLine = "\r\n";

		/// <summary>	The linux new line. </summary>
		private static string _LinuxNewLine = "\n";
		/// <summary>	Pathname of the base directory. </summary>
		private static string _BaseDirectory = System.Reflection.Assembly.GetCallingAssembly().FullName;


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
				else{ return _LinuxNewLine; }
			}
		}

		#region Application Level Plugins and Interfaces

		/// <summary>	The application plugins. </summary>
		internal static Application_Plugins _Application_Plugins = null;
		/// <summary>	The supported interfaces. </summary>
		internal static Supported_Interfaces _Supported_Interfaces = null;

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
		internal static bool HasLoadedInterfaces { get { if (_Supported_Interfaces == null || _Supported_Interfaces.InterfaceSources == null || _Supported_Interfaces.InterfaceSources.Count < 1) { return false; } return true; } }

		#endregion

		#region Local Level Plugins and Interfaces
		/// <summary>	The application plugins. </summary>
		internal static Application_Plugins _Local_Plugins = null;
		/// <summary>	The supported interfaces. </summary>
		internal static Supported_Interfaces _Local_Supported_Interfaces = null;

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets a value indicating whether this object has valid local plugins. </summary>
		///
		/// <value>	True if this object has valid local plugins, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal static bool HasValid_LocalPlugins { get { if (_Local_Plugins.InterfacePluginDlls.Any(x => x.ValidPlugin == true)) { return true; } return false; } }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Gets a value indicating whether this object has loaded local interfaces.
		/// </summary>
		///
		/// <value>	True if this object has loaded local interfaces, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal static bool HasLoaded_LocalInterfaces { get { if (_Local_Supported_Interfaces == null || _Local_Supported_Interfaces.InterfaceSources == null || _Local_Supported_Interfaces.InterfaceSources.Count < 1) { return false; } return true; } }

		#endregion
		 
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
	}
}
