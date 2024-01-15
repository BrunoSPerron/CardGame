using Godot;
using System;


public static class TextureLoader
{
    public static ImageTexture GetImageTextureFromPng(string filePath)
    {
        File file = new File();
        Image image = new Image();
        file.Open(filePath, File.ModeFlags.Read);
        image.LoadPngFromBuffer(file.GetBuffer((long)file.GetLen()));
        file.Close();
        image.Lock () ;
        ImageTexture imageTexture = new ImageTexture();
        imageTexture.CreateFromImage(image, 0);
        return imageTexture;
    }
}
