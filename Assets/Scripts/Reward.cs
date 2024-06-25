using System;
using UnityEngine;

[Serializable]
public struct Reward
{
	public Sprite sprite;
	public ItemType Type;
	public int count;
	public string label;
}


public enum ItemType
{
	gem,
	coin,
	skin,

}