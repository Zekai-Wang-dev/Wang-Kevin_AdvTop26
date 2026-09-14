using UnityEngine;

public class CombatManager : MonoBehaviour
{

    public static CombatManager Instance { get; private set; }

    public EngagedCharacter engagedPlayer = new EngagedCharacter();
    public EngagedCharacter engagedEnemy = new EngagedCharacter();

    public Character player;
    public Character enemy; 

    public float clashDuration = 2f;

    public CombatSystem combatSystem = new CombatSystem();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        if (Instance == null)
        {

            Instance = this;
        }
        else
        {

            Destroy(gameObject);

        }

        combatSystem.StartCombat(player, enemy, engagedPlayer, engagedEnemy);
        StartCoroutine(combatSystem.ClashLoop(engagedPlayer, engagedEnemy, clashDuration));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
