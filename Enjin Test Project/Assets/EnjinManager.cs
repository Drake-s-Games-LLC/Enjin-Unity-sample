using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enjin.SDK.DataTypes;
using UnityEngine.Events;
using UnityEngine.Networking;
using Enjin.SDK;
using Sirenix.OdinInspector;

namespace Enjin.SDK.Core
{
    public class EnjinManager : MonoBehaviour
    {
        [SerializeField] private EnjinUIManager _enjinUIManager;

        public EnjinUser _currentEnjinUser = null;
        bool _isConnecting = false;
        string _accessToken = null;

        private void Awake()
        {
            Enjin.IsDebugLogActive = true;
        }

        private void Start()
        {
            _enjinUIManager.ShowUserInfoUI(false);
            _enjinUIManager.RegisterAppLoginEvent(AppLogin);
            _enjinUIManager.RegisterUserLoginEvent(UserLogin);
            _enjinUIManager.RegisterRefreshUserInfoEvent(RefreshCurrentUserInfo);
            _enjinUIManager.RegisterSendCCYInfoEvent(SendEnj);
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
                    _enjinUIManager.EnjinAppId = Enjin.AppID;
                    _enjinUIManager.AppName = Enjin.GetApp().name;
                    _enjinUIManager.SetAppImage(Enjin.GetApp().image);
                    _enjinUIManager.ShowUserInfoUI();
                    _enjinUIManager.ShowUserLoginUI();
                    yield break;
                }

                tick++;
                yield return waitASecond;
            }
            
            _isConnecting = false;
            yield return null;
        }
        
        private void UserLogin()
        {
            if (Enjin.LoginState != LoginState.VALID)
                return;

            _currentEnjinUser = Enjin.GetUser(_enjinUIManager.UserName);
            _accessToken = Enjin.AccessToken;
            
            _enjinUIManager.UserName = _currentEnjinUser.name;
            _enjinUIManager.AccessToken = _accessToken;
            GetCurrentUserWalletInfo();
            _enjinUIManager.UpdateUserUIInfo(_currentEnjinUser);
            _enjinUIManager.ShowUserLoginUI(false);
        }
        
        private void GetCurrentUserWalletInfo()
        {
            if (_currentEnjinUser.identities[0].wallet.ethAddress != "")
            {
                _currentEnjinUser.identities[0].wallet = 
                    Enjin.GetWalletBalances(_currentEnjinUser.identities[0].wallet.ethAddress);
            }
        }
        
        private void RefreshCurrentUserInfo()
        {
            if (Enjin.LoginState != LoginState.VALID)
                return;
            
            _currentEnjinUser = Enjin.GetUser(_currentEnjinUser.name);
            GetCurrentUserWalletInfo();
            _enjinUIManager.UpdateUserUIInfo(_currentEnjinUser);
        }

        private void SendEnj()
        {
            if (_currentEnjinUser == null)
            {
                Debug.LogError("Send Failed: No User Logged In");
                return;
            }
            if (_currentEnjinUser.identities[0].wallet.ethAddress == "")
            {
                Debug.LogError("Send Failed: No linked Wallet");
                return;
            }

            Enjin.SendEnjRequest(_currentEnjinUser.identities[0].id,
                _enjinUIManager.SendDestinationAddress,
                _enjinUIManager.SendEnjAmount,
                RequestCallback,
                false
            );
        }

        private void RequestCallback(RequestEvent requestEvent)
        {
            Debug.Log($"[Request Callback Activated]\n" +
                      $"ID: {requestEvent.request_id}\n" +
                      $"EventType: {requestEvent.event_type}\n" +
                      $"Data: {requestEvent.data}");
        }

        
    }
}

