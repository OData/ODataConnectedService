// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData2OpenApi.ConsoleApp.Abstracts;

namespace Microsoft.OData2OpenApi.ConsoleApp
{
    internal class EdmModelProvider : IEdmModelProvider
    {
        private bool _isLocalFile;
        private string _inputCsdl;

        public EdmModelProvider(string inputCsdl, bool isLocalFile)
        {
            _isLocalFile = isLocalFile;
            _inputCsdl = inputCsdl;
        }

        public virtual IEdmModel GetEdmModel()
        {
            string csdl = GetCsdl();
            return CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
        }

        private string GetCsdl()
        {
            if (_isLocalFile)
            {
                return File.ReadAllText(_inputCsdl);
            }

            Uri input = new Uri(_inputCsdl);
            Uri requestUri = new Uri(input.OriginalString + "/$metadata");

            WebRequest request = WebRequest.Create(requestUri);

            WebResponse response = request.GetResponse();

            Stream receivedStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(receivedStream, Encoding.UTF8);

            return reader.ReadToEnd();
        }
    }
}
