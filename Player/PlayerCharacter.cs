using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D
{
    private InputGatherer inputGatherer;
    private Model model;

    public override void _Ready()
    {
        this.inputGatherer = (InputGatherer)GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Node>("Input");
        this.model = (Model)GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Node>("Model");
    }

    public override void _PhysicsProcess(double delta)
    {
        InputPackage inputPackage = this.inputGatherer.GatherInputs();

        this.model.Update(inputPackage, delta);
    }
}
