using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

namespace DigitalRuby.ThunderAndLightning
{
	public class LightningBolt
	{
		public class LineRendererMesh
		{
			private GameObject _003CGameObject_003Ek__BackingField;

			private int _003CTag_003Ek__BackingField;

			private Action<LightningCustomTransformStateInfo> _003CCustomTransform_003Ek__BackingField;

			private Transform _003CTransform_003Ek__BackingField;

			private const int defaultListCapacity = 2048;

			private static readonly Vector2 uv1 = new Vector2(0f, 0f);

			private static readonly Vector2 uv2 = new Vector2(1f, 0f);

			private static readonly Vector2 uv3 = new Vector2(0f, 1f);

			private static readonly Vector2 uv4 = new Vector2(1f, 1f);

			private readonly List<int> indices = new List<int>(2048);

			private readonly List<Vector3> vertices = new List<Vector3>(2048);

			private readonly List<Vector4> lineDirs = new List<Vector4>(2048);

			private readonly List<Color32> colors = new List<Color32>(2048);

			private readonly List<Vector3> ends = new List<Vector3>(2048);

			private readonly List<Vector4> texCoordsAndGlowModifiers = new List<Vector4>(2048);

			private readonly List<Vector4> fadeLifetimes = new List<Vector4>(2048);

			private const int boundsPadder = 1000000000;

			private int currentBoundsMinX = 1147483647;

			private int currentBoundsMinY = 1147483647;

			private int currentBoundsMinZ = 1147483647;

			private int currentBoundsMaxX = -1147483648;

			private int currentBoundsMaxY = -1147483648;

			private int currentBoundsMaxZ = -1147483648;

			private Mesh mesh;

			private MeshFilter meshFilterGlow;

			private MeshFilter meshFilterBolt;

			private MeshRenderer meshRendererGlow;

			private MeshRenderer meshRendererBolt;

			public GameObject GameObject
			{
				get
				{
					return _003CGameObject_003Ek__BackingField;
				}
				private set
				{
					_003CGameObject_003Ek__BackingField = value;
				}
			}

			public Material MaterialGlow
			{
				set
				{
					meshRendererGlow.sharedMaterial = value;
				}
			}

			public Material MaterialBolt
			{
				set
				{
					meshRendererBolt.sharedMaterial = value;
				}
			}

			public MeshRenderer MeshRendererGlow => meshRendererGlow;

			public MeshRenderer MeshRendererBolt => meshRendererBolt;

			public int Tag
			{
				get
				{
					return _003CTag_003Ek__BackingField;
				}
				set
				{
					_003CTag_003Ek__BackingField = value;
				}
			}

			public Action<LightningCustomTransformStateInfo> CustomTransform
			{
				get
				{
					return _003CCustomTransform_003Ek__BackingField;
				}
				set
				{
					_003CCustomTransform_003Ek__BackingField = value;
				}
			}

			public Transform Transform
			{
				get
				{
					return _003CTransform_003Ek__BackingField;
				}
				private set
				{
					_003CTransform_003Ek__BackingField = value;
				}
			}

			public bool Empty => vertices.Count == 0;

			public LineRendererMesh()
			{
				GameObject = new GameObject("LightningBoltMeshRenderer");
				GameObject.SetActive(value: false);
				this.mesh = new Mesh
				{
					name = "ProceduralLightningMesh"
				};
				this.mesh.MarkDynamic();
				GameObject gameObject = new GameObject("LightningBoltMeshRendererGlow");
				gameObject.transform.parent = GameObject.transform;
				GameObject gameObject2 = new GameObject("LightningBoltMeshRendererBolt");
				gameObject2.transform.parent = GameObject.transform;
				meshFilterGlow = gameObject.AddComponent<MeshFilter>();
				meshFilterBolt = gameObject2.AddComponent<MeshFilter>();
				meshFilterGlow.sharedMesh = (meshFilterBolt.sharedMesh = this.mesh);
				meshRendererGlow = gameObject.AddComponent<MeshRenderer>();
				meshRendererBolt = gameObject2.AddComponent<MeshRenderer>();
				meshRendererGlow.shadowCastingMode = (meshRendererBolt.shadowCastingMode = ShadowCastingMode.Off);
				meshRendererGlow.reflectionProbeUsage = (meshRendererBolt.reflectionProbeUsage = ReflectionProbeUsage.Off);
				meshRendererGlow.lightProbeUsage = (meshRendererBolt.lightProbeUsage = LightProbeUsage.Off);
				meshRendererGlow.receiveShadows = (meshRendererBolt.receiveShadows = false);
				Transform = GameObject.GetComponent<Transform>();

				meshRendererGlow.sortingLayerID = SortingLayer.NameToID("FX");
				meshRendererGlow.sortingOrder = 3;
				meshRendererBolt.sortingLayerID = SortingLayer.NameToID("FX");
				meshRendererBolt.sortingOrder = 3;
			}

			public void PopulateMesh()
			{
				if (vertices.Count == 0)
				{
					mesh.Clear();
				}
				else
				{
					PopulateMeshInternal();
				}
			}

			public bool PrepareForLines(int lineCount)
			{
				int num = lineCount * 4;
				if (vertices.Count + num > 64999)
				{
					return false;
				}
				return true;
			}

			public void BeginLine(Vector3 start, Vector3 end, float radius, Color32 color, float colorIntensity, Vector4 fadeLifeTime, float glowWidthModifier, float glowIntensity)
			{
				Vector4 dir = end - start;
				dir.w = radius;
				AppendLineInternal(ref start, ref end, ref dir, ref dir, ref dir, color, colorIntensity, ref fadeLifeTime, glowWidthModifier, glowIntensity);
			}

			public void AppendLine(Vector3 start, Vector3 end, float radius, Color32 color, float colorIntensity, Vector4 fadeLifeTime, float glowWidthModifier, float glowIntensity)
			{
				Vector4 dir = end - start;
				dir.w = radius;
				Vector4 dirPrev = lineDirs[lineDirs.Count - 3];
				Vector4 dirPrev2 = lineDirs[lineDirs.Count - 1];
				AppendLineInternal(ref start, ref end, ref dir, ref dirPrev, ref dirPrev2, color, colorIntensity, ref fadeLifeTime, glowWidthModifier, glowIntensity);
			}

