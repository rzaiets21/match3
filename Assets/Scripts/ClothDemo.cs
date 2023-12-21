using GGCloth;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ClothDemo : MonoBehaviour
{
	public enum AttachPos
	{
		Bottom,
		Center,
		BottomCenter,
		Top,
		WholeCenter
	}

	[SerializeField]
	private bool useLocalPosition;

	[SerializeField]
	private bool isSleeping;

	[SerializeField]
	public bool useMaxDifferenceMagnitude;

	[SerializeField]
	private float maxDifferenceMagnitudeLocal = 0.25f;

	[SerializeField]
	private float maxTimeBeforeSleep = 4f;

	[SerializeField]
	private bool sleepAfterTime;

	[SerializeField]
	private AttachPos attachPos;

	[SerializeField]
	private Transform centralTarget;

	[SerializeField]
	private Transform localPositionTarget;

	[SerializeField]
	public bool drawGrid;

	[SerializeField]
	public float scaleOutBy = 1f;

	[SerializeField]
	public float stiffnessRandom = 0.05f;

	[SerializeField]
	public Vector3 moveBy;

	[SerializeField]
	private int constraintRelaxationSteps = 1;

	[NonSerialized]
	public SquareCloth cloth = new SquareCloth();

	[SerializeField]
	private SquareClothRenderer clothRenderer;

	[SerializeField]
	private Vector3 size = new Vector3(1f, 1f, 0f);

	[SerializeField]
	public Vector3 gravity = Vector3.down * 9.81f;

	[SerializeField]
	private float damping = 0.5f;

	[SerializeField]
	private float stiffness = 0.5f;

	[SerializeField]
	private int rowCount = 4;

	[SerializeField]
	private int columnCount = 4;

	[SerializeField]
	private float minMoveDistance = 0.001f;

	[SerializeField]
	private float row;

	[SerializeField]
	private float column;

	[SerializeField]
	public float impulseMult = 0.2f;

	public bool directlyFollow;

	public bool debugOut;

	public float boardScale = 1f;

	private float lastMoveTime = -100f;

	private MultiPointAttachConstraint positionConstraint = new MultiPointAttachConstraint();

	private float maxDifferenceMagnitude => maxDifferenceMagnitudeLocal * boardScale;

	public Vector3 centralTargetPosition
	{
		get
		{
			if (useLocalPosition)
			{
				return localPositionTarget.localPosition;
			}
			return centralTarget.position;
		}
	}

	private int GetPointIndex(int column, int row, int columnCount)
	{
		return column + row * (columnCount + 1);
	}

	private void Start()
	{
		Init();
	}

	private void LateUpdate()
	{
		Vector3 vector = centralTargetPosition - positionConstraint.centralPosition;
		vector.z = 0f;
		if (sleepAfterTime)
		{
			if (vector.x > minMoveDistance || vector.y > minMoveDistance)
			{
				lastMoveTime = Time.time;
			}
			float num = Time.time - lastMoveTime;
			if (sleepAfterTime && num > maxTimeBeforeSleep)
			{
				isSleeping = true;
				return;
			}
			isSleeping = false;
		}
		if (directlyFollow)
		{
			List<PointMass> points = cloth.pointWorld.Points;
			for (int i = 0; i < points.Count; i++)
			{
				PointMass pointMass = points[i];
				pointMass.currentPosition += vector;
				pointMass.SetRestingPostion(pointMass.currentPosition);
			}
		}
		else if (useMaxDifferenceMagnitude)
		{
			vector.z = 0f;
			float magnitude = vector.magnitude;
			if (magnitude > maxDifferenceMagnitude)
			{
				Vector3 vector2 = vector.normalized * (magnitude - maxDifferenceMagnitude);
				List<PointMass> points2 = cloth.pointWorld.Points;
				for (int j = 0; j < points2.Count; j++)
				{
					PointMass pointMass2 = points2[j];
					pointMass2.currentPosition += vector2;
					pointMass2.previosPosition += vector2;
				}
			}
		}
		if (centralTarget != null)
		{
			positionConstraint.centralPosition = centralTargetPosition;
		}
		cloth.pointWorld.SetGravity(gravity);
		cloth.pointWorld.constraintRelaxationSteps = Mathf.Max(1, constraintRelaxationSteps);
		cloth.pointWorld.Step(Time.deltaTime);
		UpdateMaterialSettings();
		clothRenderer.DoUpdateMesh();
	}

	public void UpdateMaterialSettings()
	{
		if (Application.isEditor)
		{
			clothRenderer.UpdateMaterialSettings();
		}
	}

	public void Init()
	{
		cloth.isWorldPosition = !useLocalPosition;
		if (useLocalPosition)
		{
			cloth.localPositionTransform = localPositionTarget;
		}
		cloth.stiffnessRandom = stiffnessRandom;
		cloth.Init(columnCount, rowCount, size, damping, stiffness, centralTargetPosition);
		cloth.pointWorld.SetGravity(gravity);
		int num = cloth.columnCount;
		if (centralTarget != null)
		{
			positionConstraint.Init(centralTargetPosition);
			int num2 = columnCount / 2;
			int num3 = columnCount % 2 + 1;
			int num4 = rowCount / 2;
			int num5 = rowCount % 2 + 1;
			if (attachPos == AttachPos.Bottom)
			{
				for (int i = 0; i <= num; i++)
				{
					PointMass point = cloth.pointWorld.GetPoint(cloth.GetPointIndex(i, 0));
					positionConstraint.FixPoint(point);
				}
			}
			else if (attachPos == AttachPos.Center)
			{
				for (int j = 0; j < num3; j++)
				{
					for (int k = 0; k < num5; k++)
					{
						PointMass point2 = cloth.pointWorld.GetPoint(cloth.GetPointIndex(num2 + j, num4 + k));
						positionConstraint.FixPoint(point2);
					}
				}
			}
			else if (attachPos == AttachPos.BottomCenter)
			{
				for (int l = 0; l < num3; l++)
				{
					for (int m = 0; m < num5; m++)
					{
						PointMass point3 = cloth.pointWorld.GetPoint(cloth.GetPointIndex(num2 + l, 0));
						positionConstraint.FixPoint(point3);
					}
				}
			}
			else if (attachPos == AttachPos.Top)
			{
				PointMass point4 = cloth.pointWorld.GetPoint(cloth.GetPointIndex(0, columnCount));
				positionConstraint.FixPoint(point4);
				point4 = cloth.pointWorld.GetPoint(cloth.GetPointIndex(rowCount, columnCount));
				positionConstraint.FixPoint(point4);
			}
			else if (attachPos == AttachPos.WholeCenter)
			{
				for (int n = 0; n <= num; n++)
				{
					for (int num6 = 0; num6 < num5; num6++)
					{
						PointMass point5 = cloth.pointWorld.GetPoint(cloth.GetPointIndex(n, num4 + num6));
						positionConstraint.FixPoint(point5);
					}
				}
			}
			cloth.pointWorld.Prepend(positionConstraint);
		}
		clothRenderer.SetCloth(cloth);
		clothRenderer.DoUpdateMesh();
	}

	public void ScaleOutBy(float scaleOut)
	{
		List<PointMass> points = cloth.pointWorld.Points;
		for (int i = 0; i < points.Count; i++)
		{
			points[i].currentPosition *= scaleOut;
		}
	}

	public void MoveBy(Vector3 offset)
	{
		List<PointMass> points = cloth.pointWorld.Points;
		int pointIndex = cloth.GetPointIndex((int)Mathf.Lerp(0f, cloth.columnCount, column), (int)Mathf.Lerp(0f, cloth.rowCount, row));
		points[pointIndex].currentPosition += offset;
	}

	public void MoveAllBy(Vector3 offset)
	{
		List<PointMass> points = cloth.pointWorld.Points;
		for (int i = 0; i < points.Count; i++)
		{
			points[i].currentPosition += offset;
		}
	}
}
