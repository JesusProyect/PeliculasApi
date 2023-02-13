using System.ComponentModel.DataAnnotations;

namespace _2_PeliculasAPI.Helpers.Validations
{
    public class PesoImagenValidation : ValidationAttribute
    {
        private readonly int _pesoMaximoMb;

        public PesoImagenValidation(int pesoMaximoMb)
        {
            _pesoMaximoMb = pesoMaximoMb;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;

            if (value is not IFormFile formFile) return ValidationResult.Success;

            if (formFile.Length > _pesoMaximoMb * 1024 * 1024) return new ValidationResult($"El Peso Maximo de la imagen no debe ser superior a {_pesoMaximoMb}MB");

            return ValidationResult.Success;
        }
    }
}
