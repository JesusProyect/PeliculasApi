﻿using System.ComponentModel.DataAnnotations;

namespace _2_PeliculasAPI.Helpers.Validations
{
    public class TipoArchivoValidation : ValidationAttribute
    {
        private readonly string[]? _tiposValidos;

        public TipoArchivoValidation(string[] tiposValidos)
        {
            _tiposValidos = tiposValidos;
        }

        public TipoArchivoValidation(GrupoTipoArchivo grupoTipoArchivo)
        {
            if (grupoTipoArchivo == GrupoTipoArchivo.Imagen)
            {
                _tiposValidos = new string[] { "image/jpeg", "image/png", "image/gif" };

            }
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;
            if (value is not IFormFile formFile) return ValidationResult.Success;

            if (!_tiposValidos!.Contains(formFile.ContentType)) return new ValidationResult($"El tipo del archivo debe ser uno de los siguientes: {string.Join(", ", _tiposValidos!)}");

            return ValidationResult.Success; ;
        }

    }
}
