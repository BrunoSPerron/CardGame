using Godot;
using System;


public static class TextureLoader
{
    public static ImageTexture GetTextureFromPng(string filePath)
    {
        if (System.IO.Directory.Exists(filePath))
        {
            Random rand = new Random();
            string[] files = System.IO.Directory.GetFiles(filePath, "*.png");
            filePath = files[rand.Next(files.Length)];
        }
        filePath = System.IO.Path.ChangeExtension(filePath, "png");
        File file = new File();
        Image image = new Image();
        Error error = file.Open(filePath, File.ModeFlags.Read);
        if (error == Error.FileNotFound)
        {
            GD.PrintErr("Texture loader error: File not found at " + filePath);
            return new ImageTexture();
        }
        image.LoadPngFromBuffer(file.GetBuffer((long)file.GetLen()));
        file.Close();
        image.Lock () ;
        ImageTexture imageTexture = new ImageTexture();
        imageTexture.CreateFromImage(image, 0);
        return imageTexture;
    }
}
