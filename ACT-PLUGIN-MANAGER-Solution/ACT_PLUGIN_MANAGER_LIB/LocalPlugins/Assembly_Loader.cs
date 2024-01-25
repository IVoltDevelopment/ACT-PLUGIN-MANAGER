

using ACT.PluginManager.Interfaces;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.PluginManager.LocalPlugins
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	An assembly helper. </summary>
	///
	/// <remarks>	Mark Alicz, 1/17/2024. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	internal class Assembly_Loader : I_Assembly_Loader
	{
		/// <summary>	Information describing the plugin. </summary>
		private string _PluginInfo = "{\r\n\"author\":\"Mark Alicz - IVolt LLC\",\r\n\"company\":\"IVolt LLC\",\r\n\"year\":\"2023\",\r\n\"version\":\"2.056\",\r\n\"additionalinfo\":\"KDJFI33$fF3D002D21\"\r\n}";

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets information describing the plugin. </summary>
		///
		/// <value>	Information describing the plugin. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public string PluginInfo { get { return _PluginInfo; } }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Loads from file path. </summary>
		///
		/// <remarks>	Mark Alicz, 1/17/2024. </remarks>
		///
		/// <param name="dllFilePath">	Full pathname of the DLL file. </param>
		///
		/// <returns>	The data that was read from the file path. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public I_Cached_Assembly Load_From_FilePath(string dllFilePath)
		{
			Assembly _WorkingAssembly = Assembly.Load(dllFilePath);
			List<Type> _Interfaces = _WorkingAssembly.GetExportedTypes().Where(x => x.IsInterface && x.IsPublic == true).ToList();

			FileInfo _FI = new FileInfo(dllFilePath);
			var ca = new Cached_Assembly();
			ca.Init_Plugin(dllFilePath, _FI.Name, _WorkingAssembly, null);
			I_Cached_Assembly _CA = ca;

			//	I_Cached_Assembly _CA = CurrentCore<I_Cached_Assembly>.GetLocalPlugin();
			//			_CA.new Cached_Assembly(dllFilePath, _FI.Name, _WorkingAssembly, null);
			var _x = _CA.Add_Assembly_Types(_Interfaces);
			if (_x == 0) { __.LogBasicInfo("I_Assembly_Loader: " + dllFilePath + " Loaded 0 Interfaces "); }

			return _CA;
		}
	}
	
	
}
