namespace NoteSHR.Core.Models;

public interface IDataPersistence
{
    object ExportValues();

    void ConvertValues(object data)
    {
        var properties = data.GetType().GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(data);
            if (value == null) 
                continue;
            
            GetType().GetProperty(property.Name)?.SetValue(this, value);
        }
    }
}