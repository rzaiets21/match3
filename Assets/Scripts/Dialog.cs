using GGMatch3;
using System;
using TMPro;
using UnityEngine;

public class Dialog : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI yesLabel;

	[SerializeField]
	private TextMeshProUGUI noLabel;

	[SerializeField]
	private TextMeshProUGUI okLabel;

	[SerializeField]
	private TextMeshProUGUI textLabel;

	[SerializeField]
	private Transform[] widgetsToHide;

	[SerializeField]
	private VisualStyleSet yesNoStyle = new VisualStyleSet();

	[SerializeField]
	private VisualStyleSet okStyle = new VisualStyleSet();

	private Action<bool> onComplete;

	public static Dialog instance => NavigationManager.instance.GetObject<Dialog>();

	public void Show(string text, string yes, string no, Action<bool> onComplete)
	{
		InitYesNo(text, yes, no, onComplete);
		NavigationManager.instance.Push(base.gameObject, isModal: true);
	}

	public void Show(string text, string ok, Action<bool> onComplete)
	{
		InitOk(text, ok, onComplete);
		NavigationManager.instance.Push(base.gameObject, isModal: true);
	}

	private void InitYesNo(string text, string yes, string no, Action<bool> onComplete)
	{
		GGUtil.Hide(widgetsToHide);
		yesNoStyle.Apply();
		GGUtil.ChangeText(yesLabel, yes);
		GGUtil.ChangeText(noLabel, no);
		GGUtil.ChangeText(textLabel, text);
		this.onComplete = onComplete;
	}

	private void InitOk(string text, string ok, Action<bool> onComplete)
	{
		GGUtil.Hide(widgetsToHide);
		okStyle.Apply();
		GGUtil.ChangeText(okLabel, ok);
		GGUtil.ChangeText(textLabel, text);
		this.onComplete = onComplete;
	}

	private void Finish(bool success)
	{
		if (onComplete == null)
		{
			NavigationManager.instance.Pop();
		}
		else
		{
			onComplete(success);
		}
	}

	public void ButtonCallback_OnYes()
	{
		Finish(success: true);
	}

	public void ButtonCallback_OnNo()
	{
		Finish(success: false);
	}
}
