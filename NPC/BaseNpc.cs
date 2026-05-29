using Godot;

public partial class BaseNpc : CharacterBody2D
{
	[Export]
	private SpriteFrames spriteFrames;

	[Export]
	private Texture2D texture2D;

	[Export]
	public bool isSpy = false;

	[Export]
	private PSEUDOCLASS pseudoclass = PSEUDOCLASS.CIVIL;

	// DO NOT replace animationNames values in child classes, they are exclusively defined in the editor
	[Export]
	protected string[] animationNames;
	
	[Signal]
	public delegate void NpcAsesinadoEventHandler(bool eraEspia);

	protected Sprite2D sprite2D;
	protected AnimatedSprite2D animatedSprite2D;
	// protected 
	protected AnimationPlayer deathAnimationPlayer;
	protected AudioStreamPlayer2D audioDyingSound;
	protected AudioStreamPlayer2D audioIfSpy;

	protected Timer timer;
	protected bool timerStopped = false;
	protected int lowerTimeLimit = 1;
	protected int upperTimeLimit = 5;

	protected bool changeAnim = false;
	protected string currentAnim = "idle_down";

	public override void _Ready()
	{
		base._Ready();
		this.sprite2D = GetNode<Sprite2D>("Sprite2D");
		this.sprite2D.Texture = texture2D;

		// mostly just makes sure that the right animatedSprite2D is set
		this.animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		this.animatedSprite2D.SpriteFrames = this.spriteFrames;

		// this.animationNames = this.spriteFrames.GetAnimationNames();

		this.deathAnimationPlayer = GetNode<AnimatedSprite2D>("AnimatedSprite2D").GetNode<AnimationPlayer>("AnimationPlayer");
		this.audioDyingSound = GetNode<AudioStreamPlayer2D>("AudioDyingSound");
		this.audioIfSpy = GetNode<AudioStreamPlayer2D>("AudioIfSpy");

		this.animatedSprite2D.Play("idle_down");
		this.timerSetup();
	}

	protected virtual void timerSetup()
	{
		this.timer = GetNode<Timer>("Timer");
		this.timer.WaitTime = GD.RandRange(this.lowerTimeLimit, this.upperTimeLimit);
		this.timer.Start();
		this.timer.Timeout += this.TimerStop;
	}

	protected virtual void HandleAnimations()
	{
		if (this.pseudoclass == PSEUDOCLASS.CIVIL)
		{
			this.currentAnim = "dance_down";
			if (this.isSpy == true && this.changeAnim == true)
			{
				this.currentAnim = "spying_down";
			}
			return;
		}
		else if (this.pseudoclass == PSEUDOCLASS.GUARDIA)
		{
			// play idle and every X seconds vigilar
			this.currentAnim = "idle_down";
			if (this.changeAnim == true)
			{
				this.currentAnim = "vigilar_down";
			}
			return;
		}
		else if (this.pseudoclass == PSEUDOCLASS.STAFF)
		{
			// play idle and every X seconds clean
			this.currentAnim = "idle_down";
			if (this.changeAnim == true)
			{
				this.currentAnim = "clean_right";
			}
			return;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		this.HandleAnimations();
		this.animatedSprite2D.Play(this.currentAnim);
	}


	private void TimerStop()
	{
		this.timerStopped = true;
		this.changeAnim = true;
		this.timer.Start();
	}

	private void PlayDyingSound()
	{
		this.audioDyingSound.Play();

		// this audio down here should be played from scene node script instead of here
		if (this.isSpy == true)
		{
			this.audioIfSpy.Play();
		}
	}

	private void PlayVanishEffect()
	{
		this.deathAnimationPlayer.Play("dying");
	}

	private void OnDeathAnimationFinished(StringName animationName)
	{
		this.QueueFree();
	}

	public void PlayDyingEffects()
	{
		this.PlayDyingSound();
		this.PlayVanishEffect();
		
		// Nota: Es buena práctica asegurar que no te suscribas dos veces al evento 
		// si el NPC recibe daño dos veces rápidamente, pero por ahora está bien así.
		this.deathAnimationPlayer.AnimationFinished += this.OnDeathAnimationFinished;
		
		// 2. EMITIMOS LA SEÑAL PASANDO EL VALOR DE isSpy
		EmitSignal(SignalName.NpcAsesinado, this.isSpy);

		// Tus prints de comprobación (los puedes dejar para depurar)
		if (this.isSpy == true)
		{
			GD.Print("exito");
		}
		else
		{
			GD.Print("fracaso");
		}
	}
}
