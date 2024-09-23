using Godot;
using System;
using System.Drawing;

public partial class wyaldman : CharacterBody2D
{
	private int health = 28;
	private int damage = 3;
	private Area2D hurtbox;
	private Signals globalSignals;
	private AnimationPlayer animationPlayer;
	private CharacterBody2D characterBody;
	private RayCast2D floorCollider;
	private Sprite2D sprite;
	private PackedScene boulder;
	private boulder boulderInstance;
	private Vector2 target;
	private Timer Cooldown;
	private bool wasOnFloor = true;
	private bool jumpFoward = true;
	private bool isAttacking = false;
	private bool move = true;
	private AudioStreamPlayer deathSFX;
	private HealthBar healthBar;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

	[Export]
	public float gravity = 450;
	[Export]
	public float JumpVelocity = -100f;
	[Export]
	public float Speed = -50f;

	[Export]
	private int count = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		this.globalSignals = GetNode<Signals>("/root/Signals");
        this.hurtbox = GetNode<Area2D>("hurtbox");
		this.animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		this.sprite = GetNode<Sprite2D>("Sprite2D");
		this.Cooldown = GetNode<Timer>("JumpCooldown");
		this.boulder = GD.Load<PackedScene>("res://boulder.tscn");
		this.healthBar = GetNode<HealthBar>("CanvasLayer/HealthBar");
		this.healthBar.Position = new Vector2(243,5);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Node rootTree = GetParent();
		this.target = rootTree.GetNode<CharacterBody2D>("CharacterBody2D").Position;
		FacePlayer(this.target);


		foreach (CharacterBody2D body in this.hurtbox.GetOverlappingBodies())
        {
			if(body.GetType().Equals(typeof(berserkman)))
				DealDamage((berserkman)body);
        }
		if(!wasOnFloor && IsOnFloor())
		{
			this.Cooldown.Start();
			this.globalSignals.EmitSignal("Landing");
			jumpFoward = !jumpFoward;
			this.sprite.Frame = 0;
		}

		if(IsOnFloor() && this.Cooldown.IsStopped() && !isAttacking)
			this.animationPlayer.Play("jump");

		if(rng.RandiRange(0,100) < 400 && !wasOnFloor && IsOnFloor() && !IsInstanceValid(boulderInstance))
		{
			this.animationPlayer.Play("attack");
			isAttacking = true;
		}
		wasOnFloor = IsOnFloor();

	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if(this.animationPlayer.CurrentAnimation == "jump" && this.animationPlayer.CurrentAnimationPosition >= 0.5)
			velocity.Y = JumpVelocity;
		else 
			velocity.Y += gravity * (float)delta;

		if(IsOnFloor())
		{
			velocity.X = 0 ;
			move = new Random().Next(0,2) == 0;
		}

		if(!IsOnFloor() && move)
			velocity.X = Mathf.MoveToward(Velocity.X, Speed *(jumpFoward ? 1 : -1), Speed* (float)delta);

		Velocity = velocity;
		MoveAndSlide();

	}

	public void FacePlayer(Godot.Vector2 playerPosition)
	{
		var direction = playerPosition - this.Position;
		sprite.Scale = new Godot.Vector2(1, 1);
		hurtbox.Scale = new Godot.Vector2(1, 1);
		hurtbox.Position = new Vector2(Math.Abs(hurtbox.Position.X), hurtbox.Position.Y);
        if (direction.X > 0)
        {
            sprite.Scale = new Godot.Vector2(-1, 1);
            hurtbox.Scale = new Godot.Vector2(-1, 1);
            hurtbox.Position = new Vector2(-Math.Abs(hurtbox.Position.X), hurtbox.Position.Y);
        }

	}

	public void DealDamage(berserkman body)
	{
		if(body.invencibilityTimer.IsStopped())
			body.TakeDamage(this.damage);
	}

	public void TakeDamage(int damage)
	{
		this.health -= damage;
		this.healthBar.currentHealth = this.health;
		healthBar.QueueRedraw();
        if(this.health <= 0){
			GetParent().GetNode<AudioStreamPlayer>("BossBattle").Stop();
			GetParent().GetNode<AudioStreamPlayer>("DeathSFX").Play();
            this.QueueFree();
        }
	}

	public void OnHurtboxBodyEntered(berserkman body)
	{
		DealDamage(body);
	}

	public void OnAttackAnimationFinished(String animationName)
    {
		if(animationName == "attack")
		{
			this.animationPlayer.Play("RESET");
			isAttacking = false;
		}
    }

	public void InstantiateBoulder(){
		if(this.boulder != null && !IsInstanceValid(boulderInstance)){
            boulderInstance = (boulder)this.boulder.Instantiate();
            boulderInstance.startPoint = new Vector2(this.Position.X +(10 * sprite.Scale.X), this.Position.Y -70);
            boulderInstance.endPoint = new Vector2(this.Position.X +(10 * sprite.Scale.X), this.Position.Y - this.sprite.Texture.GetHeight()/4);
			boulderInstance.target = this.target;
            boulderInstance.damage = this.damage;
            boulderInstance.Scale = this.Scale;
            GetTree().CurrentScene.AddChild(boulderInstance);
        }
	}

	public void ThrowBoulder()
	{
		boulderInstance.target = this.target;
		boulderInstance.isThrowing = true;
	}
}
