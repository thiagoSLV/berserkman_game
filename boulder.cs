using Godot;
using System;
using System.Net;

public partial class boulder : Area2D
{	

    public Vector2 startPoint { get; set; }
    public Vector2 endPoint { get; set; }
    public Vector2 target { get; set; }
    public Vector2 direction { get; set; }
    private Sprite2D sprite;
    private VisibleOnScreenNotifier2D visibleNotifier;
    public int damage {get; set;}
    public int speed = 100;
    private float maxHeight = 50; // Altura máxima do arco
    public float totalTime = 1.0f; // Tempo total de movimento do projétil
    public float currentTime = 0.0f;
	public float gravity = 450;
    public bool isFalling = true;
    public bool isThrowing = false;
    private AnimationPlayer player;
	private PackedScene pebble;

	public override void _Ready()
	{
        this.Position = this.startPoint;
        this.sprite = GetNode<Sprite2D>("Sprite2D");
        this.visibleNotifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
        this.target = (target - startPoint).Normalized();
		this.pebble = GD.Load<PackedScene>("res://pebble.tscn");

	}

	public override void _PhysicsProcess(double delta)
	{
        Vector2 position = Position;
        direction = (target - endPoint).Normalized();

        if(isFalling)
        {
            position.Y += gravity * (float)delta;
            if(position.Y > this.endPoint.Y - this.sprite.Texture.GetHeight()/2)
                isFalling = false;
        }

        if(isThrowing)
        {
            position += (direction).Normalized() * this.speed * (float)delta;
        }

        Position = position;
        if (!visibleNotifier.IsOnScreen())
        {
            QueueFree();
        }
	}

	public void OnAreaBodyEntered(Node2D body)
	{
        if(body.GetType().Equals(typeof(berserkman))){
            if(((berserkman)body).invencibilityTimer.IsStopped())
                ((berserkman)body).TakeDamage(this.damage);
        }
        if(this.pebble != null)
        {
            QueueFree();
            for(int i = 0; i < 4; i++)
            {
                pebble pebbleInstance = (pebble)pebble.Instantiate();
                pebbleInstance.Position = this.Position;
                pebbleInstance.direction = direction;
                GetTree().CurrentScene.CallDeferred("add_child", pebbleInstance);
            }
        }

	}
}
