using Godot;
using System;

public partial class BaseLevel : Node2D
{
    private int puntuacion = 70;
	private Label labelPuntuacion;
    public override void _Ready()
	{
		this.labelPuntuacion = GetNode<Label>("UI/LabelPuntuacion");
		this.ActualizarTextoPuntuacion();

		Node contenedorNpcs = GetNode<Node>("NPCs");

		// Conectamos la señal de cada NPC hijo
		foreach (Node hijo in contenedorNpcs.GetChildren())
		{
			// Verificamos que el hijo sea realmente de la clase Npc de tu script
			if (hijo is BaseNpc npc)
			{
				npc.NpcAsesinado += OnNpcAsesinado;
			}
		}
	}

    private void OnNpcAsesinado(bool eraEspia)
	{
		if (eraEspia)
		{
			GD.Print("¡Objetivo eliminado! Misión cumplida.");
		}
		else
		{
			puntuacion -= 10;
			this.ActualizarTextoPuntuacion();
			GD.Print($"¡Error! Inocente eliminado. La nota baja a {puntuacion}");

			if (puntuacion < 30)
			{
				GameOver();
			}
		}
	}

	private void ActualizarTextoPuntuacion()
	{
		if (this.labelPuntuacion != null)
		{
			this.labelPuntuacion.Text = $"Nota: {puntuacion}";
			
			if (puntuacion == 40)
			{
				this.labelPuntuacion.AddThemeColorOverride("font_color", new Color(1, 0, 0));
			}
		}
	}

	private void GameOver()
	{
		GD.Print("¡Nota inferior a 40! Te echaste el ramo.");
		// Lógica de derrota (reiniciar nivel)
		// GetTree().ReloadCurrentScene(); 
	}
}
