using System.Collections;
using UnityEngine;

public class CurrencyPurchaseDialogNextButton : MonoBehaviour
{
	[SerializeField]
	private CurrencyPrefabAnimation animationIn;

	[SerializeField]
	private CurrencyPrefabAnimation animationOut;

	public void Init()
	{
		base.transform.localScale = Vector3.one;
	}

	public void AnimateIn(float delay = 0f)
	{
		animationIn.Init();
		animationIn.Play(delay);
	}

	public void AnimateOut(float delay = 0f)
	{
		animationIn.Init();
		animationOut.Play(delay);
	}

	public IEnumerator DoAnimateOut(float delay)
	{
		return animationOut.DoPlay(delay);
	}

	public IEnumerator DoAnimateIn(float delay)
	{
		return animationIn.DoPlay(delay);
	}
}