			public void Reset()
			{
				CustomTransform = null;
				Tag++;
				GameObject.SetActive(value: false);
				mesh.Clear();
				indices.Clear();
				vertices.Clear();
				colors.Clear();
				lineDirs.Clear();
				ends.Clear();
				texCoordsAndGlowModifiers.Clear();
				fadeLifetimes.Clear();
				currentBoundsMaxX = (currentBoundsMaxY = (currentBoundsMaxZ = -1147483648));
				currentBoundsMinX = (currentBoundsMinY = (currentBoundsMinZ = 1147483647));
			}

			private void PopulateMeshInternal()
			{
				GameObject.SetActive(value: true);
				mesh.SetVertices(vertices);
				mesh.SetTangents(lineDirs);
				mesh.SetColors(colors);
				mesh.SetUVs(0, texCoordsAndGlowModifiers);
				mesh.SetUVs(1, fadeLifetimes);
				mesh.SetNormals(ends);
				mesh.SetTriangles(indices, 0);
				Bounds bounds = default(Bounds);
				Vector3 b = new Vector3(currentBoundsMinX - 2, currentBoundsMinY - 2, currentBoundsMinZ - 2);
				Vector3 a = new Vector3(currentBoundsMaxX + 2, currentBoundsMaxY + 2, currentBoundsMaxZ + 2);
				bounds.center = (a + b) * 0.5f;
				bounds.size = (a - b) * 1.2f;
				mesh.bounds = bounds;
			}

			private void UpdateBounds(ref Vector3 point1, ref Vector3 point2)
			{
				int num = (int)point1.x - (int)point2.x;
				num &= num >> 31;
				int num2 = (int)point2.x + num;
				int num3 = (int)point1.x - num;
				num = currentBoundsMinX - num2;
				num &= num >> 31;
				currentBoundsMinX = num2 + num;
				num = currentBoundsMaxX - num3;
				num &= num >> 31;
				currentBoundsMaxX -= num;
				int num4 = (int)point1.y - (int)point2.y;
				num4 &= num4 >> 31;
				int num5 = (int)point2.y + num4;
				int num6 = (int)point1.y - num4;
				num4 = currentBoundsMinY - num5;
				num4 &= num4 >> 31;
				currentBoundsMinY = num5 + num4;
				num4 = currentBoundsMaxY - num6;
				num4 &= num4 >> 31;
				currentBoundsMaxY -= num4;
				int num7 = (int)point1.z - (int)point2.z;
				num7 &= num7 >> 31;
				int num8 = (int)point2.z + num7;
				int num9 = (int)point1.z - num7;
				num7 = currentBoundsMinZ - num8;
				num7 &= num7 >> 31;
				currentBoundsMinZ = num8 + num7;
				num7 = currentBoundsMaxZ - num9;
				num7 &= num7 >> 31;
				currentBoundsMaxZ -= num7;
			}

			private void AddIndices()
			{
				int num = vertices.Count;
				indices.Add(num++);
				indices.Add(num++);
				indices.Add(num);
				indices.Add(num--);
				indices.Add(num);
				indices.Add(num += 2);
			}

			private void AppendLineInternal(ref Vector3 start, ref Vector3 end, ref Vector4 dir, ref Vector4 dirPrev1, ref Vector4 dirPrev2, Color32 color, float colorIntensity, ref Vector4 fadeLifeTime, float glowWidthModifier, float glowIntensity)
			{
				AddIndices();
				color.a = (byte)Mathf.Lerp(0f, 255f, colorIntensity * 0.1f);
				Vector4 item = new Vector4(uv1.x, uv1.y, glowWidthModifier, glowIntensity);
				vertices.Add(start);
				lineDirs.Add(dirPrev1);
				colors.Add(color);
				ends.Add(dir);
				vertices.Add(end);
				lineDirs.Add(dir);
				colors.Add(color);
				ends.Add(dir);
				dir.w = 0f - dir.w;
				vertices.Add(start);
				lineDirs.Add(dirPrev2);
				colors.Add(color);
				ends.Add(dir);
				vertices.Add(end);
				lineDirs.Add(dir);
				colors.Add(color);
				ends.Add(dir);
				texCoordsAndGlowModifiers.Add(item);
				item.x = uv2.x;
				item.y = uv2.y;
				texCoordsAndGlowModifiers.Add(item);
				item.x = uv3.x;
				item.y = uv3.y;
				texCoordsAndGlowModifiers.Add(item);
				item.x = uv4.x;
				item.y = uv4.y;
				texCoordsAndGlowModifiers.Add(item);
				fadeLifetimes.Add(fadeLifeTime);
				fadeLifetimes.Add(fadeLifeTime);
				fadeLifetimes.Add(fadeLifeTime);
				fadeLifetimes.Add(fadeLifeTime);
				UpdateBounds(ref start, ref end);
			}
		}

		private sealed class _003CEnableLastRendererCoRoutine_003Ed__39 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public LightningBolt _003C_003E4__this;

			private LineRendererMesh _003ClineRenderer_003E5__2;

			private int _003Ctag_003E5__3;

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
			public _003CEnableLastRendererCoRoutine_003Ed__39(int _003C_003E1__state)
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
				LightningBolt lightningBolt = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003ClineRenderer_003E5__2 = lightningBolt.activeLineRenderers[lightningBolt.activeLineRenderers.Count - 1];
					_003Ctag_003E5__3 = ++_003ClineRenderer_003E5__2.Tag;
					_003C_003E2__current = new WaitForSecondsLightning(lightningBolt.MinimumDelay);
					_003C_003E1__state = 1;
					return true;
				case 1:
					_003C_003E1__state = -1;
					lightningBolt.EnableLineRenderer(_003ClineRenderer_003E5__2, _003Ctag_003E5__3);
					return false;
				}
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

		private sealed class _003C_003Ec__DisplayClass41_0
		{
			public LightningBolt _003C_003E4__this;

			public LineRendererMesh currentLineRenderer;

			internal void _003CRenderGroup_003Eb__0()
			{
				_003C_003E4__this.EnableCurrentLineRenderer();
				currentLineRenderer = _003C_003E4__this.GetOrCreateLineRenderer();
			}
		}

