using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SectorSettings
{
	[NonSerialized]
	public Image image;
	[NonSerialized]
	public string saveName;


	public Reward[] items;
	public int chance = 1;
	public Color fillColor;
	public Reward GetCurrentReward()
	{
		var current = PlayerPrefs.GetInt(saveName, 0);
		return items[Math.Min(current, items.Length - 1)];
	}

	public void OnRewardTaken()
	{
		PlayerPrefs.SetInt(saveName, PlayerPrefs.GetInt(saveName, 0) + 1);
		UpdateImage();
	}

	public void UpdateImage()
	{
		image.sprite = GetCurrentReward().sprite;
	}
}
