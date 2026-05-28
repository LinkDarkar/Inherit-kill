using Godot;
using System;

public partial class InstruccionesUI : CanvasLayer
{
	// [Export] hace que estas variables aparezcan en el editor de Godot.
	// PropertyHint.MultilineText permite presionar "Enter" para escribir bloques de código.
	
	[Export] 
	public string Titulo = "NIVEL X";
	
	[Export(PropertyHint.MultilineText)] 
	public string CodigoPrincipal = "Aquí va el código del anexo...";

	// Variables para tu primer HBoxContainer (puedes agregar más si las necesitas)
	[Export] public string NombreDato1 = "Clase:";
	[Export] public string ValorDato1 = "Ejemplo";

	public override void _Ready()
	{
		// 1. Obtenemos las referencias a los Labels de tu árbol
		// Asegúrate de que la ruta coincida exactamente con tu estructura
		Label lblTitulo = GetNode<Label>("PanelContainer/MarginContainer/VBoxContainer/TestTítulo");
		Label lblCodigo = GetNode<Label>("PanelContainer/MarginContainer/VBoxContainer/TestTítulo2");
		
		Label lblNombre1 = GetNode<Label>("PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/nombre");
		Label lblValor1 = GetNode<Label>("PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/datos");

		// 2. Asignamos los textos configurados en el editor a los nodos reales
		lblTitulo.Text = Titulo;
		lblCodigo.Text = CodigoPrincipal;
		
		lblNombre1.Text = NombreDato1;
		lblValor1.Text = ValorDato1;
		
		// *Nota: Si quieres ocultar un HBoxContainer cuando no escribas nada en el Inspector:
		// si (ValorDato1 == "") GetNode<Control>(".../HBoxContainer").Hide();
	}
}
