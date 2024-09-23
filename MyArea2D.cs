using Godot;
using System;

public partial class MyArea2D : Area2D
{

	public float PositionX
    {
        get { return Position.X; }
        set
        {
            Position = new Vector2(value, Position.Y);
            EmitSignal(nameof(PositionXChanged));
        }
    }

    public float PositionY
    {
        get { return Position.Y; }
        set
        {
            Position = new Vector2(Position.X, value);
            EmitSignal(nameof(PositionYChanged));
        }
    }
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

    [Signal]
    public delegate void PositionXChangedEventHandler();

    [Signal]
    public delegate void PositionYChangedEventHandler();
}
