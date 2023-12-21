using UnityEngine;

public class CameraShake : MonoBehaviour
{
	public float shakeTimer;

	public float shakeAmount;

	public static CameraShake myCameraShake;

	private Vector3 startPos;

	private void Awake()
	{
		myCameraShake = this;
		startPos = base.transform.position;
	}

	private void Update()
	{
		if (shakeTimer >= 0f)
		{
			Vector2 vector = UnityEngine.Random.insideUnitCircle * shakeAmount;
			base.transform.position = new Vector3(base.transform.position.x + vector.x * 0.3f, base.transform.position.y + vector.y, base.transform.position.z);
			shakeTimer -= Time.deltaTime;
		}
		else
		{
			base.transform.position = startPos;
		}
	}

	public void ShakeCamera(float shakePwr, float shakeDur)
	{
		shakeAmount = shakePwr;
		shakeTimer = shakeDur;
	}
}
