using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider healthSlider;
    public Slider defenceSlider;
    public Slider energySlider;
    public Text healthText;
    public Text defenceText;
    public Text energyText;

    GameObject player;
    CharacterStats playerStats;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        if (player != null)
            playerStats = player.GetComponent<CharacterStats>();
        else
            Debug.Log("Œ¥’“µΩPlayer!");
    }

    private void Update()
    {
        UpdatePlayerBarUI();
    }

    void UpdatePlayerBarUI()
    {
        healthSlider.value = (float)playerStats.CurrentHealth / playerStats.MaxHealth;
        defenceSlider.value = (float)playerStats.CurrentDefence / playerStats.BaseDefence;
        energySlider.value = (float)playerStats.CurrentEnergy / playerStats.MaxEnergy;
        healthText.text = playerStats.CurrentHealth + "/" + playerStats.MaxHealth;
        defenceText.text = playerStats.CurrentDefence + "/" + playerStats.BaseDefence;
        energyText.text = playerStats.CurrentEnergy + "/" + playerStats.MaxEnergy;
    }
}
