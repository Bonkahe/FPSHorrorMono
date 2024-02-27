using Godot;
using System;

[GlobalClass]
public partial class AudioQuePlayer : Node
{
	[Export] public PackedScene AudioQuePlayerPrefab { get; set; }
    [Export] public PackedScene AudioQuePlayer3DPrefab { get; set; }

    public void PlayAudioQue(AudioStream AudioClip, float VolumeDb)
	{
        AudioQue newAudioPlayer = AudioQuePlayerPrefab.Instantiate() as AudioQue;
        AddChild(newAudioPlayer);

        newAudioPlayer.Stream = AudioClip;
        newAudioPlayer.VolumeDb = VolumeDb;
        newAudioPlayer.PlayWithDelete();
    }

    public void PlayAudioQue3D(AudioStream AudioClip, Vector3 Position, float VolumeDb)
    {
        AudioQue3D newAudioPlayer = AudioQuePlayer3DPrefab.Instantiate() as AudioQue3D;
        AddChild(newAudioPlayer);

        newAudioPlayer.GlobalPosition = Position;
        newAudioPlayer.Stream = AudioClip;
        newAudioPlayer.VolumeDb = VolumeDb;
        newAudioPlayer.PlayWithDelete();
    }
}
