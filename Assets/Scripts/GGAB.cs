using GGOptimize;
using System.Collections.Generic;

public class GGAB : SingletonInit<GGAB>
{
	public Optimize optimize;

	public override void Init()
	{
		base.Init();
		optimize = new Optimize();
		optimize.Init(ExperimentDefinitionAssetStore.instance.experimentsDefinition);
	}

	public static int GetInt(string propertyName, int defaultValue)
	{
		return SingletonInit<GGAB>.instance.optimize.GetInt(propertyName, defaultValue);
	}

	public static bool GetBool(string propertyName, bool defaultValue)
	{
		return SingletonInit<GGAB>.instance.optimize.GetBool(propertyName, defaultValue);
	}

	public static string GetString(string propertyName, string defaultValue)
	{
		return SingletonInit<GGAB>.instance.optimize.GetString(propertyName, defaultValue);
	}

	public static List<Experiment> GetActiveExperiments()
	{
		return SingletonInit<GGAB>.instance.optimize.GetActiveExperiments();
	}
}
