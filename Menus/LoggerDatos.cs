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

		string carpetaLog;

		if (OS.HasFeature("editor"))
		{
			// 1. Ruta en el editor: carpeta "log" en la raíz del proyecto
			carpetaLog = ProjectSettings.GlobalizePath("res://log");
			
			// Si la carpeta no existe, la creamos
			if (!Directory.Exists(carpetaLog))
			{
				Directory.CreateDirectory(carpetaLog);
			}

			// 2. MAGIA ANTI-TRANSLATION: Creamos un archivo .gdignore
			// Esto le dice al motor de Godot que ignore los CSV de esta carpeta
			string rutaIgnore = Path.Combine(carpetaLog, ".gdignore");
			if (!File.Exists(rutaIgnore))
			{
				File.WriteAllText(rutaIgnore, "Ignorar carpeta para evitar archivos translation");
			}
		}
		else
		{
			// 3. Ruta en el juego exportado (.exe)
			string rutaEjecutable = OS.GetExecutablePath();
			string carpetaBase = Path.GetDirectoryName(rutaEjecutable);
			
			// Creamos la carpeta "log" junto al ejecutable
			carpetaLog = Path.Combine(carpetaBase, "log");

			if (!Directory.Exists(carpetaLog))
			{
				Directory.CreateDirectory(carpetaLog);
			}
		}

		// 4. Asignamos la ruta final de nuestro archivo Excel dentro de la nueva carpeta
		rutaArchivo = Path.Combine(carpetaLog, "log_interacciones.csv");

		// Creamos el archivo y los encabezados si no existe
		if (!Godot.FileAccess.FileExists(rutaArchivo))
		{
			using var file = Godot.FileAccess.Open(rutaArchivo, Godot.FileAccess.ModeFlags.Write);
			if (file != null)
			{
				file.StoreLine("Timestamp,IdJugador,Pregunta,Alternativas,RespuestaJugador,FueCorrecta,TiempoDeRespuesta");
			}
		}
	}

	// ... (Mantén tu función RegistrarInteraccion exactamente igual que antes abajo de esto) ...

	public void RegistrarInteraccion(string pregunta, string alternativas, string respuestaJugador, bool fueCorrecta, float tiempoRespuesta)
	{
		string timestamp = Time.GetDatetimeStringFromSystem();
		string textoCorrecta = fueCorrecta ? "Si" : "No";

		string tiempoFormateado = tiempoRespuesta.ToString(System.Globalization.CultureInfo.InvariantCulture);

		string lineaLog = $"{timestamp},{IdJugador},\"{pregunta}\",\"{alternativas}\",\"{respuestaJugador}\",{textoCorrecta},{tiempoFormateado}";

		using var file = Godot.FileAccess.Open(rutaArchivo, Godot.FileAccess.ModeFlags.ReadWrite);
		file.SeekEnd();
		file.StoreLine(lineaLog);
		
		GD.Print("Log guardado en: " + rutaArchivo);
		GD.Print(lineaLog);
	}
}
