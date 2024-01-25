using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.PluginManager.LocalPlugins
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	A JSON converter. </summary>
	///
	/// <remarks>	Mark Alicz, 1/19/2024. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static class JSON_Converter
	{
		/// <summary>	(Immutable) options for controlling the operation. </summary>
		public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
			DateParseHandling = DateParseHandling.None,
			Converters =
				{
					 new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
				},
		};
	}
}