		private sealed class _003CNotifyBolt_003Ed__42 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public LightningBoltParameters p;

			public LightningBoltDependencies dependencies;

			public Vector3 start;

			public Vector3 end;

			public Transform transform;

			private float _003ClifeTime_003E5__2;

			private LightningCustomTransformStateInfo _003Cstate_003E5__3;

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
			public _003CNotifyBolt_003Ed__42(int _003C_003E1__state)
			{
				this._003C_003E1__state = _003C_003E1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			private bool MoveNext()
			{
				switch (_003C_003E1__state)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					float delaySeconds = p.delaySeconds;
					_003ClifeTime_003E5__2 = p.LifeTime;
					_003C_003E2__current = new WaitForSecondsLightning(delaySeconds);
					_003C_003E1__state = 1;
					return true;
				}
				case 1:
					_003C_003E1__state = -1;
					if (dependencies.LightningBoltStarted != null)
					{
						dependencies.LightningBoltStarted(p, start, end);
					}
					_003Cstate_003E5__3 = ((p.CustomTransform == null) ? null : LightningCustomTransformStateInfo.GetOrCreateStateInfo());
					if (_003Cstate_003E5__3 != null)
					{
						_003Cstate_003E5__3.Parameters = p;
						_003Cstate_003E5__3.BoltStartPosition = start;
						_003Cstate_003E5__3.BoltEndPosition = end;
						_003Cstate_003E5__3.State = LightningCustomTransformState.Started;
						_003Cstate_003E5__3.Transform = transform;
						p.CustomTransform(_003Cstate_003E5__3);
						_003Cstate_003E5__3.State = LightningCustomTransformState.Executing;
					}
					if (p.CustomTransform == null)
					{
						_003C_003E2__current = new WaitForSecondsLightning(_003ClifeTime_003E5__2);
						_003C_003E1__state = 2;
						return true;
					}
					goto IL_0192;
				case 2:
					_003C_003E1__state = -1;
					break;
				case 3:
					{
						_003C_003E1__state = -1;
						goto IL_0192;
					}
					IL_0192:
					if (_003ClifeTime_003E5__2 > 0f)
					{
						p.CustomTransform(_003Cstate_003E5__3);
						_003ClifeTime_003E5__2 -= LightningBoltScript.DeltaTime;
						_003C_003E2__current = null;
						_003C_003E1__state = 3;
						return true;
					}
					break;
				}
				if (p.CustomTransform != null)
				{
					_003Cstate_003E5__3.State = LightningCustomTransformState.Ended;
					p.CustomTransform(_003Cstate_003E5__3);
					LightningCustomTransformStateInfo.ReturnStateInfoToCache(_003Cstate_003E5__3);
				}
				if (dependencies.LightningBoltEnded != null)
				{
					dependencies.LightningBoltEnded(p, start, end);
				}
				LightningBoltParameters.ReturnParametersToCache(p);
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

		private sealed class _003C_003Ec__DisplayClass44_0
		{
			public LightningBoltDependencies dependenciesRef;
		}

		private sealed class _003C_003Ec__DisplayClass44_1
		{
			public LightningBoltParameters parameters;

			public _003C_003Ec__DisplayClass44_0 CS_0024_003C_003E8__locals1;
		}

		private sealed class _003C_003Ec__DisplayClass44_2
		{
			public Transform transform;

			public _003C_003Ec__DisplayClass44_1 CS_0024_003C_003E8__locals2;

			internal void _003CProcessAllLightningParameters_003Eb__0()
			{
				CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.dependenciesRef.StartCoroutine(NotifyBolt(CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.dependenciesRef, CS_0024_003C_003E8__locals2.parameters, transform, CS_0024_003C_003E8__locals2.parameters.Start, CS_0024_003C_003E8__locals2.parameters.End));
			}
		}

		private sealed class _003C_003Ec__DisplayClass48_0
		{
			public LightningBolt _003C_003E4__this;

			public Vector3 start;

			public Vector3 end;

			public LightningBoltParameters parameters;

			public LightningLightParameters lp;

			public int startGroupIndex;

			public LightningBoltQualitySetting quality;

			internal void _003CRenderLightningBolt_003Eb__0()
			{
				_003C_003E4__this.RenderParticleSystems(start, end, parameters.TrunkWidth, parameters.LifeTime, parameters.delaySeconds);
				if (lp != null)
				{
					_003C_003E4__this.CreateLightsForGroup(_003C_003E4__this.segmentGroups[startGroupIndex], lp, quality, parameters.maxLights);
				}
			}
		}

		private sealed class _003C_003Ec__DisplayClass48_1
		{
			public LineRendererMesh currentLineRenderer;

			public _003C_003Ec__DisplayClass48_0 CS_0024_003C_003E8__locals1;

			internal void _003CRenderLightningBolt_003Eb__1()
			{
				CS_0024_003C_003E8__locals1._003C_003E4__this.EnableCurrentLineRenderer();
				currentLineRenderer = CS_0024_003C_003E8__locals1._003C_003E4__this.GetOrCreateLineRenderer();
			}
		}

		private sealed class _003CGenerateParticleCoRoutine_003Ed__54 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public float delay;

			public ParticleSystem p;

