using UnityEngine;

public class starFxController : MonoBehaviour
{
	public GameObject[] starFX;

	public int ea;

	public int currentEa;

	public float delay;

	public float currentDelay;

	public bool isEnd;

	public int idStar;

	public static starFxController myStarFxController;

	private void Awake()
	{
		myStarFxController = this;
	}

	private void Start()
	{
		Reset();
	}

	private void Update()
	{
		if (!isEnd)
		{
			currentDelay -= Time.deltaTime;
			if (currentDelay <= 0f)
			{
				if (currentEa != ea)
				{
					currentDelay = delay;
					starFX[currentEa].SetActive(value: true);
					currentEa++;
				}
				else
				{
					isEnd = true;
					currentDelay = delay;
					currentEa = 0;
				}
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
		{
			Reset();
		}
	}

	public void Reset()
	{
		for (int i = 0; i < 3; i++)
		{
			starFX[i].SetActive(value: false);
		}
		currentDelay = delay;
		currentEa = 0;
		isEnd = false;
		for (int j = 0; j < 3; j++)
		{
			starFX[j].SetActive(value: false);
		}
	}
}
