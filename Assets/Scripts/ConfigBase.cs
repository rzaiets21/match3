using UnityEngine;
using UnityEngine.Audio;

public class ConfigBase : ScriptableObject
{
	public enum GGFileIOCloudSyncTypes
	{
		WhisperSync,
		GGCloudSync,
		GGSaveLocalOnly,
		GGSnapshotCloudSync
	}

	public enum SocialProvider
	{
		GooglePlayServices,
		AmazonGameCircle
	}

	public enum InAppProvider
	{
		GooglePlayServices,
		AmazonInApp
	}

	public GGGameName gameName;

	[SerializeField]
	private bool isDebug;

	public bool changeChipOnDevice;

	public bool isTestingSettingsScreen;

	public bool allowStockpiling;

	public bool overrideEnergyDurationOnlyIfBigger;

	[SerializeField]
	private bool isDebugOnDevice;

	[SerializeField]
	public bool useSinglePlayerSettings;

	public bool returnSameMatchServerAppNameWhenDebug;

	public bool showTotalEarningsOnWinningScreen;

	public bool useProfilePictureInMultiplayer;

	public bool useDeck;

	public bool useAddaptiveAIOnMultiplayer;

	public bool fakePlayerId;

	public string playerId;

	public bool useFakeFacebookPlayerId;

	public string fakeFacebookPlayerId;

	public bool useFakeApplePlayerId;

	public string fakeApplePlayerId;

	public string facebookAppId = "752983338151813";

	public string facebookAppPlayerSuffix = "";

	public string facebookLoginPermissions = "public_profile, user_friends";

	public string facebookMockResponse;

	public string facebookDisplayName = "";

	public string facebookTestAccessToken = "CAAKs1b0mBXIBAL30WYTgiDUD4Ok9fbZBGPxz76FWxcHZBGs1XqkWOgm9uOKxjtvuk3byE9WSNtwwXJy5rY7C3k7qZBLSClZAPRHkyilRmc0nWgRirGMFZBRkmJhzKt0YdOFpZAJmrJvtXRD22YUYiw0rWPj4up8h61MRVLTjXTrzuIdLVZBoLrxF8fKYmybFskoLdtalTn0TX3fAqPxZByd5";

	public string experimentsResourceName;

	public float cloudSyncTimeDelay = 30f;

	public float cloudSyncTimeDelayWhenRequestFails = 120f;

	public int maxSyncFrequency = 10;

	public Material adsMaterial;

	public bool secureCurrency;

	public string styleName;

	public string appNameOverrideForPrint;

	public string appName;

	public string inAppAdsName;

	public string matchServerUrl;

	public string matchServerApp;

	public int maxDisconnects = 2;

	public string rankingsServerUrl;

	public string rankingsApp;

	public string iosAppId;

	public string proVersionPackage;

	public bool usingProfileData;

	public string activeConfig;

	public bool usingNewPhoton;

	public string activeConfigIOS;

	public string activeConfigWinRT;

	public bool tournamentsOnlyAvailableInPro;

	public bool noWaitingInPro;

	public string leaderboardId;

	public bool isProVersion;

	public bool canUseRate;

	public bool gameCenterAvailable;

	[SerializeField]
	private string rateProvider;

	public bool verifyPlayInApp;

	public bool testingInAppPurchases;

	public bool useGiftiz;

	public bool allDifficultiesInPro;

	public bool canUseFacebook;

	public string menuSceneName = "Assets/Scenes/MenuSceneDemo.unity";

	public bool useGuestForNonLoggedInUsers;

	public bool showUpdatedPrivacyPolicyNotice;

	public int minVersionThatHasUpdatedPrivacyPolicy;

	public bool onlyShowNoticeIfUserLoggedIntoFacebook;

	public bool canChangeCuesMidGame;

	public bool updateSpinControlFromCueSpin;

	public GGFileIOCloudSyncTypes cloudSyncType;

	public string gameCenterCategory;

	public string mopubId;

	public string suggestionUrl;

	public string bugReportUrl;

	public SocialProvider socialProvider;

	public InAppProvider inAppProvider;

	public string interstitialAdId;

	public string amazonAppKey;

