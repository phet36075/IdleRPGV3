using System.Collections;
using UnityEngine;
public class PlayerManager : MonoBehaviour
{
    public PlayerData playerData;
    public Animator allyAnimator;
    public float currentHealth;
    public DamageDisplay damageDisplay;
    public Animator animator;
    public GameObject regenEffect;
    public GameObject hitVFX;
    public Transform spawnVFXPosition;
    public Transform spawnRegenPosition;
    public bool isCritical;
    public CharacterHitEffect hitEffect;
    
    private PlayerController _playerController;
    private AIController _aiController;
    private AllyRangedCombat _allyRangedCombat;
    public GameObject gameOverUI;
    //public float regenRate = 1f;
    
    public float regenInterval = 5f;
    public AudioManager audioManager;

    public bool isPlayerDie;
    void Start()
    {
        StartCoroutine(RegenerateHp());
        _allyRangedCombat = FindObjectOfType<AllyRangedCombat>();
        _playerController = FindObjectOfType<PlayerController>();
        //hitEffect = GetComponent<CharacterHitEffect>();
       currentHealth = playerData.currentHealth;
      //  _damageDisplay = FindObjectOfType<DamageDisplay>();
        _aiController = FindObjectOfType<AIController>();
       // _audioManager = FindObjectOfType<AudioManager>();
    }
    private IEnumerator RegenerateHp()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenInterval);

            if (currentHealth < playerData.maxHealth)
            {
                Quaternion rotation = Quaternion.Euler(-90f, 0, 0f);
                GameObject regenEfx = Instantiate(regenEffect, spawnRegenPosition.position,rotation );
                Destroy(regenEfx, 1f);
                
                currentHealth += playerData.regenRate;
                currentHealth = Mathf.Min(currentHealth, playerData.maxHealth);
                Debug.Log("Current HP: " + currentHealth);
            }
        }
        // ReSharper disable once IteratorNeverReturns
    }
    
    
    public float CalculatePlayerAttackDamage(float skillDamageMultiplier = 1f)
    {
        // Start with base damage
        float attackDamage = (playerData.baseDamage + playerData.weaponDamage) * skillDamageMultiplier;
        
        // Apply damage variation
        float variationMultiplier = Random.Range(1 - playerData.damageVariation, 1 + playerData.damageVariation);
        attackDamage *= variationMultiplier;

        // Apply critical hit
        isCritical = Random.value < playerData.criticalChance;
        if (isCritical)
        {
            attackDamage *= playerData.criticalDamage; // Double damage for critical hit
            Debug.Log("Critical Hit!");
        }
        
        Debug.Log($"Attack Damage: {attackDamage} (Base: {playerData.baseDamage}, Skill Multiplier: {skillDamageMultiplier}, Critical: {isCritical})");
        return attackDamage;
    }
    public void TakeDamage(float incomingDamage , float attackerArmorPenetration )
    {
        hitEffect.StartHitEffect();
        
       // Apply armor penetration
       float effectiveDefense = Mathf.Max(0, playerData.defense - attackerArmorPenetration);

       // Calculate damage reduction
       float damageReduction = effectiveDefense / (effectiveDefense + 100f);

       // Calculate final damage
       float finalDamage = incomingDamage * (1f - damageReduction);
       
        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(currentHealth, 0f); // Ensure health doesn't go below 0

        Debug.Log($"Player took {finalDamage} damage. Current health: {currentHealth}");
        
        audioManager.PlayHitSound();
        damageDisplay.DisplayDamage(finalDamage);
           Quaternion rotation = Quaternion.Euler(-90f, 0, 0f);
            GameObject effect = Instantiate(hitVFX, spawnVFXPosition.position,rotation );
            Destroy(effect, 1f);
            
        if (currentHealth <= 0)
        {
           Die();
        }
        
        
    }

    public void Heal(float amount)
    {
        currentHealth +=amount;
        if (currentHealth >= playerData.maxHealth)
        {
            currentHealth = playerData.maxHealth;
        }
    }

    public void ChangeWeapon(float weaponDmg)
    {
        playerData.weaponDamage = weaponDmg;
    }
    void Die()
    {
        isPlayerDie = true;
       // currentHealth = PlayerData.maxHealth;
       animator.SetTrigger("Die");
       _allyRangedCombat.Die();
       _aiController.enabled = false;
       GetComponent<CapsuleCollider>().enabled = false;
       GetComponent<CharacterController>().enabled = false;
       _playerController.isAIActive = false;
       _playerController.enabled = false;
       gameOverUI.gameObject.SetActive(true);
       
    }

    public void ResetDie()
    {
        StartCoroutine(WaitLoading());
    }

    public IEnumerator WaitLoading()
    {
        yield return new WaitForSeconds(1.0f);
        animator.SetTrigger("Reset");
        allyAnimator.SetTrigger("Reset");
        yield return new WaitForSeconds(0.5f);
        isPlayerDie = false;
       // GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<CharacterController>().enabled = false;
        _playerController.isAIActive = true;
        _playerController.isAIEnabled = true;
        _playerController.enabled = true;
    }
}
