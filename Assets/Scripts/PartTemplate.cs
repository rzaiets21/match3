using UnityEngine;

public class PartTemplate : MonoBehaviour
{
	public enum CopyType
	{
		CopyAllChildren,
		CopyRoot
	}

	[SerializeField]
	public string path;

	[SerializeField]
	public CopyType copyType;

	public string linkName
	{
		get
		{
			string[] array = path.Split('/');
			if (array.Length == 0)
			{
				return "";
			}
			return array[0].Replace("[", "").Replace("]", "");
		}
	}
}
