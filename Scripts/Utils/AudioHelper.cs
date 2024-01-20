using Godot;
using System;

public static class AudioHelper
{
    public static void PlaySoundOnNode(Node node, AudioStream sound, float pitchScale = 1f)
    {
        AudioStreamPlayer player = new AudioStreamPlayer();
        node.AddChild(player);
        player.Stream = sound;
        player.PitchScale = pitchScale;
        player.Play();
        System.Threading.Thread thread = new System.Threading.Thread(delegate ()
            {
                while (player != null && player.Playing)
                    System.Threading.Thread.Sleep(1000);
                player?.QueueFree();
            });
        thread.Start();
    }
}
