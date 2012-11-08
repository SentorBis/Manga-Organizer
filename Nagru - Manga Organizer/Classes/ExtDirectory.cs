﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Nagru___Manga_Organizer
{
    /* Extends Directory.GetFiles to support multiple filters
       Author: Bean Software (2002-2008)                            */
    public static class ExtDirectory
    {
        public static string[] GetFiles(string SourceFolder,
            SearchOption SearchOption = SearchOption.AllDirectories,
            string Filter = "*.jpg|*.png|*.jpeg")
        {
            if (!Directory.Exists(SourceFolder)) return new string[0];
            List<string> lFiles = new List<string>();
            string[] sFilters = Filter.Split('|');

            //for each filter find matching file names
            for (int i = 0; i < sFilters.Length; i++)
                lFiles.AddRange(System.IO.Directory.GetFiles(SourceFolder,
                    sFilters[i], SearchOption));

            lFiles.Sort(new TrueCompare());
            return lFiles.ToArray();
        }

        /* Delete passed directory, including all files/subfolders 
           Author: Jeremy Edwards (Nov 30, 2008)                */
        public static void Delete(Object obj)
        {
            string sPath = obj as string;

            //remove any readonly setting, then delete file
            string[] asFiles = Directory.GetFiles(sPath);
            for (int i = 0; i < asFiles.Length; i++)
            {
                File.SetAttributes(asFiles[i], FileAttributes.Normal);
                File.Delete(asFiles[i]);
            }

            //delete all subfolders
            string[] asDirs = Directory.GetDirectories(sPath);
            for (int i = 0; i < asDirs.Length; i++)
                Delete(asDirs[i]);

            //delete current folder
            Directory.Delete(sPath, false);
        }

        /* Ensure chosen folder is not protected before operating 
           Author: Me */
        public static bool Restricted(string Path)
        {
            try
            {
                string[] asDirs = Directory.GetDirectories(Path, "*", SearchOption.TopDirectoryOnly);
                FileIOPermission fp;

                for (int i = 0; i < asDirs.Length; i++)
                {
                    fp = new FileIOPermission(FileIOPermissionAccess.Read |
                        FileIOPermissionAccess.Write, asDirs[i]);
                    fp.Demand();
                }

                return false;
            }
            catch (Exception)
            { return true; }
        }
    }
}
