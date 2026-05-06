using Godot;
using System;
using System.Collections.Generic;

public partial class LevelBase : Node2D
{
    [Export]
    private Node2D npcsNode;

    [Export]
    private Npc spyNpc;
    
    // should have a list of the npcs??????
    // instead of giving it directly through export thing since it could lead to dumb mistakes
    // private List<CharacterBody2D>

    [Export]
    private CanvasLayer ui;

    private Label pointsLabel;

    private bool levelFinished = false;

    public override void _Ready()
    {
        base._Ready();
        this.pointsLabel = ui.GetNode<PanelContainer>("PanelContainer2").GetNode<MarginContainer>("MarginContainer").GetNode<Label>("Label");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (IsInstanceValid(this.spyNpc) == false && this.levelFinished == false)
        {
            GD.Print("oaisdhjfgpoaidshfoiahsdpofiaj");
            this.levelFinished = true;
            this.pointsLabel.Text = "10";
        }
    }
}
