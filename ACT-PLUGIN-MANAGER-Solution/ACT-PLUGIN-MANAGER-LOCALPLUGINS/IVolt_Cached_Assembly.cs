﻿
using ACT.PluginManager.Interfaces;
using System.Reflection;

namespace ACT_PLUGIN_MANAGER.LOCALPLUGINS
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	A local cached assembly plugin. </summary>
	///
	/// <remarks>	Mark Alicz, 1/22/2024. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public class Local_Cached_Assembly_Plugin : I_Cached_Assembly
	{
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the identifier of the assembly cache. </summary>
		///
		/// <value>	The identifier of the assembly cache. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public Guid? Assembly_CacheID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the full pathname of the full plugin DLL file. </summary>
		///
		/// <value>	The full pathname of the full plugin DLL file. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public string Full_PluginDLL_Path { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the type of the assembly. </summary>
		///
		/// <value>	The type of the assembly. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public List<Type> Assembly_Types { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the name of the plugin. </summary>
		///
		/// <value>	The name of the plugin. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public string PluginName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the loaded assembly. </summary>
		///
		/// <value>	The loaded assembly. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public Assembly Loaded_Assembly { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the Date/Time of the date loaded. </summary>
		///
		/// <value>	The date loaded. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public DateTime DateLoaded { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Adds an assembly types. </summary>
		///
		/// <remarks>	Mark Alicz, 1/22/2024. </remarks>
		///
		/// <param name="types">	The types. </param>
		///
		/// <returns>	An int. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public int Add_Assembly_Types(List<Type> types)
		{
			throw new NotImplementedException();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Creates new cached assembly from DLL. </summary>
		///
		/// <remarks>	Mark Alicz, 1/22/2024. </remarks>
		///
		/// <param name="PluginDLL_Path">  	Full pathname of the plugin DLL file. </param>
		/// <param name="Plugin_Name">	  	Name of the plugin. </param>
		/// <param name="AssemblyToAdd">	  	The assembly to add. </param>
		/// <param name="assembly_CacheID">	Identifier for the assembly cache. </param>
		///
		/// <returns>	The new new cached assembly from DLL. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public I_Cached_Assembly CreateNew_Cached_Assembly_FromDLL(string PluginDLL_Path, string Plugin_Name, Assembly AssemblyToAdd, Guid? assembly_CacheID)
		{
			throw new NotImplementedException();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the version. </summary>
		///
		/// <remarks>	Mark Alicz, 1/22/2024. </remarks>
		///
		/// <returns>	The version. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public string GetVersion()
		{
			throw new NotImplementedException();
		}
	}
}