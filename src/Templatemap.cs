﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.AddAnyFile
{
    public static class TemplateMap
    {
        private static readonly string _folder;
        private static readonly List<string> _templateFiles = new List<string>();
        private const string _defaultExt = ".txt";
        private const string _templateDir = ".templates";

        static TemplateMap()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var userProfile = Path.Combine(folder, ".vs", _templateDir);

            if (Directory.Exists(userProfile))
            {
                _templateFiles.AddRange(Directory.GetFiles(userProfile, "*" + _defaultExt, SearchOption.AllDirectories));
            }

            var assembly = Assembly.GetExecutingAssembly().Location;
            _folder = Path.Combine(Path.GetDirectoryName(assembly), "Templates");
            _templateFiles.AddRange(Directory.GetFiles(_folder, "*" + _defaultExt, SearchOption.AllDirectories));
        }

        public static async Task<string> GetTemplateFilePathAsync(Project project, string file)
		{
			var name = Path.GetFileName(file);
			var safeName = name.StartsWith(".") ? name : Path.GetFileNameWithoutExtension(file);
			var relative = PackageUtilities.MakeRelative(project.GetRootFolder(), Path.GetDirectoryName(file) ?? "");

			var templateList = GetTemplates(file);

			var templateFile = TemplateMatching.GetMatchingTemplateFromFileName(templateList, file, _defaultExt);

			var template = await ReplaceTokensAsync(project, safeName, relative, templateFile);
			return NormalizeLineEndings(template);
		}

		private static List<string> GetTemplates(string file)
		{
			var defaultTemplateList = _templateFiles.ToList();
			var customTemplateList = CustomTemplatesFetching.GetTemplatesFromCurrentFolder(Path.GetDirectoryName(file), _templateDir, _defaultExt);
			var templateList = customTemplateList.Concat(defaultTemplateList).ToList();
			return templateList;
		}

		private static async Task<string> ReplaceTokensAsync(Project project, string name, string relative, string templateFile)
        {
            if (string.IsNullOrEmpty(templateFile))
            {
                return templateFile;
            }

            var rootNs = project.GetRootNamespace();
            var ns = string.IsNullOrEmpty(rootNs) ? "MyNamespace" : rootNs;

            if (!string.IsNullOrEmpty(relative))
            {
                ns += "." + ProjectHelpers.CleanNameSpace(relative);
            }

            using (var reader = new StreamReader(templateFile))
            {
                var content = await reader.ReadToEndAsync();

                return content.Replace("{namespace}", ns)
                              .Replace("{itemname}", name);
            }
        }

        private static string NormalizeLineEndings(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }

            return Regex.Replace(content, @"\r\n|\n\r|\n|\r", "\r\n");
        }
    }
}
