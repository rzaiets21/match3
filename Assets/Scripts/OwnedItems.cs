using ProtoModels;
using System.Collections.Generic;
using UnityEngine;

public class OwnedItems
{
	private sealed class _003C_003Ec__DisplayClass14_0
	{
		public string name;

		internal bool _003CisOwned_003Eb__0(OwnedItemDAO o)
		{
			return o.name == name;
		}
	}

	public static string DefaultOwnedItemsFilename = "ownedItems.bytes";

	public OwnedItemsDAO ownedItems;

	private string _003COwnedItemsFilename_003Ek__BackingField;

	public string OwnedItemsFilename
	{
		get
		{
			return _003COwnedItemsFilename_003Ek__BackingField;
		}
		private set
		{
			_003COwnedItemsFilename_003Ek__BackingField = value;
		}
	}

	public OwnedItems(string filename)
	{
		OwnedItemsFilename = filename;
		ReloadModel();
		SingletonInit<FileIOChanges>.instance.OnChange(ReloadModel);
	}

	public void ReloadModel()
	{
		if (!ProtoIO.LoadFromFileLocal<ProtoSerializer, OwnedItemsDAO>(OwnedItemsFilename, out ownedItems))
		{
			ownedItems = new OwnedItemsDAO();
			ProtoIO.SaveToFileCS(OwnedItemsFilename, ownedItems);
		}
		if (ownedItems.ownedItems == null)
		{
			ownedItems.ownedItems = new List<OwnedItemDAO>();
			ProtoIO.SaveToFileCS(OwnedItemsFilename, ownedItems);
		}
	}

	public void AddToOwned(string name, bool canStockpile)
	{
		UnityEngine.Debug.Log("addToOwned");
		if (!canStockpile && isOwned(name))
		{
			UnityEngine.Debug.Log("addToOwned - cancelled");
			return;
		}
		OwnedItemDAO ownedItemDAO = new OwnedItemDAO();
		ownedItemDAO.name = name;
		ownedItems.ownedItems.Add(ownedItemDAO);
		Save();
	}

	public OwnedItemDAO GetOrCreateItemWithName(string name)
	{
		for (int i = 0; i < ownedItems.ownedItems.Count; i++)
		{
			OwnedItemDAO ownedItemDAO = ownedItems.ownedItems[i];
			if (ownedItemDAO.name == name)
			{
				return ownedItemDAO;
			}
		}
		OwnedItemDAO ownedItemDAO2 = new OwnedItemDAO();
		ownedItemDAO2.name = name;
		ownedItems.ownedItems.Add(ownedItemDAO2);
		Save();
		return ownedItemDAO2;
	}

	public UsedItemDAO GetOrCreateUsedItemWithName(string name)
	{
		if (ownedItems.usedItems == null)
		{
			ownedItems.usedItems = new List<UsedItemDAO>();
		}
		for (int i = 0; i < ownedItems.usedItems.Count; i++)
		{
			UsedItemDAO usedItemDAO = ownedItems.usedItems[i];
			if (usedItemDAO.name == name)
			{
				return usedItemDAO;
			}
		}
		UsedItemDAO usedItemDAO2 = new UsedItemDAO();
		usedItemDAO2.name = name;
		ownedItems.usedItems.Add(usedItemDAO2);
		Save();
		return usedItemDAO2;
	}

	public bool isOwned(string name)
	{
		_003C_003Ec__DisplayClass14_0 _003C_003Ec__DisplayClass14_ = new _003C_003Ec__DisplayClass14_0();
		_003C_003Ec__DisplayClass14_.name = name;
		if (ownedItems.ownedItems == null)
		{
			return false;
		}
		return ownedItems.ownedItems.Find(_003C_003Ec__DisplayClass14_._003CisOwned_003Eb__0) != null;
	}

	public OwnedItemDAO GetItemWithName(string name)
	{
		if (ownedItems.ownedItems == null)
		{
			return null;
		}
		OwnedItemDAO result = null;
		for (int i = 0; i < ownedItems.ownedItems.Count; i++)
		{
			OwnedItemDAO ownedItemDAO = ownedItems.ownedItems[i];
			if (ownedItemDAO.name == name)
			{
				result = ownedItemDAO;
				break;
			}
		}
		return result;
	}

	public void Save()
	{
		ProtoIO.SaveToFileCS(OwnedItemsFilename, ownedItems);
	}
}
