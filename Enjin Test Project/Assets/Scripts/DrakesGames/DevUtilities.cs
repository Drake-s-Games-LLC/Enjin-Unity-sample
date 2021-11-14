using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

public class DevUtilities : MonoBehaviour
{
    [SerializeField] private EnjinUIManager enjinUIManager;
    [SerializeField] private bool runAutomatically = false;
    private EnjinConfigData enjinConfigData;

    private IEnumerator Start()
    {
        if (runAutomatically)
        {
            yield return new WaitForSeconds(0.5f);
            AutoLogIn();
        }

        yield return null;
    }

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
    
    [Button]
    private void TestSecurePost()
    {
        Enjin.SDK.Core.Enjin.TestSecurePost();
    }
}
