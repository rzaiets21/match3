using UnityEngine;

[ExecuteInEditMode]
public class CameraLookAt : MonoBehaviour
{
	[SerializeField]
	private Transform lookAt;

	private void Update()
	{
		if (!(lookAt == null))
		{
			base.transform.rotation = Quaternion.LookRotation(lookAt.position - base.transform.position);
		}
	}
}
