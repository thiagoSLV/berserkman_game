using Godot;
using System;

public partial class TrasintionArea6 : Area2D
{
	private bool slide = false;
	private PackedScene boss = GD.Load<PackedScene>("res://wyaldman.tscn");
	private Camera2D camera;
	private wyaldman wyaldmanInstance;
	private CharacterBody2D character;
	public override void _Process(double delta) {
		if(slide)
		{
			character.SetPhysicsProcess(false);
			if(boss != null && !IsInstanceValid(wyaldmanInstance)){
				wyaldmanInstance = (wyaldman)boss.Instantiate();
				wyaldmanInstance.Position = new Vector2(3790,1125);
				GetTree().CurrentScene.AddChild(wyaldmanInstance);
				wyaldmanInstance.SetPhysicsProcess(false);
				wyaldmanInstance.SetProcess(false);
			}

			character.Position = new Vector2(character.Position.X + .3f , character.Position.Y);
			if(camera.LimitRight < 3840)
				camera.LimitRight += 2;
			if(camera.LimitLeft < 3584)
				camera.LimitLeft += 2;
		}

		if(slide && camera != null && camera.LimitRight == 3840 && camera.LimitLeft == 3584)
		{
			slide = false;
			GetNode<CollisionShape2D>("StaticBody2D/CollisionShape2D").Disabled = false;
			GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
			character.SetPhysicsProcess(true);
			wyaldmanInstance.SetPhysicsProcess(true);
			wyaldmanInstance.SetProcess(true);

		}
	}

	private void OnAreaEntered(CharacterBody2D body){
		character = body;
		character.SetPhysicsProcess(false);
		camera = character.GetNode<Camera2D>("Camera2D");
		camera.LimitLeft = 3328;
		slide = true;
	}

}
