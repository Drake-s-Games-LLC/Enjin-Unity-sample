using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enjin.SDK.DataTypes;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Enjin.SDK.Core
{
    public class EnjinManager : MonoBehaviour
    {
        [SerializeField] private EnjinUIManager _enjinUIManager;

        User _currentEnjinUser = null;
        bool _isConnecting = false;
        string _accessToken = null;

        private void Awake()
        {
            Enjin.IsDebugLogActive = true;
        }

        private void Start()
        {
            _enjinUIManager.RegisterAppLoginEvent(AppLogin);

            _enjinUIManager.RegisterUserLoginEvent(UserLogin);

            _enjinUIManager.RegisterGetIdentityEvent(GetCurrentUserIdentities);
        }

        private IEnumerator AppLoginRoutine()
        {
            Enjin.StartPlatform(_enjinUIManager.EnjinPlatformURL,
                _enjinUIManager.EnjinAppId, _enjinUIManager.EnjinAppSecret);

            int tick = 0;
            YieldInstruction waitASecond = new WaitForSeconds(1f);
            while (tick < 10)
            {
                if (Enjin.LoginState == LoginState.VALID)
                {
                    Debug.Log("App auth success");
                    _enjinUIManager.EnjinAppId = Enjin.AppID;
                    _enjinUIManager.AppName = Enjin.GetApp().name;
                    StartCoroutine(DownloadAppImage(Enjin.GetApp().image));
                    _enjinUIManager.DisableAppLoginUI();
                    _enjinUIManager.EnableUserLoginUI();
                    yield break;
                }

                tick++;
                yield return waitASecond;
            }

            Debug.Log("App auth Failed");
            _isConnecting = false;

            yield return null;
        }
        
        private void AppLogin()
        {
            if (Enjin.LoginState == LoginState.VALID)
                return;

            if (_isConnecting)
                return;

            _isConnecting = true;
            StartCoroutine(AppLoginRoutine());
        }

        private void UserLogin()
        {
            if (Enjin.LoginState != LoginState.VALID)
                return;

            _currentEnjinUser = Enjin.GetUser(_enjinUIManager.UserName);
            _accessToken = Enjin.AccessToken;

            Debug.Log($"[Logined User ID] {_currentEnjinUser.id}");
            Debug.Log($"[Logined User name] {_currentEnjinUser.name}");

            _enjinUIManager.UserName = _currentEnjinUser.name;
            _enjinUIManager.AccessToken = _accessToken;
            _enjinUIManager.DisableUserLoginUI();
        }

        private void GetCurrentUserIdentities()
        {
            if (Enjin.LoginState != LoginState.VALID)
                return;

            for (int i = 0; i < _currentEnjinUser.identities.Length; ++i)
            {
                Debug.Log($"[{i} Identity ID] {_currentEnjinUser.identities[i].id}");
                Debug.Log($"[{i} Identity linking Code] {_currentEnjinUser.identities[i].linkingCode}");
                Debug.Log($"[{i} Identity Wallet :: Eth Address] {_currentEnjinUser.identities[i].wallet.ethAddress}");
            }
        }
        
        IEnumerator DownloadAppImage(string MediaUrl)
        {   
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
            yield return request.SendWebRequest();
            if(request.isNetworkError || request.isHttpError) 
                Debug.Log(request.error);
            else
            {
                Texture2D texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                Rect rec = new Rect(0, 0, texture.width, texture.height);
                Sprite spriteToUse = Sprite.Create(texture,rec,new Vector2(0.5f,0.5f),100);
                _enjinUIManager.AppImage = spriteToUse;
            }
        } 
    }
}

