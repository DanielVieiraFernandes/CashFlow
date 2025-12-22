using System.Reflection;

namespace CashFlow.Api.Utils;

public static class RecordLog
{
    public static void RecordErrorLog(System.Exception exception)
    {
        //**********************************************
        // Recupero a pasta em que está a DLL
        //**********************************************
        var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var logPath = Path.Combine(assemblyLocation!, "log");

        //**********************************************
        // Caso não exista a pasta log, a API cria
        //**********************************************
        Directory.CreateDirectory(logPath);

        var path = Path.Combine(logPath, $"log_{DateTime.Now:yyyy-MM-dd}.txt");

        string logMessage = $@"
--------------------------------------------------
[LOG DE ERRO - {DateTime.Now:yyyy-MM-dd HH:mm:ss}]
--------------------------------------------------
Origem: {exception.Source ?? "N/A"}
Tipo: {exception.GetType().FullName}
Mensagem: {exception.Message}
Detalhes do StackTrace: 
{exception.StackTrace ?? "Sem Stack Trace disponível."}
{Environment.NewLine}";

        File.AppendAllText(path, logMessage);
    }
}
