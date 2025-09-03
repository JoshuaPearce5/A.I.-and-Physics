using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    [SerializeField] public Image energyBar;
    [SerializeField] public float maxEnergy = 100;

    public float currentEnergy;

    float lerpSpeed;

    void Start()
    {
        currentEnergy = maxEnergy;
    }

    void Update()
    {
        float energyPercent = currentEnergy / maxEnergy;

        if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;

        lerpSpeed = 3f * Time.deltaTime;

        EnergyBarFiller();
        //ColorChanger();
    }


    void EnergyBarFiller()
    {
        energyBar.fillAmount = Mathf.Lerp(energyBar.fillAmount, currentEnergy / maxEnergy, lerpSpeed);
    }

    /*
    void ColorChanger()
    {
        Color energyColor = Color.Lerp(Color.red, Color.blue, (currentEnergy / maxEnergy));

        energyBar.color = energyColor;
    }
    */

    public void UseEnergy(float amount)
    {
        currentEnergy -= amount * Time.deltaTime;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
    }

    public void BurstEnergy(float amount)
    {
        currentEnergy -= amount;
    }

    public void RegainEnergy(float amount)
    {
        currentEnergy += amount * Time.deltaTime;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
    }

    public void PickupEnergy(float amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
    }

    public void RestoreEnergy()
    {
        currentEnergy = maxEnergy;
    }
}
