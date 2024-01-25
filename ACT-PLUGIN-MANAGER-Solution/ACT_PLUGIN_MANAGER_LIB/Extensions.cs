
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ACT.Core.PluginManager.Extensions
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	A string extensions. </summary>
	///
	/// <remarks>	Mark Alicz, 1/16/2024. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public static class StringExtensions
	{
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	A string extension method that combines. </summary>
		///
		/// <remarks>	Mark Alicz, 1/16/2024. </remarks>
		///
		/// <exception cref="ArgumentNullException">	Thrown when one or more required arguments are null. </exception>
		///
		/// <param name="source">						 	The source to act on. </param>
		/// <param name="secondString">				 	The second string. </param>
		/// <param name="ignoreCase">					 	True to ignore case. </param>
		/// <param name="mergeMultipleWhiteSpaces">	True to merge multiple white spaces. </param>
		///
		/// <returns>	A string. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static string Combine(this string source, string secondString, bool ignoreCase, bool mergeMultipleWhiteSpaces)
		{
			// Edge cases handling
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (secondString == null) throw new ArgumentNullException(nameof(secondString));

			StringBuilder combined = new StringBuilder();
			int maxLength = Math.Max(source.Length, secondString.Length);

			for (int i = 0; i < maxLength; i++)
			{
				if (i < source.Length && (i >= secondString.Length ||
					 (ignoreCase ? char.ToUpperInvariant(source[i]) != char.ToUpperInvariant(secondString[i]) : source[i] != secondString[i])))
				{
					combined.Append(source[i]);
				}

				if (i < secondString.Length && (i >= source.Length ||
					 (ignoreCase ? char.ToUpperInvariant(source[i]) != char.ToUpperInvariant(secondString[i]) : source[i] != secondString[i])))
				{
					combined.Append(secondString[i]);
				}
			}

			string result = combined.ToString();

			// Merge multiple whitespaces if required
			if (mergeMultipleWhiteSpaces)
			{
				result = Regex.Replace(result, @"\s+", " ");
			}

			return result;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	A string extension method that ensures that directory format. </summary>
		///
		/// <remarks>	Mark Alicz, 1/17/2024. </remarks>
		///
		/// <param name="directoryPath">	The directoryPath to act on. </param>
		/// <param name="windowsBased"> 	(Optional) True if windows based. </param>
		///
		/// <returns>	A string. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static string EnsureDirectoryFormat(this string directoryPath, bool windowsBased = true)
		{
			string _testString = "\\";
			if (!windowsBased) { _testString = "/"; }

			if (directoryPath.EndsWith(_testString, StringComparison.OrdinalIgnoreCase)==false) { return directoryPath + _testString;  }

			return directoryPath;

		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	A bool extension method that queries if a given file exists. </summary>
		///
		/// <remarks>	Mark Alicz, 1/17/2024. </remarks>
		///
		/// <param name="filePath">	The directoryPath to act on. </param>
		///
		/// <returns>	true if File Exists, False if File Doesnt Exist. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static bool FileExists(this string filePath)
		{
			if (File.Exists(filePath)) return true;
			return false;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	A string extension method that queries if a given directory exists. </summary>
		///
		/// <remarks>	Mark Alicz, 1/17/2024. </remarks>
		///
		/// <param name="directoryPath">	The directoryPath to act on. </param>
		///
		/// <returns>	True if it succeeds, false if it fails. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static bool DirectoryExists(this string directoryPath)
		{
			if (Directory.Exists(directoryPath)) return true;
			return false;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	A string extension method that null or empty. </summary>
		///
		/// <remarks>	Mark Alicz, 1/17/2024. </remarks>
		///
		/// <param name="value">	The value to act on. </param>
		///
		/// <returns>	True if it succeeds, false if it fails. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static bool NullOrEmpty(this string value)
		{
			if (value == null || value == "") { return true; }
			return false;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	An extensions. </summary>
	///
	/// <remarks>	Mark Alicz, 1/17/2024. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	internal static class Extensions
	{
	}
}
