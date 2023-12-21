using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GGSoundSystem : MonoBehaviour
{
	public enum MusicType
	{
		NoMusic,
		MainMenuMusic,
		GameMusic
	}

	public enum SFXType
	{
		ButtonPress,
		CancelPress,
		None,
		ButtonFail,
		FlyIn,
		ButtonConfirm,
		Flip,
		PurchaseSuccess,
		GiftPresented,
		GiftOpen,
		ChipSwap,
		ChipTap,
		CollectGoal,
		GoalsComplete,
		FlyRocket,
		CollectGoalStart,
		CreatePowerup,
		BombExplode,
		PlainMatch,
		SeekingMissleTakeOff,
		DiscoBallElectricity,
		DiscoBallExplode,
		CoinCollect,
		CoinCollectStart,
		BreakColorSlate,
		BreakBox,
		FlyCrossRocketAction,
		BreakChain,
		BreakIce,
		GingerbreadManRescue,
		SeekingMissleLand,
		HammerHit,
		PowerHammerHit,
		HammerStart,
		GrowingElementFinish,
		GrowingElementGrowFlower,
		CollectMoreMoves,
		CollectMoreMovesStart,
		RockBreak,
		ChocolateBreak,
		SnowDestroy,
		SnowCreate,
		BunnyOutOfHat,
		WinScreenStart,
		WinScreenReceieveAnimationStart,
		RecieveStar,
		RecieveCoin,
		YouWinAnimation
	}

	private struct TimedCounter
	{
		public float leftTime;

		public int count;
	}

	[Serializable]
	public class MusicSource
	{
		public MusicType musicType;

		public AudioSource source;
	}

	[Serializable]
	public class SoundFxClip
	{
		[Serializable]
		public class ClipVariation
		{
			public AudioClip clip;

			public float pitch = 1f;

			public float volume = 1f;
		}

		public struct VariationParameters
		{
			public float pitch;

			public AudioClip clip;

			public float volume;
		}

		public SFXType clipType;

		public AudioClip clip;

		public List<ClipVariation> variationList = new List<ClipVariation>();

		public VariationParameters GetVariation(int index)
		{
			VariationParameters result = default(VariationParameters);
			result.clip = clip;
			result.pitch = 1f;
			result.volume = 1f;
			if (variationList.Count == 0)
			{
				return result;
			}
			ClipVariation clipVariation = variationList[Mathf.Clamp(index, 0, variationList.Count - 1)];
			if (clipVariation.clip != null)
			{
				result.clip = clipVariation.clip;
			}
			result.pitch = clipVariation.pitch;
			result.volume = clipVariation.volume;
			return result;
		}
	}

	private class AudioSourceData
	{
		public AudioSource audioSource;

		public SFXType playedSound;

		public long frameIndexWhenStart;

		public AudioSourceData(AudioSource audioSource)
		{
			this.audioSource = audioSource;
		}
	}

	public struct PlayParameters
	{
		public SFXType soundType;

		public int variationIndex;

		public long frameNumber;
	}

	[SerializeField]
	private bool isDebug;

	private TimedCounter matchesCounter;

	[SerializeField]
	private MusicType defaultMusic;

	[SerializeField]
	private GameObject audioClipSourcePrefab;

	[SerializeField]
	private List<MusicSource> musics = new List<MusicSource>();

	[SerializeField]
	private List<SoundFxClip> soundFxList = new List<SoundFxClip>();

	private List<AudioSourceData> audioClipsSources = new List<AudioSourceData>();

	private static GGSoundSystem soundSystem_;

	public static GGSoundSystem instance
	{
		get
		{
			if (soundSystem_ == null)
			{
				soundSystem_ = NavigationManager.instance.GetObject<GGSoundSystem>();
			}
			return soundSystem_;
		}
	}

	public void PlayMusic(MusicType musicType)
	{
		for (int i = 0; i < musics.Count; i++)
		{
			MusicSource musicSource = musics[i];
			GGUtil.SetActive(musicSource.source.gameObject, musicSource.musicType == musicType);
		}
	}

	public static void Play(MusicType musicType)
	{
		GGSoundSystem instance = GGSoundSystem.instance;
		if (!(instance == null))
		{
			instance.PlayMusic(musicType);
		}
	}

	public static void Play(PlayParameters playParameters)
	{
		GGSoundSystem instance = GGSoundSystem.instance;
		if (!(instance == null))
		{
			instance.PlayFx(playParameters);
		}
	}

	public static void Play(SFXType soundType)
	{
		PlayParameters playParameters = default(PlayParameters);
		playParameters.soundType = soundType;
		playParameters.variationIndex = 0;
		Play(playParameters);
	}

	public static void ReportMatch()
	{
		GGSoundSystem instance = GGSoundSystem.instance;
		if (!(instance == null))
		{
			instance.DoReportMatch();
		}
	}

	public void PlayFx(PlayParameters p)
	{
		SoundFxClip clip = GetClip(p.soundType);
		Play(clip, p);
	}

	private void DoReportMatch()
	{
		matchesCounter.count++;
		matchesCounter.leftTime = 1f;
	}

	private void Start()
	{
		ConfigBase.instance.SetAudioMixerValues(GGPlayerSettings.instance);
		Play(defaultMusic);
	}

	private void Update()
	{
		matchesCounter.leftTime -= Time.deltaTime;
		if (matchesCounter.leftTime <= 0f)
		{
			matchesCounter = default(TimedCounter);
		}
	}

	private SoundFxClip GetClip(SFXType soundType)
	{
		for (int i = 0; i < soundFxList.Count; i++)
		{
			SoundFxClip soundFxClip = soundFxList[i];
			if (soundFxClip.clipType == soundType)
			{
				return soundFxClip;
			}
		}
		return null;
	}

	private long LastPlayingFrameNumber(SFXType soundType)
	{
		long num = -1L;
		for (int i = 0; i < audioClipsSources.Count; i++)
		{
			AudioSourceData audioSourceData = audioClipsSources[i];
			if (audioSourceData.playedSound == soundType && audioSourceData.audioSource.isPlaying && audioSourceData.frameIndexWhenStart > num)
			{
				num = audioSourceData.frameIndexWhenStart;
			}
		}
		return num;
	}

	private AudioSourceData NextAudioSource()
	{
		for (int i = 0; i < audioClipsSources.Count; i++)
		{
			AudioSourceData audioSourceData = audioClipsSources[i];
			if (!audioSourceData.audioSource.isPlaying)
			{
				return audioSourceData;
			}
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(audioClipSourcePrefab, base.transform);
		AudioSourceData audioSourceData2 = new AudioSourceData(gameObject.GetComponent<AudioSource>());
		GGUtil.SetActive(gameObject, active: true);
		audioClipsSources.Add(audioSourceData2);
		return audioSourceData2;
	}

	private void Play(SoundFxClip clip, PlayParameters p)
	{
		if (clip == null)
		{
			return;
		}
		SoundFxClip.VariationParameters variation = clip.GetVariation(p.variationIndex);
		AudioClip clip2 = variation.clip;
		if (!(clip2 == null) && !(variation.volume <= 0f))
		{
			long num = LastPlayingFrameNumber(p.soundType);
			if (p.frameNumber == 0L)
			{
				p.frameNumber = Time.frameCount;
			}
			if (num != p.frameNumber)
			{
				AudioSourceData audioSourceData = NextAudioSource();
				AudioSource audioSource = audioSourceData.audioSource;
				audioSource.clip = clip2;
				audioSource.loop = false;
				audioSource.pitch = variation.pitch;
				audioSource.volume = variation.volume;
				audioSource.Play();
				audioSourceData.frameIndexWhenStart = p.frameNumber;
				audioSourceData.playedSound = p.soundType;
			}
		}
	}
}
