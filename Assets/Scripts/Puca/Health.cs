using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    //[SerializeField] public Text healthText;
    [SerializeField] public Image healthBar;

    [SerializeField] public float maxHealth = 100;
    public float currentHealth;

    [SerializeField] public Energy energy;

    [SerializeField] private CheckpointManager checkpointManager;

    float lerpSpeed;

    void Start()
    {
        currentHealth = maxHealth;

        energy = FindObjectOfType<Energy>();

        checkpointManager = GameObject.FindGameObjectWithTag("CheckpointManager").GetComponent<CheckpointManager>();
    }

    void Update()
    {
        float healthPercent = currentHealth / maxHealth;

        //healthText.text = Mathf.RoundToInt(healthPercent * 100) + "%";

        if (currentHealth > maxHealth) currentHealth = maxHealth;

        lerpSpeed = 3f * Time.deltaTime;

        HealthBarFiller();
        //ColorChanger();
    }
    

    void HealthBarFiller()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth/maxHealth, lerpSpeed);
    }

    /*
    void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (currentHealth / maxHealth));

        healthBar.color = healthColor;
    }
    */

    public void TakeDamage(float amount)
    {
        Debug.Log(amount + " Damage Taken!");
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;

            energy.RestoreEnergy();

            checkpointManager.respawn();
        }
    }


    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}
