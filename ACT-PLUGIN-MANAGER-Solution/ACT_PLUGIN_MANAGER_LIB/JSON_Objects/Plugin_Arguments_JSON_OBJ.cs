/*
 
using ACT.Core.PluginManager.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ACT.Core.PluginManager.JSON_Objects
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Arguments for plugin. </summary>
	///
	/// <remarks>	Mark Alicz, 1/19/2024. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public class Plugin_Arguments
	{
		/// <summary> If This Class Is Ready and Properly Loaded</summary>
		public bool Loaded = false;

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Full DLL Name (i.e) MyDLL.dll. </summary>
		///
		/// <value>	The name of the DLL. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public string DLLName { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Full DLL Path (i.e) c:\ACT\Plugins. </summary>
		///
		/// <value>	The full path to the DLL. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public string DllFullPath { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Full Class Name (i.e) MyNameSpace.MySub.MyClass. </summary>
		///
		/// <value>	The full name of the class. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public string FullClassName { get; set; }

		/// <summary>	Defines if the class should be treated like a singleton or not. </summary>
		public bool StoreOnce;

		/// <summary>	Optional Arguments the are required to create an instance of the class. </summary>
		public List<object> Arguments = new List<object>();

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Empty Constructor for Generic Use. </summary>
		///
		/// <remarks>	Mark Alicz, 1/19/2024. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public Plugin_Arguments()
		{
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Loads the Plugin Arguments From the SystemConfiguration File Settings. </summary>
		///
		/// <remarks>	Mark Alicz, 1/19/2024. </remarks>
		///
		/// <exception cref="Exception">	Error Locating System Setting: " + Interface. </exception>
		///
		/// <param name="Interface">	The interface. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public Plugin_Arguments(string Interface)
		{			

			if (FullClassName.NullOrEmpty())
			{
				//ACT.Core.Helper.ErrorLogger.LogError(this, "Error Locating System Setting " + Interface, null, Enums.ErrorCodes.ErrorLevel.Critical);
				throw new Exception("Error Locating System Setting: " + Interface);
			}

			if (!String.IsNullOrEmpty(_StoreClass))
			{
				if (_StoreClass.ToLower() == "true")
				{
					StoreOnce = true;
				}
			}

			string _Args = SystemSettings.GetSettingByName(Interface + ".Args", SystemSettingsSections.Interface);

			if (!String.IsNullOrEmpty(_Args))
			{
				string[] _Data = _Args.SplitString(_Delimeter, StringSplitOptions.RemoveEmptyEntries);

				foreach (string _x in _Data)
				{
					Arguments.Add(_x);
				}
			}

		}		
	}
}


*/