using Godot;
using System;

public partial class axe : Area2D
{	

    public Vector2 startPoint { get; set; }
    public Vector2 endPoint { get; set; }
    public int damage {get; set;}
    private float maxHeight = 50; // Altura máxima do arco
    public float totalTime = 1.0f; // Tempo total de movimento do projétil
    public float currentTime = 0.0f;
    private VisibleOnScreenNotifier2D visibleNotifier;
    private AnimationPlayer player;
	public override void _Ready()
	{
        this.visibleNotifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
        this.player = GetNode<AnimationPlayer>("AnimationPlayer");
        if(this.Scale.X > 0)
            this.player.Play("spin");
        else
            this.player.PlayBackwards("spin");
        
	}

	public override void _PhysicsProcess(double delta)
	{
        currentTime += (float)delta;

        float t = currentTime / totalTime;
        float parabolicT = -4 * maxHeight * t * t + 4 * maxHeight * t; // Função parabólica
        Vector2 currentPos = startPoint.Lerp(endPoint, t) + new Vector2(0, -parabolicT);

        Position = currentPos;

        if (!visibleNotifier.IsOnScreen() && Position.Y > GetViewportRect().Size.Y)
        {
            QueueFree();
        }
	}

	public void OnAreaBodyEntered(berserkman body)
	{
		if(body.invencibilityTimer.IsStopped())
			body.TakeDamage(this.damage);
	}
}
