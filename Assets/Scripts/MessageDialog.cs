using GGMatch3;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageDialog : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI headerLabel;

	[SerializeField]
	private TextMeshProUGUI textLabel;

	[SerializeField]
	private TextMeshProUGUI okLabel;

	[SerializeField]
	private TextMeshProUGUI yesLabel;

	[SerializeField]
	private TextMeshProUGUI noLabel;

	[SerializeField]
	private List<RectTransform> widgetsToHide = new List<RectTransform>();

	[SerializeField]
	private VisualStyleSet okStyle = new VisualStyleSet();

	[SerializeField]
	private VisualStyleSet yesNoStyle = new VisualStyleSet();

	private Action<bool> onComplete;

	public static MessageDialog instance => NavigationManager.instance.GetObject<MessageDialog>();

	private static void SetLabel(TextMeshProUGUI label, string text)
	{
		if (!(label == null) && !(label.text == text))
		{
			label.text = text;
		}
	}

	public void ShowOk(string header, string text, string ok, Action<bool> onComplete)
	{
		GGUtil.SetActive(widgetsToHide, active: false);
		SetLabel(headerLabel, header);
		SetLabel(textLabel, text);
		SetLabel(okLabel, ok);
		okStyle.Apply();
		this.onComplete = onComplete;
		ShowOnNav();
	}

	public void ShowYesNo(string header, string text, string yes, string no, Action<bool> onComplete)
	{
		GGUtil.SetActive(widgetsToHide, active: false);
		SetLabel(headerLabel, header);
		SetLabel(textLabel, text);
		SetLabel(yesLabel, yes);
		SetLabel(noLabel, no);
		yesNoStyle.Apply();
		this.onComplete = onComplete;
		ShowOnNav();
	}

	private void ShowOnNav()
	{
		NavigationManager.instance.Push(base.gameObject, isModal: true);
	}

	private void OnComplete(bool isYes)
	{
		if (onComplete == null)
		{
			NavigationManager.instance.Pop();
		}
		else
		{
			onComplete(isYes);
		}
	}

	public void Callback_OnOk()
	{
		OnComplete(isYes: true);
	}

	public void Callback_OnYes()
	{
		OnComplete(isYes: true);
	}

	public void Callback_OnNo()
	{
		OnComplete(isYes: false);
	}
}
