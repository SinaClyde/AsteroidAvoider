using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string androidGameId;
    [SerializeField] private string iOSGameId;
    [SerializeField] private bool testMode = true;

    [SerializeField] private string androidAdUnitId;
    [SerializeField] private string iOSAdUnitId;

    public static AdManager Instance;

    private string _gameId;
    private string _adUnitId;
    private GameOverHandler _gameOverHandler;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            InitialiseAds();

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void InitialiseAds()
    {
#if UNITY_IOS
        gameId = iOSGameId;
        adUnitId = iOSAdUnitId;
#elif UNITY_ANDROID
        gameId = androidGameId;
        adUnitId = androidAdUnitId;
#elif UNITY_EDITOR
        _gameId = androidGameId;
        _adUnitId = androidAdUnitId;
#endif

        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize(_gameId, testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Advertisement.Show(placementId, this);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {_adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {

    }

    public void OnUnityAdsShowClick(string placementId)
    {

    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId.Equals(_adUnitId)
            && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            _gameOverHandler.ContinueGame();
        }
    }

    public void ShowAd(GameOverHandler gameOverHandler)
    {
        this._gameOverHandler = gameOverHandler;

        Advertisement.Load(_adUnitId, this);
    }
}
