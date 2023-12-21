using GGMatch3;
using TMPro;
using UnityEngine;

public class MonsterElementBehaviour : MonoBehaviour
{
	[SerializeField]
	private Transform scalerTransform;

	[SerializeField]
	private SpriteRenderer monsterSprite;

	[SerializeField]
	private TextMeshPro label;

	[SerializeField]
	private Animator monsterAnimator;

	public void Init(LevelDefinition.MonsterElement monsterElement)
	{
		GGUtil.SetScale(scalerTransform, new Vector3(monsterElement.size.x, monsterElement.size.y, 1f));
		Match3Settings.MonsterColorSettings monsterColorSettings = Match3Settings.instance.GeMonsterColorSettings(monsterElement.itemColor);
		if (monsterColorSettings != null)
		{
			monsterSprite.material = monsterColorSettings.material;
		}
	}

	public void SetCount(int countRemaining)
	{
		if (!(label == null))
		{
			GGUtil.SetActive(label.transform, countRemaining > 0);
			label.text = countRemaining.ToString();
		}
	}

	public void DoEatAnimation()
	{
		if (!(monsterAnimator == null))
		{
			monsterAnimator.SetTrigger("eat");
		}
	}
}
