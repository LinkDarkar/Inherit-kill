using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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

	[Export] protected PlayerCharacter playerCharacter;
	[Export] protected Camera2D camera2D;
	[Export] protected PanelContainer npcInfoContainer;
	[Export] protected Label classLabel;
	[Export] protected Label animLabel;
	

	[Export(PropertyHint.MultilineText)] 
	public string ConsejoDerrota = "aaaa";
	[Export] 
	public PackedScene EscenaSiguiente; // Arrastra aquí el siguiente nivel en el Inspector
	
	private Control popupFinal;
	private Label labelResultado;
	private Button botonContinuar;
	private bool nivelAprobado = false; // Nos ayuda a saber qué hará el botón
	private Label labelConsejo;
	public override void _Ready()
	{
		this.labelPuntuacion = GetNode<Label>("UI/LabelPuntuacion");
		this.ActualizarTextoPuntuacion();

		// --- NUEVO: BUSCAMOS LOS NODOS DEL POPUP Y CONECTAMOS EL BOTÓN ---
		this.popupFinal = GetNode<Control>("UI/PopupFinal");
		this.labelResultado = GetNode<Label>("UI/PopupFinal/VBoxContainer/LabelResultados");
		this.botonContinuar = GetNode<Button>("UI/PopupFinal/VBoxContainer/BotonContinuar");
		this.labelConsejo = GetNode<Label>("UI/PopupFinal/VBoxContainer/LabelConsejo");
		this.botonContinuar.Pressed += OnBotonContinuarPressed;
		this.popupFinal.Hide(); // Nos aseguramos de que esté oculto al iniciar
		// -----------------------------------------------------------------

		Node contenedorNpcs = GetNode<Node>("NPCs");
		HashSet<string> clasesEnElMapa = new HashSet<string>();

		foreach (Node hijo in contenedorNpcs.GetChildren())
		{
			if (hijo is BaseNpc npc)
			{
				npc.NpcAsesinado += this.OnNpcAsesinado;
				npc.NpcInteractuado += this.OnNpcInteractuado;
				clasesEnElMapa.Add(npc.pseudoclass.ToString());
			}
		}
		
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
			MostrarPopupFinal(true); // --- NUEVO: Llamamos al popup de victoria
		}
		else
		{
			puntuacion -= 10;
			this.ActualizarTextoPuntuacion();
			GD.Print($"¡Error! Inocente eliminado ({claseEliminada}). La nota baja a {puntuacion}");

			if (puntuacion < 40)
			{
				MostrarPopupFinal(false); // --- NUEVO: Llamamos al popup de derrota
			}
		}
	}

	private void MostrarPopupFinal(bool victoria)
	{
		this.nivelAprobado = victoria;
		this.popupFinal.Show();

		if (victoria)
		{
			this.labelResultado.Text = "¡Nivel Aprobado!";
			this.botonContinuar.Text = "Siguiente Escenario";
			
			if (this.labelConsejo != null) 
			{
				this.labelConsejo.Hide();
			}
		}
		else
		{
			this.labelResultado.Text = "Nivel Reprobado (Nota inferior a 4.0)";
			this.botonContinuar.Text = "Reintentar";
			
			if (this.labelConsejo != null)
			{
				this.labelConsejo.Text = "Tip:\n" + ConsejoDerrota;
				this.labelConsejo.Show();
			}
		}
	}

	private void OnBotonContinuarPressed()
	{
		if (this.nivelAprobado)
		{
			// Si aprobó y hay una escena cargada en el Inspector, avanzamos
			if (this.EscenaSiguiente != null)
			{
				GetTree().ChangeSceneToPacked(this.EscenaSiguiente);
			}
			else
			{
				GD.Print("No hay escena siguiente asignada en el Inspector.");
				// Opcional: Aquí podrías enviarlo a la pantalla de créditos/victoria final
			}
		}
		else
		{
			// Si falló, recargamos la escena actual para que lo intente de nuevo
			GetTree().ReloadCurrentScene();
		}
	}
	// -----------------------------------------------------------------

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

	// GameOver original eliminado, ya que MostrarPopupFinal(false) cumple su función
}
