namespace NoteSHR.Core.Models;

public interface IDataPersistence
{
    object ExportValues();

    void ConvertValues(Dictionary<string, object> data)
    {
        foreach (var (key, val) in data)
        {
            GetType().GetProperty(key)?.SetValue(this, val);
        }
    }
}