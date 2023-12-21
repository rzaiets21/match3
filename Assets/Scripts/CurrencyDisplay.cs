using TMPro;
using UnityEngine;

public class CurrencyDisplay : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI countLabel;

	[SerializeField]
	private Transform currencySymbol;

	[SerializeField]
	public CurrencyType currencyType;

	[SerializeField]
	private UIGGParticleCreator particleCreator = new UIGGParticleCreator();

	public void Init(long count)
	{
		particleCreator.DestroyCreatedObjects();
		GGUtil.ChangeText(countLabel, count);
	}

	public Transform GetTransform()
	{
		return countLabel.transform;
	}

	public void DisplayCount(long count)
	{
		GGUtil.ChangeText(countLabel, count);
	}

	public void ShowShineParticle()
	{
		particleCreator.CreateAndRunParticles("Shine", currencySymbol);
	}
}
