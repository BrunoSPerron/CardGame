using Godot;
using System;
using System.Collections.Generic;

public class ExplorationScreen : BaseGameScreen
{
    private readonly List<CharacterWrapper> survivors = new List<CharacterWrapper>();

    private CharacterWrapper survivorDragged;

    public ExplorationManager Parent;
    public LocationWrapper Location;

    public Card ExploreTarget;

    public override void _Ready()
    {
        if (Location.Model.ExplorationDeck.Length != 0)
            AddExploreOption();
    }

    private void AddExploreOption()
    {
        ExploreTarget = CardFactory.CreateExploreCard();
        DealOnBoard(ExploreTarget, new Vector2(300, 198));
    }

    public void AddSurvivor(CharacterWrapper cardWrapper)
    {
        Vector2 reservePosition = new Vector2(200, 200); //TODO - Better

        cardWrapper.Card.Connect("OnDragStart", this, "OnCarddragStart");
        cardWrapper.Card.Connect("OnDragEnd", this, "OnCarddragEnd");
        survivors.Add(cardWrapper);
        DealOnBoard(cardWrapper.Card, reservePosition);
    }

    public override void Destroy()
    {
        foreach (CharacterWrapper character in survivors)
            RemoveChild(character.Card);
        RemoveChild(Location.Card);
        QueueFree();
    }

    public override void DisableScreen()
    {
        base.DisableScreen();
    }

    public override void EnableScreen()
    {
        base.EnableScreen();
    }

    public void OnCarddragEnd(Card OriginCard, Card StackTarget)
    {
        if (StackTarget == ExploreTarget)
        {
            survivors.Add(survivorDragged);
            survivorDragged = null;
            CardManager.StackCards(new List<Card> { OriginCard }, StackTarget.Position);
            //TODO PROCESS EXPLORE
        }
        else
        {
            survivors.Add(survivorDragged);
            survivorDragged = null;
            StackSurvivors();
        }
    }

    public void OnCarddragStart(Card card)
    {
        survivorDragged = Parent.charactersByCardId[card.GetInstanceId()];
        survivors.Remove(survivorDragged);
        StackSurvivors();
    }

    public void SetLocation(LocationWrapper location)
    {
        if (Location != null)
            RemoveChild(Location.Card);
        Location = location;
        DealOnBoard(Location.Card, new Vector2(180, 180));
    }

    public void StackSurvivors()
    {
        List<Card> cards = CardManager.GetCardsInCharacterWrappers(survivors);
        CardManager.StackCards(cards, Location.Card.Position);
    }

    protected override void UpdateButtons()
    {
        //throw new NotImplementedException();
    }

    private void RepositionTargets()
    {
        Vector2 rootSize = GetTree().Root.GetVisibleRect().Size;
        GetNode<Position2D>("CharacterReserve").Position = new Vector2(
            rootSize.x / 2,
            rootSize.y - rootSize.y / 3);

        /*
        Vector2 testposition = GetNode<Node2D>("testposition").Position;
        for (int i = 0; i < locations.Count; i++)
        {
            locations[i].Card.MoveToPosition(testposition + Vector2.Right * i * 100);
        }*/
    }
}
