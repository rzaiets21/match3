using GGOptimize;
using UnityEngine;

public class ExperimentDefinitionAssetStore : ScriptableObjectSingleton<ExperimentDefinitionAssetStore>
{
	private static bool applicationIsQuitting;

	protected new static ExperimentDefinitionAssetStore instance_;

	public ExperimentsDefinition experimentsDefinition = new ExperimentsDefinition();

	public new static ExperimentDefinitionAssetStore instance
	{
		get
		{
			if (instance_ == null)
			{
				if (applicationIsQuitting)
				{
					return null;
				}
				instance_ = Resources.Load<ExperimentDefinitionAssetStore>(ConfigBase.instance.experimentsResourceName);
				if (instance_ == null)
				{
					instance_ = Resources.Load<ExperimentDefinitionAssetStore>(typeof(ExperimentDefinitionAssetStore).ToString());
				}
			}
			return instance_;
		}
	}

	public new void OnDestroy()
	{
		applicationIsQuitting = true;
	}
}
