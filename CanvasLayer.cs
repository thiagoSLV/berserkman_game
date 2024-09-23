using Godot;
using System;

public partial class CanvasLayer : Godot.CanvasLayer
{
	private AnimationPlayer animationPlayer;

	[Signal]
	public delegate void restartEventHandler();

    public override void _Ready()
    {
        base._Ready();
		this.animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void FadeToBlack()
	{
		this.animationPlayer.Play("fadeToBlack");
	}

	public void FadeToNormal()
	{
		this.animationPlayer.Play("fadeToNormal");
	}

	public void OnFadetoBlackFinished(string animationName)
	{
		if(animationName.Equals("fadeToBlack"))
			EmitSignal("restart");
	}
}
