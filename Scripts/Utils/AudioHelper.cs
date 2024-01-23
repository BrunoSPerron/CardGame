using Godot;
using System;

public static class AudioHelper
{
    public static void PlaySoundOnCard(Card card, AudioStream sound, float pitchScale = 1f)
    {
        AudioStreamPlayer player = new AudioStreamPlayer();
        card.AddChild(player);
        player.Stream = sound;
        player.PitchScale = pitchScale;
        player.Play();
    }
}
