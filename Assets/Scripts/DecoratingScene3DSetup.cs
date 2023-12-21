using JSONData;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DecoratingScene3DSetup : MonoBehaviour
{
	[Serializable]
	public class PivotConfig
	{
		[Serializable]
		public class Pivot
		{
			public string name;

			public Vector3 screenPosition;
		}

		public List<Pivot> pivots = new List<Pivot>();
	}

	[Serializable]
	public class DashboxConfig
	{
		public List<ShapeGraphShape> shapes = new List<ShapeGraphShape>();
	}

	[Serializable]
	public class NamedCollisionConfig
	{
		public string name;

		public CollisionConfig collisionConfig = new CollisionConfig();
	}

	[Serializable]
	public class CollisionConfig
	{
		[Serializable]
		public class Triangle
		{
			public float distanceFromCamera;

			public Vector3 p1;

			public Vector3 p2;

			public Vector3 p3;
		}

		public List<Triangle> triangles = new List<Triangle>();
	}

	[Serializable]
	public class VisualObject
	{
		public string name;

		public Transform rootTransform;

		public Transform collisionRoot;

		public Transform dashboxRoot;

		public CollisionConfig collisionConfig = new CollisionConfig();

		public DashboxConfig dashboxConfig = new DashboxConfig();

		public PivotConfig pivotConfig = new PivotConfig();

		public List<NamedCollisionConfig> namedCollisionConfigs = new List<NamedCollisionConfig>();
	}

	public int selectedObject;

	[SerializeField]
	public List<VisualObject> visualObjectList = new List<VisualObject>();

	public VisualObject GetForName(string name)
	{
		List<VisualObject> list = visualObjectList;
		for (int i = 0; i < list.Count; i++)
		{
			VisualObject visualObject = list[i];
			if (visualObject.name.ToLower() == name.ToLower())
			{
				return visualObject;
			}
		}
		return null;
	}

	public void Init()
	{
		Transform transform = base.transform;
		visualObjectList.Clear();
		foreach (Transform item in transform)
		{
			if (!item.name.ToLower().Contains("camera"))
			{
				VisualObject visualObject = new VisualObject();
				visualObject.name = item.name;
				visualObject.rootTransform = item;
				FillVisualObject(visualObject, item);
				visualObjectList.Add(visualObject);
			}
		}
	}

	private void FillVisualObject(VisualObject visualObject, Transform rootTransform)
	{
		foreach (Transform item in rootTransform)
		{
			string text = item.name.ToLower();
			if (text.StartsWith("data", StringComparison.Ordinal))
			{
				FillVisualObject(visualObject, item);
			}
			if (text.EndsWith("_collision", StringComparison.Ordinal))
			{
				visualObject.collisionRoot = item;
				FillCollisionConfig(visualObject, visualObject.collisionConfig, item, null);
			}
			if (text.EndsWith("_dashbox", StringComparison.Ordinal))
			{
				visualObject.dashboxRoot = item;
				FillDashboxConfig(visualObject.dashboxConfig, item);
			}
			if (text.EndsWith("_pivot", StringComparison.Ordinal))
			{
				FillPivotConfig(visualObject.pivotConfig, item);
			}
		}
	}

	private void FillPivotConfig(PivotConfig pivotConfig, Transform root)
	{
		Camera camera = FindMainCamera();
		Vector3 forward = camera.transform.forward;
		Vector3 position2 = camera.transform.position;
		foreach (Transform item in root)
		{
			PivotConfig.Pivot pivot = new PivotConfig.Pivot();
			pivot.name = item.name;
			Vector3 position = item.position;
			Vector3 vector = camera.WorldToScreenPoint(position);
			if (vector.z < 0f)
			{
				vector.y *= -1f;
			}
			pivot.screenPosition = vector;
			pivotConfig.pivots.Add(pivot);
		}
	}

	private void FillDashboxConfig(DashboxConfig dashboxConfig, Transform root)
	{
		MeshFilter component = root.GetComponent<MeshFilter>();
		Mesh mesh = null;
		if (component != null)
		{
			mesh = component.sharedMesh;
		}
		if (mesh != null)
		{
			FillMesh(dashboxConfig, root, mesh);
		}
		foreach (Transform item in root)
		{
			FillDashboxConfig(dashboxConfig, item);
		}
	}

	private void FillMesh(DashboxConfig dashboxConfig, Transform transform, Mesh mesh)
	{
		int[] triangle = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		Camera camera = FindMainCamera();
		ShapeGraphShape shapeGraphShape = new ShapeGraphShape();
		dashboxConfig.shapes.Add(shapeGraphShape);
		Vector3 forward = camera.transform.forward;
		Vector3 position3 = camera.transform.position;
		foreach (Vector3 position in vertices)
		{
			Vector3 position2 = transform.TransformPoint(position);
			Vector3 vector = camera.WorldToScreenPoint(position2);
			if (vector.z < 0f)
			{
				vector.y *= -1f;
			}
			shapeGraphShape.points.Add(new Vector2(vector.x, vector.y));
		}
	}

	private void FillCollisionConfig(VisualObject visualObject, CollisionConfig collisionConfig, Transform root, NamedCollisionConfig namedCollision)
	{
		MeshFilter component = root.GetComponent<MeshFilter>();
		Mesh mesh = null;
		if (component != null)
		{
			mesh = component.sharedMesh;
		}
		if (mesh != null)
		{
			FillMesh(visualObject, collisionConfig, root, mesh);
			if (namedCollision != null)
			{
				FillMesh(visualObject, namedCollision.collisionConfig, root, mesh);
			}
		}
		foreach (Transform item in root)
		{
			string name = item.name;
			NamedCollisionConfig namedCollisionConfig = namedCollision;
			if (namedCollisionConfig == null)
			{
				namedCollisionConfig = new NamedCollisionConfig();
				visualObject.namedCollisionConfigs.Add(namedCollisionConfig);
				namedCollisionConfig.name = name;
			}
			FillCollisionConfig(visualObject, collisionConfig, item, namedCollisionConfig);
		}
	}

	public Camera FindMainCamera()
	{
		Camera[] componentsInChildren = base.transform.GetComponentsInChildren<Camera>(includeInactive: true);
		if (componentsInChildren == null || componentsInChildren.Length == 0)
		{
			return null;
		}
		return componentsInChildren[0];
	}

	private void FillMesh(VisualObject visualObject, CollisionConfig collisionConfig, Transform transform, Mesh mesh)
	{
		int[] triangles = mesh.triangles;
		Vector3[] normal = mesh.normals;
		Vector3[] vertices = mesh.vertices;
		Camera camera = FindMainCamera();
		Vector3 forward = camera.transform.forward;
		Vector3 position = camera.transform.position;
		for (int i = 0; i < triangles.Length / 3; i++)
		{
			int num = i * 3;
			int num2 = triangles[num];
			int num3 = triangles[num + 1];
			int num4 = triangles[num + 2];
			CollisionConfig.Triangle triangle = new CollisionConfig.Triangle();
			Vector3 position2 = vertices[num2];
			Vector3 position3 = vertices[num3];
			Vector3 position4 = vertices[num4];
			Vector3 vector = transform.TransformPoint(position2);
			Vector3 vector2 = transform.TransformPoint(position3);
			Vector3 vector3 = transform.TransformPoint(position4);
			Vector3 vector4 = camera.WorldToScreenPoint(vector);
			Vector3 vector5 = camera.WorldToScreenPoint(vector2);
			Vector3 vector6 = camera.WorldToScreenPoint(vector3);
			if (vector4.z < 0f)
			{
				vector4.y *= -1f;
			}
			if (vector5.z < 0f)
			{
				vector5.y *= -1f;
			}
			if (vector6.z < 0f)
			{
				vector6.y *= -1f;
			}
			vector4.z = Vector3.Dot(vector - position, forward);
			vector5.z = Vector3.Dot(vector2 - position, forward);
			vector6.z = Vector3.Dot(vector3 - position, forward);
			triangle.p1 = vector4;
			triangle.p2 = vector5;
			triangle.p3 = vector6;
			Vector3 lhs = vector2 - vector;
			Vector3 rhs = vector3 - vector;
			Vector3 vector7 = Vector3.Cross(lhs, rhs);
			if (!(vector7 == Vector3.zero))
			{
				Vector3 rhs2 = vector7;
				if (Vector3.Dot(vector - position, rhs2) <= 0f)
				{
					float num5 = triangle.distanceFromCamera = Vector3.Dot((vector + vector2 + vector3) / 3f - position, forward);
					collisionConfig.triangles.Add(triangle);
				}
			}
		}
	}
}
