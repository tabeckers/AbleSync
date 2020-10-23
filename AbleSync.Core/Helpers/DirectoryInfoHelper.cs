using AbleSync.Core.Entities;
using System;
using System.IO;

namespace AbleSync.Core.Helpers
{
    // TODO Windows specific atm.
    /// <summary>
    ///     Contains helper functionality for directory info.
    /// </summary>
    public static class DirectoryInfoHelper
    {
        /// <summary>
        ///     Gets the directory info from a project entity.
        /// </summary>
        /// <param name="rootDirectory">The ablesync root.</param>
        /// <param name="project">The project to get the directory info from.</param>
        /// <returns>The projects directory info.</returns>
        public static DirectoryInfo GetFromProject(Uri rootDirectory, Project project)
        {
            if (rootDirectory == null)
            {
                throw new ArgumentNullException(nameof(rootDirectory));
            }
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            return new DirectoryInfo(ParsePathFromProject(rootDirectory, project));
        }

        /// <summary>
        ///     Parses a project path.
        /// </summary>
        /// <param name="rootDirectory">The ablesync root.</param>
        /// <param name="project">The project to parse.</param>
        /// <returns>The full path string, escaped.</returns>
        public static string ParsePathFromProject(Uri rootDirectory, Project project)
        {
            if (rootDirectory == null)
            {
                throw new ArgumentNullException(nameof(rootDirectory));
            }
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            // TODO Beun, see https://github.com/tabeckers/AbleSync/issues/19
            var parsedPath = project.RelativePath.Replace("\\", "/", StringComparison.InvariantCulture);
            var path = $"{rootDirectory.AbsolutePath}/{parsedPath}";
            path = path.Replace("%20", " ", StringComparison.InvariantCulture);

            return path;
        }
    }
}
