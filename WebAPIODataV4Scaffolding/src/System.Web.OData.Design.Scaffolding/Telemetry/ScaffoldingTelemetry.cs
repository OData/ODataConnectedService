// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Globalization;
using Microsoft.ApplicationInsights;
using Microsoft.Win32;

namespace System.Web.OData.Design.Scaffolding.Telemetry
{
    internal class ScaffoldingTelemetry
    {
        private TelemetryClient tc;

        public static bool IsParticipateVsExperienceImprovementProgram = GetIsParticipateVsExperienceImprovementProgram();

        public ScaffoldingTelemetry()
        {
            tc = new TelemetryClient();
            tc.InstrumentationKey = "0eac5cec-f2bf-467a-8682-a121ffe68a97"; // Key for odatav4scaffoldingapp

            // Set session data:
            tc.Context.Session.Id = Guid.NewGuid().ToString();
            tc.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
        }

        public void TrackEvent(string eventName)
        {
            if (tc != null && IsParticipateVsExperienceImprovementProgram == true)
            {
                tc.TrackEvent(eventName);
            }
        }

        public void TrackException(Exception exception)
        {
            if (tc != null && IsParticipateVsExperienceImprovementProgram == true)
            {
                tc.TrackException(exception);
            }
        }

        public void Flush()
        {
            if (tc != null && IsParticipateVsExperienceImprovementProgram == true)
            {
                tc.Flush();
            }
        }

        private static bool GetIsParticipateVsExperienceImprovementProgram()
        {
            string path = Environment.Is64BitOperatingSystem
                ? "SOFTWARE\\Wow6432Node\\Microsoft\\VSCommon\\12.0\\SQM"
                : "SOFTWARE\\Microsoft\\VSCommon\\12.0\\SQM";

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path))
            {
                if (key != null)
                {
                    Object o = key.GetValue("Optin");
                    if (o != null)
                    {
                        bool result = Convert.ToBoolean(o, CultureInfo.InvariantCulture);
                        return result;
                    }
                }
            }
            return false;
        }
    }
}
