using Godot;
using System;
using System.ComponentModel;

public partial class BaseLevel : Node2D
{
    private int puntuacion = 70;
	private Label labelPuntuacion;

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

		// Conectamos la señal de cada NPC hijo
		foreach (Node hijo in contenedorNpcs.GetChildren())
		{
			// Verificamos que el hijo sea realmente de la clase Npc de tu script
			if (hijo is BaseNpc npc)
			{
				npc.NpcAsesinado += this.OnNpcAsesinado;
				npc.NpcInteractuado += this.OnNpcInteractuado;
			}
		}
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
		// I don't know how this works really
		Vector2 screenPos = 
			this.selectedNpc.GlobalPosition
			- this.camera2D.GetScreenCenterPosition()
			+ GetViewport().GetVisibleRect().Size / 2.0f;

		return screenPos;
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
		// update info in labels here?????
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
			
			if (puntuacion == 40)
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
