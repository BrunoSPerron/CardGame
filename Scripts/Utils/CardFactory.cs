using Godot;
using System;


public static class CardFactory
{

    private static PackedScene cardScene;
    public static PackedScene CardScene
    {
        get
        {
            if (cardScene == null)
                cardScene = ResourceLoader.Load<PackedScene>
                    ("res://Assets/Card.tscn");
            return cardScene;
        }
    }

    private static Texture deckIconTexture_rest;
    public static Texture DeckIconTexture_rest
    {
        get
        {
            if (deckIconTexture_rest == null)
                deckIconTexture_rest = ResourceLoader.Load<Texture>
                    ("res://Art/Cards/Icons/camp_deck_icon.png");
            return deckIconTexture_rest;
        }
    }

    private static PackedScene deckIconScene;
    public static PackedScene DeckIconScene
    {
        get
        {
            if (deckIconScene == null)
                deckIconScene = ResourceLoader.Load<PackedScene>
                    ("res://Assets/UI/CardButton.tscn");
            return deckIconScene;
        }
    }

    private static Texture fullArtBackground;
    public static Texture FullArtBackground
    {
        get
        {
            if (fullArtBackground == null)
                fullArtBackground = ResourceLoader.Load<Texture>
                    ("res://Art/Cards/card_background_full.png");
            return fullArtBackground;
        }
    }

    public static Card GetCard()
    {
        Card card = CardScene.Instance<Card>();
        return card;
    }

    public static CharacterWrapper CreateCardFromCharacter(CharacterModel character)
    {
        Card card = CardScene.Instance<Card>();
        CharacterWrapper characterCardWrapper = new CharacterWrapper(
            card, character);

        Node2D cardFront = card.Front;

        CardButtonBase combatDeckButton = DeckIconScene.Instance<DeckCardButton>();
        cardFront.AddChild(combatDeckButton);
        combatDeckButton.Connect("OnClick", card, "OnCombatDeckClicked");
        combatDeckButton.Position = cardFront.GetNode<Position2D>(
            "CombatDeckPosition").Position;

        CardButtonBase restDeckButton = DeckIconScene.Instance<DeckCardButton>();
        restDeckButton.ButtonTextureOnReady = DeckIconTexture_rest;
        cardFront.AddChild(restDeckButton);
        restDeckButton.Connect("OnClick", card, "OnFieldDeckClicked");
        restDeckButton.Position = cardFront.GetNode<Position2D>(
            "RestDeckPosition").Position;

        return characterCardWrapper;
    }

    public static LocationWrapper CreateCardFromLocation(
        string scenario, HexLocationModel location)
    {
        if (location.Location.ImageFileName == null)
            return CreateDefaultWrappedLocation();

        Card card = CardScene.Instance<Card>();
        card.IsDraggable = false;
        card.IsStackTarget = true;
        card.Background.Texture = FullArtBackground;

        //TODO, check if file exists
        string path = System.IO.Path.Combine(PATHS.ModFolderPath,
            scenario + "\\Images\\Cards\\Full\\" + location.Location.ImageFileName);
        if (System.IO.File.Exists(path))
        {
            card.Front.GetNode<Sprite>("Image").Texture
                = TextureLoader.GetImageTextureFromPng(path);
        }
        else
        {
            GD.PrintErr("Card factory error: Resource missing at " + path);
            return CreateDefaultWrappedLocation();
        }

        LocationWrapper locationWrapper = new LocationWrapper(card, location);
        return locationWrapper;
    }

    public static CombatCardWrapper CreateCardFromCombatCardModel(CombatCardModel model)
    {
        Card card = CardScene.Instance<Card>();

        string path = "res://Art/Cards/Images/CombatDeck/" + model.ImageFileName;
        if (ResourceLoader.Exists(path))
            card.Front.GetNode<Sprite>(
                "Image").Texture = ResourceLoader.Load<Texture>(path);
        return new CombatCardWrapper(card, model);
    }

    public static FieldCardWrapper CreateCardFromFieldCardModel(FieldCardModel model)
    {
        Card card = CardScene.Instance<Card>();

        string path = "res://Art/Cards/Images/FieldDeck/" + model.ImageFileName;
        if (ResourceLoader.Exists(path))
            card.Front.GetNode<Sprite>(
                "Image").Texture = ResourceLoader.Load<Texture>(path);
        return new FieldCardWrapper(card, model);
    }

    public static LocationWrapper CreateDefaultWrappedLocation()
    {
        LocationModel location = new LocationModel
        {
            Name = "Desolation",
            ID = "core_desolation",
            ImageFileName = "desolation.png"
        };
        Card card = CardScene.Instance<Card>();
        card.IsDraggable = false;
        card.IsStackTarget = true;
        card.Background.Texture = FullArtBackground;
        string path = "res://Art/Cards/Images/Locations/" + location.ImageFileName;
        if (ResourceLoader.Exists(path))
            card.Front.GetNode<Sprite>(
                "Image").Texture = ResourceLoader.Load<Texture>(path);
        else
            GD.PrintErr("Card factory error: Resource missing at " + path);
        HexLocationModel hexlocation = new HexLocationModel() {
            Location=location
        };
        LocationWrapper locationWrapper = new LocationWrapper(card, hexlocation);
        return locationWrapper;
    }

    public static CharacterModel CreateDefaultCharacterInfo()
    {
        return new CharacterModel();
    }


    public static Card CreateExploreCard()
    {
        Card card = CardScene.Instance<Card>();

        card.IsDraggable = false;
        card.IsStackTarget = true;
        card.DownStateOnHover = true;
        card.Background.Texture = FullArtBackground;
        card.SetLabel("explore");

        string path = "res://Art/Cards/Images/Actions/Explore.png";
        card.Front.GetNode<Sprite>("Image").Texture = ResourceLoader.Load<Texture>(path);
        return card;
    }

    public static Card CreateSurviveCard()
    {
        Card card = CardScene.Instance<Card>();

        card.IsDraggable = false;
        card.IsStackTarget = true;
        card.DownStateOnHover = true;
        card.Background.Texture = FullArtBackground;
        card.SetLabel("Survive");

        string path = "res://Art/Cards/Images/Actions/Field.png";
        card.Front.GetNode<Sprite>("Image").Texture = ResourceLoader.Load<Texture>(path);
        return card;
    }
}