			public Vector3 pos;

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
			public _003CGenerateParticleCoRoutine_003Ed__54(int _003C_003E1__state)
			{
				this._003C_003E1__state = _003C_003E1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			private bool MoveNext()
			{
				switch (_003C_003E1__state)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003C_003E2__current = new WaitForSecondsLightning(delay);
					_003C_003E1__state = 1;
					return true;
				case 1:
					_003C_003E1__state = -1;
					p.transform.position = pos;
					if (p.emission.burstCount > 0)
					{
						ParticleSystem.Burst[] array = new ParticleSystem.Burst[p.emission.burstCount];
						p.emission.GetBursts(array);
						int count = UnityEngine.Random.Range(array[0].minCount, array[0].maxCount + 1);
						p.Emit(count);
					}
					else
					{
						ParticleSystem.MinMaxCurve rateOverTime = p.emission.rateOverTime;
						int count = (int)((rateOverTime.constantMax - rateOverTime.constantMin) * 0.5f);
						count = UnityEngine.Random.Range(count, count * 2);
						p.Emit(count);
					}
					return false;
				}
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

		public static int MaximumLightCount = 128;

		public static int MaximumLightsPerBatch = 8;

		private float _003CMinimumDelay_003Ek__BackingField;

		private bool _003CHasGlow_003Ek__BackingField;

		private CameraMode _003CCameraMode_003Ek__BackingField;

		private DateTime startTimeOffset;

		private LightningBoltDependencies dependencies;

		private float elapsedTime;

		private float lifeTime;

		private float maxLifeTime;

		private bool hasLight;

		private float timeSinceLevelLoad;

		private readonly List<LightningBoltSegmentGroup> segmentGroups = new List<LightningBoltSegmentGroup>();

		private readonly List<LightningBoltSegmentGroup> segmentGroupsWithLight = new List<LightningBoltSegmentGroup>();

		private readonly List<LineRendererMesh> activeLineRenderers = new List<LineRendererMesh>();

		private static int lightCount;

		private static readonly List<LineRendererMesh> lineRendererCache = new List<LineRendererMesh>();

		private static readonly List<LightningBoltSegmentGroup> groupCache = new List<LightningBoltSegmentGroup>();

		private static readonly List<Light> lightCache = new List<Light>();

		public float MinimumDelay
		{
			get
			{
				return _003CMinimumDelay_003Ek__BackingField;
			}
			private set
			{
				_003CMinimumDelay_003Ek__BackingField = value;
			}
		}

		public bool HasGlow
		{
			get
			{
				return _003CHasGlow_003Ek__BackingField;
			}
			private set
			{
				_003CHasGlow_003Ek__BackingField = value;
			}
		}

		public bool IsActive => elapsedTime < lifeTime;

		public CameraMode CameraMode
		{
			get
			{
				return _003CCameraMode_003Ek__BackingField;
			}
			private set
			{
				_003CCameraMode_003Ek__BackingField = value;
			}
		}

		public void SetupLightningBolt(LightningBoltDependencies dependencies)
		{
			if (dependencies == null || dependencies.Parameters.Count == 0)
			{
				UnityEngine.Debug.LogError("Lightning bolt dependencies must not be null");
				return;
			}
			if (this.dependencies != null)
			{
				UnityEngine.Debug.LogError("This lightning bolt is already in use!");
				return;
			}
			this.dependencies = dependencies;
			CameraMode = dependencies.CameraMode;
			timeSinceLevelLoad = LightningBoltScript.TimeSinceStart;
			CheckForGlow(dependencies.Parameters);
			MinimumDelay = float.MaxValue;
			if (dependencies.ThreadState != null)
			{
				startTimeOffset = DateTime.UtcNow;
				dependencies.ThreadState.AddActionForBackgroundThread(ProcessAllLightningParameters);
			}
			else
			{
				ProcessAllLightningParameters();
			}
		}

		public bool Update()
		{
			elapsedTime += LightningBoltScript.DeltaTime;
			if (elapsedTime > maxLifeTime)
			{
				return false;
			}
			if (hasLight)
			{
				UpdateLights();
			}
			return true;
		}

		public void Cleanup()
		{
			foreach (LightningBoltSegmentGroup item in segmentGroupsWithLight)
			{
				foreach (Light light in item.Lights)
				{
					CleanupLight(light);
				}
				item.Lights.Clear();
			}
			lock (groupCache)
			{
				foreach (LightningBoltSegmentGroup segmentGroup in segmentGroups)
				{
					groupCache.Add(segmentGroup);
				}
			}
			hasLight = false;
			elapsedTime = 0f;
			lifeTime = 0f;
			maxLifeTime = 0f;
			if (dependencies != null)
			{
				dependencies.ReturnToCache(dependencies);
				dependencies = null;
			}
			foreach (LineRendererMesh activeLineRenderer in activeLineRenderers)
			{
				if (activeLineRenderer != null)
				{
					activeLineRenderer.Reset();
					lineRendererCache.Add(activeLineRenderer);
				}
			}
			segmentGroups.Clear();
			segmentGroupsWithLight.Clear();
			activeLineRenderers.Clear();
		}

		public LightningBoltSegmentGroup AddGroup()
		{
			LightningBoltSegmentGroup lightningBoltSegmentGroup;
			lock (groupCache)
			{
				if (groupCache.Count == 0)
				{
					lightningBoltSegmentGroup = new LightningBoltSegmentGroup();
				}
				else
				{
					int index = groupCache.Count - 1;
					lightningBoltSegmentGroup = groupCache[index];
					lightningBoltSegmentGroup.Reset();
					groupCache.RemoveAt(index);
				}
			}
			segmentGroups.Add(lightningBoltSegmentGroup);
			return lightningBoltSegmentGroup;
		}

		public static void ClearCache()
		{
			foreach (LineRendererMesh item in lineRendererCache)
			{
				if (item != null)
				{
					UnityEngine.Object.Destroy(item.GameObject);
				}
			}
			foreach (Light item2 in lightCache)
			{
				if (item2 != null)
				{
					UnityEngine.Object.Destroy(item2.gameObject);
				}
			}
			lineRendererCache.Clear();
			lightCache.Clear();
			lock (groupCache)
			{
				groupCache.Clear();
			}
		}

		private void CleanupLight(Light l)
		{
			if (l != null)
			{
				dependencies.LightRemoved(l);
				lightCache.Add(l);
				l.gameObject.SetActive(value: false);
				lightCount--;
			}
		}

		private void EnableLineRenderer(LineRendererMesh lineRenderer, int tag)
		{
			if (lineRenderer != null && lineRenderer.GameObject != null && lineRenderer.Tag == tag && IsActive)
			{
				lineRenderer.PopulateMesh();
			}
		}

		private IEnumerator EnableLastRendererCoRoutine()
		{
			return new _003CEnableLastRendererCoRoutine_003Ed__39(0)
			{
				_003C_003E4__this = this
			};
		}

		private LineRendererMesh GetOrCreateLineRenderer()
		{
			LineRendererMesh lineRendererMesh;
			do
			{
				if (lineRendererCache.Count == 0)
				{
					lineRendererMesh = new LineRendererMesh();
					break;
				}
				int index = lineRendererCache.Count - 1;
				lineRendererMesh = lineRendererCache[index];
				lineRendererCache.RemoveAt(index);
			}
			while (lineRendererMesh == null || lineRendererMesh.Transform == null);
			lineRendererMesh.Transform.parent = null;
			lineRendererMesh.Transform.rotation = Quaternion.identity;
			lineRendererMesh.Transform.localScale = Vector3.one;
			lineRendererMesh.Transform.parent = dependencies.Parent.transform;
			lineRendererMesh.GameObject.layer = dependencies.Parent.layer;
			if (dependencies.UseWorldSpace)
			{
				lineRendererMesh.GameObject.transform.position = Vector3.zero;
			}
			else
			{
				lineRendererMesh.GameObject.transform.localPosition = Vector3.zero;
			}
			lineRendererMesh.MaterialGlow = dependencies.LightningMaterialMesh;
			lineRendererMesh.MaterialBolt = dependencies.LightningMaterialMeshNoGlow;
			if (string.IsNullOrEmpty(dependencies.SortLayerName))
			{
				string text3 = lineRendererMesh.MeshRendererGlow.sortingLayerName = (lineRendererMesh.MeshRendererBolt.sortingLayerName = null);
				int num3 = lineRendererMesh.MeshRendererGlow.sortingOrder = (lineRendererMesh.MeshRendererBolt.sortingOrder = 0);
			}
			else
			{
				string text3 = lineRendererMesh.MeshRendererGlow.sortingLayerName = (lineRendererMesh.MeshRendererBolt.sortingLayerName = dependencies.SortLayerName);
				int num3 = lineRendererMesh.MeshRendererGlow.sortingOrder = (lineRendererMesh.MeshRendererBolt.sortingOrder = dependencies.SortOrderInLayer);
			}
			activeLineRenderers.Add(lineRendererMesh);
			return lineRendererMesh;
		}

		private void RenderGroup(LightningBoltSegmentGroup group, LightningBoltParameters p)
		{
			_003C_003Ec__DisplayClass41_0 _003C_003Ec__DisplayClass41_ = new _003C_003Ec__DisplayClass41_0();
			_003C_003Ec__DisplayClass41_._003C_003E4__this = this;
			if (group.SegmentCount == 0)
			{
				return;
			}
			float num = (dependencies.ThreadState == null) ? 0f : ((float)(DateTime.UtcNow - startTimeOffset).TotalSeconds);
			float num2 = timeSinceLevelLoad + group.Delay + num;
			Vector4 fadeLifeTime = new Vector4(num2, num2 + group.PeakStart, num2 + group.PeakEnd, num2 + group.LifeTime);
			float num3 = group.LineWidth * 0.5f * LightningBoltParameters.Scale;
			int num4 = group.Segments.Count - group.StartIndex;
			float num5 = (num3 - num3 * group.EndWidthMultiplier) / (float)num4;
			float num6;
			if (p.GrowthMultiplier > 0f)
			{
				num6 = group.LifeTime / (float)num4 * p.GrowthMultiplier;
				num = 0f;
			}
			else
			{
				num6 = 0f;
				num = 0f;
			}
			_003C_003Ec__DisplayClass41_.currentLineRenderer = ((activeLineRenderers.Count == 0) ? GetOrCreateLineRenderer() : activeLineRenderers[activeLineRenderers.Count - 1]);
			if (!_003C_003Ec__DisplayClass41_.currentLineRenderer.PrepareForLines(num4))
			{
				if (_003C_003Ec__DisplayClass41_.currentLineRenderer.CustomTransform != null)
				{
					return;
				}
				if (dependencies.ThreadState != null)
				{
					dependencies.ThreadState.AddActionForMainThread(_003C_003Ec__DisplayClass41_._003CRenderGroup_003Eb__0, waitForAction: true);
				}
				else
				{
					EnableCurrentLineRenderer();
					_003C_003Ec__DisplayClass41_.currentLineRenderer = GetOrCreateLineRenderer();
				}
			}
			_003C_003Ec__DisplayClass41_.currentLineRenderer.BeginLine(group.Segments[group.StartIndex].Start, group.Segments[group.StartIndex].End, num3, group.Color, p.Intensity, fadeLifeTime, p.GlowWidthMultiplier, p.GlowIntensity);
			for (int i = group.StartIndex + 1; i < group.Segments.Count; i++)
			{
				num3 -= num5;
				if (p.GrowthMultiplier < 1f)
				{
					num += num6;
					fadeLifeTime = new Vector4(num2 + num, num2 + group.PeakStart + num, num2 + group.PeakEnd, num2 + group.LifeTime);
				}
				_003C_003Ec__DisplayClass41_.currentLineRenderer.AppendLine(group.Segments[i].Start, group.Segments[i].End, num3, group.Color, p.Intensity, fadeLifeTime, p.GlowWidthMultiplier, p.GlowIntensity);
			}
		}

		private static IEnumerator NotifyBolt(LightningBoltDependencies dependencies, LightningBoltParameters p, Transform transform, Vector3 start, Vector3 end)
		{
			return new _003CNotifyBolt_003Ed__42(0)
			{
				dependencies = dependencies,
				p = p,
				transform = transform,
				start = start,
				end = end
			};
		}

		private void ProcessParameters(LightningBoltParameters p, RangeOfFloats delay, LightningBoltDependencies depends)
		{
			MinimumDelay = Mathf.Min(delay.Minimum, MinimumDelay);
			p.delaySeconds = delay.Random(p.Random);
			if (depends.LevelOfDetailDistance > Mathf.Epsilon)
			{
				float num;
				if (p.Points.Count > 1)
				{
					num = Vector3.Distance(depends.CameraPos, p.Points[0]);
					num = Mathf.Min(Vector3.Distance(depends.CameraPos, p.Points[p.Points.Count - 1]));
				}
				else
				{
					num = Vector3.Distance(depends.CameraPos, p.Start);
					num = Mathf.Min(Vector3.Distance(depends.CameraPos, p.End));
				}
				int num2 = Mathf.Min(8, (int)(num / depends.LevelOfDetailDistance));
				p.Generations = Mathf.Max(1, p.Generations - num2);
				p.GenerationWhereForksStopSubtractor = Mathf.Clamp(p.GenerationWhereForksStopSubtractor - num2, 0, 8);
			}
			p.generationWhereForksStop = p.Generations - p.GenerationWhereForksStopSubtractor;
			lifeTime = Mathf.Max(p.LifeTime + p.delaySeconds, lifeTime);
			maxLifeTime = Mathf.Max(lifeTime, maxLifeTime);
			p.forkednessCalculated = (int)Mathf.Ceil(p.Forkedness * (float)p.Generations);
			if (p.Generations > 0)
			{
				p.Generator = (p.Generator ?? LightningGenerator.GeneratorInstance);
				p.Generator.GenerateLightningBolt(this, p, out Vector3 start, out Vector3 end);
				p.Start = start;
				p.End = end;
			}
		}

		private void ProcessAllLightningParameters()
		{
			_003C_003Ec__DisplayClass44_0 _003C_003Ec__DisplayClass44_ = new _003C_003Ec__DisplayClass44_0();
			int maxLights = MaximumLightsPerBatch / dependencies.Parameters.Count;
			RangeOfFloats delay = default(RangeOfFloats);
			List<int> list = new List<int>(dependencies.Parameters.Count + 1);
			int num = 0;
			foreach (LightningBoltParameters parameter in dependencies.Parameters)
			{
				delay.Minimum = parameter.DelayRange.Minimum + parameter.Delay;
				delay.Maximum = parameter.DelayRange.Maximum + parameter.Delay;
				parameter.maxLights = maxLights;
				list.Add(segmentGroups.Count);
				ProcessParameters(parameter, delay, dependencies);
			}
			list.Add(segmentGroups.Count);
			_003C_003Ec__DisplayClass44_.dependenciesRef = dependencies;
			using (IEnumerator<LightningBoltParameters> enumerator = _003C_003Ec__DisplayClass44_.dependenciesRef.Parameters.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					_003C_003Ec__DisplayClass44_1 _003C_003Ec__DisplayClass44_2 = new _003C_003Ec__DisplayClass44_1();
					_003C_003Ec__DisplayClass44_2.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass44_;
					_003C_003Ec__DisplayClass44_2.parameters = enumerator.Current;
					_003C_003Ec__DisplayClass44_2 _003C_003Ec__DisplayClass44_3 = new _003C_003Ec__DisplayClass44_2();
					_003C_003Ec__DisplayClass44_3.CS_0024_003C_003E8__locals2 = _003C_003Ec__DisplayClass44_2;
					_003C_003Ec__DisplayClass44_3.transform = RenderLightningBolt(_003C_003Ec__DisplayClass44_3.CS_0024_003C_003E8__locals2.parameters.quality, _003C_003Ec__DisplayClass44_3.CS_0024_003C_003E8__locals2.parameters.Generations, list[num], list[++num], _003C_003Ec__DisplayClass44_3.CS_0024_003C_003E8__locals2.parameters);
					if (_003C_003Ec__DisplayClass44_3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.dependenciesRef.ThreadState != null)
					{
						_003C_003Ec__DisplayClass44_3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.dependenciesRef.ThreadState.AddActionForMainThread(_003C_003Ec__DisplayClass44_3._003CProcessAllLightningParameters_003Eb__0);
					}
					else
					{
						_003C_003Ec__DisplayClass44_3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.dependenciesRef.StartCoroutine(NotifyBolt(_003C_003Ec__DisplayClass44_3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.dependenciesRef, _003C_003Ec__DisplayClass44_3.CS_0024_003C_003E8__locals2.parameters, _003C_003Ec__DisplayClass44_3.transform, _003C_003Ec__DisplayClass44_3.CS_0024_003C_003E8__locals2.parameters.Start, _003C_003Ec__DisplayClass44_3.CS_0024_003C_003E8__locals2.parameters.End));
					}
				}
			}
			if (dependencies.ThreadState != null)
			{
				dependencies.ThreadState.AddActionForMainThread(EnableCurrentLineRendererFromThread);
				return;
			}
			EnableCurrentLineRenderer();
			dependencies.AddActiveBolt(this);
		}

