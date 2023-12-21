using ProtoModels;
using System.Collections.Generic;

public class RoomsBackend : SingletonInit<RoomsBackend>
{
	public class RoomAccessor
	{
		public bool needsToBeRenewed;

		public RoomsBackend roomsBackend;

		public RoomDecoration.Room room;

		private List<VisualObjectAccessor> visualObjectAccessors = new List<VisualObjectAccessor>();

		private List<VariantGroupAccessor> variantGroupAccessors = new List<VariantGroupAccessor>();

		public bool isPassed
		{
			get
			{
				return room.isPassed;
			}
			set
			{
				room.isPassed = value;
				Save();
			}
		}

		private List<RoomDecoration.VisualObject> visualObjects
		{
			get
			{
				if (room.visualObjects == null)
				{
					room.visualObjects = new List<RoomDecoration.VisualObject>();
				}
				return room.visualObjects;
			}
		}

		private List<RoomDecoration.VariantGroup> variantGroups
		{
			get
			{
				if (room.variantGroups == null)
				{
					room.variantGroups = new List<RoomDecoration.VariantGroup>();
				}
				return room.variantGroups;
			}
		}

		public void SetNeedsToBeRenewed()
		{
			needsToBeRenewed = true;
			for (int i = 0; i < visualObjectAccessors.Count; i++)
			{
				visualObjectAccessors[i].needsToBeRenewed = true;
			}
		}

		public RoomAccessor CreateRenewedAccessor()
		{
			return roomsBackend.GetRoom(room.name);
		}

		public RoomAccessor(RoomDecoration.Room room, RoomsBackend roomsBackend)
		{
			this.room = room;
			this.roomsBackend = roomsBackend;
		}

		public VisualObjectAccessor GetVisualObject(string visualObjectName)
		{
			for (int i = 0; i < visualObjectAccessors.Count; i++)
			{
				VisualObjectAccessor visualObjectAccessor = visualObjectAccessors[i];
				if (visualObjectAccessor.visualObject.name == visualObjectName)
				{
					return visualObjectAccessor;
				}
			}
			VisualObjectAccessor visualObjectAccessor2 = new VisualObjectAccessor(GetOrCreateVisualObjectModel(visualObjectName), this);
			visualObjectAccessors.Add(visualObjectAccessor2);
			return visualObjectAccessor2;
		}

		public VariantGroupAccessor GetVariantGroup(string variantGroupName)
		{
			for (int i = 0; i < variantGroupAccessors.Count; i++)
			{
				VariantGroupAccessor variantGroupAccessor = variantGroupAccessors[i];
				if (variantGroupAccessor.variantGroup.name == variantGroupName)
				{
					return variantGroupAccessor;
				}
			}
			VariantGroupAccessor variantGroupAccessor2 = new VariantGroupAccessor(GetOrCreateVariantGroupModel(variantGroupName), this);
			variantGroupAccessors.Add(variantGroupAccessor2);
			return variantGroupAccessor2;
		}

		private RoomDecoration.VisualObject GetOrCreateVisualObjectModel(string visualObjectName)
		{
			RoomDecoration.VisualObject visualObjectModel = GetVisualObjectModel(visualObjectName);
			if (visualObjectModel != null)
			{
				return visualObjectModel;
			}
			visualObjectModel = new RoomDecoration.VisualObject();
			visualObjectModel.name = visualObjectName;
			visualObjects.Add(visualObjectModel);
			Save();
			return visualObjectModel;
		}

		private RoomDecoration.VisualObject GetVisualObjectModel(string visualObjectName)
		{
			List<RoomDecoration.VisualObject> visualObjects = this.visualObjects;
			for (int i = 0; i < visualObjects.Count; i++)
			{
				RoomDecoration.VisualObject visualObject = visualObjects[i];
				if (visualObject.name == visualObjectName)
				{
					return visualObject;
				}
			}
			return null;
		}

		private RoomDecoration.VariantGroup GetVariantGroupModel(string variantGroupName)
		{
			List<RoomDecoration.VariantGroup> variantGroups = this.variantGroups;
			for (int i = 0; i < variantGroups.Count; i++)
			{
				RoomDecoration.VariantGroup variantGroup = variantGroups[i];
				if (variantGroup.name == variantGroupName)
				{
					return variantGroup;
				}
			}
			return null;
		}

