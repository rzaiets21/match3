using UnityEngine;

public class HammerAnimationBehaviour : MonoBehaviour
{
	[SerializeField]
	private Animation animationPlayer;

	[SerializeField]
	private string animationName = "Take 001";

	[SerializeField]
	private int frameOfHit = 30;

	[SerializeField]
	private Transform hammerIcon;

	[SerializeField]
	private Transform powerHammerIcon;

	public float animationTime => animationPlayer[animationName].time;

	public float animationNormalizedTime
	{
		get
		{
			if (!animationPlayer.isPlaying)
			{
				return 1f;
			}
			return animationPlayer[animationName].normalizedTime;
		}
	}

	public float timeWhenHammerHit => (float)frameOfHit / animationPlayer[animationName].clip.frameRate;

	public void Init(PowerupType powerupType)
	{
		GGUtil.SetActive(hammerIcon, powerupType == PowerupType.Hammer);
		GGUtil.SetActive(powerHammerIcon, powerupType == PowerupType.PowerHammer);
	}

	public void RemoveFromGame()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
