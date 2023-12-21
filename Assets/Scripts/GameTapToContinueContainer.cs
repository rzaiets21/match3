using System;
using UnityEngine;

public class GameTapToContinueContainer : MonoBehaviour
{
	private Action onTapped;

	private bool _003CisTapped_003Ek__BackingField;

	public bool isTapped
	{
		get
		{
			return _003CisTapped_003Ek__BackingField;
		}
		protected set
		{
			_003CisTapped_003Ek__BackingField = value;
		}
	}

	public void Show(Action onTapped)
	{
		GGUtil.Show(this);
		isTapped = false;
		this.onTapped = onTapped;
	}

	public void Hide()
	{
		GGUtil.Hide(this);
	}

	public void ButtonCallback_OnTap()
	{
		isTapped = true;
		if (onTapped != null)
		{
			onTapped();
		}
	}
}
