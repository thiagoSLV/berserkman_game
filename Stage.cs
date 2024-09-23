using Godot;
using System;

public partial class Stage : Node2D
{
	private CanvasLayer transition;
	private AudioStreamPlayer mainTheme;
	private AudioStreamPlayer bossBattle;
	private AudioStreamPlayer victoryTheme;
	public override void _Ready()
	{
		this.transition = GetNode<CanvasLayer>("/root/Transition");
		this.transition.restart += Restart;
		this.transition.FadeToNormal();
		this.mainTheme = GetNode<AudioStreamPlayer>("MainTheme");
		this.bossBattle = GetNode<AudioStreamPlayer>("BossBattle");
		this.victoryTheme = GetNode<AudioStreamPlayer>("VictoryTheme");
	}

	public void Restart()
    {
		if (IsInstanceValid(this))
		{
			GetTree().ReloadCurrentScene();
		}
	}
	public void OnTrasintionArea6BodyExited(CharacterBody2D body){
		this.mainTheme.Stop();
		this.bossBattle.Play();
	}
	public void OnDeathSFXFinished(){
		foreach (Node child in GetChildren())
        {
            if (child is Node2D)
                child.SetPhysicsProcess(false);

			if(child is berserkman)
			{
				var berserkman = (berserkman)child;
				berserkman.sprite2D.Frame = 4;
                child.SetPhysicsProcess(false);

			}

			if(child is boulder || child is pebble)
				child.QueueFree();

		}
		this.victoryTheme.Play();
		SetPhysicsProcess(false);
		SetProcess(false);
	}
	public void OnVictoryThemeFinished(){
		Restart();
	}
}
