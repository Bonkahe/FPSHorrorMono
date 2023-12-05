using Godot;
using System;

public partial class PlayerBodyController : CharacterBody3D
{
    [ExportCategory("Node References")]
    [Export] public WeaponEffectsController GunEffects { get; set; }

    [Export] public Node3D CameraNode { get; set; }
    [Export] public Camera3D CameraActual { get; set; }
    [Export] public Node3D ArmsNode { get; set; }

    [ExportCategory("Animations")]
    [Export] public AnimationTree AnimationTree { get; set; }
    [ExportGroup("Animation Names")]
    [Export] public string HandStateMachinePlaybackPath { get; set; }
    [Export] public string IdleAnimationName { get; set; }
    [Export] public string AimingAnimationName { get; set; }

    [Export] public string IdleFireAnimationName { get; set; }
	[Export] public string AimingFireAnimationName { get; set; }

    [Export] public string IdleReloadAnimationName { get; set; }
    [Export] public string AimingReloadAnimationName { get; set; }

    [ExportCategory("Camera Shake Variables")]
	[Export] public Noise CameraShake_Noise { get; set; }
	[Export] public float CameraShake_NoisePanningSpeed { get; set; } = 30;
	[Export] public float CameraShake_MaxPower { get; set; } = 0.15f;
	[Export] public float CameraShake_BlendSpeed { get; set; } = 7;
	[Export] public float CameraShake_ReturnStrength { get; set; } = 5;
	[Export] public float CameraShake_NoiseStrength { get; set; } = 0.2f;

    [Export] public float CameraShake_FallingBias { get; set; } = 1.0f;
    [Export] public float CameraShake_FallingStrengthFalloff { get; set; } = 2.0f;
    [Export] public float CameraShake_FallingMaxStrength { get; set; } = 1.0f;

    [Export] public float CameraShake_JumpingStrength { get; set; } = 0.2f;

	[ExportCategory("Walking Sway Variables")]
    [Export] public float WalkingSway_StepsPerSecond { get; set; } = 5.0f;
    [Export] public float WalkingSway_MaxSwayDistance { get; set; } = 0.05f;
    [Export] public float WalkingSway_MaxSwayHandsHeight { get; set; } = -0.005f;
    [Export] public float WalkingSway_MaxSwayCameraHeight { get; set; } = 0.01f;

	[Export] public float WalkingSway_BlendSpeed { get; set; } = 5;
	[Export] public float WalkingSway_Bias { get; set; } = 0.2f;

    [ExportCategory("Rotation Variables")]
	[Export] public float VerticalRecoil { get; set; } = 2.0f;
	[Export] public float RotationSpeed { get; set; } = 0.3f;
	[Export] public float CameraActualRotationSpeed { get; set; } = 20;
	[Export] public float ArmsActualRotationSpeed { get; set; } = 12;
	[Export] public float VerticalRotationLimit { get; set; } = 80;

    [ExportCategory("Movement Variables")]
    [Export] public float Speed = 5.0f;
    [Export] public float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();


	private AnimationNodeStateMachinePlayback handStateMachinePlayback;
	private Vector3 targetRotation;
	private bool isAiming;

    private Vector3 ArmsBasePosition;
    private float walkingSway_CurrentValue;
	
	private Vector3 cameraShake_Position;
	private float timeSinceStarted;

	private float lastYVelocity;

    public override void _Ready()
	{

		ArmsBasePosition = ArmsNode.Position;

        Input.MouseMode = Input.MouseModeEnum.Captured;
		handStateMachinePlayback = (AnimationNodeStateMachinePlayback)AnimationTree.Get(HandStateMachinePlaybackPath);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			targetRotation = new Vector3(
				Mathf.Clamp((-1 * mouseMotion.Relative.Y * RotationSpeed) + targetRotation.X, -VerticalRotationLimit, VerticalRotationLimit),
				Mathf.Wrap((-1 * mouseMotion.Relative.X * RotationSpeed) + targetRotation.Y, 0, 360),
				0);
		}

		if (@event.IsActionPressed("escape"))
		{
			ToggleMouseMode();
        }

