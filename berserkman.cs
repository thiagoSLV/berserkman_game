using Godot;
using System;

public partial class berserkman : CharacterBody2D
{
	public int damage = 2;
	public int health = 28;
	public Boolean isIdle = true;
	private Signals globalSignals;
	public Boolean isWalking = false;
	public Boolean isAbleToMove = true;
	public float Speed = 300.0f;
	public float JumpVelocity = -400.0f;
	public float knockBackStrength = 150f;
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public double runningAnimationStart = 0;
	public double attackingAnimationStart = 0;
	private Area2D area2D;
	private Area2D hitbox;
	public Sprite2D sprite2D { get; set;}
	private AnimatedSprite2D animatedSprite2D;
	private CharacterBody2D characterBody;
	private bool isVisible = true;
	private Camera2D camera;
	private AnimationPlayer animationPlayer;
	private Timer attackBuffer;
    private VisibleOnScreenNotifier2D visibleNotifier;
	private Timer knockBackTimer;
	private Timer stunTimer;
	public Timer invencibilityTimer {get; set;}
	private CanvasLayer transition;
	private HealthBar healthBar;
	private AudioStreamPlayer hurtSFX;
	private AudioStreamPlayer deathSFX;

	public override void _Ready()
    {
        base._Ready();
		this.characterBody = GetNode<CharacterBody2D>("../CharacterBody2D");
		this.animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		this.sprite2D = GetNode<Sprite2D>("Sprite2D");
		this.animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		this.attackBuffer = GetNode<Timer>("attackBuffer");
		this.invencibilityTimer = GetNode<Timer>("invencibilityTimer");
		this.knockBackTimer = GetNode<Timer>("knockBackTimer");
		this.stunTimer = GetNode<Timer>("stunTimer");
		this.globalSignals = GetNode<Signals>("/root/Signals");
		this.area2D = GetNode<Area2D>("Area2D");
		this.transition = GetNode<CanvasLayer>("/root/Transition");
        this.visibleNotifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
		this.camera = GetNode<Camera2D>("Camera2D");
		this.healthBar = GetNode<HealthBar>("CanvasLayer/HealthBar");
		this.hurtSFX = GetNode<AudioStreamPlayer>("HurtSFX");
		this.deathSFX = GetNode<AudioStreamPlayer>("DeathSFX");
		this.globalSignals.Landing += Stun;

    }

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		float direction = Input.GetAxis("ui_left", "ui_right");
		isWalking = direction != 0;
		isAbleToMove = this.knockBackTimer.IsStopped() && this.stunTimer.IsStopped();
		isIdle = !isWalking && IsOnFloor();
        if(isIdle)
			ReturnToIdle();

		if(isAbleToMove && isWalking)
			velocity.X = Walk(direction);
		else if(isAbleToMove)
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);

		if(Input.IsActionJustPressed("ui_accept") && IsOnFloor() && isAbleToMove)
			velocity.Y = JumpVelocity;
		else
			velocity.Y += gravity * (float)delta;

		if(!IsOnFloor() && this.knockBackTimer.IsStopped()){
			if(this.animationPlayer.IsPlaying())
				this.animationPlayer.Stop();
			this.sprite2D.Frame = this.attackBuffer.IsStopped() ? 4 : 5;
		}

		if(Input.IsActionJustPressed("attack") && this.attackBuffer.IsStopped())
			Attack();

		if(!this.invencibilityTimer.IsStopped())
			Blink();
		else
			Modulate = new Color(1, 1, 1, 1);
		Velocity = velocity;
		MoveAndSlide();
	}

	public void ReturnToIdle()
	{
		if(!this.attackBuffer.IsStopped()) return;
		this.area2D.Monitoring = false;
		this.animationPlayer.Play("idle");
	}

	public float Walk(float direction)
	{
		if(this.animationPlayer.IsPlaying() && !this.attackBuffer.IsStopped() && IsOnFloor())
			runningAnimationStart = this.animationPlayer.CurrentAnimationPosition;

		if(IsOnFloor() && this.attackBuffer.IsStopped())
			this.animationPlayer.Play("running");
		
		if(runningAnimationStart > 0 && this.attackBuffer.IsStopped()){
			this.animationPlayer.Seek(runningAnimationStart);
			runningAnimationStart = 0;
		}

		if(IsOnFloor() && this.animationPlayer.IsPlaying())
			attackingAnimationStart = this.animationPlayer.CurrentAnimationPosition;

		this.sprite2D.FlipH = direction < 0;
		if((((MyArea2D)this.area2D).PositionX > 0 && direction < 0) || ((MyArea2D)this.area2D).PositionX < 0 && direction > 0){
			((MyArea2D)this.area2D).PositionX *= -1;
		}
		return direction * Speed;

	}

	public void Attack()
	{
		this.area2D.Monitoring = true;
		this.attackBuffer.Start();
		this.animationPlayer.Stop();

		if(!IsOnFloor())
		{
			this.sprite2D.Frame =  5;
			((MyArea2D)this.area2D).PositionY = -5;
		}
		else
		{
			this.sprite2D.Frame =  2;
			((MyArea2D)this.area2D).PositionY = 3;
		}

		if(isWalking && IsOnFloor())
		{
			this.animationPlayer.Play("running_attacking");
			this.animationPlayer.Seek(attackingAnimationStart);
		}

	}

	public void OnAttackBufferTimeout()
	{
		this.area2D.Monitoring = false;
		if(!IsOnFloor()) this.sprite2D.Frame = 4;
	}

	public void Stun()
	{
		if(!IsOnFloor()) return;
		if(this.animationPlayer.IsPlaying()) this.animationPlayer.Stop();
		this.sprite2D.Frame = 6;
		SetPhysicsProcess(false);
		this.stunTimer.Start();
	}

	public void TakeDamage(int damage)
	{
		this.hurtSFX.Play();
		this.animationPlayer.Stop();
		this.sprite2D.Frame = 6;
		this.invencibilityTimer.Start();
		this.health -= damage;
		this.healthBar.currentHealth = this.health;
		healthBar.QueueRedraw();
		if(this.health <= 0)
            Die();
		Blink();
		ApplyKnockBack();
	}

	public void Blink()
	{
		float transparency  = Engine.GetFramesDrawn() % 2 == 0 ? 0.3f : 1f;
		Modulate = new Color(1, 1, 1, transparency);
	}

	private void Respawn()
	{
		Position = new Vector2();
	}

	public void ApplyKnockBack()
	{
		float direction = this.sprite2D.FlipH?-1:1;
		this.knockBackTimer.Start();
		Velocity = new Vector2(knockBackStrength* -direction , 0);
		MoveAndSlide();
	}

	public void Die()
	{
		this.deathSFX.Play();
		SetPhysicsProcess(false);
		this.sprite2D.Visible = false;
		this.animatedSprite2D.Visible = true;
		this.animatedSprite2D.Play("death");
		this.transition.FadeToBlack();
	}

	public void OnArea2dBodyEntered(CharacterBody2D body)
    {
		var type = body.GetType();
		if(body.HasMethod("TakeDamage"))
			body.GetType().GetMethod("TakeDamage").Invoke(body, new object[] { this.damage });
    }

	public void OnVisibleOnScreenNotifier2dScreenExited(){
		if(Position.Y > camera.LimitTop)
			Die();
	}
	public void OnStunTimerTimeout(){
		SetPhysicsProcess(true);
	}
}
