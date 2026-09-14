using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityPipeline.Microsoft.CodeAnalysis.CSharp.Syntax;

[System.Serializable]
public class EngagedCharacter
{

    public Character character;
    public float currentHealth;

    public Skill chosenSkill;
    public int currentcoinCount; 

    public void chooseSkill(Skill skill)
    {
        chosenSkill = skill;
        Debug.Log($"{character.name} has chosen the skill: {skill.name}");
        setCurrentCoinCount();

    }

    public void chooserandomSkill()
    {
        if (character.skills.Count > 0)
        {
            int randomIndex = Random.Range(0, character.skills.Count);
            chosenSkill = character.skills[randomIndex];
            Debug.Log($"{character.name} has randomly chosen the skill: {chosenSkill.name}");
            setCurrentCoinCount();
        }
    }

    public void setCurrentCoinCount()
    {
        currentcoinCount = chosenSkill.coins.Count;
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

public class CombatSystem 
{

    public List<Character> turnOrder;

    public void PrepareCombat(Character player, Character enemy, EngagedCharacter engagedPlayer, EngagedCharacter engagedEnemy)
    {

        engagedPlayer.character = player;
        engagedEnemy.character = enemy;

        engagedPlayer.resetAll();
        engagedEnemy.resetAll();

        Debug.Log($"Combat prepared between {player.name} and {enemy.name}. Turn order will be determined based on speed.");

    }

    public void StartCombat(Character player, Character enemy, EngagedCharacter engagedPlayer, EngagedCharacter engagedEnemy)
    {
        PrepareCombat(player, enemy, engagedPlayer, engagedEnemy);
        turnOrder = ResolveTurnOrder(player, enemy);

        Debug.Log($"Combat started between {player.name} and {enemy.name}. Turn order: {turnOrder[0].name} goes first, followed by {turnOrder[1].name}.");

    }

    public void attemptBeginTurn(EngagedCharacter engagedPlayer, EngagedCharacter engagedEnemy)
    {

        if (engagedPlayer.chosenSkill != null || engagedEnemy.chosenSkill != null)
        {
            
            StartClash(engagedPlayer.character, engagedEnemy.character, engagedPlayer, engagedEnemy);
            Debug.Log($"Both {engagedPlayer.character.name} and {engagedEnemy.character.name} have chosen their skills. Clash initiated.");

        }
        else
        {

            engagedEnemy.chooserandomSkill();
            engagedPlayer.chooserandomSkill();
            StartClash(engagedPlayer.character, engagedEnemy.character, engagedPlayer, engagedEnemy);
            Debug.Log($"One or both characters did not choose a skill. Random skills have been chosen for {engagedPlayer.character.name} and {engagedEnemy.character.name}. Clash initiated.");

        }

    }

    public void StartClash(Character player, Character enemy, EngagedCharacter engagedPlayer, EngagedCharacter engagedEnemy)
    {

        Skill playerSkill = engagedPlayer.chosenSkill;
        Skill enemySkill = engagedEnemy.chosenSkill;

        int playerFinalPower = FlipCoin(player.basePower, playerSkill.coins[Random.Range(0, playerSkill.coins.Count)]);
        int enemyFinalPower = FlipCoin(enemy.basePower, enemySkill.coins[Random.Range(0, enemySkill.coins.Count)]);

        if (playerFinalPower > enemyFinalPower)
        {

            engagedEnemy.loseacoin();
            Debug.Log($"{player.name} wins the clash! {enemy.name} loses a coin.");

        }
        else
        {

            engagedPlayer.loseacoin();
            Debug.Log($"{enemy.name} wins the clash! {player.name} loses a coin.");

        }

    }

    public IEnumerator ClashLoop(EngagedCharacter engagedPlayer, EngagedCharacter engagedEnemy, float clashDuration)
    {

        int plrCoinCount = engagedPlayer.currentcoinCount;
        int enemyCoinCount = engagedEnemy.currentcoinCount;

        while (plrCoinCount > 0 || enemyCoinCount > 0)
        {

            attemptBeginTurn(engagedPlayer, engagedEnemy);

            plrCoinCount = engagedPlayer.currentcoinCount;
            enemyCoinCount = engagedEnemy.currentcoinCount;

            Debug.Log("Clash loop completed. Waiting for the next clash...");

            yield return new WaitForSeconds(clashDuration);

        }

        attemptresolveClash(engagedPlayer, engagedEnemy);

        yield return null;

    }

    public void attemptresolveClash(EngagedCharacter engagedPlayer, EngagedCharacter engagedEnemy)
    {

        int plrCoinCount = engagedPlayer.currentcoinCount;
        int enemyCoinCount = engagedEnemy.currentcoinCount;

        if (plrCoinCount <= 0)
        {

            engagedEnemy.takeDamage(resolveFinalDamage(engagedPlayer, plrCoinCount));
            Debug.Log($"{engagedPlayer.character.name} has no coins left. {engagedPlayer.character.name} takes damage.");
            engagedPlayer.resetSkill();
            engagedEnemy.resetSkill();

        }
        else if (enemyCoinCount <= 0)
        {
            engagedPlayer.takeDamage(resolveFinalDamage(engagedEnemy, enemyCoinCount));
            Debug.Log($"{engagedEnemy.character.name} has no coins left. {engagedEnemy.character.name} takes damage.");
            engagedPlayer.resetSkill();
            engagedEnemy.resetSkill();

        }

    }

    public float resolveFinalDamage(EngagedCharacter engagedCharacter, int coinCount)
    {

        float finalDamage = 0;

        for (int i = 0; i < coinCount; i++)
        {

            finalDamage += engagedCharacter.chosenSkill.coins[i].coinPower;

        }

        Debug.Log($"{engagedCharacter.character.name} has {coinCount} coins left. Total damage calculated: {finalDamage}");

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
        else 
        {
            finalPower = (int)(basePower/3 - coin.coinPower);
        }

        Debug.Log($"Coin flip result: {coinFlip}. Base power: {basePower}, Coin power: {coin.coinPower}, Final power: {finalPower}");

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

        Debug.Log($"Turn order resolved: {turnOrder[0].name} goes first, followed by {turnOrder[1].name}.");

        return turnOrder;

    }

}
