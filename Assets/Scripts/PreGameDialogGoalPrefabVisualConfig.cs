using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreGameDialogGoalPrefabVisualConfig : MonoBehaviour
{
	[SerializeField]
	private Image image;

	[SerializeField]
	private TextMeshProUGUI label;

	public void SetSprite(Sprite sprite)
	{
		GGUtil.SetSprite(image, sprite);
	}

	public void SetLabel(string text)
	{
		GGUtil.ChangeText(label, text);
	}
}