		private void EnableCurrentLineRendererFromThread()
		{
			EnableCurrentLineRenderer();
			dependencies.ThreadState = null;
			dependencies.AddActiveBolt(this);
		}

		private void EnableCurrentLineRenderer()
		{
			if (activeLineRenderers.Count != 0)
			{
				if (MinimumDelay <= 0f)
				{
					EnableLineRenderer(activeLineRenderers[activeLineRenderers.Count - 1], activeLineRenderers[activeLineRenderers.Count - 1].Tag);
				}
				else
				{
					dependencies.StartCoroutine(EnableLastRendererCoRoutine());
				}
			}
		}

		private void RenderParticleSystems(Vector3 start, Vector3 end, float trunkWidth, float lifeTime, float delaySeconds)
		{
			if (trunkWidth > 0f)
			{
				if (dependencies.OriginParticleSystem != null)
				{
					dependencies.StartCoroutine(GenerateParticleCoRoutine(dependencies.OriginParticleSystem, start, delaySeconds));
				}
				if (dependencies.DestParticleSystem != null)
				{
					dependencies.StartCoroutine(GenerateParticleCoRoutine(dependencies.DestParticleSystem, end, delaySeconds + lifeTime * 0.8f));
				}
			}
		}

		private Transform RenderLightningBolt(LightningBoltQualitySetting quality, int generations, int startGroupIndex, int endGroupIndex, LightningBoltParameters parameters)
		{
			_003C_003Ec__DisplayClass48_0 _003C_003Ec__DisplayClass48_ = new _003C_003Ec__DisplayClass48_0();
			_003C_003Ec__DisplayClass48_._003C_003E4__this = this;
			_003C_003Ec__DisplayClass48_.parameters = parameters;
			_003C_003Ec__DisplayClass48_.startGroupIndex = startGroupIndex;
			_003C_003Ec__DisplayClass48_.quality = quality;
			if (segmentGroups.Count == 0 || _003C_003Ec__DisplayClass48_.startGroupIndex >= segmentGroups.Count || endGroupIndex > segmentGroups.Count)
			{
				return null;
			}
			Transform result = null;
			_003C_003Ec__DisplayClass48_.lp = _003C_003Ec__DisplayClass48_.parameters.LightParameters;
			if (_003C_003Ec__DisplayClass48_.lp != null)
			{
				if (hasLight |= _003C_003Ec__DisplayClass48_.lp.HasLight)
				{
					_003C_003Ec__DisplayClass48_.lp.LightPercent = Mathf.Clamp(_003C_003Ec__DisplayClass48_.lp.LightPercent, Mathf.Epsilon, 1f);
					_003C_003Ec__DisplayClass48_.lp.LightShadowPercent = Mathf.Clamp(_003C_003Ec__DisplayClass48_.lp.LightShadowPercent, 0f, 1f);
				}
				else
				{
					_003C_003Ec__DisplayClass48_.lp = null;
				}
			}
			LightningBoltSegmentGroup lightningBoltSegmentGroup = segmentGroups[_003C_003Ec__DisplayClass48_.startGroupIndex];
			_003C_003Ec__DisplayClass48_.start = lightningBoltSegmentGroup.Segments[lightningBoltSegmentGroup.StartIndex].Start;
			_003C_003Ec__DisplayClass48_.end = lightningBoltSegmentGroup.Segments[lightningBoltSegmentGroup.StartIndex + lightningBoltSegmentGroup.SegmentCount - 1].End;
			_003C_003Ec__DisplayClass48_.parameters.FadePercent = Mathf.Clamp(_003C_003Ec__DisplayClass48_.parameters.FadePercent, 0f, 0.5f);
			if (_003C_003Ec__DisplayClass48_.parameters.CustomTransform != null)
			{
				_003C_003Ec__DisplayClass48_1 _003C_003Ec__DisplayClass48_2 = new _003C_003Ec__DisplayClass48_1();
				_003C_003Ec__DisplayClass48_2.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass48_;
				_003C_003Ec__DisplayClass48_2.currentLineRenderer = ((activeLineRenderers.Count == 0 || !activeLineRenderers[activeLineRenderers.Count - 1].Empty) ? null : activeLineRenderers[activeLineRenderers.Count - 1]);
				if (_003C_003Ec__DisplayClass48_2.currentLineRenderer == null)
				{
					if (dependencies.ThreadState != null)
					{
						dependencies.ThreadState.AddActionForMainThread(_003C_003Ec__DisplayClass48_2._003CRenderLightningBolt_003Eb__1, waitForAction: true);
					}
					else
					{
						EnableCurrentLineRenderer();
						_003C_003Ec__DisplayClass48_2.currentLineRenderer = GetOrCreateLineRenderer();
					}
				}
				if (_003C_003Ec__DisplayClass48_2.currentLineRenderer == null)
				{
					return null;
				}
				_003C_003Ec__DisplayClass48_2.currentLineRenderer.CustomTransform = _003C_003Ec__DisplayClass48_2.CS_0024_003C_003E8__locals1.parameters.CustomTransform;
				result = _003C_003Ec__DisplayClass48_2.currentLineRenderer.Transform;
			}
			for (int i = _003C_003Ec__DisplayClass48_.startGroupIndex; i < endGroupIndex; i++)
			{
				LightningBoltSegmentGroup lightningBoltSegmentGroup2 = segmentGroups[i];
				lightningBoltSegmentGroup2.Delay = _003C_003Ec__DisplayClass48_.parameters.delaySeconds;
				lightningBoltSegmentGroup2.LifeTime = _003C_003Ec__DisplayClass48_.parameters.LifeTime;
				lightningBoltSegmentGroup2.PeakStart = lightningBoltSegmentGroup2.LifeTime * _003C_003Ec__DisplayClass48_.parameters.FadePercent;
				lightningBoltSegmentGroup2.PeakEnd = lightningBoltSegmentGroup2.LifeTime - lightningBoltSegmentGroup2.PeakStart;
				float num = lightningBoltSegmentGroup2.PeakEnd - lightningBoltSegmentGroup2.PeakStart;
				float num2 = lightningBoltSegmentGroup2.LifeTime - lightningBoltSegmentGroup2.PeakEnd;
				lightningBoltSegmentGroup2.PeakStart *= _003C_003Ec__DisplayClass48_.parameters.FadeInMultiplier;
				lightningBoltSegmentGroup2.PeakEnd = lightningBoltSegmentGroup2.PeakStart + num * _003C_003Ec__DisplayClass48_.parameters.FadeFullyLitMultiplier;
				lightningBoltSegmentGroup2.LifeTime = lightningBoltSegmentGroup2.PeakEnd + num2 * _003C_003Ec__DisplayClass48_.parameters.FadeOutMultiplier;
				lightningBoltSegmentGroup2.LightParameters = _003C_003Ec__DisplayClass48_.lp;
				RenderGroup(lightningBoltSegmentGroup2, _003C_003Ec__DisplayClass48_.parameters);
			}
			if (dependencies.ThreadState != null)
			{
				dependencies.ThreadState.AddActionForMainThread(_003C_003Ec__DisplayClass48_._003CRenderLightningBolt_003Eb__0);
			}
			else
			{
				RenderParticleSystems(_003C_003Ec__DisplayClass48_.start, _003C_003Ec__DisplayClass48_.end, _003C_003Ec__DisplayClass48_.parameters.TrunkWidth, _003C_003Ec__DisplayClass48_.parameters.LifeTime, _003C_003Ec__DisplayClass48_.parameters.delaySeconds);
				if (_003C_003Ec__DisplayClass48_.lp != null)
				{
					CreateLightsForGroup(segmentGroups[_003C_003Ec__DisplayClass48_.startGroupIndex], _003C_003Ec__DisplayClass48_.lp, _003C_003Ec__DisplayClass48_.quality, _003C_003Ec__DisplayClass48_.parameters.maxLights);
				}
			}
			return result;
		}

