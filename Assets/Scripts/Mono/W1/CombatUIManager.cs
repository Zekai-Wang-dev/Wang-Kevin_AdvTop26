using System;
using Unity.VisualScripting;
using UnityEngine;

public class CombatUIManager : MonoBehaviour
{

    public GameObject coinPrefab;
    public GameObject coinLayoutGroup; 

    public CombatManager combatManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
        combatManager.combatSystem.onCoinsChanged += UpdateCoinUI;

    }

    public void UpdateCoinUI()
    {

        foreach (Transform child in coinLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < combatManager.engagedPlayer.currentcoinCount; i++)
        {

            Instantiate(coinPrefab, coinLayoutGroup.transform);

        }
    }
}
