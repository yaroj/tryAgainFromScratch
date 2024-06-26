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

/// <summary>
/// types of items that can be received as a reward
/// </summary>
public enum ItemType
{
	gem,
	coin,
	skin,

}