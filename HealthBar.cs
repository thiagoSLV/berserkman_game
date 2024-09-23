using Godot;
using System;

public partial class HealthBar : Node2D
{
	private const int MaxHealth = 100;
    private const int BarWidth = 8;
    private const int BarHeight = 56;
    private const int RectangleHeight = 1;
    private const int Margin = 1;
    private Color BackgroundColor = new Color(0, 0, 0); // Preto
    private Color HealthColor = new Color(1, 1, 0); // Amarelo
	public int currentHealth {get; set;}
    private Color RectColor = new Color(1, 1, 0); // Amarelo

    public override void _Draw()
    {
        // Desenhar o retângulo de fundo preto
        DrawRect(new Rect2(0, 0, BarWidth, BarHeight), BackgroundColor);

        // Calcular a posição inicial para desenhar os retângulos amarelos
        float startY = BarHeight - Margin - RectangleHeight;

        // Desenhar os retângulos amarelos verticalmente
        for (int i = 0; i < currentHealth; i++)
        {
            float y = startY - i * (RectangleHeight + Margin);
            DrawRect(new Rect2(Margin, y, BarWidth - 2 * Margin, RectangleHeight), HealthColor);
        }
    }

    public override void _Ready()
    {
        this.currentHealth = (BarHeight + Margin) / (RectangleHeight + Margin);
		GD.Print(this.currentHealth);
    }
}
