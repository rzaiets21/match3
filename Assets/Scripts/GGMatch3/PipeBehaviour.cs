using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class PipeBehaviour : MonoBehaviour
	{
		[SerializeField]
		private Transform rotator;

		[SerializeField]
		private Transform scaler;

		[SerializeField]
		private List<SpriteRenderer> coloredSprites = new List<SpriteRenderer>();

		[SerializeField]
		private Transform offsetPositionTransform;

		[SerializeField]
		public Transform exitTransform;

		[SerializeField]
		private List<Transform> visibleWhenEntrance = new List<Transform>();

		[SerializeField]
		private List<Transform> visibleWhenExit = new List<Transform>();

		public void SetScale(float scale)
		{
			scaler.localScale = new Vector3(scale, 1f, 1f);
		}

		public void SetOffsetPosition(Vector3 offsetPosition)
		{
			if (!(offsetPositionTransform == null))
			{
				offsetPositionTransform.localPosition = offsetPosition;
			}
		}

		public void SetColor(Color color)
		{
			for (int i = 0; i < coloredSprites.Count; i++)
			{
				SpriteRenderer spriteRenderer = coloredSprites[i];
				Color color2 = spriteRenderer.color;
				color.a = color2.a;
				spriteRenderer.color = color;
			}
		}

		public void Init(Vector3 center, Vector3 direction, bool isExit)
		{
			float num = GGUtil.VisualRotationAngleUpAxis(direction);
			if (!isExit)
			{
				num += 180f;
			}
			rotator.localRotation = Quaternion.AngleAxis(num, Vector3.back);
			base.transform.localPosition = center;
			GGUtil.SetActive(visibleWhenExit, isExit);
			GGUtil.SetActive(visibleWhenEntrance, !isExit);
		}

		public void Init(Slot slot, bool isExit)
		{
			Vector3 localPositionOfCenter = slot.localPositionOfCenter;
			Init(localPositionOfCenter, slot.gravity.forceDirections[0].ToVector3(), isExit);
		}
	}
}
