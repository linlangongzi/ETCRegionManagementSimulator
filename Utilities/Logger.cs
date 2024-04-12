using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

public sealed class Logger
{
    private static Logger instance;
    private StreamWriter writer;

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    private Logger() { }

    public static Logger Instance()
    {
        if (instance == null) instance = new Logger();
        return instance;
    }

    public async Task Start(StorageFile file)
    {
        try
        {
            // Open the StorageFile for writing
            Stream stream = await file.OpenStreamForWriteAsync();
            // Create a StreamWriter with the stream
            writer = new StreamWriter(stream);
        }
        catch (Exception ex)
        {
            // Handle exceptions here
            Debug.WriteLine($"Error starting logging: {ex.Message}");
        }
    }

    public void Log(string clientId, string message)
    {
        if (writer == null)
        {
            Debug.WriteLine("Logging has not been started. Call Start method first.");
            return;
        }

        try
        {
            // Write the log message with timestamp and client ID
            string logMessage = $"{DateTime.Now} {clientId} < {message}>";
            writer.WriteLine(logMessage);
            writer.Flush(); // Flush the buffer to ensure data is written immediately
            // Output to debug
            Debug.WriteLine(logMessage);
        }
        catch (Exception ex)
        {
            // Handle exceptions here
            Debug.WriteLine($"Error logging message: {ex.Message}");
        }
    }

    public void Stop()
    {
        try
        {
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions here
            Debug.WriteLine($"Error stopping logging: {ex.Message}");
        }
    }

    public async Task SetLogFile(StorageFile file)
    {
        try
        {
            // Dispose the current writer if exists
            writer?.Dispose();
            // Open the StorageFile for writing
            Stream stream = await file.OpenStreamForWriteAsync();
            // Assign the new stream to the writer
            writer = new StreamWriter(stream);
        }
        catch (Exception ex)
        {
            // Handle exceptions here
            Debug.WriteLine($"Error setting log file: {ex.Message}");
        }
    }
}