		private void CreateLightsForGroup(LightningBoltSegmentGroup group, LightningLightParameters lp, LightningBoltQualitySetting quality, int maxLights)
		{
			if (lightCount == MaximumLightCount || maxLights <= 0)
			{
				return;
			}
			float num = (lifeTime - group.PeakEnd) * lp.FadeOutMultiplier;
			float num2 = (group.PeakEnd - group.PeakStart) * lp.FadeFullyLitMultiplier;
			float num3 = group.PeakStart * lp.FadeInMultiplier + num2 + num;
			maxLifeTime = Mathf.Max(maxLifeTime, group.Delay + num3);
			segmentGroupsWithLight.Add(group);
			int segmentCount = group.SegmentCount;
			float num4;
			float num5;
			if (quality == LightningBoltQualitySetting.LimitToQualitySetting)
			{
				int qualityLevel = QualitySettings.GetQualityLevel();
				if (LightningBoltParameters.QualityMaximums.TryGetValue(qualityLevel, out LightningQualityMaximum value))
				{
					num4 = Mathf.Min(lp.LightPercent, value.MaximumLightPercent);
					num5 = Mathf.Min(lp.LightShadowPercent, value.MaximumShadowPercent);
				}
				else
				{
					UnityEngine.Debug.LogError("Unable to read lightning quality for level " + qualityLevel.ToString());
					num4 = lp.LightPercent;
					num5 = lp.LightShadowPercent;
				}
			}
			else
			{
				num4 = lp.LightPercent;
				num5 = lp.LightShadowPercent;
			}
			maxLights = Mathf.Max(1, Mathf.Min(maxLights, (int)((float)segmentCount * num4)));
			int num6 = Mathf.Max(1, segmentCount / maxLights);
			int num7 = maxLights - (int)((float)maxLights * num5);
			int nthShadowCounter = num7;
			for (int i = group.StartIndex + (int)((float)num6 * 0.5f); i < group.Segments.Count && !AddLightToGroup(group, lp, i, num6, num7, ref maxLights, ref nthShadowCounter); i += num6)
			{
			}
		}

