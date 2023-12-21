using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DigitalRuby.ThunderAndLightning
{
	public class DemoPlayerControllerScript : MonoBehaviour
	{
		public Text SpellLabel;

		public float Speed = 3f;

		public float RotateSpeed = 3f;

		public LightningSpellScript[] Spells;

		private int spellIndex;

		private bool spellMouseButtonDown;

		private GameObject rightHand;

		private void OnCollisionEnter(Collision collision)
		{
			ContactPoint[] contacts = collision.contacts;
			for (int i = 0; i < contacts.Length; i++)
			{
				ContactPoint contactPoint = contacts[i];
				Rigidbody component = contactPoint.otherCollider.gameObject.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.velocity += base.gameObject.transform.forward * 5f;
				}
			}
		}

		private void Start()
		{
			rightHand = base.gameObject.transform.Find("RightArm").Find("RightHand").gameObject;
			UpdateSpell();
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				DemoScript.ReloadCurrentScene();
				return;
			}
			CharacterController component = GetComponent<CharacterController>();
			base.transform.Rotate(0f, UnityEngine.Input.GetAxis("Horizontal") * RotateSpeed, 0f);
			Vector3 a = base.transform.TransformDirection(Vector3.forward);
			float d = Speed * UnityEngine.Input.GetAxis("Vertical");
			component.SimpleMove(a * d);
			if (UnityEngine.Input.GetKeyDown(KeyCode.Plus) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadPlus))
			{
				NextSpell();
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.Minus) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadMinus))
			{
				PreviousSpell();
			}
			LightningSpellScript lightningSpellScript = Spells[spellIndex];
			if (Input.GetButton("Fire1") && (spellMouseButtonDown || !Input.GetMouseButton(0) || GuiElementShouldPassThrough()))
			{
				if (lightningSpellScript.SpellStart != null && lightningSpellScript.SpellStart.GetComponent<Rigidbody>() == null)
				{
					lightningSpellScript.SpellStart.transform.position = rightHand.transform.position;
				}
				if (Input.GetMouseButton(0))
				{
					spellMouseButtonDown = true;
					Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
					RaycastHit hitInfo;
					Vector3 a2 = (!Physics.Raycast(ray, out hitInfo, lightningSpellScript.MaxDistance, lightningSpellScript.CollisionMask)) ? (ray.origin + ray.direction * lightningSpellScript.MaxDistance) : hitInfo.point;
					lightningSpellScript.Direction = (a2 - lightningSpellScript.SpellStart.transform.position).normalized;
				}
				else
				{
					spellMouseButtonDown = false;
					lightningSpellScript.Direction = base.gameObject.transform.forward;
				}
				lightningSpellScript.CastSpell();
			}
			else
			{
				spellMouseButtonDown = false;
				lightningSpellScript.StopSpell();
			}
		}

		private bool GuiElementShouldPassThrough()
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = UnityEngine.Input.mousePosition;
			List<RaycastResult> list = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerEventData, list);
			foreach (RaycastResult item in list)
			{
				if (item.gameObject.GetComponent<Button>() != null || (item.gameObject.GetComponent<Text>() == null && item.gameObject.GetComponent<Image>() == null))
				{
					return false;
				}
			}
			return true;
		}

		private void UpdateSpell()
		{
			SpellLabel.text = Spells[spellIndex].name;
			Spells[spellIndex].ActivateSpell();
		}

		private void ChangeSpell(int dir)
		{
			Spells[spellIndex].StopSpell();
			Spells[spellIndex].DeactivateSpell();
			spellIndex += dir;
			if (spellIndex < 0)
			{
				spellIndex = Spells.Length - 1;
			}
			else if (spellIndex >= Spells.Length)
			{
				spellIndex = 0;
			}
			UpdateSpell();
		}

		public void PreviousSpell()
		{
			ChangeSpell(-1);
		}

		public void NextSpell()
		{
			ChangeSpell(1);
		}
	}
}
