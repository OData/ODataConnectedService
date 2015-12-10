// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding.Versions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xml;

    internal class VersionFileReader
    {
        private const string IdAttribute = "Id";
        private const string VersionAttribute = "Version";

        /// <summary>
        /// This function returns all the versions from a specified versions file based on the specifid XPath. The versions file
        /// should have an Id attribute and Version attribute.
        /// </summary>
        /// <param name="xmlFile">The file containing the version information.</param>
        /// <param name="elementXPath">The specified XPath.</param>
        /// <returns>The version <see cref="Dictionary"/> if the file and XPath is valid, else returns null.</returns>
        internal static IDictionary<string, string> GetVersions(string xmlFile, string elementXPath)
        {
            if (String.IsNullOrWhiteSpace(xmlFile))
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, Resources.ArgumentNullOrEmpty, "xmlFile"));
            }

            if (String.IsNullOrWhiteSpace(elementXPath))
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, Resources.ArgumentNullOrEmpty, "elementXPath"));
            }

            XmlDocument document = new XmlDocument();
            string xmlFilePath = Path.Combine(Path.GetDirectoryName(typeof(VersionFileReader).Assembly.Location), xmlFile);

            if (!File.Exists(xmlFilePath))
            {
                return null;
            }

            using (TextReader reader = File.OpenText(xmlFilePath))
            {
                document.Load(reader);
            }

            Dictionary<string, string> versions = new Dictionary<string, string>();
            foreach (XmlElement xmlElement in document.SelectNodes(elementXPath))
            {
                string id = xmlElement.GetAttribute(IdAttribute);
                string version = xmlElement.GetAttribute(VersionAttribute);

                versions.Add(id, version);
            }

            return versions;
        }
    }
}
