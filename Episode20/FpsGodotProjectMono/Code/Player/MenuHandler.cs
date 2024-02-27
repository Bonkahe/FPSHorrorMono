using Godot;
using System;

public partial class MenuHandler : Node
{
    [Export] public Control LaunchMenu { get; set; }
    [Export] public Control PauseMenu { get; set; }
    [Export] public Control DeathMenu { get; set; }

    [Export] public Node3D PlayerNode { get; set; }
    [Export] public Camera3D LevelCamera { get; set; }
    [Export] public ShaderMaterial TransitionMaterial { get; set; }

    [Export] public float TransitionTime { get; set; } = 2.0f;

    public bool IsPlaying { get; private set; } = false;


    public override void _Ready()
    {
        GetTree().Paused = true;
        TransitionMaterial.SetShaderParameter("EffectStrength", 3.0f);
        LaunchMenu.Visible = true;
        TweenOutTrans();
        PlayerNode.Visible = false;
        LevelCamera.Current = true;

        PlayerHealthController playerHealthController = PlayerNode.GetNode("PlayerHealthController") as PlayerHealthController;
        if (playerHealthController != null)
        {
            playerHealthController.Connect(PlayerHealthController.SignalName.OnPlayerDeath, Callable.From((Vector3 CameraLocation, Vector3 CameraRotation) => OnDeath(CameraLocation, CameraRotation)));
        }
    }



    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("escape") && IsPlaying)
        {
            TogglePauseMenu();
        }
    }

    public void ToggleFullScreen()
    {
        if (DisplayServer.WindowGetMode() != DisplayServer.WindowMode.Fullscreen)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }
        else
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }
    }

    private void TogglePauseMenu()
    {
        if (PauseMenu.Visible == true)
        {
            HidePauseMenu();
        }
        else
        {
            ShowPauseMenu();
        }
    }

    public void OnDeath(Vector3 CameraLocation, Vector3 CameraRotation)
    {
        GetTree().Paused = true;
        IsPlaying = false;
        TweenInTrans(Callable.From(() => ShowDeathMenu(CameraLocation, CameraRotation)));
    }


    public void ShowDeathMenu(Vector3 CameraLocation, Vector3 CameraRotation)
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        DeathMenu.Visible = true;
        LevelCamera.GlobalPosition = CameraLocation;
        LevelCamera.GlobalRotation = CameraRotation;
        LevelCamera.Current = true;
        PlayerNode.QueueFree();
        TweenOutTrans();
        GetTree().Paused = false;
    }

    public void ShowPauseMenu()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        PauseMenu.Visible = true;
        GetTree().Paused = true;
    }

    public void HidePauseMenu()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        PauseMenu.Visible = false;
        GetTree().Paused = false;
    }

    public void BeginLaunch()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        LaunchMenu.Visible = false;
        TweenInTrans(Callable.From(() => FinishLaunch()));
    }

    public void FinishLaunch()
    {
        PlayerNode.Visible = true;
        LevelCamera.Current = false;
        TweenOutTrans();
        GetTree().Paused = false;
        IsPlaying = true;
    }

    public void RestartLevel()
    {
        GetTree().ReloadCurrentScene();
    }
    public void ExitGame()
    {
        TweenInTrans(Callable.From(() => GetTree().Quit()));
    }

    public void TweenOutTrans(Callable? CompletionCallable = null)
    {
        Tween newTween = GetTree().CreateTween();
        newTween.SetPauseMode(Tween.TweenPauseMode.Process);

        newTween.TweenMethod(new Callable(this, "SetTransitionValue"), 3.0f, 0.0f, TransitionTime);
        if (CompletionCallable != null)
        {
            newTween.TweenCallback(CompletionCallable.Value);
        }
    }

    public void TweenInTrans(Callable? CompletionCallable = null)
    {
        Tween newTween = GetTree().CreateTween();
        newTween.SetPauseMode(Tween.TweenPauseMode.Process);

        newTween.TweenMethod(new Callable(this, "SetTransitionValue"), 0.0f, 3.0f, TransitionTime);
        if (CompletionCallable != null)
        {
            newTween.TweenCallback(CompletionCallable.Value);
        }
    }

    public void SetTransitionValue(float newValue)
    {
        TransitionMaterial.SetShaderParameter("EffectStrength", newValue);
    }
}
