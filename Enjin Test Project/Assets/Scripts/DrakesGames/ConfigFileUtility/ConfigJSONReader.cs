using UnityEngine;
using System.IO;

public static class ConfigJSONReader
{
    private const string jsonLocalPath = @"C:\Users\Drake\Desktop\Enjin\Temp\enjinTestProjectConfig.json";

    public static EnjinConfigData GetConfigData()
    {
        EnjinConfigData enjinConfigData;
        using (StreamReader r = new StreamReader(jsonLocalPath))
        {
            string jsonRaw = r.ReadToEnd();
            Debug.Log("JSON String From File: " + jsonRaw);
            enjinConfigData = JsonUtility.FromJson<EnjinConfigData>(jsonRaw);
        }

        return enjinConfigData;
    }
}
