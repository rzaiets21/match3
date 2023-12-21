using GGMatch3;
using ProtoModels;
using System.Collections.Generic;

public class ConflictResolverGGTableTennis2015 : ConflictResolverBase
{
	private enum ConflictVersion
	{
		Local,
		Remote
	}

	private struct VersionArguments
	{
		public int stagesPassed;

		public long coins;
	}

	public List<string> filesToSync = new List<string>();

	public override List<string> FilesToSync()
	{
		return filesToSync;
	}

	public override bool ResolveConflict(GGSnapshotCloudSync.CloudSyncConflict conflict)
	{
		Match3StagesDB instance = Match3StagesDB.instance;
		Match3Stages match3Stages = instance.ModelFromData(conflict.serverData);
		int passedStages = instance.passedStages;
		int passedStages2 = match3Stages.passedStages;
		GGPlayerSettings instance2 = GGPlayerSettings.instance;
		GGPlayerSettings gGPlayerSettings = instance2.CreateFromData(conflict.serverData);
		VersionArguments localVersion = default(VersionArguments);
		VersionArguments remoteVersion = default(VersionArguments);
		localVersion.coins = instance2.walletManager.CurrencyCount(CurrencyType.coins);
		localVersion.stagesPassed = passedStages;
		remoteVersion.coins = gGPlayerSettings.walletManager.CurrencyCount(CurrencyType.coins);
		remoteVersion.stagesPassed = passedStages2;
		switch (GetVersionToResolve(localVersion, remoteVersion))
		{
		case ConflictVersion.Local:
			conflict.ResolveConflictUsingLocalVersion();
			return true;
		case ConflictVersion.Remote:
			conflict.ResolveConflictUsingServerVersion();
			return true;
		default:
			return false;
		}
	}

	private ConflictVersion GetVersionToResolve(VersionArguments localVersion, VersionArguments remoteVersion)
	{
		if (localVersion.stagesPassed != remoteVersion.stagesPassed)
		{
			if (localVersion.stagesPassed <= remoteVersion.stagesPassed)
			{
				return ConflictVersion.Remote;
			}
			return ConflictVersion.Local;
		}
		if (localVersion.coins != remoteVersion.coins)
		{
			if (localVersion.coins <= remoteVersion.coins)
			{
				return ConflictVersion.Remote;
			}
			return ConflictVersion.Local;
		}
		return ConflictVersion.Local;
	}

	public override void OnConflict()
	{
	}
}
