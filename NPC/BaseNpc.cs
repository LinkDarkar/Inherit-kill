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
	public PSEUDOCLASS pseudoclass = PSEUDOCLASS.CIVIL;

	// DO NOT replace animationNames values in child classes, they are exclusively defined in the editor
	[Export]
	protected string[] animationNames;
	
	[Signal]
	public delegate void NpcAsesinadoEventHandler(bool eraEspia);
	[Signal]
	public delegate void NpcInteractuadoEventHandler(BaseNpc baseNpc);

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
	public string currentAnim = "idle_down";

	public override void _Ready()
	{
		base._Ready();
		this.sprite2D = GetNode<Sprite2D>("Sprite2D");
		this.sprite2D.Texture = texture2D;

		// mostly just makes sure that the right animatedSprite2D is set
		this.animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		this.animatedSprite2D.SpriteFrames = this.spriteFrames;

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
		if (this.animationNames == null)
		{
			GD.Print($"{this.Name} has no animations to choose from");
			return;
		}

		if (this.changeAnim == true)
        {
            // TODO add exclusive SPY counter to make sure there is not too much time
            // between spy exclusive behaviour
            int newAnimIndex = GD.RandRange(0, this.animationNames.Length - 1);
            this.currentAnim = this.animationNames[newAnimIndex];
            this.changeAnim = false;
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

	public void PlayInteractionEffects()
	{
		this.HandlePlayerInteraction();
		// add glow???
	}

	public void HandlePlayerInteraction()
	{
		EmitSignal(SignalName.NpcInteractuado, this);
	}
}
