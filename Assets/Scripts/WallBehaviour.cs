using GGMatch3;
using UnityEngine;

public class WallBehaviour : MonoBehaviour
{
	[SerializeField]
	private Transform wallVertical;

	[SerializeField]
	private Transform wallHorizontal;

	public void Init(IntVector2 direction)
	{
		GGUtil.SetActive(this, active: true);
		bool flag = Mathf.Abs(direction.x) > 0;
		bool active = !flag;
		GGUtil.SetActive(wallVertical, flag);
		GGUtil.SetActive(wallHorizontal, active);
		Vector3 localPosition = new Vector3(direction.x, direction.y, 0f) * 0.5f;
		wallVertical.localPosition = localPosition;
		wallHorizontal.localPosition = localPosition;
	}
}
