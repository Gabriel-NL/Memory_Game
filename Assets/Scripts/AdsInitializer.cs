using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer
    : MonoBehaviour,
        IUnityAdsInitializationListener,
        IUnityAdsLoadListener,
        IUnityAdsShowListener
{
    string _androidGameId = "5679776";

    [SerializeField]
    string _androidAdUnitId = "Interstitial_Android";

    [SerializeField]
    string _iOsAdUnitId = "Interstitial_iOS";
    string _iOSGameId;
    bool _testMode = false;
    private string _gameId;
    CustomDebuger custom_Debuger;

    void Start()
    {
        custom_Debuger= new CustomDebuger();
        custom_Debuger.ClearLines();
        custom_Debuger.ReadTextFile();
        InitializeAds();
        
    }

    public void InitializeAds()
    {
#if UNITY_IOS
        _gameId = _iOSGameId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
#elif UNITY_EDITOR
        _gameId = _androidGameId; // Only for testing the functionality in the Editor
#endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
            custom_Debuger.AddLine("Initialized!");
        }
    }

    public void ShowAnVideoAd()
    {
        custom_Debuger.AddLine("Loading Ad: " + _androidAdUnitId);
        Advertisement.Load(_androidAdUnitId, this); // Corrected to use _androidAdUnitId instead of _gameId
    }

    public void OnInitializationComplete()
    {
        custom_Debuger.AddLine("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        custom_Debuger.AddLine($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    // Load content to the Ad Unit:
    public void LoadAd()
    {
        custom_Debuger.AddLine("Loading Ad: " + _androidAdUnitId);
        Advertisement.Load(_androidAdUnitId, this);
    }

    // Show the loaded content in the Ad Unit:
    public void ShowAd()
    {
        custom_Debuger.AddLine("Showing Ad: " + _androidAdUnitId);
        Advertisement.Show(_androidAdUnitId, this);
    }

    // Implement Load Listener and Show Listener interface methods:

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        custom_Debuger.AddLine($"Ad loaded: {adUnitId}");
        // Optionally show the ad automatically once it is loaded.
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        custom_Debuger.AddLine($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        custom_Debuger.AddLine($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        custom_Debuger.AddLine("Ad started: " + adUnitId);
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        custom_Debuger.AddLine("Ad clicked: " + adUnitId);
    }

    public void OnUnityAdsShowComplete(
        string adUnitId,
        UnityAdsShowCompletionState showCompletionState
    )
    {
        custom_Debuger.AddLine($"Ad completed: {adUnitId}, State: {showCompletionState}");
    }
}
