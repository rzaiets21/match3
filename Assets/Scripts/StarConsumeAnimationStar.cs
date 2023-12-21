using UnityEngine;
using UnityEngine.UI;

public class StarConsumeAnimationStar : MonoBehaviour
{
	[SerializeField]
	public Image whiteOutImage;

	[SerializeField]
	public CanvasGroup mainGroup;

	public void Init()
	{
		GGUtil.SetActive(whiteOutImage, active: false);
		mainGroup.alpha = 1f;
	}
}
