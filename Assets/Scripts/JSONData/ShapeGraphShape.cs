using System;
using System.Collections.Generic;
using UnityEngine;

namespace JSONData
{
	[Serializable]
	public class ShapeGraphShape
	{
		public enum Orientation
		{
			CCW,
			CW
		}

		public List<Vector2> points = new List<Vector2>();

		public Orientation GetOrientation()
		{
			if (GGUtil.SignedArea(points) < 0f)
			{
				return Orientation.CCW;
			}
			return Orientation.CW;
		}

		public bool IsInside(Vector2 pos)
		{
			int num = 0;
			for (int i = 0; i < points.Count; i++)
			{
				Vector2 b = points[i];
				Vector2 normalized = (((i == points.Count - 1) ? points[0] : points[i + 1]) - b).normalized;
				int num2 = Mathf.RoundToInt(Mathf.Sign(Vector3.Cross(rhs: (pos - b).normalized, lhs: normalized).z));
				if (num != 0 && num != num2)
				{
					return false;
				}
				num = num2;
			}
			return true;
		}
	}
}
