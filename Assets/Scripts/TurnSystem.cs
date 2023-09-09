using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }
    public event EventHandler OnSelectedCharacterChange;
    public event EventHandler OnSelectedSkillChange;
    public event EventHandler OnTurnChange;

    [SerializeField] private Character selectedCharacter;
    [SerializeField] private LayerMask characterLayerMask;
    private BaseSkill selectedSkill;

    private enum Turn
    {
        NULL,
        PLAYER,
        ENEMY
    }
    private Turn actualTurn = Turn.NULL;

    private int turnsCount = 0;

    [SerializeField] private List<Character> playerCharacters;
    [SerializeField] private List<Character> enemyCharacters;
    [SerializeField] private List<Character> charactersToPlayThisTurn;


    private bool isBusy;

    private void Awake() 
    {
        if(Instance != null)
        {
            Debug.LogError("Two TurnSystem was instanciated. Destroying one! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() 
    {
        foreach(Character character in FindObjectsOfType<Character>())
        {
            AddCharacter(character);
        }

        ChangeTurn();
    }

    private void AddCharacter(Character character)
    {
        if(character.OwnedByPlayer())
            playerCharacters.Add(character);
        else
            enemyCharacters.Add(character);
    }

    public void RemoveCharacter(Character character)
    {
        if(character.OwnedByPlayer())
            playerCharacters.Remove(character);
        else
            enemyCharacters.Remove(character);
        
        if(charactersToPlayThisTurn.Contains(character))
            charactersToPlayThisTurn.Remove(character);
    }

    public List<Character> GetPlayerCharacters() => playerCharacters;
    public List<Character> GetEnemyCharacters() => enemyCharacters;
    public List<Character> GetOpositeCharacters(Character character) => character.OwnedByPlayer() ? enemyCharacters : playerCharacters;
    
    public List<Character> GetCharactersStillToPlayThisTurn() => charactersToPlayThisTurn;
    public int GetTurnCount() => turnsCount;

    public void SkipTurn()
    {
        ChangeTurn();
    }

    private void ChangeTurn()
    {
        switch(actualTurn)
        {
            case Turn.NULL:
                actualTurn = Turn.PLAYER;
                charactersToPlayThisTurn = new List<Character>(playerCharacters);
                break;
            case Turn.PLAYER:
                actualTurn = Turn.ENEMY;
                charactersToPlayThisTurn = new List<Character>(enemyCharacters);
                break;
            case Turn.ENEMY:
                actualTurn = Turn.PLAYER;
                charactersToPlayThisTurn = new List<Character>(playerCharacters);
                break;
        }

        Debug.Log("END OF TURN " + turnsCount + "\n" + "Next Turn is for: "+ actualTurn.ToString());
        turnsCount++;

        OnTurnChange?.Invoke(this, EventArgs.Empty);
    }

    public bool CharacterHasMoveThisTurn(Character character) => charactersToPlayThisTurn.Contains(character);

    public bool IsPlayerTurn() => actualTurn == Turn.PLAYER;

    void Update()
    {
        if(isBusy)
            return;

        if(actualTurn != Turn.PLAYER)
            return;

        if(charactersToPlayThisTurn.Count <= 0)
        {
            ChangeTurn();
            return;
        }

        HandlePlayerturn();
    }

    private void HandlePlayerturn()
    {
        if(EventSystem.current.IsPointerOverGameObject())
            return;

        if(TryHandleCharacterSelection()) 
            return;

        if(selectedCharacter == null)
            return;

        if(selectedSkill == null)
            return;
        
        HandleSelectedSkill();
    }

    private bool TryHandleCharacterSelection()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, characterLayerMask))
            {
                if(raycastHit.transform.TryGetComponent<Character>(out Character character))
                {
                    if(character == selectedCharacter)
                        return false;
                    
                    if(!character.OwnedByPlayer())
                        return false;

                    if(!CharacterHasMoveThisTurn(character))
                        return false;

                    if(selectedSkill != null)
                    {
                        if(selectedSkill.IsValidSkillGridPosition(character.GetGridPosition()))
                            return false;
                    }
                    
                    SetSelectedCharacter(character);
                    return true;
                }
            }
        }
        return false;
    }

    private void HandleSelectedSkill()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseRaycast.GetPosition());
            if(selectedSkill.IsValidSkillGridPosition(mouseGridPosition))
            {
                SetBusy();
                selectedSkill.Cast(mouseGridPosition, ClearBusy);
                ClearSelected();
            }
        }
    }

    private void SetBusy() => isBusy = true;

    private void ClearBusy()
    {
        isBusy = false;
    }

    public void CastCharacterTurn(Character character)
    {
        charactersToPlayThisTurn.Remove(character);
    }

    private void ClearSelected()
    {
        CastCharacterTurn(selectedCharacter);
        SetSelectedCharacter(null);
        SetSelectedSkill(null);
    }

    private void SetSelectedCharacter(Character character)
    {
        selectedCharacter = character;

        OnSelectedCharacterChange?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedSkill(BaseSkill baseSkill)
    {
        selectedSkill = baseSkill;

        OnSelectedSkillChange?.Invoke(this, EventArgs.Empty);
    }

    public Character GetSelectedCharacter()
    {
        return selectedCharacter;
    }

    public BaseSkill GetSelectedSkill()
    {
        return selectedSkill;
    }
}
