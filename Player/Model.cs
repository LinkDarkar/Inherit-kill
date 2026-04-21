using System;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using Godot;
using Godot.Collections;

public partial class Model : Node
{
    private CharacterBody2D player;

    private Sprite2D sprite2D;
    private Sprite2D attackSprite;

    private AnimationPlayer animationPlayer;
    private AnimationPlayer animationPlayerAttack;

    private MoveBase currentMove;
    private Dictionary<MOVES, MoveBase> moves;

    private DIRECTION lastLookDirection = DIRECTION.DOWN;
    private DIRECTION attackDirection = DIRECTION.DOWN;

    public override void _Ready()
    {
        this.sprite2D = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Sprite2D>("Sprite2D");
        this.attackSprite = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Sprite2D>("AttackSprite");

        this.animationPlayer = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Sprite2D>("Sprite2D").GetNode<AnimationPlayer>("AnimationPlayer");
        this.animationPlayerAttack = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Sprite2D>("AttackSprite").GetNode<AnimationPlayer>("AnimationPlayer");

        // create node variables to give the nodes of the states and then give them to the dictionary
        Node idle = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Node>("Model").GetNode<Node>("Idle");
        Node moving = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Node>("Model").GetNode<Node>("Moving");
        Node interacting = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Node>("Model").GetNode<Node>("Interacting");
        Node attacking = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Node>("Model").GetNode<Node>("Attacking");

        this.moves = new Godot.Collections.Dictionary<MOVES, MoveBase>
        {
            {MOVES.IDLE, (MoveBase)idle},
            {MOVES.MOVING, (MoveBase)moving},
            {MOVES.INTERACTING, (MoveBase)interacting},
            {MOVES.ATTACKING, (MoveBase)attacking}
        };

        // then put the player inside all the states so that they can call the player
        this.player = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player");
        this.currentMove = this.moves[MOVES.IDLE];
        this.moves[MOVES.IDLE].player = this.player;
        this.moves[MOVES.MOVING].player = this.player;
        this.moves[MOVES.INTERACTING].player = this.player;
        this.moves[MOVES.ATTACKING].player = this.player;

        this.animationPlayerAttack.AnimationFinished += this.OnAttackAnimationFinished;
    }

    private void OnAttackAnimationFinished(StringName animationName)
    {
        if (animationName.ToString().StartsWith("attack") == false)
        {
            return;
        }

        this.SwitchTo(MOVES.IDLE);
    }

    public void Update(InputPackage inputPackage, double delta)
    {
        MOVES relevance = this.currentMove.TransitionLogic(inputPackage);
        if (relevance != MOVES.NULL && relevance != this.currentMove.moveType)
        {
            this.SwitchTo(relevance);
        }

        this.currentMove.Update(inputPackage, delta);

        this.ChangeVisuals();
        this.HandleAnimations(inputPackage);
    }

    private void SwitchTo(MOVES state)
    {
        this.currentMove.OnExitState();
        this.currentMove = this.moves[state];

        if (state == MOVES.ATTACKING)
        {
            this.attackDirection = this.lastLookDirection;
        }

        this.currentMove.OnEnterState();
    }

    private void ChangeVisuals()
    {
        if (this.currentMove.moveType == MOVES.ATTACKING)
        {
            this.sprite2D.Visible = false;
            this.attackSprite.Visible = true;
        }
        else
        {
            this.sprite2D.Visible = true;
            this.attackSprite.Visible = false;
        }
    }

    private void HandleAnimations(InputPackage inputPackage)
    {
        this.lastLookDirection = inputPackage.lastDirection;

        switch (this.currentMove.moveType)
        {
            case MOVES.IDLE:
                this.PlayIdleAnimation(this.lastLookDirection);
                break;
            case MOVES.MOVING:
                this.PlayMoveAnimation(this.lastLookDirection);
                break;
            case MOVES.ATTACKING:
                this.PlayAttackAnimation(this.attackDirection);
                break;
            default:
                this.PlayIdleAnimation(this.lastLookDirection);
                break;
        }
    }

    private void PlayIdleAnimation(DIRECTION lastLookdirection)
    {
        string animation = lastLookdirection switch
        {
            DIRECTION.UP => "idle_up",
            DIRECTION.DOWN => "idle_down",
            DIRECTION.LEFT => "idle_left",
            DIRECTION.RIGHT => "idle_right",
            _ => "idle_down"
        };
        
        if (this.animationPlayer.CurrentAnimation != animation)
        {
            this.animationPlayer.Play(animation);
        }
    }

    private void PlayMoveAnimation(DIRECTION lastLookdirection)
    {
        string animation = lastLookdirection switch
        {
            DIRECTION.UP => "walk_up",
            DIRECTION.DOWN => "walk_down",
            DIRECTION.LEFT => "walk_left",
            DIRECTION.RIGHT => "walk_right",
            _ => "walk_down"
        };

        if (this.animationPlayer.CurrentAnimation != animation)
        {
            this.animationPlayer.Play(animation);
        }
    }

    private void PlayAttackAnimation(DIRECTION lastLookdirection)
    {
        string animation = lastLookdirection switch
        {
            DIRECTION.UP => "attack_up",
            DIRECTION.DOWN => "attack_down",
            DIRECTION.LEFT => "attack_left",
            DIRECTION.RIGHT => "attack_right",
            _ => "attack_down"
        };

        this.animationPlayerAttack.Play(animation);
    }
}