		if (@event.IsActionPressed("Aim"))
		{
			isAiming = true;
			handStateMachinePlayback.Travel(AimingAnimationName);
		}
		if (@event.IsActionReleased("Aim"))
		{
			isAiming = false;
			handStateMachinePlayback.Travel(IdleAnimationName);
		}
		if (@event.IsActionPressed("Fire"))
        {
			FireWeapon();
        }
        if (@event.IsActionPressed("Reload"))
        {
            ReloadWeapon();
        }
    }

	private void FireWeapon()
	{
		if (!GunEffects.HasRoundAvailable)
		{
			ReloadWeapon();
            return;
		}

        handStateMachinePlayback.Travel(IdleFireAnimationName);
		if (isAiming)
		{
			handStateMachinePlayback.Travel(AimingFireAnimationName);
		}
		else
		{
			handStateMachinePlayback.Travel(IdleFireAnimationName);
		}
	}

	private void ReloadWeapon()
	{
        if (isAiming)
        {
            handStateMachinePlayback.Travel(AimingReloadAnimationName);
        }
        else
        {
            handStateMachinePlayback.Travel(IdleReloadAnimationName);
        }
        isAiming = false;
    }

	private void ToggleMouseMode()
	{
		if (Input.MouseMode == Input.MouseModeEnum.Visible)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else
		{
            Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		timeSinceStarted += (float)delta * CameraShake_NoisePanningSpeed;
		if (cameraShake_Position.Length() > 0.0001f)
		{
            Vector3 noise = (new Vector3(CameraShake_Noise.GetNoise1D(timeSinceStarted), CameraShake_Noise.GetNoise1D(timeSinceStarted + 1000), CameraShake_Noise.GetNoise1D(timeSinceStarted + 2000)) * CameraShake_NoiseStrength) * (cameraShake_Position.Length() / CameraShake_MaxPower);
            CameraActual.Position = CameraActual.Position.Lerp(
                cameraShake_Position + noise,
                (float)delta * CameraShake_BlendSpeed);

            cameraShake_Position = cameraShake_Position.Lerp(Vector3.Zero, (float)delta * CameraShake_ReturnStrength);
		}
		else
		{
			CameraActual.Position = Vector3.Zero;
		}

		Vector3 velocity = Velocity;


		if (!IsOnFloor())
		{
			velocity.Y -= gravity * (float)delta;
            walkingSway_CurrentValue = Mathf.Max(walkingSway_CurrentValue - (float)delta * WalkingSway_BlendSpeed, 0);
			lastYVelocity = velocity.Y;
        }
		else
		{
			if (lastYVelocity < -CameraShake_FallingBias)
			{
                ImpulseCamera(Vector3.Down, Mathf.SmoothStep(CameraShake_FallingBias, CameraShake_FallingStrengthFalloff + CameraShake_FallingBias, Mathf.Abs(lastYVelocity)) * CameraShake_FallingMaxStrength);

            }

            if (velocity.Length() > WalkingSway_Bias)
			{
				walkingSway_CurrentValue = Mathf.Min(walkingSway_CurrentValue + (float)delta * WalkingSway_BlendSpeed, 1.0f);
			}
			else
			{
				walkingSway_CurrentValue = Mathf.Max(walkingSway_CurrentValue - (float)delta * WalkingSway_BlendSpeed, 0);
			}
		}
        lastYVelocity = velocity.Y;

        if (walkingSway_CurrentValue > 0)
		{
			float stepSpeed = (float)delta * 3.0f;
			float stepBounce = (EaseInOutSine(-1.0f, 1.0f, timeSinceStarted * stepSpeed * 2.0f + 0.2f) * -1.0f) * WalkingSway_MaxSwayHandsHeight;
			if(stepBounce == WalkingSway_MaxSwayHandsHeight)
			{
				//sound effect for stepping.
			}
			cameraShake_Position.Y += (EaseInOutSine(-1.0f, 1.0f, timeSinceStarted * stepSpeed * 2.0f + 0.2f) * -1.0f) * WalkingSway_MaxSwayCameraHeight * (isAiming ? 0.2f : 1.0f);

            GunEffects.HandOffsetPosition = CameraNode.Transform.Basis * (new Vector3(
				EaseInOutSine(-1.0f, 1.0f, timeSinceStarted * stepSpeed) * WalkingSway_MaxSwayDistance,
                stepBounce,
				0.0f) * walkingSway_CurrentValue * (isAiming ? 0.2f : 1.0f));
		}
		else
		{
			GunEffects.HandOffsetPosition = Vector3.Zero;
		}


		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
            ImpulseCamera(Vector3.Up, CameraShake_JumpingStrength);
        }


		Vector2 inputDir = Input.GetVector("Move_left", "Move_right", "Move_up", "Move_down");
		Vector3 direction = (CameraNode.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed * (isAiming ? 0.5f : 1.0f);
			velocity.Z = direction.Z * Speed * (isAiming ? 0.5f : 1.0f);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}



		Velocity = velocity;
		MoveAndSlide();

		CameraNode.Rotation = new Vector3(
			Mathf.LerpAngle(CameraNode.Rotation.X, Mathf.DegToRad(targetRotation.X), CameraActualRotationSpeed * (float)delta),
			Mathf.LerpAngle(CameraNode.Rotation.Y, Mathf.DegToRad(targetRotation.Y), CameraActualRotationSpeed * (float)delta),
			0);

		ArmsNode.Rotation = new Vector3(
			Mathf.LerpAngle(ArmsNode.Rotation.X, Mathf.DegToRad(targetRotation.X), ArmsActualRotationSpeed * (float)delta),
			Mathf.LerpAngle(ArmsNode.Rotation.Y, Mathf.DegToRad(targetRotation.Y), ArmsActualRotationSpeed * (float)delta),
			0);
	}

    public void ImpulseCameraWithRecoil(Vector3 impulseVector, float impulsePower)
    {
        targetRotation.X += VerticalRecoil;
        ImpulseCamera(impulseVector, impulsePower);
    }

    public void ImpulseCamera(Vector3 impulseVector, float impulsePower)
    {
        cameraShake_Position += impulseVector * impulsePower;
        cameraShake_Position = cameraShake_Position.Normalized() * Mathf.Min(cameraShake_Position.Length(), CameraShake_MaxPower);
    }

    public float EaseInOutSine(float start, float end, float value)
	{
		end -= start;
		return -end * 0.5f * (Mathf.Cos(Mathf.Pi * value) - 1.0f) + start;
	}

}
