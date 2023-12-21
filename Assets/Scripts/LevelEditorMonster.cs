using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorMonster : MonoBehaviour
{
	[Serializable]
	public class ElementDesc
	{
		public IntVector2 size;

		public Image image;
	}

	[SerializeField]
	private List<ElementDesc> elements = new List<ElementDesc>();

	public void Init(LevelEditorVisualizer viz, LevelDefinition level, LevelDefinition.MonsterElement monsterElement)
	{
		for (int i = 0; i < elements.Count; i++)
		{
			ElementDesc elementDesc = elements[i];
			GGUtil.SetActive(elementDesc.image, elementDesc.size == monsterElement.size);
		}
		Vector3 localPosition = viz.GetLocalPosition(level, monsterElement.position);
		Vector3 localPosition2 = viz.GetLocalPosition(level, monsterElement.oppositeCornerPosition);
		Vector3 localPosition3 = Vector3.Lerp(localPosition, localPosition2, 0.5f);
		base.transform.localPosition = localPosition3;
		Match3Settings.MonsterColorSettings monsterColorSettings = Match3Settings.instance.GeMonsterColorSettings(monsterElement.itemColor);
		if (monsterColorSettings != null)
		{
			for (int j = 0; j < elements.Count; j++)
			{
				elements[j].image.material = monsterColorSettings.material;
			}
		}
	}
}
