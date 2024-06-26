using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WheelManager : MonoBehaviour
{
	public float iconOffset;
	public float labelOffset;
	public float offsetMultiplierLength;
	public Vector3 offsetMultiplier;
	public int[] FirstNSectors;
	public float wheelSpeed;
	public SectorSettings[] sectors;


	[SerializeField] private Image _resultImage;
	[SerializeField] private TextMeshProUGUI _resultLabel;
	[SerializeField] private GameObject _sectorPrefab;
	[SerializeField] private Button _spinWheelButton;

	private int _numberOfSectors;
	private int _totalChance = 0;
	pri float _sectorProportion;

	const string SAVESPINCOUNTNAME = "spinCount";



	private void Awake()
	{
		_numberOfSectors = sectors.Length;
		CreateWheel();
	}


	private void CreateWheel()
	{
		Image[] imageSlices = new Image[_numberOfSectors];
		CreateSectors(ref imageSlices);
		ApplySectorSettings(ref imageSlices);
	}

	private void CreateSectors(ref Image[] imageSlices)
	{
		_sectorProportion = 1f / _numberOfSectors;
		offsetMultiplier = new Vector3(offsetMultiplierLength * Mathf.Sin(_sectorProportion * Mathf.PI), offsetMultiplierLength * Mathf.Cos(_sectorProportion * Mathf.PI), 0);
		float totalSliceProp = 0;
		for (int i = 0; i < _numberOfSectors; i++)
		{
			GameObject newSlice = Instantiate(_sectorPrefab, transform);
			newSlice.transform.localPosition = new Vector3(0, 0, 0);
			sectors[i].saveName = i.ToString();
			newSlice.name = sectors[i].GetCurrentReward().label;

			totalSliceProp += _sectorProportion;
			float rotAngle = 360 * (0.5f + _sectorProportion - totalSliceProp);
			newSlice.transform.Rotate(0, 0, rotAngle);

			Image img = newSlice.GetComponent<Image>();
			imageSlices[i] = img;

		}
	}
	private void ApplySectorSettings(ref Image[] imageSlices)
	{
		CalcTotalChance();
		SetColors(ref imageSlices);
		SetIcons(ref imageSlices);
		SetText(ref imageSlices);
	}

	private void CalcTotalChance()
	{
		_totalChance = 0;
		foreach (var sector in sectors)
		{
			_totalChance += sector.chance;
		}
	}

	private void SetColors(ref Image[] imageSlices)
	{
		for (int i = 0; i < _numberOfSectors; i++)
		{
			imageSlices[i].fillAmount = _sectorProportion;
			imageSlices[i].color = sectors[i].fillColor;
		}
	}
	private void SetIcons(ref Image[] imageSlices)
	{
		for (int i = 0; i < _numberOfSectors; i++)
		{
			Image icon = imageSlices[i].transform.GetChild(0).GetComponent<Image>();

			if (sectors[i].GetCurrentReward().sprite == null) { icon.gameObject.SetActive(false); continue; }
			sectors[i].image = icon;

			icon.transform.localPosition = offsetMultiplier * iconOffset;
			icon.transform.localScale *= Mathf.Clamp(6 * _sectorProportion, 0, 1.2f);
			sectors[i].UpdateImage();
			icon.transform.Rotate(0, 0, -180 * _sectorProportion);
			icon.preserveAspect = true;

		}
	}

	private void SetText(ref Image[] imageSector)
	{
		for (int i = 0; i < _numberOfSectors; i++)
		{

			TMP_Text labelText = imageSector[i].GetComponentInChildren<TMP_Text>();
			labelText.transform.Rotate(0, 0, -180 * _sectorProportion);

			labelText.text = sectors[i].GetCurrentReward().label;


			labelText.transform.localPosition = offsetMultiplier * labelOffset;
		}
	}



	public void SpinWheel()
	{
		int _randomSelectedChioceID = -1;
		int currentSpinNumber = PlayerPrefs.GetInt(SAVESPINCOUNTNAME, 0);
		if (FirstNSectors.Length > currentSpinNumber)
		{
			_randomSelectedChioceID = FirstNSectors[currentSpinNumber] % _numberOfSectors;
		}
		else
		{
			int currentChance = 0;
			int randomValue = Random.Range(0, _totalChance);
			for (int i = 0; i < _numberOfSectors; i++)
			{
				currentChance += sectors[i].chance;
				if (currentChance > randomValue)
				{
					_randomSelectedChioceID = i;
					break;
				}
			}
		}
		PlayerPrefs.SetInt(SAVESPINCOUNTNAME, currentSpinNumber + 1);
		StartCoroutine(RollWheel(_randomSelectedChioceID));

	}

	private IEnumerator RollWheel(int targetSector)
	{
		_spinWheelButton.enabled = false;
		float AngleToRotate = 360 * (targetSector + 0.5f) / _numberOfSectors + 540 - transform.rotation.eulerAngles.z;

		while (AngleToRotate > 0)
		{
			AngleToRotate -= wheelSpeed * Time.deltaTime;
			transform.Rotate(0, 0, wheelSpeed * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		ShowRewardUI(sectors[targetSector]);

		yield break;
	}

	private void ShowRewardUI(SectorSettings sectorSettings)
	{
		_resultImage.gameObject.SetActive(true);
		var currentReward = sectorSettings.GetCurrentReward();
		_resultImage.sprite = currentReward.sprite;
		_resultLabel.text = currentReward.label;


		sectorSettings.OnRewardTaken();
	}

}
