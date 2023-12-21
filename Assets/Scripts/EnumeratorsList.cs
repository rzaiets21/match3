using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumeratorsList
{
	protected struct EnumeratorDesc
	{
		public IEnumerator enumerator;

		public float delay;

		public Action onStart;

		public Action interuptionHandler;

		public bool started;

		public bool useScaledTime;

		public EnumeratorDesc(IEnumerator enumerator, float delay, Action onStart, Action interuptionHandler, bool useScaledTime = false)
		{
			this.enumerator = enumerator;
			this.delay = delay;
			this.onStart = onStart;
			started = false;
			this.interuptionHandler = interuptionHandler;
			this.useScaledTime = useScaledTime;
		}
	}

	protected List<EnumeratorDesc> list = new List<EnumeratorDesc>();

	public EnumeratorsList Add(IEnumerator e, float delay = 0f, Action onStart = null, Action interuptionHandler = null, bool useScaledTime = false)
	{
		if (e == null && onStart == null)
		{
			return this;
		}
		list.Add(new EnumeratorDesc(e, delay, onStart, interuptionHandler, useScaledTime));
		return this;
	}

	public EnumeratorsList Clear()
	{
		list.Clear();
		return this;
	}

	public bool Update()
	{
		bool flag = false;
		for (int i = 0; i < list.Count; i++)
		{
			EnumeratorDesc enumeratorDesc = list[i];
			if (enumeratorDesc.delay > 0f)
			{
				if (enumeratorDesc.useScaledTime)
				{
					enumeratorDesc.delay -= Time.deltaTime;
				}
				else
				{
					enumeratorDesc.delay -= Time.deltaTime;
				}
				list[i] = enumeratorDesc;
				flag = true;
				continue;
			}
			if (!enumeratorDesc.started && enumeratorDesc.onStart != null)
			{
				enumeratorDesc.started = true;
				enumeratorDesc.onStart();
				list[i] = enumeratorDesc;
			}
			IEnumerator enumerator = enumeratorDesc.enumerator;
			bool flag2 = false;
			if (enumerator != null)
			{
				flag2 = enumerator.MoveNext();
			}
			flag |= flag2;
		}
		return flag;
	}
}
