using Godot;
using System;
using System.IO; 

public partial class LoggerDatos : Node
{
	public static LoggerDatos Instance { get; private set; }
	
	public string IdJugador { get; set; } = "Desconocido"; 
	
	// 1. NUEVA VARIABLE PARA EL CORREO
	public string CorreoJugador { get; set; } = "SinCorreo"; 
	
	private string rutaArchivo;

	public override void _Ready()
	{
		Instance = this;
		string carpetaLog;

		if (OS.HasFeature("editor"))
		{
			carpetaLog = ProjectSettings.GlobalizePath("res://log");
			if (!Directory.Exists(carpetaLog)) Directory.CreateDirectory(carpetaLog);

			string rutaIgnore = Path.Combine(carpetaLog, ".gdignore");
			if (!File.Exists(rutaIgnore)) File.WriteAllText(rutaIgnore, "Ignorar carpeta");
		}
		else
		{
			string rutaEjecutable = OS.GetExecutablePath();
			string carpetaBase = Path.GetDirectoryName(rutaEjecutable);
			carpetaLog = Path.Combine(carpetaBase, "log");
			if (!Directory.Exists(carpetaLog)) Directory.CreateDirectory(carpetaLog);
		}

		rutaArchivo = Path.Combine(carpetaLog, "log_interacciones.csv");

		if (!Godot.FileAccess.FileExists(rutaArchivo))
		{
			using var file = Godot.FileAccess.Open(rutaArchivo, Godot.FileAccess.ModeFlags.Write);
			if (file != null)
			{
				// 2. AGREGAMOS "Correo" AL ENCABEZADO DEL EXCEL
				file.StoreLine("Timestamp,IdJugador,Correo,Pregunta,Alternativas,RespuestaJugador,FueCorrecta,TiempoDeRespuesta");
			}
		}
	}

	public void RegistrarInteraccion(string pregunta, string alternativas, string respuestaJugador, bool fueCorrecta, float tiempoRespuesta)
	{
		string timestamp = Time.GetDatetimeStringFromSystem();
		string textoCorrecta = fueCorrecta ? "Si" : "No";
		string tiempoFormateado = tiempoRespuesta.ToString(System.Globalization.CultureInfo.InvariantCulture);

		// 3. AGREGAMOS LA VARIABLE "CorreoJugador" A LA LÍNEA DEL LOG
		string lineaLog = $"{timestamp},{IdJugador},{CorreoJugador},\"{pregunta}\",\"{alternativas}\",\"{respuestaJugador}\",{textoCorrecta},{tiempoFormateado}";

		using var file = Godot.FileAccess.Open(rutaArchivo, Godot.FileAccess.ModeFlags.ReadWrite);
		
		if (file == null)
		{
			GD.PrintErr($"¡ERROR! No se pudo escribir en el log. ¿El archivo CSV está abierto en Excel? Ciérralo y vuelve a intentar.");
			return; 
		}

		file.SeekEnd();
		file.StoreLine(lineaLog);
		
		GD.Print("Log guardado en: " + rutaArchivo);
	}
}
