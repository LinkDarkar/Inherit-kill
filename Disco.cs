using Godot;
using System;

public partial class Disco : Node2D 
{
	private int puntuacion = 70;
	private Label labelPuntuacion;

	public override void _Ready()
	{
		// 1. Obtenemos el Label que está dentro de tu escena instanciada "UI"
		labelPuntuacion = GetNode<Label>("UI/LabelPuntuacion");
		ActualizarTextoPuntuacion();

		// 2. Buscamos tu nodo agrupador llamado exactamente "NPCs"
		Node contenedorNpcs = GetNode<Node>("NPCs");

		// Conectamos la señal de cada NPC hijo
		foreach (Node hijo in contenedorNpcs.GetChildren())
		{
			// Verificamos que el hijo sea realmente de la clase Npc de tu script
			if (hijo is Npc npc)
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
			// Lógica de victoria (cargar siguiente nivel, mostrar pantalla, etc.)
		}
		else
		{
			// Penalización por objetivo incorrecto
			puntuacion -= 10;
			ActualizarTextoPuntuacion();
			GD.Print($"¡Error! Inocente eliminado. La nota baja a {puntuacion}");

			// Condición de derrota si baja de 4.0 (40 puntos)
			if (puntuacion < 30)
			{
				GameOver();
			}
		}
	}

	private void ActualizarTextoPuntuacion()
	{
		if (labelPuntuacion != null)
		{
			labelPuntuacion.Text = $"Nota: {puntuacion}";
			
			// Si llega a 40, lo pintamos de rojo como advertencia de que está a punto de reprobar
			if (puntuacion == 40)
			{
				labelPuntuacion.AddThemeColorOverride("font_color", new Color(1, 0, 0));
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
