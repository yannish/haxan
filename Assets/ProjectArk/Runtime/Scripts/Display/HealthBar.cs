using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0649 // never assigned.



public class HealthBar : MonoBehaviour 
{
	public GameObject pip;

	private Health health;

	[SerializeField]
	private float heightOffset;

	public Image image;


	public void BindTo(Health health)
	{
		this.health = health;
		image = GetComponentInChildren<Image>();

		HandleHealthChange(health.current);

		health.OnHealthChangedTo += HandleHealthChange;

		health.OnHide += Hide;
		health.OnShow += Show;

		for (int i = 0; i < health.max; i++)
		{
			var newPip = Instantiate(pip, image.transform);
			newPip.GetComponent<RectTransform>().rotation = Quaternion.AngleAxis(
				i * (360f / health.max),
				Vector3.forward
				);
		}

		Hide();
	}

	void HandleHealthChange(int newAmount)
	{
		image.fillAmount = health.currentPercent;
	}

	void Hide() { image.gameObject.SetActive(false); }
	void Show() { image.gameObject.SetActive(true); }

	private void LateUpdate()
	{
		if (health == null)
			return;

		transform.position = Globals.Camera.WorldToScreenPoint(
			health.transform.position + Vector3.up * heightOffset
			);
	}
}
