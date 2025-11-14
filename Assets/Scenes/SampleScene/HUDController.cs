using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class HUDController : MonoBehaviour
{
    public TextMeshProUGUI HPText;
    public int Health = 3;
    private const string HPCHAR = "\u2665";

    private void Start()
    {
        UpdateHP();
    }
    
    private void UpdateHP()
    {
        HPText.text = "";
        for (int i = 0; i < Health; i++)
            HPText.text += HPCHAR;
    }
}
