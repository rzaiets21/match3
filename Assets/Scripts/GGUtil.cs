using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GGUtil
{
	public class ColorProvider
	{
		private Color[] colors = new Color[8]
		{
			Color.red,
			Color.green,
			Color.blue,
			Color.cyan,
			Color.grey,
			Color.magenta,
			Color.white,
			Color.yellow
		};

		public Color GetColor(int index)
		{
			if (index < 0)
			{
				return colors[0];
			}
			int num = index % colors.Length;
			return colors[num];
		}
	}

	public class UIUtil
	{
		private Vector3[] worldCorners = new Vector3[4];

		public Vector2 GetWorldDimensions(RectTransform trans)
		{
			trans.GetWorldCorners(worldCorners);
			float x = worldCorners[2].x - worldCorners[1].x;
			float y = worldCorners[1].y - worldCorners[0].y;
			return new Vector2(x, y);
		}

		public Pair<Vector2, Vector2> GetAABB(List<RectTransform> transforms)
		{
			Vector2 vector = Vector2.one * float.PositiveInfinity;
			Vector2 vector2 = Vector2.one * float.NegativeInfinity;
			for (int i = 0; i < transforms.Count; i++)
			{
				RectTransform rectTransform = transforms[i];
				Vector3 b = GetWorldDimensions(rectTransform) * 0.5f;
				Vector3 position = rectTransform.position;
				Vector3 vector3 = position - b;
				Vector3 vector4 = position + b;
				if (vector3.x < vector.x)
				{
					vector.x = vector3.x;
				}
				if (vector3.y < vector.y)
				{
					vector.y = vector3.y;
				}
				if (vector4.x > vector2.x)
				{
					vector2.x = vector4.x;
				}
				if (vector4.y > vector2.y)
				{
					vector2.y = vector4.y;
				}
			}
			return new Pair<Vector2, Vector2>(vector, vector2);
		}

		public void PositionRectInsideRect(RectTransform constrainRect, RectTransform rect, Vector2 screenSpacePosition)
		{
			constrainRect.GetWorldCorners(worldCorners);
			Vector2 vector = screenSpacePosition;
			vector.x /= constrainRect.rect.size.x;
			vector.y /= constrainRect.rect.size.y;
			Vector2 v = new Vector2(Mathf.LerpUnclamped(worldCorners[0].x, worldCorners[3].x, vector.x), Mathf.LerpUnclamped(worldCorners[0].y, worldCorners[1].y, vector.y));
			rect.position = v;
		}

		public void RestrictRectTransform(RectTransform rectTrans, RectTransform constrainingRectTrans)
		{
			rectTrans.GetLocalCorners(worldCorners);
			float num = worldCorners[3].x - worldCorners[0].x;
			float num2 = worldCorners[1].y - worldCorners[0].y;
			constrainingRectTrans.GetLocalCorners(worldCorners);
			float x = worldCorners[0].x;
			float x2 = worldCorners[3].x;
			float y = worldCorners[1].y;
			float y2 = worldCorners[0].y;
			Vector3 position = rectTrans.position;
			position.x = Mathf.Clamp(position.x, x + num * 0.5f, x2 - num * 0.5f);
			position.y = Mathf.Clamp(position.y, y2 + num2 * 0.5f, y - num2 * 0.5f);
			position.z = 0f;
			rectTrans.position = position;
		}

		public void AnchorRectInsideScreen(RectTransform rectTrans, Camera camera)
		{
			RectTransform rectTransform = rectTrans.parent as RectTransform;
			Vector2 worldDimensions = GetWorldDimensions(rectTransform);
			Vector3 b = camera.ViewportToWorldPoint(Vector2.zero);
			Vector3 vector = camera.ViewportToWorldPoint(Vector2.one) - b;
			rectTrans.anchorMin = Vector2.zero;
			rectTrans.anchorMax = new Vector2(vector.x / worldDimensions.x, vector.y / worldDimensions.y);
			rectTrans.anchoredPosition = rectTransform.anchoredPosition;
			rectTrans.offsetMax = Vector2.zero;
			rectTrans.offsetMin = Vector2.zero;
		}
	}

	public static ColorProvider colorProvider = new ColorProvider();

	public static UIUtil uiUtil = new UIUtil();

	public static void SetScale(Transform transform, Vector3 scale)
	{
		if (!(transform == null))
		{
			transform.localScale = scale;
		}
	}

	public static void SetFill(Image image, float fillAmount)
	{
		if (!(image == null))
		{
			image.fillAmount = fillAmount;
		}
	}

	public static void SetSprite(Image image, Sprite sprite)
	{
		if (!(image == null))
		{
			image.sprite = sprite;
		}
	}

	public static void SetAlpha(Image image, float alpha)
	{
		if (!(image == null))
		{
			Color color = image.color;
			color.a = alpha;
			image.color = color;
		}
	}

	public static void SetColor(Image image, Color color)
	{
		if (!(image == null))
		{
			image.color = color;
		}
	}

	public static void SetAlpha(CanvasGroup canvasGroup, float alpha)
	{
		if (!(canvasGroup == null))
		{
			canvasGroup.alpha = alpha;
		}
	}

	public static void Call(Action action)
	{
		action?.Invoke();
	}

	public static bool HasText(string text)
	{
		return !string.IsNullOrWhiteSpace(text);
	}

	public static void ChangeText(TextMeshProUGUI label, int text)
	{
		if (!(label == null))
		{
			ChangeText(label, text.ToString());
		}
	}

	public static void ChangeText(TextMeshProUGUI label, long text)
	{
		if (!(label == null))
		{
			ChangeText(label, text.ToString());
		}
	}

	public static void ChangeText(TextMeshProUGUI label, string text)
	{
		if (!(label == null) && !(label.text == text))
		{
			label.text = text;
		}
	}

	public static void SetActive(List<Transform> transform, bool active)
	{
		if (transform != null)
		{
			for (int i = 0; i < transform.Count; i++)
			{
				SetActive(transform[i], active);
			}
		}
	}

	public static void SetActive(Transform[] transform, bool active)
	{
		if (transform != null)
		{
			for (int i = 0; i < transform.Length; i++)
			{
				SetActive(transform[i], active);
			}
		}
	}

	public static void SetActive(Transform transform, bool active)
	{
		if (!(transform == null))
		{
			SetActive(transform.gameObject, active);
		}
	}

	public static void SetActive(ParticleSystem ps, bool active)
	{
		if (!(ps == null))
		{
			SetActive(ps.gameObject, active);
		}
	}

	public static void SetActive(MonoBehaviour beh, bool active)
	{
		if (!(beh == null))
		{
			SetActive(beh.gameObject, active);
		}
	}

	public static void Hide(List<Transform> list)
	{
		SetActive(list, active: false);
	}

	public static void Hide(Transform[] list)
	{
		SetActive(list, active: false);
	}

	public static void Hide(List<RectTransform> list)
	{
		SetActive(list, active: false);
	}

	public static void Hide(GameObject beh)
	{
		SetActive(beh, active: false);
	}

	public static void Hide(ParticleSystem particleSystem)
	{
		SetActive(particleSystem, active: false);
	}

	public static void Hide(MonoBehaviour beh)
	{
		SetActive(beh, active: false);
	}

	public static void Hide(Transform t)
	{
		SetActive(t, active: false);
	}

	public static void Hide(RectTransform t)
	{
		SetActive(t, active: false);
	}

	public static void Show(Transform trans)
	{
		SetActive(trans, active: true);
	}

	public static void Show(ParticleSystem ps)
	{
		SetActive(ps, active: true);
	}

	public static void Show(TrailRenderer trail)
	{
		SetActive(trail, active: true);
	}

	public static void SetActive(TrailRenderer trail, bool active)
	{
		if (!(trail == null))
		{
			trail.gameObject.SetActive(active);
		}
	}

	public static void Show(GameObject trans)
	{
		SetActive(trans, active: true);
	}

	public static void Show(RectTransform trans)
	{
		SetActive(trans, active: true);
	}

	public static void Show(MonoBehaviour beh)
	{
		SetActive(beh, active: true);
	}

	public static void SetActive(List<RectTransform> transList, bool active)
	{
		if (transList != null)
		{
			for (int i = 0; i < transList.Count; i++)
			{
				SetActive(transList[i], active);
			}
		}
	}

	public static void SetActive(RectTransform trans, bool active)
	{
		if (!(trans == null))
		{
			SetActive(trans.gameObject, active);
		}
	}

	public static bool isPartOfHierarchy(GameObject go)
	{
		if (go == null)
		{
			return false;
		}
		return go.transform.parent != null;
	}

	public static void SetActive(GameObject go, bool active)
	{
		if (!(go == null) && go.activeSelf != active)
		{
			go.SetActive(active);
		}
	}

	public static float VisualRotationAngleUpAxis(Vector3 vec)
	{
		float num = Vector3.Angle(Vector3.up, vec);
		if (vec.x < 0f || vec.y < 0f)
		{
			num *= -1f;
		}
		return num;
	}

	public static float SignedAngle(Vector2 from, Vector2 to)
	{
		float num = Vector2.Angle(from, to);
		float num2 = Mathf.Sign(from.x * to.y - from.y * to.x);
		return num * num2;
	}

	public static float SignedArea(List<Vector2> orderedVertices)
	{
		float num = 0f;
		for (int i = 0; i < orderedVertices.Count - 1; i++)
		{
			Vector2 vector = orderedVertices[i];
			Vector2 vector2 = orderedVertices[i + 1];
			num += (vector2.x - vector.x) * (vector2.y + vector.y);
		}
		if (orderedVertices.Count > 1)
		{
			Vector2 vector3 = orderedVertices[orderedVertices.Count - 1];
			Vector2 vector4 = orderedVertices[0];
			num += (vector4.x - vector3.x) * (vector4.y + vector3.y);
		}
		return num * 0.5f;
	}

	public static void Shuffle<T>(List<T> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			T value = list[i];
			int index = UnityEngine.Random.Range(0, list.Count);
			list[i] = list[index];
			list[index] = value;
		}
	}

	public static void Shuffle<T>(List<T> list, RandomProvider randomProvider)
	{
		for (int i = 0; i < list.Count; i++)
		{
			T value = list[i];
			int index = randomProvider.Range(0, list.Count);
			list[i] = list[index];
			list[index] = value;
		}
	}

	public static void Intersection<T>(List<T> a, List<T> b, List<T> resultIn) where T : struct
	{
		resultIn.Clear();
		for (int i = 0; i < a.Count; i++)
		{
			T item = a[i];
			for (int j = 0; j < b.Count; j++)
			{
				T val = b[j];
				if (item.Equals(val))
				{
					resultIn.Add(item);
				}
			}
		}
	}

	public static string FirstCharToUpper(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return string.Empty;
		}
		return char.ToUpper(s[0]).ToString() + s.Substring(1);
	}

	public static int ToLayer(LayerMask layer)
	{
		int num = layer.value;
		int num2 = (num <= 0) ? 31 : 0;
		while (num > 1)
		{
			num >>= 1;
			num2++;
		}
		return num2;
	}

	public static void SetLayerRecursively(GameObject obj, LayerMask newLayer)
	{
		SetLayerRecursively(obj, ToLayer(newLayer));
	}

	public static void SetLayerRecursively(GameObject obj, int newLayer)
	{
		if (!(null == obj))
		{
			obj.layer = newLayer;
			foreach (Transform item in obj.transform)
			{
				if (!(null == item))
				{
					SetLayerRecursively(item.gameObject, newLayer);
				}
			}
		}
	}

	public static void CopyWorldTransform(Transform from, Transform to)
	{
		to.position = from.position;
		to.localScale = from.localScale;
		to.rotation = from.rotation;
	}
}
