using Godot;
using System;
using System.Numerics;

public partial class melee_enemy : CharacterBody2D
{
	private int health = 4;
	private int damage = 2;
	private Area2D hurtbox;
	private Area2D detectionArea;
	private AnimationPlayer animationPlayer;
	private CharacterBody2D characterBody;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
        this.hurtbox = GetNode<Area2D>("hurtbox");
        this.detectionArea = GetNode<Area2D>("detectionArea");
		this.animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		foreach (CharacterBody2D body in this.hurtbox.GetOverlappingBodies())
        {
			if(body.GetType().Equals(typeof(berserkman)))
				DealDamage((berserkman)body);
        }

		foreach (CharacterBody2D body in this.detectionArea.GetOverlappingBodies())
			FacePlayer(body.GlobalPosition);
	}

	public void Attack()
	{
		this.animationPlayer.Play("attack");
	}

	public void FacePlayer(Godot.Vector2 playerPosition)
	{
		var direction = playerPosition - this.GlobalPosition;
		Scale = new Godot.Vector2(-1,1);
		if(direction.X < 0)
			Scale = new Godot.Vector2(1,1);
	}
	public void DealDamage(berserkman body)
	{
		if(body.invencibilityTimer.IsStopped())
			body.TakeDamage(this.damage);
	}
	public void TakeDamage(int damage)
	{
		this.health -= damage;
        if(this.health <= 0){
            this.QueueFree();
        }
	}

	public void OnHurtboxBodyEntered(berserkman body)
	{
		DealDamage(body);
	}

	public void OnDetectionAreaBodyEntered(berserkman body)
	{
		Attack();
	}

	public void OnDetectionAreaBodyExited(berserkman body)
	{
		this.animationPlayer.Stop();
	}
}
