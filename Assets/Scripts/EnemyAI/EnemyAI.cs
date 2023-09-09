using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    private State state;
    private float timer;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
            return;

        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    } 
                    else
                    {
                        TurnSystem.Instance.SkipTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f;
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        foreach (Character enemyCharacter in  TurnSystem.Instance.GetEnemyCharacters())
        {
            if (TryTakeEnemyAIAction(enemyCharacter, onEnemyAIActionComplete))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryTakeEnemyAIAction(Character enemyCharacter, Action onEnemyAIActionComplete)
    {
        if(!TurnSystem.Instance.GetCharactersStillToPlayThisTurn().Contains(enemyCharacter))
        {
            Debug.Log(enemyCharacter.name + " não possui Jogadas!");
            return false;
        }

        EnemyAIAction bestEnemyAIAction = null;
        BaseSkill bestBaseSkill = null;

        foreach (BaseSkill baseAction in enemyCharacter.GetSkills())
        {
            if(enemyCharacter.GetMana() < baseAction.GetManaPointsRequired())
            {
                Debug.Log(enemyCharacter.name + " não tem mana ("+ enemyCharacter.GetMana() +") para " + baseAction.GetSkillName());
                continue;
            }

            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseSkill = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseSkill = baseAction;
                }
            }
        }

        if (bestEnemyAIAction != null)
        {
            bestBaseSkill.Cast(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            TurnSystem.Instance.CastCharacterTurn(enemyCharacter);
            return true;
        }
        else
        {

            return false;
        }
    }

}
