using Godot;
using System;

public partial class MenuPrincipal : Control
{
	public override void _Ready()
	{
		// Recuerda respetar las mayúsculas/minúsculas de los nodos hijos
		Button btnJugar = GetNode<Button>("VBoxContainer/botonJugar");
		Button btnSalir = GetNode<Button>("VBoxContainer/botonSalir");

		btnJugar.Pressed += OnBotonJugarPressed;
		btnSalir.Pressed += OnBotonSalirPressed;
	}

	private void OnBotonJugarPressed()
	{
		// CAMBIA "res://nivel_1.tscn" por el nombre real del archivo de tu escena de juego
		GetTree().ChangeSceneToFile("res://node_2d.tscn");
	}

	private void OnBotonSalirPressed()
	{
		GetTree().Quit();
	}
}
