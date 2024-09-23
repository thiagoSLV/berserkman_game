using Godot;
using System;

public partial class CharacterCamera : Camera2D
{
	private Signals globalSignals;
	private float shakeStrength = 0.0f;
	private float randomStrength = 30.0f;
	private float shakeFade = 5.0f;
	private Timer shakeTimer;
	private float intensity = 1;
	private float duration = 1;
	float startTime = 0;
	private RandomNumberGenerator rng = new RandomNumberGenerator();


	public override void _Ready()
	{
		this.globalSignals = GetNode<Signals>("/root/Signals");
		this.globalSignals.Landing += ShakeScreen;
		this.shakeTimer = GetNode<Timer>("ShakeTimer");
	}

	public override void _Process(double delta)
	{
		if(!this.shakeTimer.IsStopped())
		{
			float rand_x = rng.RandfRange(-1,1);
			float rand_y = rng.RandfRange(-1,1);
			Offset = new Vector2(rand_x, rand_y);
		}
		else
		{
			Offset = Vector2.Zero;
		}
	}

	public void ShakeScreen()
	{
		this.shakeTimer.Start();
	}
















	// public void ShakeScreen(){
	// 	if(Input.IsActionJustPressed("ui_left"))
	// 		ApplyShake();

	// 	if(shakeStrength > 0)
	// 	{
	// 		shakeStrength = Mathf.Lerp(shakeStrength,0,shakeFade* seila);
	// 		Offset = randomOffset();
	// 	}
	// 	else {
	// 		Offset = Vector2.Zero;
	// 	}
    // }

	// public void ApplyShake()
	// {
	// 	shakeStrength = randomStrength;
	// }

	// public Vector2 randomOffset()
	// {
	// 	return new Vector2(rng.RandfRange(-shakeStrength, shakeStrength), rng.RandfRange(-shakeStrength, shakeStrength));
	// }
}
