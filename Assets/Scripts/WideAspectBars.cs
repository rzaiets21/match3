using UnityEngine;

public class WideAspectBars : MonoBehaviour
{
	private struct AlphaState
	{
		public bool isActive;

		public float startAlpha;

		public float endAlpha;

		public float time;

		public float duration;

		public bool hideAtEnd;

		public bool notActive => !isActive;
	}

	[SerializeField]
	private CanvasGroup canvasGroup;

	private AlphaState state;

	public void Hide()
	{
		SetAlpha(0f);
		GGUtil.SetActive(base.gameObject, active: false);
	}

	public void SetAlpha(float alpha)
	{
		state = default(AlphaState);
		canvasGroup.alpha = alpha;
	}

	public void AnimateShow()
	{
		state = default(AlphaState);
		canvasGroup.alpha = 0f;
		state.startAlpha = canvasGroup.alpha;
		state.endAlpha = 1f;
		GGUtil.SetActive(this, active: true);
		state.duration = 1f;
		state.isActive = true;
	}

	public void AnimateHide()
	{
		state = default(AlphaState);
		state.startAlpha = canvasGroup.alpha;
		state.endAlpha = 0f;
		GGUtil.SetActive(this, active: true);
		state.duration = 1f;
		state.hideAtEnd = true;
		state.isActive = true;
	}

	private void Update()
	{
		if (state.notActive)
		{
			return;
		}
		state.time += Time.deltaTime;
		float t = Mathf.InverseLerp(0f, state.duration, state.time);
		float alpha = Mathf.Lerp(state.startAlpha, state.endAlpha, t);
		canvasGroup.alpha = alpha;
		if (state.time >= state.duration)
		{
			state.isActive = false;
			if (state.hideAtEnd)
			{
				Hide();
			}
		}
	}
}
