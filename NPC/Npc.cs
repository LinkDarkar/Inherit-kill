using Godot;
using System;

public partial class Npc : CharacterBody2D
{
    [Export]
    private SpriteFrames spriteFrames;

    [Export]
    private Texture2D texture2D;

    [Export]
    public bool isSpy = false;

    [Export]
    private PSEUDOCLASS pseudoclass = PSEUDOCLASS.CIVIL;

    private Sprite2D sprite2D;
    private AnimatedSprite2D animatedSprite2D;
    private AnimationPlayer deathAnimationPlayer;
    private AudioStreamPlayer2D audioDyingSound;
    private Timer timer;
    private bool timerStopped = false;
    private bool changeAnim = false;

    private string currentAnim = "idle_down";

    public override void _Ready()
    {
        base._Ready();
        this.sprite2D = GetNode<Sprite2D>("Sprite2D");
        this.sprite2D.Texture = texture2D;

        this.animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        this.animatedSprite2D.SpriteFrames = this.spriteFrames;

        this.deathAnimationPlayer = GetNode<AnimatedSprite2D>("AnimatedSprite2D").GetNode<AnimationPlayer>("AnimationPlayer");
        this.audioDyingSound = GetNode<AudioStreamPlayer2D>("AudioDyingSound");

        this.timer = GetNode<Timer>("Timer");
        this.timer.WaitTime = 5;
        this.timer.Start();

        this.animatedSprite2D.Play("idle_down");

        this.timer.Timeout += this.TimerStop;
    }

    private void HandleAnimations()
    {
        if (this.pseudoclass == PSEUDOCLASS.CIVIL)
        {
            this.currentAnim = "dance_down";

            if (this.isSpy == true && this.changeAnim == true)
            {
                this.currentAnim = "spying_down";
                // this.changeAnim = false;
                // GD.Print("tries to spy down");
            }
            return;
        }
        else if (this.pseudoclass == PSEUDOCLASS.GUARDIA)
        {
            // play idle and every 5 seconds vigilar
            this.currentAnim = "idle_down";
            if (this.changeAnim == true)
            {
                this.currentAnim = "vigilar_down";
            }
            return;
        }
        else if (this.pseudoclass == PSEUDOCLASS.STAFF)
        {
            // play idle and every 5 seconds clean
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
        this.changeAnim = !this.changeAnim;
        // GD.Print("timer stopped");
        this.timer.Start();
    }

    private void PlayDyingSound()
    {
        this.audioDyingSound.Play();
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
        this.deathAnimationPlayer.AnimationFinished += this.OnDeathAnimationFinished;
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
