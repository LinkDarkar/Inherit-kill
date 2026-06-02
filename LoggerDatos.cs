using Godot;
using System;
using System.IO; 

public partial class LoggerDatos : Node
{
	public static LoggerDatos Instance { get; private set; }
	
	public string IdJugador { get; set; } = "Desconocido"; 
	
	private string rutaArchivo;

	public override void _Ready()
	{
		Instance = this;

		if (OS.HasFeature("editor"))
		{
			rutaArchivo = ProjectSettings.GlobalizePath("res://log_interacciones.csv");
		}
		else
		{
			string rutaEjecutable = OS.GetExecutablePath();
			string carpetaBase = Path.GetDirectoryName(rutaEjecutable);
			rutaArchivo = Path.Combine(carpetaBase, "log_interacciones.csv");
		}

		// SOLUCIÓN: Especificamos explícitamente "Godot.FileAccess"
		if (!Godot.FileAccess.FileExists(rutaArchivo))
		{
			using var file = Godot.FileAccess.Open(rutaArchivo, Godot.FileAccess.ModeFlags.Write);
			file.StoreLine("Timestamp,IdJugador,Pregunta,Alternativas,RespuestaJugador,FueCorrecta,TiempoDeRespuesta");
		}
	}

	public void RegistrarInteraccion(string pregunta, string alternativas, string respuestaJugador, bool fueCorrecta, float tiempoRespuesta)
	{
		string timestamp = Time.GetDatetimeStringFromSystem();
		string textoCorrecta = fueCorrecta ? "Si" : "No";

		string tiempoFormateado = tiempoRespuesta.ToString(System.Globalization.CultureInfo.InvariantCulture);

		string lineaLog = $"{timestamp},{IdJugador},\"{pregunta}\",\"{alternativas}\",\"{respuestaJugador}\",{textoCorrecta},{tiempoFormateado}";

		// SOLUCIÓN: Especificamos explícitamente "Godot.FileAccess"
		using var file = Godot.FileAccess.Open(rutaArchivo, Godot.FileAccess.ModeFlags.ReadWrite);
		file.SeekEnd();
		file.StoreLine(lineaLog);
		
		GD.Print("Log guardado en: " + rutaArchivo);
		GD.Print(lineaLog);
	}
}
