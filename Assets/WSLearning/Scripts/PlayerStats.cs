using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public float HP;
    public float MaxHP;
    public float Mana;
    public float Sprint;
    public int Score;

    public TextMeshProUGUI HPText;
    public Image HPBar;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            HP -= 10;
        }

        HPText.text = "Здоровье: " + HP;
        HPBar.fillAmount = HP / 100;
    }
}
