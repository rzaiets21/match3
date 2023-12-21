using System;
using System.Linq;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
	[Serializable]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<GameObject, string> _003C_003E9__14_0;

		internal string _003CStart_003Eb__14_0(GameObject go)
		{
			return go.name;
		}
	}

	public GameObject[] particles;

	public int maxButtons = 10;

	public bool spawnOnAwake = true;

	public string removeTextFromButton;

	public string removeTextFromMaterialButton;

	public float autoChangeDelay;

	private int page;

	private int pages;

	private GameObject currentGO;

	private Color currentColor;

	private bool isPS;

	private bool _active = true;

	private int counter = -1;

	public GUIStyle bigStyle;

	public void Start()
	{
		particles = Enumerable.ToArray(Enumerable.OrderBy(particles, _003C_003Ec._003C_003E9._003CStart_003Eb__14_0));
		pages = (int)Mathf.Ceil((particles.Length - 1) / maxButtons);
		if (spawnOnAwake)
		{
			counter = 0;
			ReplaceGO(particles[counter]);
		}
		if (autoChangeDelay > 0f)
		{
			InvokeRepeating("NextModel", autoChangeDelay, autoChangeDelay);
		}
	}

	public void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
		{
			if (_active)
			{
				_active = false;
			}
			else
			{
				_active = true;
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
		{
			NextModel();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
		{
			counter--;
			if (counter < 0)
			{
				counter = particles.Length - 1;
			}
			ReplaceGO(particles[counter]);
		}
	}

	public void NextModel()
	{
		counter++;
		if (counter > particles.Length - 1)
		{
			counter = 0;
		}
		ReplaceGO(particles[counter]);
	}

	public void Duplicate()
	{
		UnityEngine.Object.Instantiate(currentGO, currentGO.transform.position, currentGO.transform.rotation);
	}

	public void DestroyAll()
	{
		ParticleSystem[] array = (ParticleSystem[])UnityEngine.Object.FindObjectsOfType(typeof(ParticleSystem));
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i].gameObject);
		}
	}

	public void OnGUI()
	{
		if (!_active)
		{
			return;
		}
		if (particles.Length > maxButtons)
		{
			if (GUI.Button(new Rect(20f, (maxButtons + 1) * 18, 75f, 18f), "Prev"))
			{
				if (page > 0)
				{
					page--;
				}
				else
				{
					page = pages;
				}
			}
			if (GUI.Button(new Rect(95f, (maxButtons + 1) * 18, 75f, 18f), "Next"))
			{
				if (page < pages)
				{
					page++;
				}
				else
				{
					page = 0;
				}
			}
			GUI.Label(new Rect(60f, (maxButtons + 2) * 18, 150f, 22f), "Page" + (page + 1) + " / " + (pages + 1));
		}
		if (GUI.Button(new Rect(20f, (maxButtons + 4) * 18, 150f, 18f), "Duplicate"))
		{
			Duplicate();
		}
		int num = particles.Length - page * maxButtons;
		if (num > maxButtons)
		{
			num = maxButtons;
		}
		for (int i = 0; i < num; i++)
		{
			string text = particles[i + page * maxButtons].transform.name;
			if (removeTextFromButton != "")
			{
				text = text.Replace(removeTextFromButton, "");
			}
			if (GUI.Button(new Rect(20f, i * 18 + 18, 150f, 18f), text))
			{
				DestroyAll();
				GameObject gameObject = currentGO = UnityEngine.Object.Instantiate(particles[i + page * maxButtons]);
				counter = i + page * maxButtons;
			}
		}
	}

	public void ReplaceGO(GameObject _go)
	{
		if (currentGO != null)
		{
			UnityEngine.Object.Destroy(currentGO);
		}
		GameObject gameObject = currentGO = UnityEngine.Object.Instantiate(_go);
	}

	public void PlayPS(ParticleSystem _ps, int _nr)
	{
		Time.timeScale = 1f;
		_ps.Play();
	}
}
