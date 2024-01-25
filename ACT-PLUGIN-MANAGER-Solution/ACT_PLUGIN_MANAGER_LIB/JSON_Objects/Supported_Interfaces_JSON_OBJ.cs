


using System;
using System.Collections.Generic;

using System.Globalization;
using System.Text;
using ACT.Core.PluginManager.Extensions;
using ACT.Core.PluginManager.LocalPlugins;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace ACT.Core.PluginManager.JSON_Objects
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	A supported interfaces. </summary>
	///
	/// <remarks>	Mark Alicz, 1/19/2024. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public class Supported_Interfaces
	{

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Query if 'InterfaceName' has local plugin for interface. </summary>
		///
		/// <remarks>	Mark Alicz, 1/24/2024. </remarks>
		///
		/// <param name="InterfaceName">	Name of the interface. </param>
		///
		/// <returns>	True if local plugin for interface, false if not. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public bool Has_Plugin_For_Interface(string InterfaceName, bool LocalPlugin)
		{
			if (LocalPlugin)
			{
				if (__._Local_Supported_Interfaces == null) { return false; }
				if (__._Local_Plugins == null) { return false; }

				try
				{
					List<string> _AllInterfacesDefined = __._Local_Supported_Interfaces.InterfaceSources.SelectMany(x => x.Interfaces).ToList();
					if (_AllInterfacesDefined.Any(x => x == InterfaceName))
					{
						if (__._Local_Plugins.InterfacePluginDlls.Any(x => x.Interface == InterfaceName)) { return true; }
					}
				}
				catch
				{
					return false;
				}
				return false;
			}
			else
			{
				if (__._Supported_Interfaces == null) { return false; }
				if (__._Application_Plugins == null) { return false; }

				try
				{
					List<string> _AllInterfacesDefined = __._Supported_Interfaces.InterfaceSources.SelectMany(x => x.Interfaces).ToList();
					if (_AllInterfacesDefined.Any(x => x == InterfaceName))
					{
						if (__._Application_Plugins.InterfacePluginDlls.Any(x => x.Interface == InterfaceName)) { return true; }
					}
				}
				catch
				{
					return false;
				}
				return false;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the about. </summary>
		///
		/// <value>	The about. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		[JsonProperty("about", NullValueHandling = NullValueHandling.Ignore)]
		public string About { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the name of the application. </summary>
		///
		/// <value>	The name of the application. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		[JsonProperty("app_name", NullValueHandling = NullValueHandling.Ignore)]
		public string AppName { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the identifier of the registered application. </summary>
		///
		/// <value>	The identifier of the registered application. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		[JsonProperty("registered_app_id", NullValueHandling = NullValueHandling.Ignore)]
		public string RegisteredAppId { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the interface sources. </summary>
		///
		/// <value>	The interface sources. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		[JsonProperty("interface_sources", NullValueHandling = NullValueHandling.Ignore)]
		public List<InterfaceSource> InterfaceSources { get; set; }

		#region Non Serialized Properties

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Registration Decoded. </summary>
		///
		/// <value>	The identifier of the registered application. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		[JsonIgnore()]
		internal string RegisteredAppId_Decoded
		{
			get
			{
				if (RegisteredAppId.NullOrEmpty()) { return ""; }
				else { try { return Encoding.UTF8.GetString(Convert.FromBase64String(RegisteredAppId)); } catch { return RegisteredAppId; } }
			}
		}
		#endregion

		#region Methods

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Creates a new object from the given JSON. </summary>
		///
		/// <remarks>	Mark Alicz, 1/19/2024. </remarks>
		///
		/// <param name="json">	The JSON. </param>
		///
		/// <returns>	The Supported_Interfaces. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static Supported_Interfaces FromJson(string json) => JsonConvert.DeserializeObject<Supported_Interfaces>(json, JSON_Converter.Settings);

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Converts this object to a JSON. </summary>
		///
		/// <remarks>	Mark Alicz, 1/19/2024. </remarks>
		///
		/// <returns>	This object as a string. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public string ToJson() => JsonConvert.SerializeObject(this, JSON_Converter.Settings);

		#endregion
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	An interface source. </summary>
	///
	/// <remarks>	Mark Alicz, 1/19/2024. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public class InterfaceSource
	{
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the name of the source. </summary>
		///
		/// <value>	The name of the source. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		[JsonProperty("source_dll_name", NullValueHandling = NullValueHandling.Ignore)]
		public string SourceName { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the interfaces. </summary>
		///
		/// <value>	The interfaces. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		[JsonProperty("interfaces", NullValueHandling = NullValueHandling.Ignore)]
		public List<string> Interfaces { get; set; }
	}

}
