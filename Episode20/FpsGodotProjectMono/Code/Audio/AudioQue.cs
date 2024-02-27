using Godot;
using System;

public partial class AudioQue : AudioStreamPlayer
{
	
	public async void PlayWithDelete()
	{
		Play();
        await ToSignal(GetTree().CreateTimer(Stream.GetLength(), true, false, true), SceneTreeTimer.SignalName.Timeout);
		QueueFree();
	}
}
