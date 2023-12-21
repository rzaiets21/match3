using UnityEngine;
using UnityEngine.UI;

public class ManualScripts : MonoBehaviour
{
	public GameObject[] PageObj;

	public int currentPage;

	public int minPage = 1;

	public int maxPage = 5;

	public Text txtPage;

	private void Start()
	{
		for (int i = 0; i <= maxPage; i++)
		{
			PageObj[i].SetActive(value: false);
		}
		PageObj[minPage].SetActive(value: true);
		currentPage = minPage;
		txtPage.text = "PAGE " + currentPage + " / " + maxPage;
	}

	public void ChangedPage(int i)
	{
		currentPage = Mathf.Clamp(currentPage + i, minPage, maxPage);
		for (int j = 0; j <= maxPage; j++)
		{
			PageObj[j].SetActive(value: false);
		}
		PageObj[currentPage].SetActive(value: true);
		txtPage.text = "PAGE " + currentPage + " / " + maxPage;
	}
}
