using UnityEngine;

[CreateAssetMenu]

public class CharacterStatHealthModifierSO : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        PlayerController playerController = character.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.GainHealth((int)val);
        Debug.Log(character + " restores " + val + " health");
    }
}