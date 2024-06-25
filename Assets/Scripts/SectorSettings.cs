using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SectorSettings
{
	public Reward[] items;
	[NonSerialized]
	public string saveName;
	public int chance = 1;
	public Image image;
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
