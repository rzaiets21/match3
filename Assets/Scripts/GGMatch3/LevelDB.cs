using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	[Serializable]
	public class LevelDB : ScriptableObjectSingleton<LevelDB>
	{
		private static Dictionary<string, LevelDB> levelDBDictionary = new Dictionary<string, LevelDB>();

		[SerializeField]
		private string _currentLevelName = "";

		public List<LevelDefinition> levels = new List<LevelDefinition>();

		public string currentLevelName
		{
			get
			{
				return _currentLevelName;
			}
			set
			{
				_currentLevelName = value;
			}
		}

		public int currentLevelIndex
		{
			get
			{
				for (int i = 0; i < levels.Count; i++)
				{
					if (levels[i].name == currentLevelName)
					{
						return i;
					}
				}
				return 0;
			}
		}

		public static LevelDB NamedInstance(string levelDBName)
		{
			LevelDB value = null;
			if (levelDBDictionary.TryGetValue(levelDBName, out value))
			{
				return value;
			}
			value = Resources.Load<LevelDB>(levelDBName);
			if (value == null)
			{
				value = ScriptableObjectSingleton<LevelDB>.instance;
			}
			value.UpdateData();
			levelDBDictionary.Add(levelDBName, value);
			return value;
		}

		public LevelDefinition Get(string levelName)
		{
			for (int i = 0; i < levels.Count; i++)
			{
				LevelDefinition levelDefinition = levels[i];
				if (levelDefinition.name == levelName)
				{
					return levelDefinition;
				}
			}
			return null;
		}
	}
}
