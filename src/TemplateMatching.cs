using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MadsKristensen.AddAnyFile
{
	public static class TemplateMatching
    {

		public static string GetMatchingTemplateFromFileName(List<string> templateFilePaths, string file, string defaultExt)
		{
			return GetMatchingTemplateFromFileName(templateFilePaths, file, defaultExt, File.Exists);
		}

		public static string GetMatchingTemplateFromFileName(List<string> templateFilePaths, string file, string defaultExt, Predicate<string> fileExistsPredicate)
		{
			var extension = Path.GetExtension(file).ToLowerInvariant();
			var name = Path.GetFileName(file);
			var safeName = name.StartsWith(".") ? name : Path.GetFileNameWithoutExtension(file);

			string templateFile = null;

			// Look for direct file name matches
			templateFile = templateFilePaths.FirstOrDefault(path => Path.GetFileName(path).Equals(name + defaultExt, StringComparison.OrdinalIgnoreCase));
			if (templateFile != null)
			{
				return Path.Combine(Path.GetDirectoryName(templateFile), name + defaultExt);
			}

			// Look for convention matches (matching with the end of file name)
			templateFile = templateFilePaths.FirstOrDefault(path => (safeName + defaultExt).EndsWith(Path.GetFileName(path), StringComparison.OrdinalIgnoreCase));
			if(templateFile != null)
			{
				return templateFile;
			}

			// Look for file extension matches
			templateFile = templateFilePaths.FirstOrDefault(path => Path.GetFileName(path).Equals(extension + defaultExt, StringComparison.OrdinalIgnoreCase) && fileExistsPredicate(path));
			if (templateFile != null)
			{
				var tmpl = AdjustForSpecific(safeName, extension);
				return Path.Combine(Path.GetDirectoryName(templateFile), tmpl + defaultExt);
			}

			return null;
		}

        private static string AdjustForSpecific(string safeName, string extension)
        {
            if (Regex.IsMatch(safeName, "^I[A-Z].*"))
            {
                return extension += "-interface";
            }

            return extension;
        }
    }
}
