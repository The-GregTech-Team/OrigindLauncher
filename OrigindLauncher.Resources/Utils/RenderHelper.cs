using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace OrigindLauncher.Resources.Utils
{
    public static class RenderHelper
    {
        public static bool IsIntelVideoCard =>
            new ManagementObjectSearcher(new SelectQuery("Select * From Win32_VideoController")).Get()
            .Cast<ManagementBaseObject>()
            .Any(obj => obj.Properties.Cast<PropertyData>().Where(property => property.Name == "AdapterCompatibility")
            .Any(property => property.Value.ToString().Contains("Intel")));



    }
}
