using Godot;
using System;
using System.Collections.Generic; // Necesario para usar HashSet

public partial class Disco : Node2D 
{
	private int puntuacion = 70;
	private Label labelPuntuacion;

	[Export] 
	public string PreguntaNivel = "Identificar al espía por comportamiento";
	
	// Ya no es [Export]. El código la armará sola.
	private string alternativasDinamicas = ""; 
	
	private double cronometroRespuesta = 0;

	public override void _Ready()
	{
		labelPuntuacion = GetNode<Label>("UI/LabelPuntuacion");
		ActualizarTextoPuntuacion();

		Node contenedorNpcs = GetNode<Node>("NPCs");
		
		// Usamos HashSet para guardar las clases sin que se repitan
		HashSet<string> clasesEnElMapa = new HashSet<string>();

		foreach (Node hijo in contenedorNpcs.GetChildren())
		{
			// Nota: Cambia BaseNpc por Npc si ese es el nombre real de tu clase principal
			if (hijo is BaseNpc npc) 
			{
				// Conectamos la nueva señal
				npc.NpcAsesinado += OnNpcAsesinado;
				
				// Agregamos la clase de este NPC a nuestra lista de alternativas
				clasesEnElMapa.Add(npc.pseudoclass.ToString());
			}
		}

		// Convertimos el HashSet en un texto separado por comas (Ej: "CIVIL, GUARDIA, STAFF")
		alternativasDinamicas = string.Join(", ", clasesEnElMapa);
	}

	public override void _Process(double delta)
	{
		cronometroRespuesta += delta;
	}

	// Actualizamos los parámetros para recibir el string
	private void OnNpcAsesinado(bool eraEspia, string claseEliminada)
	{
		// Registramos la métrica perfecta, sin intervención manual
		LoggerDatos.Instance?.RegistrarInteraccion(
			pregunta: PreguntaNivel,
			alternativas: alternativasDinamicas, // Se calculó solo en el _Ready
			respuestaJugador: claseEliminada,    // Viene directamente del NPC que el jugador clickeó
			fueCorrecta: eraEspia,
			tiempoRespuesta: (float)cronometroRespuesta
		);

		cronometroRespuesta = 0;

		if (eraEspia)
		{
			GD.Print("¡Objetivo eliminado! Misión cumplida.");
		}
		else
		{
			puntuacion -= 10;
			ActualizarTextoPuntuacion();
			GD.Print($"¡Error! Asesinaste a un {claseEliminada}. La nota baja a {puntuacion}");

			if (puntuacion < 40)
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
			
			if (puntuacion <= 40)
			{
				labelPuntuacion.AddThemeColorOverride("font_color", new Color(1, 0, 0));
			}
		}
	}

	private void GameOver()
	{
		GD.Print("¡Nota inferior a 40! Te echaste el ramo.");
	}
}
