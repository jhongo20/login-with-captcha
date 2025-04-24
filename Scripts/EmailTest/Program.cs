using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace EmailTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Iniciando prueba de envío de correo electrónico...");

            // Configuración del correo electrónico
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            bool useSsl = true;
            string username = "jhongopruebas@gmail.com";
            string password = "tnoeowgsvuhfxfcb";
            string senderName = "AuthSystem";
            string senderEmail = "jhongopruebas@gmail.com";

            // Destinatario y asunto
            string recipientEmail = "jhongopruebas@gmail.com";
            string subject = "Correo de prueba desde AuthSystem";

            // Contenido del correo
            string htmlContent = @"
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background-color: #4a6da7; color: white; padding: 10px 20px; text-align: center; }
        .content { padding: 20px; background-color: #f9f9f9; }
        .footer { text-align: center; padding: 10px; font-size: 12px; color: #666; }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Correo de prueba</h1>
        </div>
        <div class=""content"">
            <p>Hola,</p>
            <p>Este es un correo de prueba enviado desde el sistema AuthSystem.</p>
            <p>Si estás recibiendo este correo, significa que la configuración de correo electrónico está funcionando correctamente.</p>
            <p>Saludos cordiales,<br>El equipo de AuthSystem</p>
        </div>
        <div class=""footer"">
            <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>
        </div>
    </div>
</body>
</html>";

            string textContent = @"
Correo de prueba

Hola,

Este es un correo de prueba enviado desde el sistema AuthSystem.

Si estás recibiendo este correo, significa que la configuración de correo electrónico está funcionando correctamente.

Saludos cordiales,
El equipo de AuthSystem

Este es un correo electrónico automático, por favor no respondas a este mensaje.";

            try
            {
                // Crear el mensaje
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress("", recipientEmail));
                message.Subject = subject;

                // Crear el cuerpo del mensaje
                var builder = new BodyBuilder
                {
                    HtmlBody = htmlContent,
                    TextBody = textContent
                };
                message.Body = builder.ToMessageBody();

                Console.WriteLine($"Conectando al servidor SMTP: {smtpServer}:{smtpPort}...");

                // Enviar el mensaje
                using (var client = new SmtpClient())
                {
                    // Configurar opciones de seguridad
                    var secureSocketOptions = SecureSocketOptions.StartTls;

                    // Conectar al servidor SMTP
                    await client.ConnectAsync(smtpServer, smtpPort, secureSocketOptions);
                    Console.WriteLine("Conexión establecida. Autenticando...");

                    // Autenticar
                    await client.AuthenticateAsync(username, password);
                    Console.WriteLine("Autenticación exitosa. Enviando mensaje...");

                    // Enviar el mensaje
                    await client.SendAsync(message);
                    Console.WriteLine("Mensaje enviado. Desconectando...");

                    // Desconectar
                    await client.DisconnectAsync(true);
                }

                Console.WriteLine($"¡Correo enviado correctamente a {recipientEmail}!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo electrónico: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
                }
            }

            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}
