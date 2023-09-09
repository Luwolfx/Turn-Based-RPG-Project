using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : BaseSkill
{
    [SerializeField] [Range(1, 10)] private int attackRange = 5;
    [SerializeField] private float hitChance = 100;
    [SerializeField] private float mpRequired = 4;
    private Vector3 targetPosition;
    private Character caster;
    public override void Cast(GridPosition gridPosition, Action onCastComplete)
    {
        caster = TurnSystem.Instance.GetSelectedCharacter();

        base.StartCast(onCastComplete);
        targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        StartCoroutine(SkillHandler(gridPosition));
    }

    public override string GetSkillName() => "Heal";

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

                if(LevelGrid.Instance.HasOpositeCharacterOnGrid(character, testGridPosition))
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

        foreach(Character character in LevelGrid.Instance.GetCharacterListAtGridPosition(gridPosition))
        {
            character.HealDamage(2 * caster.GetStats().GetInteligence());
        }
        
        base.EndCast();
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        // EMPTY
        // ENEMY DOES NOT HEAL HIMSELF
        throw new NotImplementedException();
    }
}
