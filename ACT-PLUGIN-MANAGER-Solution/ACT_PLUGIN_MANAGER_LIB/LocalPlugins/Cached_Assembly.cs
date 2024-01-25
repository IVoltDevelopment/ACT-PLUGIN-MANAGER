


using ACT.Core.PluginManager.Extensions;
using System.Reflection;
using ACT.PluginManager.Interfaces;

namespace ACT.Core.PluginManager.LocalPlugins
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Holds the Plugin Assembly and a Single Type After Loading. </summary>
	///
	/// <remarks>	Mark Alicz, 1/11/2024. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public class Cached_Assembly : I_Cached_Assembly
	{

		/// <summary>	Information describing the plugin. </summary>
		private string _PluginInfo = "{\r\n\"author\":\"Mark Alicz - IVolt LLC\",\r\n\"company\":\"IVolt LLC\",\r\n\"year\":\"2023\",\r\n\"version\":\"2.056\",\r\n\"additionalinfo\":\"KDJFI33$fF3D002D21\"\r\n}";

		/// <summary>	Identifier for the assembly cache. </summary>
		private Guid? _Assembly_CacheID = Guid.Empty;

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets a value indicating whether this object is loaded. </summary>
		///
		/// <value>	True if this object is loaded, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public bool IsLoaded { get; private set; } = false;

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Default constructor. </summary>
		///
		/// <remarks>	Mark Alicz, 1/22/2024. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public Cached_Assembly() { }



		#region Methods Start
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Initializes the plugin. </summary>
		///
		/// <remarks>	Mark Alicz, 1/22/2024. </remarks>
		///
		/// <param name="PluginDLL_Path">  	Full pathname of the plugin DLL file. </param>
		/// <param name="Plugin_Name">	  	Name of the plugin. </param>
		/// <param name="AssemblyToAdd">	  	The assembly to add. </param>
		/// <param name="assembly_CacheID">	The identifier of the assembly cache. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public void Init_Plugin(string PluginDLL_Path, string Plugin_Name, Assembly AssemblyToAdd, Guid? assembly_CacheID)
		{
			Full_PluginDLL_Path = PluginDLL_Path;
			PluginName = Plugin_Name;
			Loaded_Assembly = AssemblyToAdd;
			Assembly_CacheID = assembly_CacheID;
			IsLoaded = true;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the version. </summary>
		///
		/// <remarks>	Mark Alicz, 1/16/2024. </remarks>
		///
		/// <returns>	The version. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public string GetVersion()
		{
			try
			{
				if (Loaded_Assembly == null) { return null; }

				AssemblyFileVersionAttribute fileVersionAttribute = Loaded_Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
				string fileVersion = fileVersionAttribute?.Version;

				// Get the AssemblyVersionAttribute							 z c
				AssemblyVersionAttribute assemblyVersionAttribute = Loaded_Assembly.GetCustomAttribute<AssemblyVersionAttribute>();
				string? assemblyVersion = assemblyVersionAttribute?.Version;

				string _ReturnVersion = fileVersion.Combine(assemblyVersion, true, true);
				return _ReturnVersion;
			}
			catch { return null; } // TODO LOG
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Create a Cached Assembly using an overrideable Plugin Interface Method. </summary>
		///
		/// <remarks>	Mark Alicz, 1/17/2024. </remarks>
		///
		/// <param name="PluginDLL_Path">  	Full pathname of the plugin DLL file. </param>
		/// <param name="Plugin_Name">	  	Name of the plugin. </param>
		/// <param name="AssemblyToAdd">	  	The assembly to add. </param>
		/// <param name="assembly_CacheID">	The identifier of the assembly cache. </param>
		///
		/// <returns>	A Cached_Assembly. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public I_Cached_Assembly CreateNew_Cached_Assembly_FromDLL(string PluginDLL_Path, string Plugin_Name, Assembly AssemblyToAdd, Guid? assembly_CacheID)
		{
			var _X = new Cached_Assembly();
			_X.Init_Plugin(PluginDLL_Path, Plugin_Name, AssemblyToAdd, assembly_CacheID);
			if (_X.IsLoaded)
			{
				return _X;
			}
			return null;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Adds an assembly types. </summary>
		///
		/// <remarks>	Mark Alicz, 1/17/2024. </remarks>
		///
		/// <param name="types">	The types. </param>
		///
		/// <returns>	Count of Added Interface Types. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public int Add_Assembly_Types(List<Type> types)
		{
			int _StartCount = 0;
			int _TmpReturn = 0;

			if (Assembly_Types == null) { Assembly_Types = new List<Type>(); }

			if (Assembly_Types != null && Assembly_Types.Count == 0)
			{
				Assembly_Types.AddRange(types);
				_TmpReturn = Assembly_Types.Count;
			}
			else
			{
				_StartCount = Assembly_Types.Count;
				foreach (Type type in types)
				{
					if (Assembly_Types.Contains(type) == false) { Assembly_Types.Add(type); }
					_TmpReturn = Assembly_Types.Count - _StartCount;
				}
			}

			return _TmpReturn;
		}

		public string PluginInfo { get { return _PluginInfo; }; }
		#endregion

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the identifier of the assembly cache. </summary>
		///
		/// <value>	The identifier of the assembly cache. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public Guid? Assembly_CacheID { get { return _Assembly_CacheID; } set { _Assembly_CacheID = value; } }

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

		public Assembly Loaded_Assembly { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the Date/Time of the date loaded. </summary>
		///
		/// <value>	The date loaded. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public DateTime DateLoaded { get; set; } = DateTime.Now;
	}
}