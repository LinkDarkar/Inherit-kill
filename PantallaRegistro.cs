using Godot;
using System;

public partial class PantallaRegistro : Control
{
	private LineEdit inputRut;
	private LineEdit inputCorreo;

	public override void _Ready()
	{
		inputRut = GetNode<LineEdit>("VBoxContainer/InputRUT");
		inputCorreo = GetNode<LineEdit>("VBoxContainer/InputCorreo");
	}

	// Conecta la señal 'pressed' de tu botón a este método
	public void _on_boton_empezar_pressed()
	{
		// Validamos que no dejen los campos vacíos
		if (inputRut.Text == "" || inputCorreo.Text == "")
		{
			GD.Print("Por favor, ingrese todos los datos.");
			return;
		}

		// Guardamos el ID del jugador en nuestro Singleton global
		// Puedes concatenar el RUT y el Correo si lo deseas
		LoggerDatos.Instance.IdJugador = inputRut.Text;

		// Cambiamos a la escena de tu Menú Principal o directamente al Nivel 1
		GetTree().ChangeSceneToFile("res://Menus/menu_principal.tscn"); 
	}
}
