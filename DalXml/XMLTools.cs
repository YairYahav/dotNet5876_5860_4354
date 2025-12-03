namespace Dal;

using DO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

static class XMLTools
{
    const string s_xmlDir = @"..\xml\";
    private static readonly object s_fileLock = new object();
    private const int MaxRetries = 3;
    private const int RetryDelayMs = 100;

    static XMLTools()
    {
        if (!Directory.Exists(s_xmlDir))
            Directory.CreateDirectory(s_xmlDir);
    }

    #region SaveLoadWithXMLSerializer
    public static void SaveListToXMLSerializer<T>(List<T> list, string xmlFileName) where T : class
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            lock (s_fileLock)
            {
                RetryOnIOException(() =>
                {
                    using FileStream file = new(xmlFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                    new XmlSerializer(typeof(List<T>)).Serialize(file, list);
                });
            }
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"fail to create xml file: {s_xmlDir + xmlFilePath}, {ex.Message}");
        }
    }
    
    public static List<T> LoadListFromXMLSerializer<T>(string xmlFileName) where T : class
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            if (!File.Exists(xmlFilePath)) return new();
            
            lock (s_fileLock)
            {
                return RetryOnIOException(() =>
                {
                    using FileStream file = new(xmlFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    XmlSerializer x = new(typeof(List<T>));
                    return x.Deserialize(file) as List<T> ?? new();
                });
            }
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"fail to load xml file: {xmlFilePath}, {ex.Message}");
        }
    }
    #endregion

    #region SaveLoadWithXElement
    public static void SaveListToXMLElement(XElement rootElem, string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            lock (s_fileLock)
            {
                RetryOnIOException(() =>
                {
                    rootElem.Save(xmlFilePath);
                });
            }
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"fail to create xml file: {s_xmlDir + xmlFilePath}, {ex.Message}");
        }
    }
    
    public static XElement LoadListFromXMLElement(string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            lock (s_fileLock)
            {
                return RetryOnIOException(() =>
                {
                    if (File.Exists(xmlFilePath))
                        return XElement.Load(xmlFilePath);
                    XElement rootElem = new(xmlFileName);
                    rootElem.Save(xmlFilePath);
                    return rootElem;
                });
            }
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"fail to load xml file: {xmlFilePath}, {ex.Message}");
        }
    }
    #endregion

    #region XmlConfig
    public static int GetAndIncreaseConfigIntVal(string xmlFileName, string elemName)
    {
        lock (s_fileLock)
        {
            XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
            int nextId = root.ToIntNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
            root.Element(elemName)?.SetValue((nextId + 1).ToString());
            XMLTools.SaveListToXMLElement(root, xmlFileName);
            return nextId;
        }
    }
    
    public static int GetConfigIntVal(string xmlFileName, string elemName)
    {
        lock (s_fileLock)
        {
            XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
            int num = root.ToIntNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
            return num;
        }
    }
    
    public static DateTime GetConfigDateVal(string xmlFileName, string elemName)
    {
        lock (s_fileLock)
        {
            XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
            DateTime dt = root.ToDateTimeNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
            return dt;
        }
    }
    
    public static void SetConfigIntVal(string xmlFileName, string elemName, int elemVal)
    {
        lock (s_fileLock)
        {
            XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
            root.Element(elemName)?.SetValue((elemVal).ToString());
            XMLTools.SaveListToXMLElement(root, xmlFileName);
        }
    }
    
    public static void SetConfigDateVal(string xmlFileName, string elemName, DateTime elemVal)
    {
        lock (s_fileLock)
        {
            XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
            root.Element(elemName)?.SetValue((elemVal).ToString());
            XMLTools.SaveListToXMLElement(root, xmlFileName);
        }
    }
    #endregion

    #region Helper Methods
    private static void RetryOnIOException(Action action)
    {
        for (int i = 0; i < MaxRetries; i++)
        {
            try
            {
                action();
                return;
            }
            catch (IOException) when (i < MaxRetries - 1)
            {
                Thread.Sleep(RetryDelayMs);
            }
        }
        // Final attempt without catching
        action();
    }

    private static T RetryOnIOException<T>(Func<T> func)
    {
        for (int i = 0; i < MaxRetries; i++)
        {
            try
            {
                return func();
            }
            catch (IOException) when (i < MaxRetries - 1)
            {
                Thread.Sleep(RetryDelayMs);
            }
        }
        // Final attempt without catching
        return func();
    }
    #endregion

    #region ExtensionFuctions
    public static T? ToEnumNullable<T>(this XElement element, string name) where T : struct, Enum =>
        Enum.TryParse<T>((string?)element.Element(name), out var result) ? (T?)result : null;
    public static DateTime? ToDateTimeNullable(this XElement element, string name) =>
        DateTime.TryParse((string?)element.Element(name), out var result) ? (DateTime?)result : null;
    public static double? ToDoubleNullable(this XElement element, string name) =>
        double.TryParse((string?)element.Element(name), out var result) ? (double?)result : null;
    public static int? ToIntNullable(this XElement element, string name) =>
        int.TryParse((string?)element.Element(name), out var result) ? (int?)result : null;
    #endregion

}
