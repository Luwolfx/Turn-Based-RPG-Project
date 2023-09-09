using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    private GridSystem gridSystem;
    private GridPosition gridPosition;
    private List<Character> characterList;

    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        characterList = new List<Character>();
    }

    public GridPosition GetGridPosition() => gridPosition;

    public void AddCharacter(Character character) => characterList.Add(character);

    public void RemoveCharacter(Character character) => characterList.Remove(character);

    public List<Character> GetCharacterList() => characterList;

    public override string ToString()
    {
        string characterString = "";
        foreach(Character character in characterList)
        {
            characterString += character + "\n";
        }
        return gridPosition.ToString() + "\n" + characterString;
    }

    public bool HasAnyCharacters()=>  characterList.Count > 0;

    public bool HasPlayerCharacters()=>  characterList.Find(x => x.gameObject.tag == "Player");

    public bool HasEnemyCharacters()=>  characterList.Find(x => x.gameObject.tag == "Enemy");

    public bool HasGround()
    {
        Vector3 checkArea = LevelGrid.Instance.GetWorldPosition(gridPosition);
        List<bool> checks = new List<bool>();

        checks.Add(CheckCollision(checkArea, 1 << 6));
        checks.Add(CheckCollision(checkArea + Vector3.right * .7f, 1 << 6));
        checks.Add(CheckCollision(checkArea - Vector3.right * .7f, 1 << 6));
        checks.Add(CheckCollision(checkArea + Vector3.forward * .7f, 1 << 6));
        checks.Add(CheckCollision(checkArea - Vector3.forward * .7f, 1 << 6));

        if(checks.Contains(false))
            return false;
        else
            return true;
    }

    public bool CheckCollision(LayerMask layer)
    {
        Vector3 checkArea = LevelGrid.Instance.GetWorldPosition(gridPosition);
        List<bool> checks = new List<bool>();

        checks.Add(CheckCollision(checkArea, layer));
        checks.Add(CheckCollision(checkArea + Vector3.right * .7f, layer));
        checks.Add(CheckCollision(checkArea - Vector3.right * .7f, layer));
        checks.Add(CheckCollision(checkArea + Vector3.forward * .7f, layer));
        checks.Add(CheckCollision(checkArea - Vector3.forward * .7f, layer));

        if(checks.Contains(false))
            return false;
        else
            return true;
    }

    public bool CheckCollision(Vector3 position, LayerMask layer)
    {
        Collider[] hitColliders = Physics.OverlapBox(position, new Vector3(.01f, .2f, .01f), Quaternion.identity, layer);

        if(hitColliders.Length > 0)
            return true;
        else
            return false;

    }

}
