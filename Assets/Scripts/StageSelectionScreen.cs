using GGMatch3;
using TMPro;
using UnityEngine;

public class StageSelectionScreen : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI stageLabel;

	[SerializeField]
	private string stageNameFormat;

	public void Show()
	{
		NavigationManager.instance.Push(base.gameObject);
	}

	public void Init()
	{
		Match3StagesDB.Stage currentStage = Match3StagesDB.instance.currentStage;
		int index = currentStage.index;
		string levelName = currentStage.levelReference.levelName;
		string levelDBName = currentStage.levelReference.levelDBName;
		stageLabel.text = string.Format(stageNameFormat, index, levelName, levelDBName);
	}

	public void OnEnable()
	{
		Init();
	}

	public void ButtonCallback_OnLeft()
	{
		int stagesPassed = Match3StagesDB.instance.stagesPassed;
		stagesPassed--;
		if (stagesPassed < 0)
		{
			stagesPassed = Match3StagesDB.instance.stages.Count - 1;
		}
		Match3StagesDB.instance.stagesPassed = Mathf.Clamp(stagesPassed, 0, Match3StagesDB.instance.stages.Count - 1);
		Init();
	}

	public void ButtonCallback_OnRight()
	{
		Match3StagesDB.instance.stagesPassed++;
		Match3StagesDB.instance.stagesPassed = Mathf.Clamp(Match3StagesDB.instance.stagesPassed, 0, Match3StagesDB.instance.stages.Count - 1);
		Init();
	}

	public void ButtonCallback_OnBack()
	{
		NavigationManager.instance.Pop();
	}
}
