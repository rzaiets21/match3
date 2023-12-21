using UnityEngine;
using UnityEngine.SceneManagement;

namespace DigitalRuby.ThunderAndLightning
{
	public class DemoScript : MonoBehaviour
	{
		private enum RotationAxes
		{
			MouseXAndY,
			MouseX,
			MouseY
		}

		public ThunderAndLightningScript ThunderAndLightningScript;

		public LightningBoltScript LightningBoltScript;

		public ParticleSystem CloudParticleSystem;

		public float MoveSpeed = 250f;

		public bool EnableMouseLook = true;

		private const float fastCloudSpeed = 50f;

		private float deltaTime;

		private float fpsIncrement;

		private string fpsText;

		private RotationAxes axes;

		private float sensitivityX = 15f;

		private float sensitivityY = 15f;

		private float minimumX = -360f;

		private float maximumX = 360f;

		private float minimumY = -60f;

		private float maximumY = 60f;

		private float rotationX;

		private float rotationY;

		private Quaternion originalRotation;

		private static readonly GUIStyle style = new GUIStyle();

		private void UpdateThunder()
		{
			if (ThunderAndLightningScript != null)
			{
				if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
				{
					ThunderAndLightningScript.CallNormalLightning();
				}
				else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
				{
					ThunderAndLightningScript.CallIntenseLightning();
				}
				else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3) && CloudParticleSystem != null)
				{
					ParticleSystem.MainModule main = CloudParticleSystem.main;
					main.simulationSpeed = ((main.simulationSpeed == 1f) ? 50f : 1f);
				}
			}
		}

		private void UpdateMovement()
		{
			float num = MoveSpeed * LightningBoltScript.DeltaTime;
			if (UnityEngine.Input.GetKey(KeyCode.W))
			{
				Camera.main.transform.Translate(0f, 0f, num);
			}
			if (UnityEngine.Input.GetKey(KeyCode.S))
			{
				Camera.main.transform.Translate(0f, 0f, 0f - num);
			}
			if (UnityEngine.Input.GetKey(KeyCode.A))
			{
				Camera.main.transform.Translate(0f - num, 0f, 0f);
			}
			if (UnityEngine.Input.GetKey(KeyCode.D))
			{
				Camera.main.transform.Translate(num, 0f, 0f);
			}
		}

		private void UpdateMouseLook()
		{
			if (EnableMouseLook)
			{
				if (axes == RotationAxes.MouseXAndY)
				{
					rotationX += UnityEngine.Input.GetAxis("Mouse X") * sensitivityX;
					rotationY += UnityEngine.Input.GetAxis("Mouse Y") * sensitivityY;
					rotationX = ClampAngle(rotationX, minimumX, maximumX);
					rotationY = ClampAngle(rotationY, minimumY, maximumY);
					Quaternion rhs = Quaternion.AngleAxis(rotationX, Vector3.up);
					Quaternion rhs2 = Quaternion.AngleAxis(rotationY, -Vector3.right);
					base.transform.localRotation = originalRotation * rhs * rhs2;
				}
				else if (axes == RotationAxes.MouseX)
				{
					rotationX += UnityEngine.Input.GetAxis("Mouse X") * sensitivityX;
					rotationX = ClampAngle(rotationX, minimumX, maximumX);
					Quaternion rhs3 = Quaternion.AngleAxis(rotationX, Vector3.up);
					base.transform.localRotation = originalRotation * rhs3;
				}
				else
				{
					rotationY += UnityEngine.Input.GetAxis("Mouse Y") * sensitivityY;
					rotationY = ClampAngle(rotationY, minimumY, maximumY);
					Quaternion rhs4 = Quaternion.AngleAxis(0f - rotationY, Vector3.right);
					base.transform.localRotation = originalRotation * rhs4;
				}
			}
		}

		private void UpdateQuality()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.F1))
			{
				QualitySettings.SetQualityLevel(0);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.F2))
			{
				QualitySettings.SetQualityLevel(1);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.F3))
			{
				QualitySettings.SetQualityLevel(2);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.F4))
			{
				QualitySettings.SetQualityLevel(3);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.F5))
			{
				QualitySettings.SetQualityLevel(4);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.F6))
			{
				QualitySettings.SetQualityLevel(5);
			}
		}

		private void UpdateOther()
		{
			deltaTime += (LightningBoltScript.DeltaTime - deltaTime) * 0.1f;
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				ReloadCurrentScene();
			}
		}

		private void OnGUI()
		{
			int width = Screen.width;
			int height = Screen.height;
			int num = (int)((float)Screen.height * 0.08f);
			Rect position = new Rect((int)((double)width * 0.01), height - num, width, (int)((double)num * 0.9));
			style.alignment = TextAnchor.LowerLeft;
			style.fontSize = num / 2;
			style.normal.textColor = Color.white;
			if ((fpsIncrement += LightningBoltScript.DeltaTime) > 1f)
			{
				fpsIncrement -= 1f;
				float num2 = deltaTime * 1000f;
				fpsText = string.Format(arg1: 1f / deltaTime, format: "{0:0.0} ms ({1:0.} fps)", arg0: num2);
			}
			GUI.Label(position, fpsText, style);
		}

		private void Update()
		{
			UpdateThunder();
			UpdateMovement();
			UpdateMouseLook();
			UpdateQuality();
			UpdateOther();
		}

		private void Start()
		{
			originalRotation = base.transform.localRotation;
			if (CloudParticleSystem != null)
			{
				var mainModule = CloudParticleSystem.main;
				
				mainModule.simulationSpeed = 50f;
			}
		}

		public static float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
			return Mathf.Clamp(angle, min, max);
		}

		public static void ReloadCurrentScene()
		{
			SceneManager.LoadScene(0, LoadSceneMode.Single);
		}
	}
}
