using UnityEngine;

public class RandomRotateOnEnable : MonoBehaviour
{
	[SerializeField]
	private Vector3 rotationAxis;

	[SerializeField]
	private float minRotation;

	[SerializeField]
	private float maxRotation;

	private void OnEnable()
	{
		Quaternion localRotation = Quaternion.AngleAxis(UnityEngine.Random.Range(minRotation, maxRotation), rotationAxis);
		base.transform.localRotation = localRotation;
	}
}
