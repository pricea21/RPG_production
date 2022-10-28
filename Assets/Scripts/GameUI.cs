using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
	public TextMeshProUGUI goldText;
	public TextMeshProUGUI winText;
    public Image winBackground;

	//instance 
	public static GameUI instance;

	void Awake()
	{
		instance = this;
	}

	public void UpdateGoldText(int gold)
	{
		goldText.text = "<b>Gold:</b>" + gold;
	}

	 public void SetWinText()
    {
        winBackground.gameObject.SetActive(true);
        winText.text = "Congratulations! You saved the princess!";
    }
}
