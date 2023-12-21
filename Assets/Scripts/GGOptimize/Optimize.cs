using ProtoModels;
using System.Collections.Generic;
using UnityEngine;

namespace GGOptimize
{
	public class Optimize
	{
		public ExperimentsDefinition experimentsDefinition = new ExperimentsDefinition();

		protected ExperimentsData dataModel;

		protected List<Experiment> activeExperiments_ = new List<Experiment>();

		protected int userBucket
		{
			get
			{
				if (dataModel == null)
				{
					dataModel = new ExperimentsData();
				}
				return dataModel.userBucket;
			}
		}

		protected ExperimentsData.ExperimentData GetExperimentData(Experiment e)
		{
			if (dataModel == null)
			{
				dataModel = new ExperimentsData();
			}
			if (dataModel.experiments == null)
			{
				dataModel.experiments = new List<ExperimentsData.ExperimentData>();
			}
			for (int i = 0; i < dataModel.experiments.Count; i++)
			{
				ExperimentsData.ExperimentData experimentData = dataModel.experiments[i];
				if (experimentData.guid == e.guid)
				{
					return experimentData;
				}
			}
			return null;
		}

		public bool IsNewUserOnExperiment(Experiment e)
		{
			return GetExperimentData(e)?.isNewUserOnExperiment ?? false;
		}

		public int GetUserBucket(Experiment experiment)
		{
			if (!experiment.useLocalBucket)
			{
				return userBucket;
			}
			ExperimentsData.ExperimentData experimentData = GetExperimentData(experiment);
			GGPlayerSettings instance = GGPlayerSettings.instance;
			if ((experimentData == null || !experimentData.isBucketSet) && instance.Model.experiments != null)
			{
				dataModel = instance.Model.experiments;
			}
			experimentData = GetExperimentData(experiment);
			if (experimentData == null || !experimentData.isBucketSet)
			{
				if (experimentData == null)
				{
					experimentData = new ExperimentsData.ExperimentData();
					experimentData.guid = experiment.guid;
					dataModel.experiments.Add(experimentData);
				}
				experimentData.isBucketSet = true;
				experimentData.userBucket = Random.Range(0, 100);
				instance.Model.experiments = dataModel;
				instance.Save();
			}
			return experimentData.userBucket;
		}

		protected void SetUserIsNewOnExperiment(Experiment experiment)
		{
			if (dataModel == null)
			{
				dataModel = new ExperimentsData();
			}
			if (dataModel.experiments == null)
			{
				dataModel.experiments = new List<ExperimentsData.ExperimentData>();
			}
			ExperimentsData.ExperimentData experimentData = GetExperimentData(experiment);
			GGPlayerSettings instance = GGPlayerSettings.instance;
			if (experimentData == null)
			{
				experimentData = new ExperimentsData.ExperimentData();
				experimentData.guid = experiment.guid;
				dataModel.experiments.Add(experimentData);
			}
			experimentData.isNewUserOnExperiment = true;
		}

		public List<Experiment> GetActiveExperiments()
		{
			activeExperiments_.Clear();
			List<Experiment> experiments = experimentsDefinition.experiments;
			for (int i = 0; i < experiments.Count; i++)
			{
				Experiment experiment = experiments[i];
				if (experiment.IsActive(this))
				{
					activeExperiments_.Add(experiment);
				}
			}
			return activeExperiments_;
		}

		public NamedProperty GetNamedProperty(string name)
		{
			List<Experiment> activeExperiments = GetActiveExperiments();
			NamedProperty namedProperty = null;
			for (int num = activeExperiments.Count - 1; num >= 0; num--)
			{
				Experiment experiment = activeExperiments[num];
				namedProperty = experiment.GetActiveVariation(GetUserBucket(experiment)).GetProperty(name);
				if (namedProperty != null)
				{
					break;
				}
			}
			if (namedProperty == null)
			{
				namedProperty = experimentsDefinition.defaultProperties.GetProperty(name);
			}
			return namedProperty;
		}

		public int GetInt(string propertyName, int defaultValue)
		{
			return GetNamedProperty(propertyName)?.GetInt() ?? defaultValue;
		}

		public bool GetBool(string propertyName, bool defaultValue)
		{
			return GetNamedProperty(propertyName)?.GetBool() ?? defaultValue;
		}

		public string GetString(string propertyName, string defaultValue)
		{
			NamedProperty namedProperty = GetNamedProperty(propertyName);
			if (namedProperty != null)
			{
				return namedProperty.GetString();
			}
			return defaultValue;
		}

		public void Init(ExperimentsDefinition experimentsDefinition)
		{
			this.experimentsDefinition = experimentsDefinition;
			GGPlayerSettings instance = GGPlayerSettings.instance;
			dataModel = instance.Model.experiments;
			if (dataModel != null && dataModel.isUserBucketSet)
			{
				return;
			}
			dataModel = new ExperimentsData();
			List<Experiment> experiments = experimentsDefinition.experiments;
			if (instance.Model.version == ConfigBase.instance.initialPlayerVersion)
			{
				for (int i = 0; i < experiments.Count; i++)
				{
					Experiment experiment = experiments[i];
					if (experiment.acceptsNewUsers)
					{
						SetUserIsNewOnExperiment(experiment);
					}
				}
			}
			dataModel.userBucket = Random.Range(0, 100);
			dataModel.isUserBucketSet = true;
			instance.Model.experiments = dataModel;
			instance.Save();
		}
	}
}
