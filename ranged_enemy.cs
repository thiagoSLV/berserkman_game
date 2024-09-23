using Godot;
using System;

public partial class ranged_enemy : CharacterBody2D
{
	private int health = 2;
	private int damage = 3;
	private Area2D hurtbox;
	private Area2D detectionArea;
	private AnimationPlayer animationPlayer;
	private CharacterBody2D characterBody;
	private PackedScene axe;
	private axe axeInstance;
	private Vector2 target;

	[Export]
	private int count = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
        this.hurtbox = GetNode<Area2D>("hurtbox");
        this.detectionArea = GetNode<Area2D>("detectionArea");
		this.animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		this.axe = GD.Load<PackedScene>("res://axe.tscn");
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
		{
			this.target = body.Position;
			FacePlayer(this.target);
			if(!IsInstanceValid(axeInstance))
				this.animationPlayer.Play("attack");

		}
	}

	public void FacePlayer(Godot.Vector2 playerPosition)
	{
		var direction = playerPosition - this.Position;
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

	public void ThrowAxe()
	{
		if(this.axe != null){
			axeInstance = (axe)this.axe.Instantiate();
			axeInstance.startPoint = this.Position;
			axeInstance.endPoint = target;
			axeInstance.damage = this.damage;
			axeInstance.Scale = this.Scale;
			GetTree().CurrentScene.AddChild(axeInstance);
		}
	}

	public void OnHurtboxBodyEntered(berserkman body)
	{
		DealDamage(body);
	}

	// public void OnDetectionAreaBodyEntered(berserkman body)
	// {
	// 	ThrowAxe();
	// }

	public void OnAttackAnimationFinished(String animationName)
    {
		this.animationPlayer.Play("RESET");
    }

}
