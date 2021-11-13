using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Enjin.SDK.DataTypes;
using UnityEngine.Networking;

public class EnjinUIManager : MonoBehaviour
{
    [SerializeField] PlatformSelector _platformSelector;

    [SerializeField] GameObject[] _appLoginObjects;
    [SerializeField] Button _loginAppButton;
    [SerializeField] InputField _appIdInputField;
    [SerializeField] InputField _appSecretInputField;
    
    [SerializeField] Canvas _userInfoCanvas;
    [SerializeField] private GameObject _userLoginUI;
    [SerializeField] private GameObject _userWalletLinkCodesPanel;
    [SerializeField] private Text _walletLinkCodeText;
    [SerializeField] Button _loginUserButton;
    [SerializeField] InputField _userName;
    [SerializeField] InputField _userToken;

    [SerializeField] Text _loggedInAppId;
    [SerializeField] Text _loggedInUserName;
    [SerializeField] Text _accessToken;
    [SerializeField] private Text _appName;

    [SerializeField] private Image _appImage;
    [SerializeField] private Image _qrCode;

    [SerializeField] private Button _refreshUserInfoButton;

    [SerializeField] private GameObject _linkedUserInfoUI;
    [SerializeField] private Text _ethAdressText;
    [SerializeField] private Text _ethBalanceText;
    [SerializeField] private Text _enjBalanceText;

    [SerializeField] private InputField _sendDestinationAddress;
    [SerializeField] private Dropdown _sendCCYDropdown;
    [SerializeField] private Button _sendCCYButton;

    #region Public Accessors
    public int EnjinAppId
    {
        get { return System.Convert.ToInt32(_appIdInputField.text); }
        set { _loggedInAppId.text = value.ToString(); }
    }

    public string EnjinAppSecret
    {
        get { return _appSecretInputField.text; }
    }

    public string EnjinPlatformURL
    {
        get { return _platformSelector.GetPlatformURL(); }
    }

    public string UserName
    {
        get { return _userName.text; }
        set { _loggedInUserName.text = value; }
    }
    
    public string UserToken
    {
        get { return _userToken.text; }
    }

    public string AccessToken
    {
        get { return _accessToken.text; }
        set { _accessToken.text = value; }
    }

    public string AppName
    {
        set => _appName.text = value;
    }

    public string SendDestinationAddress => _sendDestinationAddress.text;
    public string SelectedSendCCY => _sendCCYDropdown.options[_sendCCYDropdown.value].text;

    public void SetAppImage(string MediaUrl)
    {
        StartCoroutine(DownloadAppImage(MediaUrl));
    }
    
    #endregion
    
    #region UI Panel Control
    public void UpdateUserUIInfo(User enjinUser)
    {
        if (enjinUser == null)
        {
            Debug.LogError("No Current User");
            return;
        }

        _loggedInUserName.text = enjinUser.name;

        for (int i = 0; i < enjinUser.identities.Length; ++i)
        {
            Debug.Log($"[{i} Identity ID] {enjinUser.identities[i].id}");
            Debug.Log($"[{i} Identity Wallet :: Eth Address] {enjinUser.identities[i].wallet.ethAddress}");
            if (enjinUser.identities[i].wallet.ethAddress == "")
            {
                _userWalletLinkCodesPanel.SetActive(true);
                _walletLinkCodeText.text = enjinUser.identities[i].linkingCode;
                StartCoroutine(DownloadQRImage(enjinUser.identities[i].linkingCodeQr));
            }
            else
            {
                _ethAdressText.text = enjinUser.identities[i].wallet.ethAddress;
                _ethBalanceText.text = enjinUser.identities[i].wallet.ethBalance.ToString("F9");
                _enjBalanceText.text = enjinUser.identities[i].wallet.enjBalance.ToString("F9");
                _userWalletLinkCodesPanel.SetActive(false);
                _linkedUserInfoUI.SetActive(true);
            }
        }
    }
    public void ShowUserInfoUI(bool shouldEnable = true)
    {
        _userInfoCanvas.enabled = shouldEnable;
    }

    public void ShowUserLoginUI(bool shouldEnable = true)
    {
        _userLoginUI.gameObject.SetActive(shouldEnable);
    }

    #endregion
        
    #region Listener Registration
    public void RegisterAppLoginEvent(UnityAction action)
    {
        _loginAppButton.onClick.AddListener(action);
    }

    public void RegisterUserLoginEvent(UnityAction action)
    {
        _loginUserButton.onClick.AddListener(action);
    }

    public void RegisterRefreshUserInfoEvent(UnityAction action)
    {
        _refreshUserInfoButton.onClick.AddListener(action);
    }

    public void RegisterSendCCYInfoEvent(UnityAction action)
    {
        _sendCCYButton.onClick.AddListener(action);
    }

    #endregion

    #region Utility
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
            _appImage.sprite = spriteToUse;
        }
    }
    
    IEnumerator DownloadQRImage(string MediaUrl)
    {   
        Debug.Log("Sending Request for QR Code");
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if(request.isNetworkError || request.isHttpError) 
            Debug.Log(request.error);
        else
        {
            Debug.Log("QR Code Found");
            Texture2D texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
            Rect rec = new Rect(0, 0, texture.width, texture.height);
            Sprite spriteToUse = Sprite.Create(texture,rec,new Vector2(0.5f,0.5f),100);
            _qrCode.sprite = spriteToUse;
        }
    }
    
    #endregion

}
