using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class CultistCorpseDissolve : Node3D
{
    [Export] public float MaxRandomDelay { get; private set; } = 2.0f;
    [Export] public float DissolveDuration { get; private set; } = 3.0f;
    [Export] public Array<MeshInstance3D> DissolvedMeshes { get; private set; } = new Array<MeshInstance3D>();
    [Export] public PackedScene DissolveParticles { get; private set; }
    [Export] public Array<Node3D> DissolveSpawnPoints { get; private set; } = new Array<Node3D>();

    float fadeDelay = 0.0f;
    float fadeCurrent = 0.0f;

    bool spawnedParticles = false;

    List<GpuParticles3D> spawnedParticlesCache = new List<GpuParticles3D>();


    public override void _Ready()
	{
        RandomNumberGenerator rng = new RandomNumberGenerator();
        fadeDelay = rng.RandfRange(0, MaxRandomDelay);
    }

	public override void _Process(double delta)
	{
        if (fadeDelay > 0)
        {
            fadeDelay -= (float)delta;
        }
        else
        {
            if (!spawnedParticles)
            {
                spawnedParticles = true;

                foreach (var point in DissolveSpawnPoints)
                {
                    GpuParticles3D particles = DissolveParticles.Instantiate() as GpuParticles3D;
                    point.AddChild(particles);
                    particles.GlobalPosition = point.GlobalPosition;
                    particles.GlobalRotation = point.GlobalRotation;
                    particles.Emitting = true;
                    particles.Restart();

                    spawnedParticlesCache.Add(particles);

                    var time = (particles.Lifetime * 2.0f) / particles.SpeedScale;
                    GetTree().CreateTimer(time).Timeout += particles.QueueFree;
                }
            }

            fadeCurrent += (float)delta / DissolveDuration;
            float currentMappedFade = Mathf.Remap(fadeCurrent, 0.0f, 1.0f, -0.1f, 1.1f);

            foreach (var mesh in DissolvedMeshes)
            {
                mesh.SetInstanceShaderParameter("DissolveWeight", currentMappedFade);
            }

            if (fadeCurrent >= 1.0f)
            {
                foreach (var particles in spawnedParticlesCache)
                {
                    if (IsInstanceValid(particles))
                    {
                        Vector3 position = particles.GlobalPosition;
                        Vector3 rotation = particles.GlobalRotation;
                        particles.GetParent().RemoveChild(particles);
                        GetParent().AddChild(particles);
                        particles.GlobalPosition = position;
                        particles.GlobalRotation = rotation;
                    }
                }
                QueueFree();
            }
        }
    }
}
