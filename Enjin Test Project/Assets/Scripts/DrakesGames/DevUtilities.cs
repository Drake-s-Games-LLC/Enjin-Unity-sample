using UnityEngine;
using Sirenix.OdinInspector;

public class DevUtilities : MonoBehaviour
{
    [SerializeField] private EnjinUIManager enjinUIManager;
    private EnjinConfigData enjinConfigData;

    [Button]
    private void AutoLogIn()
    {
        enjinConfigData = ConfigJSONReader.GetConfigData();
        if (enjinConfigData == null)
        {
            Debug.LogWarning("Failed to get config data");
            return;
        }
        
        enjinUIManager.AutoLogIn(enjinConfigData);
    }
}
