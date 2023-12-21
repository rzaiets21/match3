using System;
using UnityEngine;

public class RoomSceneRenderObject : MonoBehaviour
{
	private struct AlphaChange
	{
		public bool isActive;

		public float startAlpha;

		public float endAlpha;

		public float duration;

		public float time;

		public DecorateRoomScreen screen;
	}

	[SerializeField]
	private Material textureMaterial;

	[NonSerialized]
	private float animationAlpha_ = 1f;

	[NonSerialized]
	private float alpha_ = 1f;

	private AlphaChange alphaChange;

	public float animationAlpha
	{
		get
		{
			return animationAlpha_;
		}
		set
		{
			animationAlpha_ = value;
			ApplyAlpha(alpha, animationAlpha_);
		}
	}

	private float alpha
	{
		get
		{
			return alpha_;
		}
		set
		{
			alpha_ = value;
			ApplyAlpha(alpha_, animationAlpha);
		}
	}

	public void SetAlpha(float alpha)
	{
		this.alpha = alpha;
		alphaChange.isActive = false;
	}

	public void AnimateAlphaTo(float endAlpha, float duration, DecorateRoomScreen screen)
	{
		alphaChange.isActive = true;
		alphaChange.screen = screen;
		alphaChange.startAlpha = alpha;
		alphaChange.endAlpha = endAlpha;
		alphaChange.duration = duration;
		alphaChange.time = 0f;
	}

	private void ApplyAlpha(float alpha, float animationAlpha)
	{
		float a = alpha * animationAlpha;
		if (!(textureMaterial == null))
		{
			Color color = textureMaterial.color;
			color.a = a;
			textureMaterial.color = color;
		}
	}

	private void Update()
	{
		if (alphaChange.isActive)
		{
			alphaChange.time += Time.deltaTime;
			float num = Mathf.InverseLerp(0f, alphaChange.duration, alphaChange.time);
			alpha = Mathf.Lerp(alphaChange.startAlpha, alphaChange.endAlpha, num);
			if (alphaChange.screen != null)
			{
				alphaChange.screen.SetSpeachBubbleAlpha(alpha);
			}
			if (num >= 1f)
			{
				alphaChange.isActive = false;
			}
		}
	}
}
