using GGMatch3;
using UnityEngine;

public class LevelEditorGeneratorSetup : MonoBehaviour
{
	[SerializeField]
	private LevelEditorVisualizer.UIElementPool chipsPool = new LevelEditorVisualizer.UIElementPool();

	public float ChipHeight => chipsPool.prefabHeight;

	public void Init(GeneratorSetup generatorSetup, Vector3 startPosition)
	{
		float prefabHeight = chipsPool.prefabHeight;
		chipsPool.Clear(hideNotActive: false);
		for (int i = 0; i < generatorSetup.chips.Count; i++)
		{
			GeneratorSetup.GeneratorChipSetup chip = generatorSetup.chips[i];
			LevelEditorGeneratorSetupChip levelEditorGeneratorSetupChip = chipsPool.Next<LevelEditorGeneratorSetupChip>();
			levelEditorGeneratorSetupChip.Init(chip, generatorSetup);
			GGUtil.Show(levelEditorGeneratorSetupChip);
			Vector3 localPosition = startPosition + Vector3.up * i * prefabHeight;
			levelEditorGeneratorSetupChip.transform.localPosition = localPosition;
		}
		Vector3 localPosition2 = startPosition + Vector3.up * generatorSetup.chips.Count * prefabHeight;
		LevelEditorGeneratorSetupChip levelEditorGeneratorSetupChip2 = chipsPool.Next<LevelEditorGeneratorSetupChip>();
		levelEditorGeneratorSetupChip2.Init(null, generatorSetup);
		GGUtil.Show(levelEditorGeneratorSetupChip2);
		levelEditorGeneratorSetupChip2.transform.localPosition = localPosition2;
		chipsPool.HideNotActive();
	}
}
