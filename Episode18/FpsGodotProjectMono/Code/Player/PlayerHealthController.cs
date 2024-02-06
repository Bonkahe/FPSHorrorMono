using Godot;
using System;

public partial class PlayerHealthController : Node
{
	[Export] public PlayerBodyController PlayerBody;

	[Export] public ShaderMaterial DamageEffectMaterial;

	[Export] public float HealDelay { get; set; } = 10.0f;
	[Export] public float HealRate { get; set; } = 0.5f;

	[Export] public float FlashFadeRate { get; set; } = 2.0f;
	[Export] public float HealthTotal { get; set; } = 3.0f;

	public float CurrentHealth { get; private set; }

	private float CurrentHealDelay = 0;
    private float CurrentOverlayWeight = 0;
	private float CurrentFlashPower = 0;

    public override void _Ready()
    {
		CurrentHealth = HealthTotal;
    }

    public void OnDamaged(Vector3 hitLocation, Vector3 force, Node3D AggressorBodyNode)
	{
        //Subtract the damage but clamp it so it doesn't go below 0.
        CurrentHealth = Mathf.Clamp(CurrentHealth - 1, 0, HealthTotal);

		//Handle death later.
        if (CurrentHealth == 0)
        {
            GetTree().ReloadCurrentScene();
			return;
        }

		//Update heal delay.
        CurrentHealDelay = HealDelay;
		CurrentFlashPower = 1;


        PlayerBody.ImpulseCamera(force, force.Length());
		PlayerBody.Velocity += force;
    }

	//Helper function used to get the current desired weight based on the health.
	private float GetTargetOverlayWeight()
	{
		return Mathf.Clamp((1.0f - CurrentHealth / HealthTotal) * 3.0f + CurrentFlashPower, 0.0f, 3.0f);
	}

    public override void _Process(double delta)
	{
		//Handle healing after a delay
		if (CurrentHealth < HealthTotal)
		{
			if (CurrentHealDelay > 0)
			{
                CurrentHealDelay -= (float)delta;
			}
			else
			{
				CurrentHealth += HealRate * (float)delta;
				if (CurrentHealth > HealthTotal)
				{
					CurrentHealth = HealthTotal;
                }
            }
        }

        CurrentFlashPower -= (float)delta;

        //Retrieve the new target
        float newTargetOverlayWeight = GetTargetOverlayWeight();

		//If we are at the target make no change
		if (newTargetOverlayWeight == CurrentOverlayWeight && CurrentFlashPower == 0)
		{
			return;
		}

		CurrentFlashPower = Mathf.Clamp(CurrentFlashPower - (float)delta * FlashFadeRate, 0, 1);
		//If we are below it set it directly for rapid response to hits.
        if (newTargetOverlayWeight > CurrentOverlayWeight)
		{
			CurrentOverlayWeight = newTargetOverlayWeight;
		}
		else if (newTargetOverlayWeight < CurrentOverlayWeight) //if it's below fade down to it as a set speed.
		{
			CurrentOverlayWeight -= FlashFadeRate * (float)delta;
			if (CurrentOverlayWeight < newTargetOverlayWeight)
			{
				CurrentOverlayWeight = newTargetOverlayWeight;
			}
		}

		//Whatever the result update the material.
		DamageEffectMaterial.SetShaderParameter("EffectStrength", CurrentOverlayWeight);
    }
}