		private bool AddLightToGroup(LightningBoltSegmentGroup group, LightningLightParameters lp, int segmentIndex, int nthLight, int nthShadows, ref int maxLights, ref int nthShadowCounter)
		{
			Light orCreateLight = GetOrCreateLight(lp);
			group.Lights.Add(orCreateLight);
			Vector3 vector = (group.Segments[segmentIndex].Start + group.Segments[segmentIndex].End) * 0.5f;
			if (dependencies.CameraIsOrthographic)
			{
				if (dependencies.CameraMode == CameraMode.OrthographicXZ)
				{
					vector.y = dependencies.CameraPos.y + lp.OrthographicOffset;
				}
				else
				{
					vector.z = dependencies.CameraPos.z + lp.OrthographicOffset;
				}
			}
			if (dependencies.UseWorldSpace)
			{
				orCreateLight.gameObject.transform.position = vector;
			}
			else
			{
				orCreateLight.gameObject.transform.localPosition = vector;
			}
			if (lp.LightShadowPercent == 0f || ++nthShadowCounter < nthShadows)
			{
				orCreateLight.shadows = LightShadows.None;
			}
			else
			{
				orCreateLight.shadows = LightShadows.Soft;
				nthShadowCounter = 0;
			}
			if (++lightCount != MaximumLightCount)
			{
				return --maxLights == 0;
			}
			return true;
		}

