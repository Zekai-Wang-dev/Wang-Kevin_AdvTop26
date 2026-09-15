using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public EngagedCharacter engagedPlayer = new EngagedCharacter();
    public EngagedCharacter engagedEnemy = new EngagedCharacter();

    public Character player;
    public Character enemy; 

    public float clashDuration = 2f;
    public float coinClashDuration = 1f;

    public CombatSystem combatSystem = new CombatSystem();

    public Event coinsChanged; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        combatSystem.StartCombat(player, enemy, engagedPlayer, engagedEnemy);
        StartCoroutine(ClashLoop(engagedPlayer , engagedEnemy, clashDuration));

    }



    public IEnumerator ClashLoop(EngagedCharacter engagedPlayer, EngagedCharacter engagedEnemy, float clashDuration)
    {

        int plrCoinCount = 1;
        int enemyCoinCount = 1;

        while (plrCoinCount > 0 && enemyCoinCount > 0)
        {

            combatSystem.attemptBeginTurn(engagedPlayer, engagedEnemy);

            plrCoinCount = engagedPlayer.currentcoinCount;
            enemyCoinCount = engagedEnemy.currentcoinCount;

            Debug.Log("Clash loop completed. Waiting for the next clash...");

            yield return new WaitForSeconds(clashDuration);

        }

        combatSystem.attemptresolveClash(engagedPlayer, engagedEnemy);

        yield return null;

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
