using System;
using Godot;


public class TestScreen : BaseGameScreen
{
    public override void Destroy()
    {
        
    }

    public override void _Ready()
    {
        CharacterShowcase(5);
    }

    private void CharacterShowcase(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CharacterCreationModel c = JsonLoader.GetCharacterCreationModel("basegame", "BaseCharacter");
            CharacterWrapper wrapper = CardFactory.CreateFrom(CharacterCreator.CreateFromModel(c));
            DealOnBoard(wrapper.Card, new Vector2(150 + i * 98, 150));
        }
    }
}
