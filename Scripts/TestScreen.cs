using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Godot;


public class TestScreen : BaseGameScreen
{
    public override void Destroy()
    {
        
    }

    public override void _Ready()
    {
        NavigateTestScreen();
    }

    public void NavigateTestScreen()
    {
        Clear();
        AddButton("Show Latest Character", "Navigate_Latest",
            Cardinal.NW, new Vector2(10, 10));
        AddButton("Character Showcase", "Navigate_ShowcaseAll",
            Cardinal.NW, new Vector2(10, 80));
    }

    public void Navigate_Latest()
    {
        NavigateTestScreen();
        ShowLatestCharacter(new string[] { "Bodies", "Heads", "Hairs", "Eyes" });

    }

    public void Navigate_ShowcaseAll()
    {
        Clear();
        ShowAllPieces(new string[] { "Bodies", "Heads", "Hairs", "Eyes" }, "baseGame");

        AddButton("R", "NavigateTestScreen",
            Cardinal.SE, new Vector2(-10, -10));
    }

    private void Clear()
    {
        foreach (Node n in GetChildren())
            n.QueueFree();
    }


    public void CharacterShowcase(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CharacterCreationModel c = JsonLoader.GetCharacterCreationModel(new FileToLoad
            {
                FileName = "BaseCharacter",
                Mod = "basegame"
            });
            CharacterModel model = CharacterCreator.CreateFromModel(c);
            CharacterWrapper wrapper = CardFactory.CreateFrom(model);
            DealOnBoard(wrapper.Card, new Vector2(50 + i % 7 * 92, 70 + (i/7 * 130)));
        }
    }

    public void ShowAllPieces(string[] layers, string mod)
    {
        List<string>[] layersPossibilities = new List<string>[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            List<string> newLayer = new List<string>();
            string folderPath = System.IO.Path.Combine(
                PATHS.ModFolderPath, mod, "Images\\Cards", layers[i]);

            if (System.IO.Directory.Exists(folderPath))
                newLayer = System.IO.Directory.GetFiles(folderPath, "*.png").ToList();
            else
                GD.PrintErr("Test screen folder error: folder not found at ", folderPath);

            newLayer.Shuffle();
            layersPossibilities[i] = newLayer;
        }

        bool AllIsDealt = false;
        int iteration = 0;
        while(!AllIsDealt)
        {
            CharacterModel model = new CharacterModel();
            CharacterWrapper wrapper = CardFactory.CreateFrom(model);

            Sprite baseImage = wrapper.Card.Front.GetNode<Sprite>("Image");
            for (int i = 0; i < layers.Length; i++)
            {
                List<string> currentPossibilities = layersPossibilities[i];
                int nbOfPossibilities = currentPossibilities.Count;
                if (nbOfPossibilities > 0)
                {
                    string texturePath = currentPossibilities[0];
                    currentPossibilities.RemoveAt(0);

                    Sprite sprite = new Sprite
                    {
                        Texture = TextureLoader.GetTextureFromPng(texturePath),
                        Centered = false,
                        Position = baseImage.Position
                    };
                    wrapper.Card.Front.AddChild(sprite);
                }
                else
                {
                    string folderPath = System.IO.Path.Combine(
                        PATHS.ModFolderPath, mod, "Images\\Cards", layers[i]);

                    string[] files = System.IO.Directory.GetFiles(folderPath, "*.png");
                    string texturePath = files[RANDOM.rand.Next(files.Length)];
                    Sprite sprite = new Sprite
                    {
                        Texture = TextureLoader.GetTextureFromPng(texturePath),
                        Centered = false,
                        Position = baseImage.Position
                    };
                    wrapper.Card.Front.AddChild(sprite);
                }
            }

            DealOnBoard(wrapper.Card,
                new Vector2(50 + iteration % 7 * 92, 70 + (iteration / 7 * 130)));
            iteration++;

            AllIsDealt = true;
            foreach (List<string> item in layersPossibilities)
                if (item.Count != 0)
                    AllIsDealt = false;
        }
    }

    public void ShowLatestCharacter(string[] layers)
    {
        CharacterWrapper wrapper = CardFactory.CreateFrom(
            CharacterCreator.CreateFromModel(new CharacterCreationModel()));

        Vector2 pos = wrapper.Card.Front.GetNode<Node2D>("Image").Position;

        foreach (string s in layers)
        {
            wrapper.Card.Front.AddChild(new Sprite
            {
                Centered = false,
                Texture = GetLatestTexture(s),
                Position = pos
            });
        }

        DealOnBoard(wrapper.Card, new Vector2(350, 150));
    }

    // === private stuff ===


    private Texture GetLatestTexture(string folder)
    {
        string Folder = System.IO.Path.Combine(
            PATHS.ModFolderPath, "BaseGame\\Images\\Cards", folder);
        var files = new System.IO.DirectoryInfo(Folder).GetFiles("*.png");
        string latestfile = "";

        DateTime lastModified = DateTime.MinValue;

        foreach (System.IO.FileInfo file in files)
        {
            if (file.LastWriteTimeUtc > lastModified)
            {
                lastModified = file.LastWriteTimeUtc;
                latestfile = file.FullName;
            }
        }
        return TextureLoader.GetTextureFromPng(latestfile);
    }
}
