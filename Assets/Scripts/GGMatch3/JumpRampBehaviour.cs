using UnityEngine;

namespace GGMatch3
{
	public class JumpRampBehaviour : MonoBehaviour
	{
		[SerializeField]
		private Transform rotator;

		[SerializeField]
		private MeshRenderer spriteMesh;

		private float spriteOffset;

		public void Init(Vector3 position, IntVector2 direction)
		{
			base.transform.localPosition = position;
			Quaternion localRotation = Quaternion.AngleAxis(GGUtil.VisualRotationAngleUpAxis(direction.ToVector3()) - 90f, Vector3.back);
			rotator.localRotation = localRotation;
			GGUtil.SetActive(this, active: true);
		}

		private void Update()
		{
			if (!(spriteMesh == null))
			{
				spriteOffset += Time.deltaTime * Match3Settings.instance.exitScrollSpeed;
				spriteOffset = Mathf.Repeat(spriteOffset, 1f);
				spriteMesh.sharedMaterial.mainTextureOffset = new Vector2(0f, spriteOffset);
			}
		}
	}
}
