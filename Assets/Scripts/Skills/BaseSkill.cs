using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkill : MonoBehaviour
{
    protected Action onCastComplete;

    protected Character character;
    protected CharacterAnimation characterAnimation;
    protected bool isCasting;

    public abstract string GetSkillName();

    public virtual bool IsPassiveSkill() => false;

    public abstract void Cast(GridPosition gridPosition, Action onCastComplete);

    public abstract List<GridPosition> GetValidGridPositionList();

    public virtual bool IsValidSkillGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public virtual float GetHitChance(float hitRatio, float targetDodge)
    {
        float chanceToHit = 100;
        return chanceToHit + hitRatio - targetDodge;
    }

    public virtual bool GetIfEnemyWasHit(Character targetCharacter)
    {
        float characterHitRatio = character.GetStats().GetHitRatio();
        float targetDodgeChance = targetCharacter.GetStats().GetDodgeChance();

        float hitChance = GetHitChance(characterHitRatio, targetDodgeChance)/100;
        bool isHit = UnityEngine.Random.value < hitChance;

        // Debug.Log(GetSkillName()+ " has " +hitChance+ "% of chance of hitting target!  ( Hitted?"+isHit+" )");
        return isHit;
    }

    public virtual float GetManaPointsRequired() => 0;

    protected virtual void Awake() 
    {
        character = GetComponent<Character>();
        characterAnimation = GetComponent<CharacterAnimation>();
    }

    protected virtual void StartCast(Action onCastComplete)
    {
        this.onCastComplete = onCastComplete;
        isCasting = true;
    }

    protected virtual void EndCast()
    {
        isCasting = false;
        onCastComplete();
    }

    protected virtual bool IsCasting() => isCasting;

    public virtual EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        List<GridPosition> validActionGridPositionList = GetValidGridPositionList();

        foreach (GridPosition gridPosition in validActionGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }

        if (enemyAIActionList.Count > 0)
        {
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
            return enemyAIActionList[0];
        } else
        {
            // No possible Enemy AI Actions
            return null;
        }
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

}
