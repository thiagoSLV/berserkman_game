using Godot;
using System;

public partial class CurrentHealth : Node2D
{
	 private const int ContainerWidth = 8;
    private const int ContainerHeight = 56;
    private const int RectWidth = 6;
    private const int RectHeight = 1;
    private const int Margin = 1;
    private  Color BackgroundColor = new Color(0, 0, 0); // Preto
    private  Color RectColor = new Color(1, 1, 0); // Amarelo
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Desenhar o retângulo de fundo preto
        DrawRect(new Rect2(0, 0, ContainerWidth, ContainerHeight), BackgroundColor);

        // Número de retângulos que cabem no contêiner verticalmente
        int numRects = (ContainerHeight + Margin) / (RectHeight + Margin);

        for (int i = 0; i < numRects; i++)
        {
            // Calcular a posição y para cada retângulo
            int y = i * (RectHeight + Margin);
            if (y + RectHeight <= ContainerHeight) // Certificar-se de que o retângulo não excede o contêiner
            {
                DrawRect(new Rect2(1, y, RectWidth, RectHeight), RectColor);
            }
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
