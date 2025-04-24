using AuthSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace AuthSystem.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Clase para sembrar plantillas de correo electrónico predeterminadas
    /// </summary>
    public static class EmailTemplatesSeed
    {
        /// <summary>
        /// Siembra las plantillas de correo electrónico predeterminadas
        /// </summary>
        /// <param name="modelBuilder">Constructor de modelos</param>
        public static void SeedEmailTemplates(this ModelBuilder modelBuilder)
        {
            var templates = new List<EmailTemplate>
            {
                // Plantilla para la creación de usuarios
                new EmailTemplate
                {
                    Id = Guid.Parse("8c7e0a8d-4b8d-4cd9-a6ed-7de69f4a5e8e"),
                    Name = "UserCreated",
                    Subject = "Bienvenido a AuthSystem",
                    HtmlContent = @"
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
                            <div class='container'>
                                <div class='header'>
                                    <h1>¡Bienvenido a AuthSystem!</h1>
                                </div>
                                <div class='content'>
                                    <p>Hola <strong>{{FullName}}</strong>,</p>
                                    <p>Tu cuenta ha sido creada exitosamente en nuestro sistema.</p>
                                    <p>Detalles de tu cuenta:</p>
                                    <ul>
                                        <li><strong>Usuario:</strong> {{Username}}</li>
                                        <li><strong>Correo electrónico:</strong> {{Email}}</li>
                                        <li><strong>Fecha de creación:</strong> {{CurrentDate}}</li>
                                    </ul>
                                    <p>Ya puedes iniciar sesión en nuestra plataforma y comenzar a utilizarla.</p>
                                    <p>Si tienes alguna pregunta, no dudes en contactarnos.</p>
                                    <p>Saludos cordiales,<br>El equipo de AuthSystem</p>
                                </div>
                                <div class='footer'>
                                    <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>
                                </div>
                            </div>
                        </body>
                        </html>",
                    TextContent = @"
                        ¡Bienvenido a AuthSystem!

                        Hola {{FullName}},

                        Tu cuenta ha sido creada exitosamente en nuestro sistema.

                        Detalles de tu cuenta:
                        - Usuario: {{Username}}
                        - Correo electrónico: {{Email}}
                        - Fecha de creación: {{CurrentDate}}

                        Ya puedes iniciar sesión en nuestra plataforma y comenzar a utilizarla.

                        Si tienes alguna pregunta, no dudes en contactarnos.

                        Saludos cordiales,
                        El equipo de AuthSystem

                        Este es un correo electrónico automático, por favor no respondas a este mensaje.",
                    Description = "Plantilla para el correo de bienvenida cuando se crea un usuario",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },

                // Plantilla para la actualización de usuarios
                new EmailTemplate
                {
                    Id = Guid.Parse("9d8f0b9e-5c9e-5de0-b7fe-8ef7a5f6b9f9"),
                    Name = "UserUpdated",
                    Subject = "Tu cuenta ha sido actualizada",
                    HtmlContent = @"
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
                            <div class='container'>
                                <div class='header'>
                                    <h1>Actualización de Cuenta</h1>
                                </div>
                                <div class='content'>
                                    <p>Hola <strong>{{FullName}}</strong>,</p>
                                    <p>Tu cuenta ha sido actualizada en nuestro sistema.</p>
                                    <p>Detalles de tu cuenta:</p>
                                    <ul>
                                        <li><strong>Usuario:</strong> {{Username}}</li>
                                        <li><strong>Correo electrónico:</strong> {{Email}}</li>
                                        <li><strong>Fecha de actualización:</strong> {{UpdateDate}}</li>
                                    </ul>
                                    <p>Si no has realizado esta actualización o tienes alguna pregunta, por favor contacta a nuestro equipo de soporte inmediatamente.</p>
                                    <p>Saludos cordiales,<br>El equipo de AuthSystem</p>
                                </div>
                                <div class='footer'>
                                    <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>
                                </div>
                            </div>
                        </body>
                        </html>",
                    TextContent = @"
                        Actualización de Cuenta

                        Hola {{FullName}},

                        Tu cuenta ha sido actualizada en nuestro sistema.

                        Detalles de tu cuenta:
                        - Usuario: {{Username}}
                        - Correo electrónico: {{Email}}
                        - Fecha de actualización: {{UpdateDate}}

                        Si no has realizado esta actualización o tienes alguna pregunta, por favor contacta a nuestro equipo de soporte inmediatamente.

                        Saludos cordiales,
                        El equipo de AuthSystem

                        Este es un correo electrónico automático, por favor no respondas a este mensaje.",
                    Description = "Plantilla para el correo de notificación cuando se actualiza un usuario",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },

                // Plantilla para el código de activación
                new EmailTemplate
                {
                    Id = Guid.Parse("7a6f0c7b-3a7c-4bc8-95ed-6de58f4a4e7d"),
                    Name = "ActivationCode",
                    Subject = "Código de Activación - AuthSystem",
                    HtmlContent = @"
                        <html>
                        <head>
                            <style>
                                body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                                .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; }
                                .header { background-color: #4a6da7; color: white; padding: 10px 20px; text-align: center; }
                                .content { padding: 20px; background-color: #f9f9f9; }
                                .code { font-size: 24px; font-weight: bold; text-align: center; padding: 15px; background-color: #e9e9e9; margin: 20px 0; letter-spacing: 5px; }
                                .footer { text-align: center; padding: 10px; font-size: 12px; color: #666; }
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <h1>Código de Activación</h1>
                                </div>
                                <div class='content'>
                                    <p>Hola <strong>{{FullName}}</strong>,</p>
                                    <p>Has solicitado un código de activación para tu cuenta en AuthSystem.</p>
                                    <p>Tu código de activación es:</p>
                                    <div class='code'>{{ActivationCode}}</div>
                                    <p>Este código es válido por {{ExpirationTime}} a partir de ahora.</p>
                                    <p>Si no has solicitado este código, por favor ignora este mensaje o contacta a nuestro equipo de soporte.</p>
                                    <p>Saludos cordiales,<br>El equipo de AuthSystem</p>
                                </div>
                                <div class='footer'>
                                    <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>
                                </div>
                            </div>
                        </body>
                        </html>",
                    TextContent = @"
                        Código de Activación

                        Hola {{FullName}},

                        Has solicitado un código de activación para tu cuenta en AuthSystem.

                        Tu código de activación es: {{ActivationCode}}

                        Este código es válido por {{ExpirationTime}} a partir de ahora.

                        Si no has solicitado este código, por favor ignora este mensaje o contacta a nuestro equipo de soporte.

                        Saludos cordiales,
                        El equipo de AuthSystem

                        Este es un correo electrónico automático, por favor no respondas a este mensaje.",
                    Description = "Plantilla para el correo con código de activación",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },

                // Plantilla para el restablecimiento de contraseña
                new EmailTemplate
                {
                    Id = Guid.Parse("6b5e0d6a-2a6b-3ab7-84dc-5cf6b3e3a6c6"),
                    Name = "PasswordReset",
                    Subject = "Restablecimiento de Contraseña - AuthSystem",
                    HtmlContent = @"
                        <html>
                        <head>
                            <style>
                                body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                                .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; }
                                .header { background-color: #4a6da7; color: white; padding: 10px 20px; text-align: center; }
                                .content { padding: 20px; background-color: #f9f9f9; }
                                .button { display: inline-block; padding: 10px 20px; background-color: #4a6da7; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }
                                .footer { text-align: center; padding: 10px; font-size: 12px; color: #666; }
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <h1>Restablecimiento de Contraseña</h1>
                                </div>
                                <div class='content'>
                                    <p>Hola <strong>{{FullName}}</strong>,</p>
                                    <p>Has solicitado restablecer la contraseña de tu cuenta en AuthSystem.</p>
                                    <p>Para restablecer tu contraseña, haz clic en el siguiente enlace:</p>
                                    <p style='text-align: center;'><a href='{{ResetUrl}}' class='button'>Restablecer Contraseña</a></p>
                                    <p>O copia y pega la siguiente URL en tu navegador:</p>
                                    <p>{{ResetUrl}}</p>
                                    <p>Este enlace es válido por {{ExpirationTime}} a partir de ahora.</p>
                                    <p>Si no has solicitado restablecer tu contraseña, por favor ignora este mensaje o contacta a nuestro equipo de soporte.</p>
                                    <p>Saludos cordiales,<br>El equipo de AuthSystem</p>
                                </div>
                                <div class='footer'>
                                    <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>
                                </div>
                            </div>
                        </body>
                        </html>",
                    TextContent = @"
                        Restablecimiento de Contraseña

                        Hola {{FullName}},

                        Has solicitado restablecer la contraseña de tu cuenta en AuthSystem.

                        Para restablecer tu contraseña, visita el siguiente enlace:
                        {{ResetUrl}}

                        Este enlace es válido por {{ExpirationTime}} a partir de ahora.

                        Si no has solicitado restablecer tu contraseña, por favor ignora este mensaje o contacta a nuestro equipo de soporte.

                        Saludos cordiales,
                        El equipo de AuthSystem

                        Este es un correo electrónico automático, por favor no respondas a este mensaje.",
                    Description = "Plantilla para el correo de restablecimiento de contraseña",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            };

            modelBuilder.Entity<EmailTemplate>().HasData(templates);
        }
    }
}
