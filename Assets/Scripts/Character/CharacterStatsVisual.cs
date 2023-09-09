using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterStatsVisual : MonoBehaviour
{
    [SerializeField] private TMP_Text charName;
    [SerializeField] private RectTransform charLifeBarValue;
    [SerializeField] private RectTransform charManaBarValue;
    private Character character;

    void Start()
    {
        character = transform.parent.GetComponent<Character>();
        charName.text = character.gameObject.name;
    }

    public void UpdateBars()
    {
        float lifeToBarValue = character.GetHealth() / character.GetStats().GetMaxHealth();
        charLifeBarValue.localScale = new Vector3(lifeToBarValue, 1f, 1f);

        float manaToBarValue = character.GetMana() / character.GetStats().GetMaxMana();
        charManaBarValue.localScale = new Vector3(manaToBarValue, 1f, 1f);
    }
}
