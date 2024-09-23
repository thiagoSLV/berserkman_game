using Godot;
using System;
using System.Net;

public partial class pebble : Area2D
{	

    public Vector2 direction { get; set; }
    private Sprite2D sprite;
    private VisibleOnScreenNotifier2D visibleNotifier;
    private float angle;
    public int damage {get; set;}
    public int speed = 100;
    private float maxHeight = 50; // Altura máxima do arco
    public float totalTime = 1.0f; // Tempo total de movimento do projétil
    public float currentTime = 0.0f;
	public float gravity = 450;
    public bool isFalling = true;
    public bool isThrowing = false;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

    private AnimationPlayer player;

	public override void _Ready()
	{
        this.sprite = GetNode<Sprite2D>("Sprite2D");
        this.visibleNotifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
        angle = rng.RandfRange(0, Mathf.Pi / 4);

	}

	public override void _PhysicsProcess(double delta)
	{
        Vector2 position = Position;
        position += direction.Normalized().Rotated(angle) * this.speed * (float)delta;
        Position = position;

        if (!visibleNotifier.IsOnScreen())
            QueueFree();

	}

    public void OnAreaBodyEntered(Node2D body)
	{
        if(body.GetType().Equals(typeof(berserkman))){
            if(((berserkman)body).invencibilityTimer.IsStopped())
                ((berserkman)body).TakeDamage(this.damage);
        }
	}
}
