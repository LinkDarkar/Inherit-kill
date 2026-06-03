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
    private Sprite2D interactSprite;

    private Area2D hitbox;

    private AnimationPlayer animationPlayer;
    private AnimationPlayer animationPlayerAttack;
    private AnimationPlayer animationPlayerInteract;

    public MoveBase currentMove;
    private Dictionary<MOVES, MoveBase> moves;

    private DIRECTION lastLookDirection = DIRECTION.DOWN;
    private DIRECTION attackDirection = DIRECTION.DOWN;

    // this boolean is for the interact state
    // since we have no interact animation, we don't want to repeat the sfx constantly
    // and we ALSO WANT to maintain the state even if the animation ended
    // we have this to, in case we need it, stop the animation playing once it ended once
    public bool shouldStopAnim = false;

    public override void _Ready()
    {
        this.sprite2D = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Sprite2D>("Sprite");
        this.attackSprite = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Sprite2D>("AttackSprite");
        this.interactSprite = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Sprite2D>("InteractSprite"); 

        this.hitbox = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Area2D>("Area2DAttack");

        this.animationPlayer = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Sprite2D>("Sprite").GetNode<AnimationPlayer>("AnimationPlayer");
        this.animationPlayerAttack = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Sprite2D>("AttackSprite").GetNode<AnimationPlayer>("AnimationPlayer");
        this.animationPlayerInteract = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Sprite2D>("InteractSprite").GetNode<AnimationPlayer>("AnimationPlayer");

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
        this.animationPlayerInteract.AnimationFinished += this.OnInteractAnimationFinished;
    }

    private void OnAttackAnimationFinished(StringName animationName)
    {
        if (animationName.ToString().StartsWith("attack") == false)
        {
            return;
        }

        this.SwitchTo(MOVES.IDLE);
    }

    private void OnInteractAnimationFinished(StringName animationName)
    {
        this.shouldStopAnim = true;
    }

    public void Update(InputPackage inputPackage, double delta)
    {
        MOVES relevance = this.currentMove.TransitionLogic(inputPackage);
        if (relevance != MOVES.NULL && relevance != this.currentMove.moveType)
        {
            this.SwitchTo(relevance);
            if (relevance == MOVES.INTERACTING)
            {
                // if it enters interacting, shouldStopAnim becomes false
                // so the anim and sfx plays once
                this.shouldStopAnim = false;
            }
        }

        this.currentMove.Update(inputPackage, delta);

        this.ChangeVisuals();
        this.HandleAnimations(inputPackage);
    }

    private void SwitchTo(MOVES state)
    {
        this.currentMove.OnExitState();
        this.currentMove = this.moves[state];

        if (state == MOVES.ATTACKING || state == MOVES.INTERACTING)
        {
            this.attackDirection = this.lastLookDirection;
            this.UpdateHitboxPosition();
        }

        this.currentMove.OnEnterState();
    }

    private void ChangeVisuals()
    {
        if (this.currentMove.moveType == MOVES.ATTACKING)
        {
            this.sprite2D.Visible = false;
            this.attackSprite.Visible = true;
            this.interactSprite.Visible = false;
        }
        else if (this.currentMove.moveType == MOVES.INTERACTING)
        {
            this.sprite2D.Visible = false;
            this.attackSprite.Visible = false;
            this.interactSprite.Visible = true;
        }
        else
        {
            this.sprite2D.Visible = true;
            this.attackSprite.Visible = false;
            this.interactSprite.Visible = false;
        }
    }

    private void UpdateHitboxPosition()
    {
        Vector2 positionOffset = this.attackDirection switch
        {
            DIRECTION.UP => new Vector2(0,-16),
            DIRECTION.DOWN => new Vector2(0,-3),
            DIRECTION.LEFT => new Vector2(-6,-12),
            DIRECTION.RIGHT => new Vector2(6,-12),
            _ => new Vector2(0,-3)
        };
        this.hitbox.Position = positionOffset;

        float rotation = this.attackDirection switch
        {
            DIRECTION.UP => 0f,
            DIRECTION.DOWN => 0f,
            DIRECTION.LEFT => 90f,
            DIRECTION.RIGHT => 90,
            _ => 0f
        };
        this.hitbox.RotationDegrees = rotation;
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
            case MOVES.INTERACTING:
                this.PlayInteractAnimation(this.attackDirection);
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

    private void PlayInteractAnimation(DIRECTION lastLookDirection)
    {
        string animation = lastLookDirection switch
        {
            DIRECTION.UP => "interact_up",
            DIRECTION.DOWN => "interact_down",
            DIRECTION.LEFT => "interact_left",
            DIRECTION.RIGHT => "interact_right",
            _ => "interact_down"
        };

        if (this.shouldStopAnim == true)
        {
            // if it already played once and it finished, the animation doesn't play again
            return;
        }

        this.animationPlayerInteract.Play(animation);
    }
}