	public bool noAdsOnPromotionDay;

	public int initialCoins = 100;

	public int initialStars = 3;

	public int promotionCoins;

	public string promotionStart;

	public string promotionEnd;

	public string promotionMessage;

	private static ConfigBase _instance;

	public int notificationLatestTime = 22;

	public int notificationEarliestTime = 9;

	public AudioMixer masterMixer;

	public int minAudioVal = -80;

	public int maxAudioVal;

	public int initialVolumeLevel = 50;

	public int initialPlayerVersion = 3;

	public string facebookError = "100";

	public float multiplayerSkillPointsScale = 1f;

	public float multiplayerSkillPointsOffset;

	public int coinsCap = 100000000;

	public int tokensCap = 100000;

	public int ggDollarsCap = 100000000;

	public int eloCap = 9999;

	public int freeCoins = 50;

	public int coinsForLike = 50;

	public bool hasLootBoxes;

	public CurrencyType freeCoinsCurrencyType = CurrencyType.diamonds;

	public bool overrideTimeForFreeCoins;

	public float freeCoinsTimeHours;

	[SerializeField]
	private bool GGOfficialUserEnabled_;

	public string GGofficialUserName = "Giraffe Games";

	public string GGOfficialImageURL = "";

	public bool debug
	{
		get
		{
			if (isDebug)
			{
				if (!Application.isEditor && Application.platform != RuntimePlatform.OSXPlayer)
				{
					return isDebugOnDevice;
				}
				return true;
			}
			return false;
		}
	}

	public bool isFakePlayerIdOn
	{
		get
		{
			if (Application.isEditor && fakePlayerId)
			{
				return !string.IsNullOrEmpty(playerId);
			}
			return false;
		}
	}

	public string printAppName
	{
		get
		{
			if (!string.IsNullOrEmpty(appNameOverrideForPrint))
			{
				return appNameOverrideForPrint;
			}
			return appName;
		}
	}

	public string platformRateProvider => rateProvider;

	public bool shouldShowAmazonAds => !string.IsNullOrEmpty(amazonAppKey);

	public bool isProVersionEnabled => isProVersion;

	public bool isProVersionAvailable => proVersionPackage != null;

	public static ConfigBase instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (Resources.Load("Config", typeof(ConfigBase)) as ConfigBase);
				if (_instance.activeConfig != null)
				{
					ConfigBase configBase = Resources.Load(_instance.activeConfig, typeof(ConfigBase)) as ConfigBase;
					if (configBase != null)
					{
						_instance = configBase;
					}
				}
			}
			return _instance;
		}
	}

	public bool GGOfficialUserEnabled
	{
		get
		{
			if (GGOfficialUserEnabled_)
			{
				return Application.isEditor;
			}
			return false;
		}
	}

	public bool IsSyncEnabledInCurrentScene()
	{
		if (string.IsNullOrEmpty(menuSceneName))
		{
			return true;
		}
		return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == instance.menuSceneName;
	}

	public string GetSuggestionUrl(string playerName, string appName, string pid = "")
	{
		string text = suggestionUrl;
		text = text + "?player_name=" + playerName + "&game=" + appName;
		if (!string.IsNullOrEmpty(pid))
		{
			text = text + "&player_id=" + pid;
		}
		return text;
	}

	public string GetBugReportUrl(string playerName, string appName, string pid = "")
	{
		string text = bugReportUrl;
		text = text + "?player_name=" + playerName + "&game=" + appName;
		if (!string.IsNullOrEmpty(pid))
		{
			text = text + "&player_id=" + pid;
		}
		return text;
	}

	public void SetAudioMixerValues(GGPlayerSettings playerSettings)
	{
		if (!(masterMixer == null))
		{
			masterMixer.SetFloat("MusicVolume", playerSettings.isMusicOff ? (-80) : 0);
			masterMixer.SetFloat("SfxVolume", playerSettings.isSoundFXOff ? (-80) : 0);
		}
	}

	public string GetMatchServerAppName()
	{
		if (returnSameMatchServerAppNameWhenDebug)
		{
			return matchServerApp;
		}
		return matchServerApp + (debug ? "testing" : "");
	}
}
