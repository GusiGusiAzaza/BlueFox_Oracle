using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace BlueFoxTests_Oracle.Components
{
    internal enum CreationMode
    {
        Create,
        Append,
        Exists
    }

    internal static class FileSystemManager
    {
        public static string WorkingDirectory = Environment.CurrentDirectory;                                  //Current WORKING directory
        public static string ProjectDirectory = Directory.GetParent(WorkingDirectory).Parent?.FullName;        //Current PROJECT directory

        public static bool CheckPathValidity(params string[] arr)
        {
            var invalidpaths = new List<string>();
            foreach (var path in arr)
                if (!File.Exists(path))
                {
                    if (Directory.Exists(path))
                        continue;

                    invalidpaths.Add(path);
                }

            if (invalidpaths.Count > 0)
                throw new FoxDirectoryNotFoundException("File directory not found", invalidpaths);
            return true;
        }

        public static void CreateFolder(string directory, string foldername)
        {
            CheckPathValidity(directory);
            var path = Path.Combine(directory, foldername);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.Combine(directory, foldername));
                MessageBox.Show($"\n-- Folder \'{foldername}\' created in \'{directory}\'");
            }
        }

        public static void CreateFile(string directory, string filename, string content, CreationMode mode)
        {
            CheckPathValidity(directory);
            var path = Path.Combine(directory, filename);
            switch (mode)
            {
                case CreationMode.Create:
                    {
                        File.Create(path);
                        MessageBox.Show($"\n-- File \'{filename}\' created in \'{directory}\'");
                    }
                    break;
                case CreationMode.Append:
                    {
                        File.AppendAllText(path, content);
                        MessageBox.Show($"\n-- Added text to \'{filename}\' in \'{directory}\'");
                    }
                    break;
                case CreationMode.Exists:
                    {
                        if (File.Exists(path))
                            throw new DirectoryNotFoundException(
                                $"CreateFile(mode Exists): \'{Path.Combine(directory, filename)}\' already exists");

                        goto case CreationMode.Create;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public static string ReadFile(string path)
        {
            CheckPathValidity(path);
            return File.ReadAllText(path);
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
