

using Godot;
using System;

[Tool]
public partial class NavigationMeshHandler : NavigationRegion3D
{
	[Export] public Mesh MeshToImport { get; set; }
	[Export] public bool ImportMesh { get; set; }

    public override void _Process(double delta)
    {
        if (ImportMesh && MeshToImport != null && NavigationMesh != null)
        {
            ImportMesh = false;
            GD.Print("test123");
            NavigationMesh.CreateFromMesh(MeshToImport);
        }
    }
}
