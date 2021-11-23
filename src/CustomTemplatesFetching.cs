using System.Collections.Generic;
using System.IO;

namespace MadsKristensen.AddAnyFile
{
	public class CustomTemplatesFetching
    {

		public static IEnumerable<string> GetTemplatesFromCurrentFolder(string dir, string templateDir, string defaultExt)
        {
            var current = new DirectoryInfo(dir);
            var dynaList = new List<string>();

            while (current != null)
            {
                var tmplDir = Path.Combine(current.FullName, templateDir);

                if (Directory.Exists(tmplDir))
                {
                    dynaList.AddRange(Directory.GetFiles(tmplDir, "*" + defaultExt, SearchOption.AllDirectories));
                }

                current = current.Parent;
            }

			return dynaList;
        }

    }
}

