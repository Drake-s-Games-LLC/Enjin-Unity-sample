using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enjin.SDK.DataTypes;
using UnityEngine.Events;
using UnityEngine.Networking;
using Enjin.SDK;

namespace Enjin.SDK.Core
{
    public class EnjinManager : MonoBehaviour
    {
        [SerializeField] private EnjinUIManager _enjinUIManager;

        public User _currentEnjinUser = null;
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
            _enjinUIManager.RegisterSendCCYInfoEvent(SendCCY);
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
                    _enjinUIManager.SetAppImage(Enjin.GetApp().image);
                    _enjinUIManager.ShowUserInfoUI();
                    _enjinUIManager.ShowUserLoginUI();
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

        private void SendCCY()
        {
            Debug.Log("Sending CCY: " + _enjinUIManager.SelectedSendCCY);
            Debug.Log("Destination: " + _enjinUIManager.SendDestinationAddress);
            
        }
    }
}

