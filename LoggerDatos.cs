using Godot;
using System;

public partial class LoggerDatos : Node
{
	// Esto nos permite llamar a LoggerDatos.Instance desde cualquier parte del juego
	public static LoggerDatos Instance { get; private set; }
	
	// Aquí guardaremos el RUT del jugador cuando inicie sesión
	public string IdJugador { get; set; } = "Desconocido"; 
	
	// "user://" guarda el archivo en la carpeta segura de datos de la aplicación del usuario
	private string rutaArchivo = "user://log_interacciones.csv";

	public override void _Ready()
	{
		Instance = this;

		// Si el archivo no existe, lo creamos y le ponemos los encabezados (las columnas)
		if (!FileAccess.FileExists(rutaArchivo))
		{
			using var file = FileAccess.Open(rutaArchivo, FileAccess.ModeFlags.Write);
			file.StoreLine("Timestamp,IdJugador,Pregunta,Alternativas,RespuestaJugador,FueCorrecta,TiempoDeRespuesta");
		}
	}

	// Llama a esta función cada vez que el jugador mate a un NPC
	public void RegistrarInteraccion(string pregunta, string alternativas, string respuestaJugador, bool fueCorrecta, float tiempoRespuesta)
	{
		string timestamp = Time.GetDatetimeStringFromSystem();
		string textoCorrecta = fueCorrecta ? "Si" : "No";

		// Formateamos la línea con comas para el CSV
		string lineaLog = $"{timestamp},{IdJugador},\"{pregunta}\",\"{alternativas}\",\"{respuestaJugador}\",{textoCorrecta},{tiempoRespuesta}";

		// Abrimos el archivo, nos vamos al final, y agregamos la nueva línea
		using var file = FileAccess.Open(rutaArchivo, FileAccess.ModeFlags.ReadWrite);
		file.SeekEnd();
		file.StoreLine(lineaLog);
		
		GD.Print("Log guardado: " + lineaLog);
	}
}
