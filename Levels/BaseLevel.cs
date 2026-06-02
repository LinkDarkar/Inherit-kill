using Godot;
using System;
using System.Collections.Generic; // Para el HashSet de datos
using System.ComponentModel;      // De la otra branch

public partial class BaseLevel : Node2D
{
	private int puntuacion = 70;
	private Label labelPuntuacion;

	// --- VARIABLES PARA EL LOG DE DATOS ---
	[Export] 
	public string PreguntaNivel = "Identificar al espía por comportamiento";
	
	private string alternativasDinamicas = ""; 
	private double cronometroRespuesta = 0;

	// --- VARIABLES DE INTERACCIÓN ---
	private MOVES prevPlayerState = MOVES.IDLE;
	private BaseNpc selectedNpc = null;

	// Player
	[Export]
	protected PlayerCharacter playerCharacter;
	[Export]
	protected Camera2D camera2D;

	// NPC Info
	[Export]
	protected PanelContainer npcInfoContainer;
	[Export]
	protected Label classLabel;
	[Export]
	protected Label animLabel;

	public override void _Ready()
	{
		this.labelPuntuacion = GetNode<Label>("UI/LabelPuntuacion");
		this.ActualizarTextoPuntuacion();

		Node contenedorNpcs = GetNode<Node>("NPCs");

		// Usamos HashSet para guardar las clases sin que se repitan
		HashSet<string> clasesEnElMapa = new HashSet<string>();

		// Conectamos las señales de interacción y muerte
		foreach (Node hijo in contenedorNpcs.GetChildren())
		{
			if (hijo is BaseNpc npc)
			{
				npc.NpcAsesinado += this.OnNpcAsesinado;
				npc.NpcInteractuado += this.OnNpcInteractuado;
				clasesEnElMapa.Add(npc.pseudoclass.ToString());
			}
		}
		
		// Convertimos el HashSet en un texto separado por comas
		alternativasDinamicas = string.Join(", ", clasesEnElMapa);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		this.InteractionManager();
		this.UpdateNpcInfo();
	}

	private void InteractionManager()
	{
		if (this.prevPlayerState != this.playerCharacter.model.currentMove.moveType)
		{
			if (this.prevPlayerState == MOVES.INTERACTING)
			{
				this.OnNpcInteractuadoFinished();
			}
			this.prevPlayerState = this.playerCharacter.model.currentMove.moveType;
		}

		if (this.selectedNpc != null && this.playerCharacter.model.currentMove.moveType == MOVES.INTERACTING)
		{
			npcInfoContainer.GlobalPosition = this.ObtainOnScreenCoords();
		}
	}
	
	private Vector2 ObtainOnScreenCoords()
	{
		Vector2 screenPos = 
			this.selectedNpc.GlobalPosition
			- this.camera2D.GetScreenCenterPosition()
			+ GetViewport().GetVisibleRect().Size / 2.0f;

		return screenPos;
	}

	public override void _Process(double delta)
	{
		cronometroRespuesta += delta;
	}

	private void OnNpcAsesinado(bool eraEspia, string claseEliminada)
	{
		LoggerDatos.Instance?.RegistrarInteraccion(
			pregunta: PreguntaNivel,
			alternativas: alternativasDinamicas, 
			respuestaJugador: claseEliminada,    
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
			this.ActualizarTextoPuntuacion();
			GD.Print($"¡Error! Inocente eliminado ({claseEliminada}). La nota baja a {puntuacion}");

			if (puntuacion < 40)
			{
				GameOver();
			}
		}
	}

	private void UpdateNpcInfo()
	{
		if (this.selectedNpc != null)
		{
			this.classLabel.Text = $"Class: {this.selectedNpc.pseudoclass}";
			this.animLabel.Text = $"Anim: {this.selectedNpc.currentAnim}";
		}
	}

	private void OnNpcInteractuado(BaseNpc baseNpc)
	{
		this.npcInfoContainer.Visible = true;
		this.selectedNpc = baseNpc;
	}

	private void OnNpcInteractuadoFinished()
	{
		this.npcInfoContainer.Visible = false;
	}

	private void ActualizarTextoPuntuacion()
	{
		if (this.labelPuntuacion != null)
		{
			this.labelPuntuacion.Text = $"Nota: {puntuacion}";
			
			if (puntuacion <= 40)
			{
				this.labelPuntuacion.AddThemeColorOverride("font_color", new Color(1, 0, 0));
			}
		}
	}

	private void GameOver()
	{
		GD.Print("¡Nota inferior a 40! Te echaste el ramo.");
	}
}