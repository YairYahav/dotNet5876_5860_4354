using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DalApi;

static class DalConfig
{
    internal record DalImplementation
    (
        string Package,
        string Namespace,
        string Class
    );

    internal static string s_dalName;
    internal static Dictionary<string, DalImplementation> s_dalPackages;

    static DalConfig()
    {
        XElement dalConfig;
        try
        {
            dalConfig = XElement.Load(@"..\xml\dal-config.xml") ??
                throw new DalConfigException("dal-config.xml file is not found");
        }
        catch (Exception ex)
        {
            throw new DalConfigException("Error loading dal-config.xml", ex);
        }

        s_dalName =
            dalConfig.Element("dal")?.Value ?? throw new DalConfigException("<dal> element is missing");

        var packages = dalConfig.Element("dal-packages")?.Elements() ??
            throw new DalConfigException("<dal-packages> element is missing");

        s_dalPackages = (from item in packages
                         let pkg = item.Value
                         let ns = item.Attribute("namespace")?.Value ?? "Dal"
                         let cls = item.Attribute("class")?.Value ?? pkg
                         select (item.Name, new DalImplementation(pkg, ns, cls))
                        ).ToDictionary(p => "" + p.Name, p => p.Item2);

        if (!s_dalPackages.ContainsKey(s_dalName))
        {
            throw new DalConfigException($"DAL implementation named '{s_dalName}' not found in dal-packages.");
        }
    }
}

[Serializable]
public class DalConfigException : Exception
{
    public DalConfigException(string msg) : base(msg) { }
    public DalConfigException(string msg, Exception ex) : base(msg, ex) { }
}

