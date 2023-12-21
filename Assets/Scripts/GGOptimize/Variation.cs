using System;
using System.Collections.Generic;

namespace GGOptimize
{
	[Serializable]
	public class Variation
	{
		public string name;

		public List<NamedProperty> properties = new List<NamedProperty>();

		protected Dictionary<string, NamedProperty> propertyDictionary_;

		protected Dictionary<string, NamedProperty> propertyDictionary
		{
			get
			{
				if (propertyDictionary_ == null)
				{
					propertyDictionary_ = new Dictionary<string, NamedProperty>();
					for (int i = 0; i < properties.Count; i++)
					{
						NamedProperty namedProperty = properties[i];
						if (!propertyDictionary_.ContainsKey(namedProperty.name))
						{
							propertyDictionary_.Add(namedProperty.name, namedProperty);
						}
						propertyDictionary_[namedProperty.name] = namedProperty;
					}
				}
				return propertyDictionary_;
			}
		}

		public Variation()
		{
		}

		public Variation(string name)
		{
			this.name = name;
		}

		public NamedProperty GetProperty(string name)
		{
			if (name == null)
			{
				return null;
			}
			Dictionary<string, NamedProperty> propertyDictionary = this.propertyDictionary;
			if (propertyDictionary == null)
			{
				for (int num = properties.Count - 1; num >= 0; num--)
				{
					NamedProperty namedProperty = properties[num];
					if (namedProperty.name == name)
					{
						return namedProperty;
					}
				}
				return null;
			}
			if (!propertyDictionary.ContainsKey(name))
			{
				return null;
			}
			return propertyDictionary[name];
		}
	}
}
