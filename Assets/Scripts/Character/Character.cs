using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private CharacterStats stats;
    [SerializeField] private CharacterStatsVisual visualStats;
    private GridPosition gridPosition;
    private Move movement;
    private BaseSkill[] skillsArray;

    private float damageTaken;
    private float manaUsed;

    private void Awake() 
    {
        movement = GetComponent<Move>();
        damageTaken = manaUsed = 0;
        UpdateSkills();
    }

    public void UpdateSkills()
    {
        skillsArray = GetComponents<BaseSkill>();
    }

    void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddCharacterAtGridPosition(gridPosition, this);
    }

    private void Update() 
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(newGridPosition != gridPosition)
        {
            LevelGrid.Instance.CharacterMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }

    public Move GetMoveSkill()
    {
        return movement;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public BaseSkill[] GetSkills()
    {
        return skillsArray;
    }

    public CharacterStats GetStats()
    {
        return stats;
    }

    public bool OwnedByPlayer()
    {
        return gameObject.tag == "Player";
    }

    public bool HasSkillName(string skillName)
    {
        return skillsArray.ToList().Find(x => x.GetSkillName() == skillName);
    }

    public bool HasSkill(System.Type targetSkill)
    {
        foreach(BaseSkill skill in skillsArray)
        {
            if(skill.GetType() == targetSkill)
                return true;
        }

        return false;
    }

    public float GetHealth() => stats.GetMaxHealth() - damageTaken;

    public float GetMana() => stats.GetMaxMana() - manaUsed;

    public void UseMana(float manaUsed)
    {
        this.manaUsed += manaUsed;

        visualStats.UpdateBars();
    }

    public void HealDamage(float healAmount)
    {
        damageTaken -= healAmount;

        if(damageTaken < 0) 
            damageTaken = 0;
        
        visualStats.UpdateBars();
    }

    public void TakeDamage(float damage)
    {
        damageTaken += damage;

        if(GetHealth() <= 0)
            Die();
        else
            visualStats.UpdateBars();
    }

    private void Die()
    {
        TurnSystem.Instance.RemoveCharacter(this);
        LevelGrid.Instance.RemoveCharacterOfGrid(this);
        gameObject.SetActive(false);
    }
}
