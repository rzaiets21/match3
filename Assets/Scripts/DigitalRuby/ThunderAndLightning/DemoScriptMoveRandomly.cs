using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class DemoScriptMoveRandomly : MonoBehaviour
	{
		private float elapsed = float.MaxValue;

		private Vector3 startStartPos;

		private Vector3 startEndPos;

		private Vector3 endStartPos;

		private Vector3 endEndPos;

		public Transform Transform1;

		public Transform Transform2;

		public float MoveTimeSeconds = 1f;

		private void Update()
		{
			if (!(MoveTimeSeconds <= 0f))
			{
				if (elapsed >= MoveTimeSeconds)
				{
					elapsed = 0f;
					Vector3 vector = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 10f));
					Vector3 vector2 = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 10f));
					startStartPos = Transform1.transform.position;
					endStartPos = Transform2.transform.position;
					startEndPos = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), 0f);
					endEndPos = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), 0f);
				}
				elapsed += LightningBoltScript.DeltaTime;
				Transform1.position = Vector3.Lerp(startStartPos, startEndPos, elapsed / MoveTimeSeconds);
				Transform2.position = Vector3.Lerp(endStartPos, endEndPos, elapsed / MoveTimeSeconds);
			}
		}
	}
}
