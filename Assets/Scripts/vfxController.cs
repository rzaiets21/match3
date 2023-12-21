using UnityEngine;

public class vfxController : MonoBehaviour
{
	public GameObject[] starFx01Prefabs;

	public GameObject[] starFx02Prefabs;

	public GameObject[] starFx03Prefabs;

	public GameObject[] starFx04Prefabs;

	public GameObject[] starFx05Prefabs;

	public GameObject[] DesStarFxObjs;

	public GameObject[] bgFxPrefabs;

	public int currentStarImage;

	public int currentStarFx;

	public int currentLevel;

	public int currentBgFx;

	private void Start()
	{
		currentStarImage = 0;
		currentStarFx = 0;
		currentLevel = 3;
		currentBgFx = 1;
	}

	public void ChangedStarImage(int i)
	{
		currentStarImage = i;
		PlayStarFX();
	}

	public void ChangedStarFX(int i)
	{
		currentStarFx = i;
		PlayStarFX();
	}

	public void ChangedLevel(int i)
	{
		currentLevel = i;
		PlayStarFX();
	}

	public void ChangedBgFx(int i)
	{
		currentBgFx = i;
		PlayStarFX();
	}

	public void PlayStarFX()
	{
		DesStarFxObjs = GameObject.FindGameObjectsWithTag("Effects");
		GameObject[] desStarFxObjs = DesStarFxObjs;
		for (int i = 0; i < desStarFxObjs.Length; i++)
		{
			UnityEngine.Object.Destroy(desStarFxObjs[i].gameObject);
		}
		if (currentBgFx != 0)
		{
			Object.Instantiate(bgFxPrefabs[currentBgFx]);
		}
		switch (currentStarImage)
		{
		case 0:
			Object.Instantiate(starFx01Prefabs[currentStarFx]);
			starFxController.myStarFxController.ea = currentLevel;
			break;
		case 1:
			Object.Instantiate(starFx02Prefabs[currentStarFx]);
			starFxController.myStarFxController.ea = currentLevel;
			break;
		case 2:
			Object.Instantiate(starFx03Prefabs[currentStarFx]);
			starFxController.myStarFxController.ea = currentLevel;
			break;
		case 3:
			Object.Instantiate(starFx04Prefabs[currentStarFx]);
			starFxController.myStarFxController.ea = currentLevel;
			break;
		case 4:
			Object.Instantiate(starFx05Prefabs[currentStarFx]);
			starFxController.myStarFxController.ea = currentLevel;
			break;
		}
	}
}
