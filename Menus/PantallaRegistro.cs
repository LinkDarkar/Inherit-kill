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
		if (inputRut.Text == "" || inputCorreo.Text == "")
		{
			GD.Print("Por favor, ingrese todos los datos.");
			return;
		}

		// Enviamos el RUT
		LoggerDatos.Instance.IdJugador = inputRut.Text;
		
		// Enviamos el Correo
		LoggerDatos.Instance.CorreoJugador = inputCorreo.Text;

		// Cambiamos a la escena del nivel
		GetTree().ChangeSceneToFile("res://Menus/menu_principal.tscn"); // Ajusta a tu ruta real
	}
}
