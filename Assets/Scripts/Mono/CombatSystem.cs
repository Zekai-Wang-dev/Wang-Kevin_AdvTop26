using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPipeline.Microsoft.CodeAnalysis.CSharp.Syntax;

public class EngagedCharacter
{

    public Character character;
    public float currentHealth;

    public Skill chosenSkill;
    public int currentcoinCount; 

    public void chooseSkill(Skill skill)
    {
        chosenSkill = skill;
    }

    public void chooserandomSkill()
    {
        if (character.skills.Count > 0)
        {
            int randomIndex = Random.Range(0, character.skills.Count);
            chosenSkill = character.skills[randomIndex];
        }
    }

    public void takeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
    }

    public void resetHealth()
    {
        currentHealth = character.health;
    }

    public void resetSkill()
    {
        chosenSkill = null;
    }

    public void resetCoinCount()
    {
        
        if (chosenSkill != null)
        {

            currentcoinCount = chosenSkill.coins.Count;

        }

    }

    public void loseacoin()
    {

        currentcoinCount--;

        if (currentcoinCount < 0)
        {

            currentcoinCount = 0;
        }

    }

    public void resetAll()
    {
        resetHealth();
        resetSkill();

    }


}

public class CombatSystem : MonoBehaviour
{

    public EngagedCharacter engagedPlayer = new EngagedCharacter();
    public EngagedCharacter engagedEnemy = new EngagedCharacter();

    public float clashDuration = 2f; // Duration of the clash in seconds

    public List<Character> turnOrder = new List<Character>();

    public void PrepareCombat(Character player, Character enemy)
    {

        engagedPlayer.character = player;
        engagedEnemy.character = enemy;

        engagedPlayer.resetAll();
        engagedEnemy.resetAll();

    }

    public void StartCombat(Character player, Character enemy)
    {
        PrepareCombat(player, enemy);
        turnOrder = ResolveTurnOrder(player, enemy);


    }

    public void attemptBeginTurn()
    {

        if (engagedPlayer.chosenSkill != null || engagedEnemy.chosenSkill != null)
        {
            engagedEnemy.chooserandomSkill();

            StartCoroutine(ClashLoop());
        }
        else
        {

            Debug.Log("Both characters must choose a skill before starting the clash.");

        }

    }

    public void StartClash(Character player, Character enemy)
    {

        Skill playerSkill = engagedPlayer.chosenSkill;
        Skill enemySkill = engagedEnemy.chosenSkill;

        int playerFinalPower = FlipCoin(player.basePower, playerSkill.coins[Random.Range(0, playerSkill.coins.Count)]);
        int enemyFinalPower = FlipCoin(enemy.basePower, enemySkill.coins[Random.Range(0, enemySkill.coins.Count)]);

        if (playerFinalPower > enemyFinalPower)
        {

            engagedEnemy.loseacoin();

        }
        else
        {

            engagedPlayer.loseacoin();

        }

    }

    public IEnumerator ClashLoop()
    {

        StartClash(engagedPlayer.character, engagedEnemy.character);

        int plrCoinCount = engagedPlayer.currentcoinCount;
        int enemyCoinCount = engagedEnemy.currentcoinCount;

        if (plrCoinCount <= 0 || enemyCoinCount <= 0)
        {
            attemptresolveClash();
            yield return null;

        }

        yield return new WaitForSeconds(clashDuration);

    }

    public void attemptresolveClash()
    {

        int plrCoinCount = engagedPlayer.currentcoinCount;
        int enemyCoinCount = engagedEnemy.currentcoinCount;

        if (plrCoinCount <= 0)
        {

            engagedEnemy.takeDamage(resolveFinalDamage(engagedPlayer, plrCoinCount));
            
        }
        else if (enemyCoinCount <= 0)
        {

            engagedPlayer.takeDamage(resolveFinalDamage(engagedEnemy, enemyCoinCount));

        }

    }

    public float resolveFinalDamage(EngagedCharacter engagedCharacter, int coinCount)
    {

        float finalDamage = 0;

        for (int i = 0; i < coinCount; i++)
        {

            finalDamage += engagedCharacter.chosenSkill.coins[i].coinPower;

        }

        return finalDamage;
    }

    private int FlipCoin(float basePower, Coin coin)
    {
        int finalPower = 0; 
        int coinFlip = Random.Range(0, 2); // Generates a random number between 0 and 1

        if (coinFlip == 0)
        {

            finalPower = (int)(basePower/3 + coin.coinPower); 

        }

        return finalPower; 

    }

    public List<Character> ResolveTurnOrder(Character player, Character enemy)
    {

        List<Character> turnOrder = new List<Character>();

        if (player.speed > enemy.speed)
        {
            turnOrder.Add(player);
            turnOrder.Add(enemy);
        }
        else if (player.speed < enemy.speed)
        {
            turnOrder.Add(enemy);
            turnOrder.Add(player);
        }
        else
        {

            if (Random.value > 0.5f)
            {
                turnOrder.Add(player);
                turnOrder.Add(enemy);
            }
            else
            {
                turnOrder.Add(enemy);
                turnOrder.Add(player);
            }
        }

        return turnOrder;

    }

}
