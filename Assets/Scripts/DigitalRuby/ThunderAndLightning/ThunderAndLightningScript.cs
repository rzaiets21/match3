using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class ThunderAndLightningScript : MonoBehaviour
	{
		private class LightningBoltHandler
		{
			private sealed class _003CProcessLightning_003Ed__9 : IEnumerator<object>, IEnumerator, IDisposable
			{
				private int _003C_003E1__state;

				private object _003C_003E2__current;

				public LightningBoltHandler _003C_003E4__this;

				public bool intense;

				public Vector3? _start;

				public Vector3? _end;

				public bool visible;

				private AudioClip[] _003Csounds_003E5__2;

				private float _003Cintensity_003E5__3;

				object IEnumerator<object>.Current
				{
					[DebuggerHidden]
					get
					{
						return _003C_003E2__current;
					}
				}

				object IEnumerator.Current
				{
					[DebuggerHidden]
					get
					{
						return _003C_003E2__current;
					}
				}

				[DebuggerHidden]
				public _003CProcessLightning_003Ed__9(int _003C_003E1__state)
				{
					this._003C_003E1__state = _003C_003E1__state;
				}

				[DebuggerHidden]
				void IDisposable.Dispose()
				{
				}

				private bool MoveNext()
				{
					int num = _003C_003E1__state;
					LightningBoltHandler lightningBoltHandler = _003C_003E4__this;
					switch (num)
					{
					default:
						return false;
					case 0:
					{
						_003C_003E1__state = -1;
						lightningBoltHandler.script.lightningInProgress = true;
						float time;
						if (intense)
						{
							float t = UnityEngine.Random.Range(0f, 1f);
							_003Cintensity_003E5__3 = Mathf.Lerp(2f, 8f, t);
							time = 5f / _003Cintensity_003E5__3;
							_003Csounds_003E5__2 = lightningBoltHandler.script.ThunderSoundsIntense;
						}
						else
						{
							float t2 = UnityEngine.Random.Range(0f, 1f);
							_003Cintensity_003E5__3 = Mathf.Lerp(0f, 2f, t2);
							time = 30f / _003Cintensity_003E5__3;
							_003Csounds_003E5__2 = lightningBoltHandler.script.ThunderSoundsNormal;
						}
						if (lightningBoltHandler.script.skyboxMaterial != null && lightningBoltHandler.script.ModifySkyboxExposure)
						{
							lightningBoltHandler.script.skyboxMaterial.SetFloat("_Exposure", Mathf.Max(_003Cintensity_003E5__3 * 0.5f, lightningBoltHandler.script.skyboxExposureStorm));
						}
						lightningBoltHandler.Strike(_start, _end, intense, _003Cintensity_003E5__3, lightningBoltHandler.script.Camera, visible ? lightningBoltHandler.script.Camera : null);
						lightningBoltHandler.CalculateNextLightningTime();
						if (_003Cintensity_003E5__3 >= 1f && _003Csounds_003E5__2 != null && _003Csounds_003E5__2.Length != 0)
						{
							_003C_003E2__current = new WaitForSecondsLightning(time);
							_003C_003E1__state = 1;
							return true;
						}
						break;
					}
					case 1:
					{
						_003C_003E1__state = -1;
						AudioClip audioClip = null;
						do
						{
							audioClip = _003Csounds_003E5__2[UnityEngine.Random.Range(0, _003Csounds_003E5__2.Length - 1)];
						}
						while (_003Csounds_003E5__2.Length > 1 && audioClip == lightningBoltHandler.script.lastThunderSound);
						lightningBoltHandler.script.lastThunderSound = audioClip;
						lightningBoltHandler.script.audioSourceThunder.PlayOneShot(audioClip, _003Cintensity_003E5__3 * 0.5f * lightningBoltHandler.VolumeMultiplier);
						break;
					}
					}
					return false;
				}

				bool IEnumerator.MoveNext()
				{
					//ILSpy generated this explicit interface implementation from .override directive in MoveNext
					return this.MoveNext();
				}

				[DebuggerHidden]
				void IEnumerator.Reset()
				{
					throw new NotSupportedException();
				}
			}

			private float _003CVolumeMultiplier_003Ek__BackingField;

			private ThunderAndLightningScript script;

			private readonly System.Random random = new System.Random();

			public float VolumeMultiplier
			{
				get
				{
					return _003CVolumeMultiplier_003Ek__BackingField;
				}
				set
				{
					_003CVolumeMultiplier_003Ek__BackingField = value;
				}
			}

			public LightningBoltHandler(ThunderAndLightningScript script)
			{
				this.script = script;
				CalculateNextLightningTime();
			}

			private void UpdateLighting()
			{
				if (script.lightningInProgress)
				{
					return;
				}
				if (script.ModifySkyboxExposure)
				{
					script.skyboxExposureStorm = 0.35f;
					if (script.skyboxMaterial != null && script.skyboxMaterial.HasProperty("_Exposure"))
					{
						script.skyboxMaterial.SetFloat("_Exposure", script.skyboxExposureStorm);
					}
				}
				CheckForLightning();
			}

			private void CalculateNextLightningTime()
			{
				script.nextLightningTime = DigitalRuby.ThunderAndLightning.LightningBoltScript.TimeSinceStart + script.LightningIntervalTimeRange.Random(random);
				script.lightningInProgress = false;
				if (script.ModifySkyboxExposure && script.skyboxMaterial.HasProperty("_Exposure"))
				{
					script.skyboxMaterial.SetFloat("_Exposure", script.skyboxExposureStorm);
				}
			}

			public IEnumerator ProcessLightning(Vector3? _start, Vector3? _end, bool intense, bool visible)
			{
				return new _003CProcessLightning_003Ed__9(0)
				{
					_003C_003E4__this = this,
					_start = _start,
					_end = _end,
					intense = intense,
					visible = visible
				};
			}

			private void Strike(Vector3? _start, Vector3? _end, bool intense, float intensity, Camera camera, Camera visibleInCamera)
			{
				float min = intense ? (-1000f) : (-5000f);
				float max = intense ? 1000f : 5000f;
				float num = intense ? 500f : 2500f;
				float num2 = (UnityEngine.Random.Range(0, 2) == 0) ? UnityEngine.Random.Range(min, 0f - num) : UnityEngine.Random.Range(num, max);
				float lightningYStart = script.LightningYStart;
				float num3 = (UnityEngine.Random.Range(0, 2) == 0) ? UnityEngine.Random.Range(min, 0f - num) : UnityEngine.Random.Range(num, max);
				Vector3 vector = script.Camera.transform.position;
				vector.x += num2;
				vector.y = lightningYStart;
				vector.z += num3;
				if (visibleInCamera != null)
				{
					Quaternion rotation = visibleInCamera.transform.rotation;
					visibleInCamera.transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);
					float x = UnityEngine.Random.Range((float)visibleInCamera.pixelWidth * 0.1f, (float)visibleInCamera.pixelWidth * 0.9f);
					float z = UnityEngine.Random.Range(visibleInCamera.nearClipPlane + num + num, max);
					vector = visibleInCamera.ScreenToWorldPoint(new Vector3(x, 0f, z));
					vector.y = lightningYStart;
					visibleInCamera.transform.rotation = rotation;
				}
				Vector3 vector2 = vector;
				num2 = UnityEngine.Random.Range(-100f, 100f);
				lightningYStart = ((UnityEngine.Random.Range(0, 4) == 0) ? UnityEngine.Random.Range(-1f, 600f) : (-1f));
				num3 += UnityEngine.Random.Range(-100f, 100f);
				vector2.x += num2;
				vector2.y = lightningYStart;
				vector2.z += num3;
				vector2.x += num * camera.transform.forward.x;
				vector2.z += num * camera.transform.forward.z;
				while ((vector - vector2).magnitude < 500f)
				{
					vector2.x += num * camera.transform.forward.x;
					vector2.z += num * camera.transform.forward.z;
				}
				vector = (_start ?? vector);
				vector2 = (_end ?? vector2);
				if (Physics.Raycast(vector, (vector - vector2).normalized, out RaycastHit hitInfo, float.MaxValue))
				{
					vector2 = hitInfo.point;
				}
				int generations = script.LightningBoltScript.Generations;
				RangeOfFloats trunkWidthRange = script.LightningBoltScript.TrunkWidthRange;
				if (UnityEngine.Random.value < script.CloudLightningChance)
				{
					script.LightningBoltScript.TrunkWidthRange = default(RangeOfFloats);
					script.LightningBoltScript.Generations = 1;
				}
				script.LightningBoltScript.LightParameters.LightIntensity = intensity * 0.5f;
				script.LightningBoltScript.Trigger(vector, vector2);
				script.LightningBoltScript.TrunkWidthRange = trunkWidthRange;
				script.LightningBoltScript.Generations = generations;
			}

			private void CheckForLightning()
			{
				if (Time.time >= script.nextLightningTime)
				{
					bool intense = UnityEngine.Random.value < script.LightningIntenseProbability;
					script.StartCoroutine(ProcessLightning(null, null, intense, script.LightningAlwaysVisible));
				}
			}

			public void Update()
			{
				UpdateLighting();
			}
		}

		public LightningBoltPrefabScript LightningBoltScript;

		public Camera Camera;

		public RangeOfFloats LightningIntervalTimeRange = new RangeOfFloats
		{
			Minimum = 10f,
			Maximum = 25f
		};

		public float LightningIntenseProbability = 0.2f;

		public AudioClip[] ThunderSoundsNormal;

		public AudioClip[] ThunderSoundsIntense;

		public bool LightningAlwaysVisible = true;

		public float CloudLightningChance = 0.5f;

		public bool ModifySkyboxExposure;

		public float BaseLightRange = 2000f;

		public float LightningYStart = 500f;

		public float VolumeMultiplier = 1f;

		private float skyboxExposureOriginal;

		private float skyboxExposureStorm;

		private float nextLightningTime;

		private bool lightningInProgress;

		private AudioSource audioSourceThunder;

		private LightningBoltHandler lightningBoltHandler;

		private Material skyboxMaterial;

		private AudioClip lastThunderSound;

		private bool _003CEnableLightning_003Ek__BackingField;

		public float SkyboxExposureOriginal => skyboxExposureOriginal;

		public bool EnableLightning
		{
			get
			{
				return _003CEnableLightning_003Ek__BackingField;
			}
			set
			{
				_003CEnableLightning_003Ek__BackingField = value;
			}
		}

		private void Start()
		{
			EnableLightning = true;
			if (Camera == null)
			{
				Camera = Camera.main;
			}
			if (RenderSettings.skybox != null)
			{
				skyboxMaterial = (RenderSettings.skybox = new Material(RenderSettings.skybox));
			}
			skyboxExposureOriginal = (skyboxExposureStorm = ((skyboxMaterial == null || !skyboxMaterial.HasProperty("_Exposure")) ? 1f : skyboxMaterial.GetFloat("_Exposure")));
			audioSourceThunder = base.gameObject.AddComponent<AudioSource>();
			lightningBoltHandler = new LightningBoltHandler(this);
			lightningBoltHandler.VolumeMultiplier = VolumeMultiplier;
		}

		private void Update()
		{
			if (lightningBoltHandler != null && EnableLightning)
			{
				lightningBoltHandler.VolumeMultiplier = VolumeMultiplier;
				lightningBoltHandler.Update();
			}
		}

		public void CallNormalLightning()
		{
			CallNormalLightning(null, null);
		}

		public void CallNormalLightning(Vector3? start, Vector3? end)
		{
			StartCoroutine(lightningBoltHandler.ProcessLightning(start, end, intense: false, visible: true));
		}

		public void CallIntenseLightning()
		{
			CallIntenseLightning(null, null);
		}

		public void CallIntenseLightning(Vector3? start, Vector3? end)
		{
			StartCoroutine(lightningBoltHandler.ProcessLightning(start, end, intense: true, visible: true));
		}
	}
}
