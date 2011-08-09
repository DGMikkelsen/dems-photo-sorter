using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoSorter.Business
{
    public class GroupByExtension
    {
        // This query will sort all the files under the specified folder
        //  and subfolder into groups keyed by the file extension.
        public static IOrderedEnumerable<IGrouping<string, FileInfo>> GetGrouped(string rootFolder)
        {
            string startFolder = rootFolder;
            var dir = new DirectoryInfo(startFolder);

            // This method assumes that the application has discovery permissions
            // for all folders under the specified path.
            IEnumerable<FileInfo> fileList = dir.GetFiles("*.*", SearchOption.AllDirectories);

            // Create the query.
            var queryGroupByExt =
                from file in fileList
                group file by GetGroupingProperty(file) into fileGroup
                orderby fileGroup.Key
                select fileGroup;

            return queryGroupByExt;
        }

        public static void Execute(IEnumerable<IGrouping<string, FileInfo>> commands, string newRootFolder)
        {
            foreach (var command in commands)
            {
                var folder = newRootFolder + @"\" + command.Key;
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                foreach (var fileInfo in command)
                {
                    var dest = folder + @"\" + fileInfo.Name;
                    fileInfo.CopyTo(dest);
                }
            }
        }

        private static string GetGroupingProperty(FileInfo file)
        {
            return file.CreationTime.ToLongDateString();
        }
    }
}
