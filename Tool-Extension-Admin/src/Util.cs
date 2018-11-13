using Landis.Utilities;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Landis.Extensions;

namespace Landis.Extensions.Admin
{
    public static class Util
    {
        /// <summary>
        /// Load or creates a dataset which will have its contents changed.
        /// </summary>
        /// <remarks>
        /// When the dataset is saved, the javascript file with extension
        /// information will be updated.
        /// </remarks>
        public static Dataset OpenDatasetForChange(string path)
        {
            //Console.WriteLine(path);

            // Linux Path separator Nov 2018
            string[] tempPath = path.Split("..");
            string newPath = Path.Combine(tempPath[0], "../extensions/extensions.xml");
            //Console.WriteLine(newPath);
            // ./Linux Path separator
            
            Dataset dataset = Dataset.LoadOrCreate(newPath);
            // Dataset.SavedEvent += DelegateToCallAfterFileSaved;
            return dataset;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets an alphabetical list of extensions from a dataset.
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public static List<ExtensionInfo> GetExtsInAlphaOrder(Dataset dataset)
        {
            List<ExtensionInfo> extensions = new List<ExtensionInfo>(dataset.Count);
            for (int i = 0; i < dataset.Count; i++)
                extensions.Add(dataset[i]);
            extensions.Sort(CompareNames);
            return extensions;
        }

        //---------------------------------------------------------------------

        public static  int CompareNames(ExtensionInfo x,
                                        ExtensionInfo y)
        {
            return x.Name.CompareTo(y.Name);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Surrounds a string with double quotes if it contains whitespace.
        /// </summary>
        public static string QuoteIfNeeded(string str)
        {
            if (str == null)
                return str;
            Regex pattern = new Regex(@"\s+");
            if (pattern.IsMatch(str))
                return string.Format("\"{0}\"", str);
            else
                return str;
        }
    }
}