		private Light GetOrCreateLight(LightningLightParameters lp)
		{
			Light light;
			do
			{
				if (lightCache.Count == 0)
				{
					light = new GameObject("LightningBoltLight").AddComponent<Light>();
					light.type = LightType.Point;
					break;
				}
				light = lightCache[lightCache.Count - 1];
				lightCache.RemoveAt(lightCache.Count - 1);
			}
			while (light == null);
			light.bounceIntensity = lp.BounceIntensity;
			light.shadowNormalBias = lp.ShadowNormalBias;
			light.color = lp.LightColor;
			light.renderMode = lp.RenderMode;
			light.range = lp.LightRange;
			light.shadowStrength = lp.ShadowStrength;
			light.shadowBias = lp.ShadowBias;
			light.intensity = 0f;
			light.gameObject.transform.parent = dependencies.Parent.transform;
			light.gameObject.SetActive(value: true);
			dependencies.LightAdded(light);
			return light;
		}

		private void UpdateLight(LightningLightParameters lp, IEnumerable<Light> lights, float delay, float peakStart, float peakEnd, float lifeTime)
		{
			if (elapsedTime < delay)
			{
				return;
			}
			float num = (lifeTime - peakEnd) * lp.FadeOutMultiplier;
			float num2 = (peakEnd - peakStart) * lp.FadeFullyLitMultiplier;
			peakStart *= lp.FadeInMultiplier;
			peakEnd = peakStart + num2;
			lifeTime = peakEnd + num;
			float num3 = elapsedTime - delay;
			if (num3 >= peakStart)
			{
				if (num3 <= peakEnd)
				{
					foreach (Light light in lights)
					{
						light.intensity = lp.LightIntensity;
					}
					return;
				}
				float t = (num3 - peakEnd) / (lifeTime - peakEnd);
				foreach (Light light2 in lights)
				{
					light2.intensity = Mathf.Lerp(lp.LightIntensity, 0f, t);
				}
			}
			else
			{
				float t2 = num3 / peakStart;
				foreach (Light light3 in lights)
				{
					light3.intensity = Mathf.Lerp(0f, lp.LightIntensity, t2);
				}
			}
		}

		private void UpdateLights()
		{
			foreach (LightningBoltSegmentGroup item in segmentGroupsWithLight)
			{
				UpdateLight(item.LightParameters, item.Lights, item.Delay, item.PeakStart, item.PeakEnd, item.LifeTime);
			}
		}

		private IEnumerator GenerateParticleCoRoutine(ParticleSystem p, Vector3 pos, float delay)
		{
			return new _003CGenerateParticleCoRoutine_003Ed__54(0)
			{
				p = p,
				pos = pos,
				delay = delay
			};
		}

		private void CheckForGlow(IEnumerable<LightningBoltParameters> parameters)
		{
			foreach (LightningBoltParameters parameter in parameters)
			{
				HasGlow = (parameter.GlowIntensity >= Mathf.Epsilon && parameter.GlowWidthMultiplier >= Mathf.Epsilon);
				if (HasGlow)
				{
					break;
				}
			}
		}
	}
}
