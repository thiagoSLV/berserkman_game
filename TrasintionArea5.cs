using Godot;
using System;

public partial class TrasintionArea5 : Area2D
{
	private bool slide = false;
	private Camera2D camera;
	private CharacterBody2D character;
	public override void _Process(double delta) {
		if(slide)
		{
			character.SetPhysicsProcess(false);
			character.Position = new Vector2(character.Position.X + .3f , character.Position.Y);
			if(camera.LimitRight < 3112)
				camera.LimitRight += 2;
			if(camera.LimitLeft < 2840)
				camera.LimitLeft += 2;
		}

		if(camera != null && camera.LimitRight == 3112 && camera.LimitLeft == 2840)
		{
			slide = false;
			camera.LimitRight = 3598;
			GetNode<CollisionShape2D>("StaticBody2D/CollisionShape2D").Disabled = false;
			GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
			character.SetPhysicsProcess(true);
		}
	}

	private void OnAreaEntered(CharacterBody2D body){
		character = body;
		character.SetPhysicsProcess(false);
		camera = character.GetNode<Camera2D>("Camera2D");
		camera.LimitLeft = 2576;
		slide = true;
	}

}
