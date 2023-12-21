using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public delegate void OnClickDelegate(Vector2 screenPosition);

	public class PointerData
	{
		public int pointerId;

		public Vector2 position;

		public bool isDown;

		public int downFrame;

		public Vector2 startPosition;

		public float downTime;
	}

	[SerializeField]
	private int minDistance = 6;

	[SerializeField]
	private float minTime = 0.3f;

	[SerializeField]
	private int minFrames = 3;

	private List<PointerData> inputPointers = new List<PointerData>();

	public bool IsAnyDown
	{
		get
		{
			for (int i = 0; i < inputPointers.Count; i++)
			{
				if (inputPointers[i].isDown)
				{
					return true;
				}
			}
			return false;
		}
	}

	public event OnClickDelegate onClick;

	public void Clear()
	{
		for (int i = 0; i < inputPointers.Count; i++)
		{
			inputPointers[i].isDown = false;
		}
	}

	public bool IsDown(int pointerId)
	{
		return GetPointerData(pointerId)?.isDown ?? false;
	}

	public Vector2 Position(int pointerId)
	{
		return GetPointerData(pointerId)?.position ?? Vector2.zero;
	}

	public PointerData FirstDownPointer()
	{
		for (int i = 0; i < inputPointers.Count; i++)
		{
			PointerData pointerData = inputPointers[i];
			if (pointerData.isDown)
			{
				return pointerData;
			}
		}
		return null;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		UpdateMousePosition(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		UpdateMousePosition(eventData);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		UpdateMousePosition(eventData);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		int pointerId = eventData.pointerId;
		PointerData orCreatePointerData = GetOrCreatePointerData(pointerId);
		orCreatePointerData.downFrame = Time.frameCount;
		orCreatePointerData.isDown = true;
		orCreatePointerData.position = eventData.position;
		orCreatePointerData.startPosition = orCreatePointerData.position;
		orCreatePointerData.downTime = Time.unscaledTime;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		int pointerId = eventData.pointerId;
		PointerData orCreatePointerData = GetOrCreatePointerData(pointerId);
		orCreatePointerData.isDown = false;
		orCreatePointerData.position = eventData.position;
		float num = Vector2.Distance(orCreatePointerData.startPosition, orCreatePointerData.position);
		float num2 = Time.unscaledTime - orCreatePointerData.downTime;
		int num3 = Time.frameCount - orCreatePointerData.downFrame;
		bool num4 = num2 < minTime || num3 < minFrames;
		bool flag = num <= (float)minDistance;
		if ((num4 & flag) && this.onClick != null)
		{
			this.onClick(orCreatePointerData.position);
		}
	}

	private void UpdateMousePosition(PointerEventData eventData)
	{
		int pointerId = eventData.pointerId;
		GetOrCreatePointerData(pointerId).position = eventData.position;
	}

	private PointerData GetOrCreatePointerData(int pointerId)
	{
		PointerData pointerData = GetPointerData(pointerId);
		if (pointerData != null)
		{
			return pointerData;
		}
		return AddPointerData(pointerId);
	}

	private PointerData GetPointerData(int pointerId)
	{
		for (int i = 0; i < inputPointers.Count; i++)
		{
			PointerData pointerData = inputPointers[i];
			if (pointerData.pointerId == pointerId)
			{
				return pointerData;
			}
		}
		return null;
	}

	private PointerData AddPointerData(int pointerId)
	{
		PointerData pointerData = new PointerData();
		pointerData.pointerId = pointerId;
		inputPointers.Add(pointerData);
		return pointerData;
	}
}
