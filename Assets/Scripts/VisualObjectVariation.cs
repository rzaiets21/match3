using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class VisualObjectVariation : MonoBehaviour
{
	private sealed class _003CDoScaleAnimation_003Ed__16 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public VisualObjectVariation _003C_003E4__this;

		public float delay;

		public bool hide;

		private List<VisualSprite> _003CanimatedSprites_003E5__2;

		private DecoratingSceneConfig.ScaleAnimationSettings _003Cconfig_003E5__3;

		private float _003Ctime_003E5__4;

		private Vector3 _003CstartScale_003E5__5;

		private float _003Cduration_003E5__6;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CDoScaleAnimation_003Ed__16(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			VisualObjectVariation visualObjectVariation = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003CanimatedSprites_003E5__2 = new List<VisualSprite>();
				for (int i = 0; i < visualObjectVariation.sprites.Count; i++)
				{
					VisualSprite visualSprite = visualObjectVariation.sprites[i];
					if (!visualSprite.visualSprite.isShadow)
					{
						_003CanimatedSprites_003E5__2.Add(visualSprite);
					}
				}
				_003Cconfig_003E5__3 = ScriptableObjectSingleton<DecoratingSceneConfig>.instance.GetScaleAnimationSettingsOrDefault(visualObjectVariation.visualObjectBehaviour.visualObject.animationSettingsName);
				_003Ctime_003E5__4 = 0f;
				_003CstartScale_003E5__5 = _003Cconfig_003E5__3.scaleFrom;
				if (delay > 0f)
				{
					if (hide)
					{
						for (int j = 0; j < _003CanimatedSprites_003E5__2.Count; j++)
						{
							VisualSprite visualSprite2 = _003CanimatedSprites_003E5__2[j];
							Color color = visualSprite2.spriteRenderer.color;
							color.a = 0.1f;
							visualSprite2.spriteRenderer.color = color;
						}
					}
					goto IL_013b;
				}
				goto IL_015c;
			case 1:
				_003C_003E1__state = -1;
				goto IL_013b;
			case 2:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_015c:
				_003Cduration_003E5__6 = _003Cconfig_003E5__3.duration;
				break;
				IL_013b:
				if (_003Ctime_003E5__4 < delay)
				{
					_003Ctime_003E5__4 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003Ctime_003E5__4 -= delay;
				goto IL_015c;
			}
			if (_003Ctime_003E5__4 < _003Cduration_003E5__6)
			{
				_003Ctime_003E5__4 += Time.deltaTime;
				float num2 = Mathf.InverseLerp(0f, _003Cduration_003E5__6, _003Ctime_003E5__4);
				float num3 = num2;
				num3 = _003Cconfig_003E5__3.scaleCurve.Evaluate(num2);
				Vector3 one = Vector3.one;
				one = ((!(num3 > 1f)) ? Vector3.LerpUnclamped(_003CstartScale_003E5__5, Vector3.one, num3) : Vector3.LerpUnclamped(_003Cconfig_003E5__3.scaleFrom, Vector3.one, num3));
				for (int k = 0; k < _003CanimatedSprites_003E5__2.Count; k++)
				{
					_003CanimatedSprites_003E5__2[k].pivotTransform.localScale = one;
				}
				float t = _003Cconfig_003E5__3.localPositionCurve.Evaluate(num2);
				Vector3 b = Vector3.LerpUnclamped(_003Cconfig_003E5__3.localPositionFrom, Vector3.zero, t);
				for (int l = 0; l < _003CanimatedSprites_003E5__2.Count; l++)
				{
					VisualSprite visualSprite3 = _003CanimatedSprites_003E5__2[l];
					visualSprite3.pivotTransform.localPosition = visualSprite3.visualSprite.pivotPosition + b;
					visualSprite3.spriteRenderer.color = Color.white;
				}
				_003C_003E2__current = null;
				_003C_003E1__state = 2;
				return true;
			}
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	public Sprite savedThumbnailSprite;

	public List<VisualSprite> sprites = new List<VisualSprite>();

	[NonSerialized]
	public GraphicsSceneConfig.Variation variation;

	public VisualObjectBehaviour visualObjectBehaviour;

	private IEnumerator animationEnum;

	public Sprite thumbnailSprite
	{
		get
		{
			if (savedThumbnailSprite != null)
			{
				return savedThumbnailSprite;
			}
			string thumbnailNamePrefix = visualObjectBehaviour.visualObject.sceneObjectInfo.thumbnailNamePrefix;
			if (!string.IsNullOrEmpty(thumbnailNamePrefix))
			{
				for (int i = 0; i < sprites.Count; i++)
				{
					VisualSprite visualSprite = sprites[i];
					if (!visualSprite.visualSprite.isShadow && !(visualSprite.spriteRenderer == null))
					{
						Sprite sprite = visualSprite.spriteRenderer.sprite;
						if (visualSprite.visualSprite.spriteName.ToLower().StartsWith(thumbnailNamePrefix.ToLower()))
						{
							return sprite;
						}
					}
				}
			}
			for (int j = 0; j < sprites.Count; j++)
			{
				VisualSprite visualSprite2 = sprites[j];
				if (!visualSprite2.visualSprite.isShadow && visualSprite2.spriteRenderer != null)
				{
					return visualSprite2.spriteRenderer.sprite;
				}
			}
			return null;
		}
	}

	public void SetActive(bool isActive)
	{
		animationEnum = null;
		if (isActive)
		{
			ResetSprites();
		}
		GGUtil.SetActive(base.gameObject, isActive);
	}

	public void SetVariationActive(string variationName)
	{
		SetActive(base.name == variationName);
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	public VisualSprite CreateSprite(GraphicsSceneConfig.VisualSprite vSprite)
	{
		GameObject gameObject = new GameObject(vSprite.spriteName);
		gameObject.transform.parent = base.transform;
		VisualSprite visualSprite = gameObject.AddComponent<VisualSprite>();
		visualSprite.Init(vSprite);
		return visualSprite;
	}

	public void Init(VisualObjectBehaviour visualObjectBehaviour, GraphicsSceneConfig.Variation variation)
	{
		this.visualObjectBehaviour = visualObjectBehaviour;
		this.variation = variation;
		savedThumbnailSprite = variation.thumbnailSprite;
		for (int i = 0; i < variation.sprites.Count; i++)
		{
			GraphicsSceneConfig.VisualSprite vSprite = variation.sprites[i];
			VisualSprite item = CreateSprite(vSprite);
			sprites.Add(item);
		}
	}

	public void DestroySelf()
	{
		Destroy(base.gameObject);
	}

	public static void Destroy(GameObject obj)
	{
		if (!Application.isPlaying)
		{
			UnityEngine.Object.DestroyImmediate(obj);
		}
		else
		{
			UnityEngine.Object.Destroy(obj);
		}
	}

	private void ResetSprites()
	{
		for (int i = 0; i < sprites.Count; i++)
		{
			sprites[i].ResetVisually();
		}
	}

	public void ScaleAnimation(float delay, bool hide = false)
	{
		ResetSprites();
		animationEnum = DoScaleAnimation(delay, hide);
	}

	private IEnumerator DoScaleAnimation(float delay, bool hide)
	{
		return new _003CDoScaleAnimation_003Ed__16(0)
		{
			_003C_003E4__this = this,
			delay = delay,
			hide = hide
		};
	}

	private void Update()
	{
		if (animationEnum != null && !animationEnum.MoveNext())
		{
			animationEnum = null;
		}
	}
}
