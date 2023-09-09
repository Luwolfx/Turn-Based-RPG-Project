using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : BaseSkill
{
    [SerializeField] [Range(1, 10)] private int attackRange = 8;
    [SerializeField] private float hitChance = 60;
    [SerializeField] private float mpRequired = 3;
    private Vector3 targetPosition;

    public override void Cast(GridPosition gridPosition, Action onCastComplete)
    {
        base.StartCast(onCastComplete);
        targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        StartCoroutine(SkillHandler(gridPosition));
    }

    public override string GetSkillName() => "Fireball";

    public override float GetHitChance(float hitRatio, float targetDodge)
    {
        return hitChance + hitRatio - targetDodge;
    }

    public override float GetManaPointsRequired() => mpRequired;

    public override List<GridPosition> GetValidGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition characterGridPosition = character.GetGridPosition();

        for(int x = -attackRange; x <= attackRange; x++)
        {
            for(int z = -attackRange; z <= attackRange; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = characterGridPosition + offsetGridPosition;

                if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                if(characterGridPosition == testGridPosition)
                    continue;

                if(!LevelGrid.Instance.HasOpositeCharacterOnGrid(character, testGridPosition))
                    continue;

                if(!LevelGrid.Instance.HasGroundOnGridPosition(testGridPosition))
                    continue;
                
                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public IEnumerator SkillHandler(GridPosition gridPosition)
    {
        character.GetMoveSkill().FaceTarget(targetPosition);
        characterAnimation.TriggerAnimation("MagicAttack");
        yield return new WaitForSeconds(1f);

        character.UseMana(mpRequired);

        List<Character> charactersToAttack = new List<Character>(LevelGrid.Instance.GetCharacterListAtGridPosition(gridPosition));
        foreach(Character targetCharacter in charactersToAttack)
        {
            if(!base.GetIfEnemyWasHit(targetCharacter))
                continue;

            targetCharacter.TakeDamage(2 + character.GetStats().GetInteligence());

            TryBurnCharacter(targetCharacter);
        }
        

        base.EndCast();
    }

    public void TryBurnCharacter(Character target)
    {
        if(target != null)
        {
            float chanceToBeingBurned = 30f;
            if (UnityEngine.Random.value < chanceToBeingBurned/100)
                target.gameObject.AddComponent<BurnDebuff>();
        }
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Character targetCharacter = LevelGrid.Instance.GetCharacterAtGridPosition(gridPosition);
        
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100,
        };
    }
}
