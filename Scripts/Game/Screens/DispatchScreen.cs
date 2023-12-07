using Godot;
using System;
using System.Collections.Generic;

public class DispatchScreen : BaseGameScreen
{
    private readonly List<CharacterWrapper> survivors = new List<CharacterWrapper>();
    private readonly List<LocationWrapper> locations = new List<LocationWrapper>();

    private Dictionary<ulong, CharacterWrapper> charactersByCardId = new Dictionary<ulong, CharacterWrapper>();
    private Dictionary<ulong, LocationWrapper> locationsByCardId = new Dictionary<ulong, LocationWrapper>();

    private Button processButton;

    // ===== GD Methods Override =====

    public override void _Ready()
    {
        processButton = AddButton("Proceed", "OnClickProceed", Cardinal.SE, new Vector2(-10, -10));
        UpdateButtons();
    }

    // ===== Methods unique to this class =====

    public void AddLocation(LocationWrapper cardWrapper)
    {
        Vector2 reservePosition = GetNode<Node2D>("CharacterReserve").Position;
        locations.Add(cardWrapper);
        locationsByCardId.Add(cardWrapper.Card.GetInstanceId(), cardWrapper);
        DealOnBoard(cardWrapper.Card, reservePosition);
        RepositionLocations();
    }

    public void AddLocation(LocationModel location)
    {
        LocationWrapper locationWrapper = CardFactory.CreateCardFromLocation(
            Game.State.Scenario, location);
        AddLocation(locationWrapper);
    }

    public void AddSurvivor(CharacterWrapper cardWrapper)
    {
        Vector2 reservePosition = GetNode<Node2D>("CharacterReserve").Position;
        survivors.Add(cardWrapper);
        charactersByCardId.Add(cardWrapper.Card.GetInstanceId(), cardWrapper);
        cardWrapper.Card.Connect("OnDragStart", this, "OnCarddragStart");
        cardWrapper.Card.Connect("OnDragEnd", this, "OnCarddragEnd");
        cardWrapper.Card.Connect("OnCombatDeckClick", this, "OnCombatDeckClick");
        cardWrapper.Card.Connect("OnFieldDeckClick", this, "OnFieldDeckClick");

        DealOnBoard(cardWrapper.Card, reservePosition);
        UpdateButtons();
    }

    public void AddSurvivor(CharacterModel survivor)
    {
        AddSurvivor(CardFactory.CreateCardFromCharacter(survivor));
    }

    public override void Destroy()
    {
        foreach (CharacterWrapper character in survivors)
            RemoveChild(character.Card);
        foreach (LocationWrapper location in locations)
            RemoveChild(location.Card);
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
        if (StackTarget == null)
        {
            CharacterWrapper character = charactersByCardId[OriginCard.GetInstanceId()];
            /*if (character.Location != null)
            {
                character.Location.Characters.Add(character);
            }*/
        }
        else
        {
            LocationWrapper location = locationsByCardId[StackTarget.GetInstanceId()];
            CharacterWrapper character = charactersByCardId[OriginCard.GetInstanceId()];
            //character.Location?.Characters.Remove(character);
            //character.Location = location;
            location.Characters.Add(character);
            UpdateButtons();
        }
    }

    public void OnCarddragStart(Card card)
    {
        CharacterWrapper character = charactersByCardId[card.GetInstanceId()];
        /*if (character.Location != null)
        {
            character.Location.Characters.Remove(character);
        }*/
    }

    public void OnClickProceed()
    {
        Game.OnDispatch();
    }

    public void OnCombatDeckClick(Card card)
    {
        OpenDeckModificationPanel(charactersByCardId[card.GetInstanceId()].CombatDeck);
    }
    
    public void OnFieldDeckClick(Card card)
    {
        OpenDeckModificationPanel(charactersByCardId[card.GetInstanceId()].FieldDeck);
    }

    private void RepositionLocations()
    {
        Vector2 rootSize = GetTree().Root.GetVisibleRect().Size;
        GetNode<Position2D>("CharacterReserve").Position = new Vector2(
            rootSize.x / 2,
            rootSize.y - rootSize.y / 3);


        // TODO replace this bit
        Vector2 testposition = GetNode<Node2D>("testposition").Position;
        for (int i = 0; i < locations.Count; i++)
        {
            locations[i].Card.MoveToPosition(testposition + Vector2.Right * i * 100);
        }
    }

    protected override void UpdateButtons()
    {
        bool isEnabled = true;
        foreach (CharacterWrapper survivor in survivors)
        {
            /*if (survivor.Location == null)
            {
                isEnabled = false;
                break;
            }*/
        }
        if (isEnabled)
            processButton.Enable();
        else
            processButton.Disable();

    }
}
