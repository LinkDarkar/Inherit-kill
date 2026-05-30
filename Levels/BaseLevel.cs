using Godot;
using System;
using System.Collections.Generic; // Necesario para usar HashSet

public partial class BaseLevel : Node2D
{
	private int puntuacion = 70;
	private Label labelPuntuacion;

	// --- VARIABLES PARA EL LOG DE DATOS ---
	[Export] 
	public string PreguntaNivel = "Identificar al espía por comportamiento";
	
	// El código la armará sola leyendo los NPCs del mapa
	private string alternativasDinamicas = ""; 
	
	// Cronómetro para medir cuánto tarda el jugador
	private double cronometroRespuesta = 0;

	public override void _Ready()
	{
		this.labelPuntuacion = GetNode<Label>("UI/LabelPuntuacion");
		this.ActualizarTextoPuntuacion();

		Node contenedorNpcs = GetNode<Node>("NPCs");

		// Usamos HashSet para guardar las clases sin que se repitan
		HashSet<string> clasesEnElMapa = new HashSet<string>();

		// Conectamos la señal de cada NPC hijo y recolectamos sus clases
		foreach (Node hijo in contenedorNpcs.GetChildren())
		{
			// Verificamos que el hijo sea realmente de la clase Npc
			if (hijo is BaseNpc npc)
			{
				npc.NpcAsesinado += OnNpcAsesinado;
				clasesEnElMapa.Add(npc.pseudoclass.ToString());
			}
		}

		// Convertimos el HashSet en un texto separado por comas (Ej: "CIVIL, GUARDIA, STAFF")
		alternativasDinamicas = string.Join(", ", clasesEnElMapa);
	}

	// Este método corre automáticamente cada frame del juego
	public override void _Process(double delta)
	{
		cronometroRespuesta += delta;
	}

	private void OnNpcAsesinado(bool eraEspia, string claseEliminada)
	{
		// Registramos la métrica perfecta para el CSV
		LoggerDatos.Instance?.RegistrarInteraccion(
			pregunta: PreguntaNivel,
			alternativas: alternativasDinamicas, 
			respuestaJugador: claseEliminada,    
			fueCorrecta: eraEspia,
			tiempoRespuesta: (float)cronometroRespuesta
		);

		// Reiniciamos el cronómetro a 0
		cronometroRespuesta = 0;

		if (eraEspia)
		{
			GD.Print("¡Objetivo eliminado! Misión cumplida.");
			// Lógica de victoria
		}
		else
		{
			puntuacion -= 10;
			this.ActualizarTextoPuntuacion();
			GD.Print($"¡Error! Inocente eliminado ({claseEliminada}). La nota baja a {puntuacion}");

			// Se corrige a 40 para simular la reprobación
			if (puntuacion < 40)
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
			
			// Cambiamos a rojo si la nota está en riesgo crítico
			if (puntuacion <= 40)
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
