using Godot;
using System;

public partial class TrasintionArea : Area2D
{
	private bool slide = false;
	private Camera2D camera;
	private CharacterBody2D character;
	public override void _Process(double delta) {
		if(slide)
		{
			character.SetPhysicsProcess(false);
			character.Position = new Vector2(character.Position.X, character.Position.Y + .3f);
			if(camera.LimitTop < 240)
				camera.LimitTop += 2;
			if(camera.LimitBottom < 240)
				camera.LimitBottom += 2;
		}
		if(camera != null && camera.LimitTop == 240 && camera.LimitBottom == 240)
		{
			slide = false;
			character.SetPhysicsProcess(true);
		}
	}

	private void OnAreaEntered(CharacterBody2D body){
		character = body;
		character.SetPhysicsProcess(false);
		camera = character.GetNode<Camera2D>("Camera2D");
		camera.LimitLeft = 2304;
		slide = true;
	}

	private void OnAreaExited(CharacterBody2D body){
	}
}
