using UnityEngine;

[CreateAssetMenu]
public class CharacterStatHealthModifierSO : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        // Health health = character.GetComponent<Health>();
        // if(health != null)
        // health.AddHealth((int)val);
        Debug.Log(character + " restores " + val + " health");
    }
}
