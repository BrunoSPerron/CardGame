using Godot;
using System;

public class PlayFieldScreen : BaseGameScreen
{
    private CardHand cardHand;

    public CharacterWrapper Character;

    public FieldDeckManager Deck;

    public override void _Ready()
    {
        cardHand = new CardHand();
        Deck = Character.FieldDeck;
        AddChild(cardHand);
        cardHand.Position = new Vector2(CONSTS.SCREEN_CENTER.x, CONSTS.SCREEN_SIZE.y - 25);
        DrawNewHand();
    }

    private void DrawNewHand()
    {
        cardHand.DiscardHand();
        cardHand.AddCards(Deck.MultiDraw(5));
    }

    public override void Destroy()
    {
        // TODO
    }
}
