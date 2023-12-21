using ProtoModels;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDB : ScriptableObject
{
	[SerializeField]
	public string filename = "inventory.bytes";

	private DecoratingRooms _model;

	private static InventoryDB _instance;

	public DecoratingRooms model
	{
		get
		{
			if (_model == null)
			{
				ReloadModel();
			}
			return _model;
		}
	}

	public static InventoryDB instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Resources.Load<InventoryDB>("InventoryDB");
				SingletonInit<FileIOChanges>.instance.OnChange(_instance.ReloadModel);
			}
			return _instance;
		}
	}

	public void ResetAll()
	{
		_model = new DecoratingRooms();
		_model.rooms = new List<DecoratingRoom>();
		SaveModel();
	}

	public void ReloadModel()
	{
		if (!ProtoIO.LoadFromFileLocal(filename, out _model))
		{
			_model = new DecoratingRooms();
			_model.rooms = new List<DecoratingRoom>();
			SaveModel();
		}
		if (_model.rooms == null)
		{
			_model.rooms = new List<DecoratingRoom>();
			SaveModel();
		}
	}

	public void SaveModel()
	{
		ProtoIO.SaveToFileCS(filename, _model);
	}
}
