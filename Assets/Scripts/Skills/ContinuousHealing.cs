using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ContinuousHealing : BaseSkill
{
    [SerializeField] [Range(1, 10)] private int attackRange = 5;
    [SerializeField] private float hitChance = 100;
    [SerializeField] private float mpRequired = 6;
    private Vector3 targetPosition;

    public override void Cast(GridPosition gridPosition, Action onCastComplete)
    {
        base.StartCast(onCastComplete);
        targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        StartCoroutine(SkillHandler(gridPosition));
    }

    public override string GetSkillName() => "Continuous Healing";

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

                if(LevelGrid.Instance.GetCharacterListAtGridPosition(testGridPosition).Find(x => x.HasSkill(typeof(HealingBuff))))
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
            HealingBuff healingBuff = character.gameObject.AddComponent<HealingBuff>();
            healingBuff.SetCaster(this.character);
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
