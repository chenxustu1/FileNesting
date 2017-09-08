using System.Collections.Generic;
using System.IO;
using EnvDTE;

namespace MadsKristensen.FileNesting
{
    internal class KnownFileTypeNester : IFileNester
    {
        private static Dictionary<string, string[]> _mapping = new Dictionary<string, string[]>(){
            {".js", new [] {".coffee", ".litcoffee", ".iced", ".ts", ".tsx", ".dart", ".html", ".cshtml", ".vbhtml", ".aspx", ".master", ".ascx"}},
            {".css", new [] {".less", ".scss", ".sass", ".styl", ".html", ".cshtml", ".vbhtml", ".aspx", ".master", ".ascx"}},
            {".generated.cs",new []{".cshtml"} },
            {".cs",new []{".cshtml"} },
            {".ts", new [] {".html", ".cshtml", ".vbhtml", ".aspx", ".master", ".ascx"}},
            {".map", new [] {".js", ".css"}},
        };

        public NestingResult Nest(string fileName)
        {
            string extension = GetFileExtension(fileName).ToLowerInvariant();

            if (!_mapping.ContainsKey(extension))
                return NestingResult.Continue;

            foreach (string ext in _mapping[extension])
            {
                string parent = ChangeFileExtension(fileName, ext);
                ProjectItem item = FileNestingPackage.DTE.Solution.FindProjectItem(parent);

                if (item != null)
                {
                    item.ProjectItems.AddFromFile(fileName);
                    return NestingResult.StopProcessing;
                }
            }

            return NestingResult.Continue;
        }

        public bool IsEnabled()
        {
            return FileNestingPackage.Options.EnableKnownFileTypeRule;
        }

        public static string GetFileExtension(string fileName)
        {
            var dotIndex = fileName.IndexOf(".");
            if (dotIndex == -1) { return ""; }
            var fileExt = fileName.Substring(dotIndex);
            return fileExt;
        }

        public static string ChangeFileExtension(string fileName, string extName)
        {
            var dotIndex = fileName.IndexOf(".");
            if (dotIndex == -1) { return fileName; }
            return fileName.Substring(0, dotIndex) + extName;
        }
    }
}
