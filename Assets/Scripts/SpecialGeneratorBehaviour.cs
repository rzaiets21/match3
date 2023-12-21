using GGMatch3;
using System.Collections.Generic;
using UnityEngine;

public class SpecialGeneratorBehaviour : MonoBehaviour
{
	[SerializeField]
	private ComponentPool pool = new ComponentPool();

	[SerializeField]
	private float availableSize = 0.9f;

	public void Init(GeneratorSlotSettings generatorSlotSettings)
	{
		List<LevelDefinition.ChipGenerationSettings.ChipSetting> chipSettings = generatorSlotSettings.chipSettings;
		float num = availableSize;
		if (chipSettings.Count > 0)
		{
			num = availableSize / (float)chipSettings.Count;
		}
		float num2 = (0f - availableSize) * 0.5f;
		for (int i = 0; i < chipSettings.Count; i++)
		{
			LevelDefinition.ChipGenerationSettings.ChipSetting chipSetting = chipSettings[i];
			ChipDisplaySettings chipDisplaySettings = Match3Settings.instance.GetChipDisplaySettings(chipSetting.chipType, chipSetting.itemColor);
			if (chipDisplaySettings == null)
			{
				UnityEngine.Debug.Log("MISSING DISPLAY FOR " + chipSetting.chipType);
				continue;
			}
			GameObject gameObject = pool.Next(activate: true);
			gameObject.transform.localPosition = Vector3.zero;
			SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
			if (!(component == null))
			{
				Bounds bounds = chipDisplaySettings.displaySprite.bounds;
				float num3 = bounds.extents.x * 2f;
				Vector3 extent = bounds.extents;
				float d = 1f / num3 * num;
				gameObject.transform.localPosition = Vector3.right * (num2 + ((float)i + 0.5f) * num);
				gameObject.transform.localScale = Vector3.one * d;
				component.sprite = chipDisplaySettings.displaySprite;
			}
		}
	}
}
