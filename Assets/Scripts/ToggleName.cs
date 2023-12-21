using UnityEngine;

public class ToggleName : MonoBehaviour
{
	public GameObject[] nameEffectText;

	private void Start()
	{
		for (int i = 0; i < 4; i++)
		{
			nameEffectText[i].SetActive(value: true);
		}
	}

	public void ToggleChanged(bool isHide)
	{
		for (int i = 0; i < 4; i++)
		{
			nameEffectText[i].SetActive(!isHide);
		}
	}
}
