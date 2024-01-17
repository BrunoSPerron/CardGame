using Godot;
using System;
using System.IO;


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

    public static CharacterWrapper CreateCardFromCharacter(CharacterModel character)
    {
        Card card = CardScene.Instance<Card>();

        Node2D cardFront = card.Front;

        Vector2 lifeCounterPosition = cardFront.GetNode<Position2D>(
            "CounterOnePosition").Position;
        AddCounter(card, "LifeCounter", lifeCounterPosition, CardIcon.HEART);

        Vector2 powerCounterPosition = cardFront.GetNode<Position2D>(
            "CounterTwoPosition").Position;
        AddCounter(card, "PowerCounter", powerCounterPosition, CardIcon.SWORD);

        Vector2 actionCounterPosition = cardFront.GetNode<Position2D>(
            "CounterThreePosition").Position;
        AddCounter(card, "ActionCounter", actionCounterPosition, CardIcon.TIME);

        CardButtonBase combatDeckButton = DeckIconScene.Instance<CardButtonBase>();
        cardFront.AddChild(combatDeckButton);
        combatDeckButton.Connect("OnClick", card, "OnCombatDeckClicked");
        combatDeckButton.Position = cardFront.GetNode<Position2D>(
            "CombatDeckPosition").Position;

        CardButtonBase restDeckButton = DeckIconScene.Instance<CardButtonBase>();
        restDeckButton.ButtonTextureOnReady = DeckIconTexture_rest;
        cardFront.AddChild(restDeckButton);
        restDeckButton.Connect("OnClick", card, "OnFieldDeckClicked");
        restDeckButton.Position = cardFront.GetNode<Position2D>(
            "RestDeckPosition").Position;

        CardButtonBase inventoryButton = DeckIconScene.Instance<CardButtonBase>();
        inventoryButton.ButtonTextureOnReady = ResourceLoader.Load<Texture>
            ("res://Art/Cards/Icons/inventory.png"); ;
        cardFront.AddChild(inventoryButton);
        inventoryButton.Connect("OnClick", card, "OnInventoryClicked");
        inventoryButton.Position = cardFront.GetNode<Position2D>(
            "InventoryPosition").Position;

        return new CharacterWrapper(card, character);
    }

    public static LocationWrapper CreateCardFromLocation(
        string mod, WorldHexModel location)
    {
        if (location.Location.Image == null)
            return CreateDefaultWrappedLocation();

        Card card = CardScene.Instance<Card>();
        card.IsDraggable = false;
        card.IsStackTarget = true;
        card.Background.Texture = FullArtBackground;
        AddActionCostCounter(card, location.Location.TravelCost);
        string texturePath = System.IO.Path.Combine(PATHS.ModFolderPath,
            mod + "\\Images\\Cards\\Full\\" + location.Location.Image);
        string pathWithExtension = System.IO.Path.ChangeExtension(texturePath, "png");
        if (System.IO.File.Exists(pathWithExtension))
        {
            card.Front.GetNode<Sprite>("Image").Texture
                = TextureLoader.GetImageTextureFromPng(pathWithExtension);
        }
        else
        {
            GD.PrintErr("Card factory error: Resource missing at " + pathWithExtension);
            return CreateDefaultWrappedLocation();
        }

        LocationWrapper locationWrapper = new LocationWrapper(card, location);
        locationWrapper.PopulateEncounters();
        return locationWrapper;
    }

    public static CombatCardWrapper CreateCardFromCombatCardModel(CombatCardModel model)
    {
        Card card = CardScene.Instance<Card>();

        PackedScene pixelText = ResourceLoader.Load<PackedScene>(
            "res://Assets/UI/PixelText.tscn");
        PixelText cost = pixelText.Instance<PixelText>();
        cost.Name = "CardCostLabel";
        cost.Position = card.Front.GetNode<Position2D>("CardCostPosition").Position;
        cost.SetLabel(model.Cost.ToString());
        card.Front.AddChild(cost);

        string texturePath = System.IO.Path.Combine(PATHS.ModFolderPath,
            model.Mod + "\\Images\\Cards\\Box\\" + model.Image);
        string pathWithExtension = System.IO.Path.ChangeExtension(texturePath, "png");
        if (System.IO.File.Exists(pathWithExtension))
        {
            card.Front.GetNode<Sprite>("Image").Texture
                = TextureLoader.GetImageTextureFromPng(pathWithExtension);
        }
        else
        {
            GD.PrintErr("CardFactoryError: Image not found at: " + pathWithExtension);
        }
        return new CombatCardWrapper(card, model);
    }

    public static FieldCardWrapper CreateCardFromFieldCardModel(FieldCardModel model)
    {
        Card card = CardScene.Instance<Card>();
        
        PackedScene pixelText = ResourceLoader.Load<PackedScene>(
            "res://Assets/UI/PixelText.tscn");
        PixelText cost = pixelText.Instance<PixelText>();
        cost.Name = "CardCostLabel";
        cost.Position = card.Front.GetNode<Position2D>("CardCostPosition").Position;
        cost.SetLabel(model.Cost.ToString());
        card.Front.AddChild(cost);

        PixelText textBox = pixelText.Instance<PixelText>();
        textBox.SetLabel(model.TextBox);
        card.Front.AddChild(textBox);
        textBox.Position = card.Front.GetNode<Position2D>("TextBoxPosition").Position;

        string mod = model.Mod;
        string[] splittedImageName = model.Image.Split(
            new string[] { "__" }, StringSplitOptions.None);
        string cardName = splittedImageName[0];
        if (splittedImageName.Length > 1)
        {
            mod = splittedImageName[0];
            cardName = splittedImageName[1];
        }

        if (mod == "core")
        {
            switch (cardName)
            {
                case "drool":
                    //TODO default image
                    break;
                default:
                    GD.PrintErr("Card not found in core: \"" + cardName + "\"");
                    break;
            }
        }
        else
        {
            string texturePath = System.IO.Path.Combine(PATHS.ModFolderPath,
                mod + "\\Images\\Cards\\Box\\" + model.Image);

            string pathWithExtension = System.IO.Path.ChangeExtension(texturePath, "png");
            if (System.IO.File.Exists(pathWithExtension))
            {
                card.Front.GetNode<Sprite>("Image").Texture
                    = TextureLoader.GetImageTextureFromPng(pathWithExtension);
            }
            else
            {
                GD.PrintErr("CardFactoryError: Image not found at: " + pathWithExtension);
            }
        }

        return new FieldCardWrapper(card, model);
    }

    public static LocationWrapper CreateDefaultWrappedLocation()
    {
        LocationModel location = new LocationModel
        {
            Name = "Desolation",
            ID = "core_desolation",
            Image = "desolation.png"
        };
        Card card = CardScene.Instance<Card>();
        card.IsDraggable = false;
        card.IsStackTarget = true;
        card.Background.Texture = FullArtBackground;
        AddActionCostCounter(card, 0);
        string path = "res://Art/Cards/Images/Locations/" + location.Image;
        if (ResourceLoader.Exists(path))
            card.Front.GetNode<Sprite>(
                "Image").Texture = ResourceLoader.Load<Texture>(path);
        else
            GD.PrintErr("Card factory error: Resource missing at " + path);
        WorldHexModel hexlocation = new WorldHexModel() {
            Location=location
        };
        LocationWrapper locationWrapper = new LocationWrapper(card, hexlocation);
        return locationWrapper;
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
        AddActionCostCounter(card, 1);
        return card;
    }

    public static Card CreatePlayTarget()
    {
        Card card = CardScene.Instance<Card>();

        card.Front.GetNode("Image").QueueFree();
        Sprite background = card.Front.GetNode<Sprite>("Background");
        background.Texture = ResourceLoader.Load<Texture>("res://Art/hoverable_color.png");
        background.Scale = CONSTS.SCREEN_SIZE;
        background.RegionRect = new Rect2(Vector2.Zero, Vector2.One);

        CollisionShape2D collider = card.GetNode<CollisionShape2D>("CollisionShape2D");
        collider.Shape = new RectangleShape2D()
        {
            Extents = new Vector2(CONSTS.SCREEN_CENTER.x, CONSTS.SCREEN_CENTER.y - 70),
        };
        collider.Position = new Vector2 (0f, -70f);
        card.Position = CONSTS.SCREEN_CENTER;
        card.MoveToPosition(CONSTS.SCREEN_CENTER);

        card.IsStackTarget = true;
        card.IsDraggable = false;
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
        AddActionCostCounter(card, 1);
        return card;
    }

    // === private methods ===

    private static void AddActionCostCounter(Card card, int cost = 0)
    {
        IconCounter counter = new IconCounter()
        {
            Name = "ActionCostCounter",
            defaultIcon = CardIcon.TIME,
            Position = card.Front.GetNode<Position2D>("ActionCostPosition").Position
        };
        counter.SetMax(cost);
        card.Front.AddChild(counter);
    }

    private static void AddCounter(
        Card card, string name, Vector2 position, CardIcon icon, int value = 0)
    {
        IconCounter counter = new IconCounter()
        {
            Name = name,
            defaultIcon = icon,
            Position = position
        };
        counter.SetMax(value);
        card.Front.AddChild(counter);
    }
}
