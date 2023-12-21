using UnityEngine;

public class DestroyParticleSystemAfterFinish : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem ps;

	private bool isDestroying;

	private void Start()
	{
		if (ps == null)
		{
			ps = GetComponent<ParticleSystem>();
		}
	}

	private void OnDisable()
	{
		DoDestroy();
	}

	private void DoDestroy()
	{
		if (!isDestroying)
		{
			isDestroying = true;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		if (!(ps == null) && !ps.IsAlive())
		{
			DoDestroy();
		}
	}
}
