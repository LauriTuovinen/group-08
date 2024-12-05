using UnityEngine;

[CreateAssetMenu]

public class CharacterStatStrengthModifier : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        PlayerController playerController = character.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.GainStrength((int)val);
        Debug.Log(character + "gains " + val + " str");
    }
}