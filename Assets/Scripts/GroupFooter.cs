using TMPro;
using UnityEngine;

public class GroupFooter : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI label;

	[SerializeField]
	private bool enableFooter;

	public void Init(DecoratingScene scene)
	{
		GGUtil.Hide(this);
		if (enableFooter)
		{
			DecoratingScene.GroupDefinition groupDefinition = scene.CurrentGroup();
			if (groupDefinition != null && !string.IsNullOrWhiteSpace(groupDefinition.title))
			{
				GGUtil.Show(this);
				label.text = groupDefinition.title;
			}
		}
	}
}