		private RoomDecoration.VariantGroup GetOrCreateVariantGroupModel(string variantGroupName)
		{
			RoomDecoration.VariantGroup variantGroupModel = GetVariantGroupModel(variantGroupName);
			if (variantGroupModel != null)
			{
				return variantGroupModel;
			}
			variantGroupModel = new RoomDecoration.VariantGroup();
			variantGroupModel.name = variantGroupName;
			variantGroups.Add(variantGroupModel);
			Save();
			return variantGroupModel;
		}

		public bool IsOwned(string visualObjectName)
		{
			return GetVisualObjectModel(visualObjectName)?.isOwned ?? false;
		}

		public void Save()
		{
			roomsBackend.Save();
		}
	}

	public class VariantGroupAccessor
	{
		public bool needsToBeRenewed;

		public RoomDecoration.VariantGroup variantGroup;

		public RoomAccessor roomAccessor;

		public VariantGroupAccessor(RoomDecoration.VariantGroup variantGroup, RoomAccessor roomAccessor)
		{
			this.variantGroup = variantGroup;
			this.roomAccessor = roomAccessor;
		}

		public void Save()
		{
			roomAccessor.Save();
		}
	}

	public class VisualObjectAccessor
	{
		public bool needsToBeRenewed;

		public RoomDecoration.VisualObject visualObject;

		public RoomAccessor roomAccessor;

		public VisualObjectAccessor(RoomDecoration.VisualObject visualObject, RoomAccessor roomAccessor)
		{
			this.visualObject = visualObject;
			this.roomAccessor = roomAccessor;
		}

		public void Save()
		{
			roomAccessor.Save();
		}
	}

	public string filename = "r.bytes";

	private RoomDecoration model_;

	private List<RoomAccessor> roomAccessors = new List<RoomAccessor>();

	public RoomDecoration model
	{
		get
		{
			if (model_ == null)
			{
				ReloadModel();
			}
			return model_;
		}
	}

	public int selectedRoomIndex
	{
		get
		{
			return model_.selectedRoomIndex;
		}
		set
		{
			model_.selectedRoomIndex = value;
			Save();
		}
	}

	public List<RoomDecoration.Room> rooms
	{
		get
		{
			if (model.rooms == null)
			{
				model.rooms = new List<RoomDecoration.Room>();
			}
			return model.rooms;
		}
	}

	public RoomAccessor GetRoom(string roomName)
	{
		for (int i = 0; i < roomAccessors.Count; i++)
		{
			RoomAccessor roomAccessor = roomAccessors[i];
			if (roomAccessor.room.name == roomName)
			{
				return roomAccessor;
			}
		}
		RoomAccessor roomAccessor2 = new RoomAccessor(GetOrCreateRoomModel(roomName), this);
		roomAccessors.Add(roomAccessor2);
		return roomAccessor2;
	}

	private RoomDecoration.Room GetOrCreateRoomModel(string roomName)
	{
		RoomDecoration.Room roomModel = GetRoomModel(roomName);
		if (roomModel != null)
		{
			return roomModel;
		}
		roomModel = new RoomDecoration.Room();
		roomModel.name = roomName;
		rooms.Add(roomModel);
		Save();
		return roomModel;
	}

	public void Save()
	{
		ProtoIO.SaveToFileCS(filename, model);
	}

	private RoomDecoration.Room GetRoomModel(string roomName)
	{
		List<RoomDecoration.Room> rooms = this.rooms;
		for (int i = 0; i < rooms.Count; i++)
		{
			RoomDecoration.Room room = rooms[i];
			if (room.name == roomName)
			{
				return room;
			}
		}
		return null;
	}

	private void RenewRoomAccessors()
	{
		for (int i = 0; i < roomAccessors.Count; i++)
		{
			roomAccessors[i].SetNeedsToBeRenewed();
		}
		roomAccessors.Clear();
	}

	public void Reset()
	{
		model.rooms.Clear();
		Save();
		RenewRoomAccessors();
	}

	public void ReloadModel()
	{
		if (!ProtoIO.LoadFromFileLocal(filename, out model_))
		{
			model_ = new RoomDecoration();
		}
		RenewRoomAccessors();
	}

	public RoomDecoration GetModel()
	{
		return model;
	}

	public void SetNewModel(RoomDecoration _model)
	{
		model_ = _model;
		Save();
		RenewRoomAccessors();
	}

	public override void Init()
	{
		base.Init();
		ReloadModel();
		SingletonInit<FileIOChanges>.instance.OnChange(ReloadModel);
	}
}
