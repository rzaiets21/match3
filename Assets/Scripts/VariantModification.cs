using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VariantModification
{
	[SerializeField]
	public int variantIndex;

	[SerializeField]
	public string groupName;

	[SerializeField]
	public Material materialToApply;

	[SerializeField]
	public int materialIndex;

	[SerializeField]
	public MeshRenderer rendererToApplyTo;

	private List<Material> materialsHelper = new List<Material>();

	public bool IsApplicable(int index)
	{
		return variantIndex == index;
	}

	public void Apply(bool useSharedMaterial)
	{
		if (materialToApply == null || rendererToApplyTo == null)
		{
			return;
		}
		if (useSharedMaterial)
		{
			if (materialIndex > 0)
			{
				Material[] sharedMaterials = rendererToApplyTo.sharedMaterials;
				sharedMaterials[materialIndex] = materialToApply;
				rendererToApplyTo.sharedMaterials = sharedMaterials;
			}
			else
			{
				rendererToApplyTo.sharedMaterial = materialToApply;
			}
		}
		else if (materialIndex > 0)
		{
			materialsHelper.Clear();
			rendererToApplyTo.GetMaterials(materialsHelper);
			if (materialsHelper[materialIndex] != materialToApply)
			{
				materialsHelper[materialIndex] = materialToApply;
				rendererToApplyTo.materials = materialsHelper.ToArray();
			}
		}
		else
		{
			rendererToApplyTo.material = materialToApply;
		}
	}
}
