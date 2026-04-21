using System;
using System.Net;
using System.Security.Cryptography;
using Godot;
using Godot.Collections;

public partial class Model : Node
{
    private CharacterBody2D player;
    private AnimationPlayer animationPlayer;
    private MoveBase currentMove;
    private Dictionary<MOVES, MoveBase> moves;

    private enum LOOK_DIRECTION
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    };

    public override void _Ready()
    {
        this.animationPlayer = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Sprite2D>("Sprite2D").GetNode<AnimationPlayer>("AnimationPlayer");

        // create node variables to give the nodes of the states and then give them to the dictionary
        Node idle = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Node>("Model").GetNode<Node>("Idle");
        Node moving = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Node>("Model").GetNode<Node>("Moving");
        Node interacting = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Node>("Model").GetNode<Node>("Interacting");

        this.moves = new Godot.Collections.Dictionary<MOVES, MoveBase>
        {
            {MOVES.IDLE, (MoveBase)idle},
            {MOVES.MOVING, (MoveBase)moving},
            {MOVES.INTERACTING, (MoveBase)interacting}
        };

        // then put the player inside all the states so that they can call the player
        this.player = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player");
        this.currentMove = this.moves[MOVES.IDLE];
        this.moves[MOVES.IDLE].player = this.player;
        this.moves[MOVES.IDLE].animationPlayer = this.animationPlayer;
        this.moves[MOVES.MOVING].player = this.player;
        this.moves[MOVES.MOVING].animationPlayer = this.animationPlayer;
        this.moves[MOVES.INTERACTING].player = this.player;
        this.moves[MOVES.INTERACTING].animationPlayer = this.animationPlayer;

    }

    public void Update(InputPackage inputPackage, double delta)
    {
        MOVES relevance = this.currentMove.TransitionLogic(inputPackage);
        if (relevance != MOVES.NULL)
        {
            this.SwitchTo(relevance);
        }

        this.currentMove.Update(inputPackage, delta);
    }

    private void SwitchTo(MOVES state)
    {
        this.currentMove.OnExitState();
        this.currentMove = this.moves[state];
        this.currentMove.OnEnterState();
    }
}
