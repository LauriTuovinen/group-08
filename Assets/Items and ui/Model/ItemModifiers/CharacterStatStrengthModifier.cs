using UnityEngine;

[CreateAssetMenu]

public class CharacterStatStrengthModifier :  CharacterStatModifierSO
{
     public override void AffectCharacter(GameObject character, float val)
    {
        // Strength str = character.GetComponent<Strength>();
        // if(str != null)
        // str.GainStrength((int)val);
        Debug.Log(character + "gains " + val + " str");
    }
}
