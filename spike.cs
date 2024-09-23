using Godot;
using System;

public partial class spike : Area2D
{
	public void OnBodyEntered(berserkman body)
	{
		body.Die();
	}
}
