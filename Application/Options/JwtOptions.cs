using System.ComponentModel.DataAnnotations;

namespace Application.Options
{
    public class JwtOptions
    {
        [Required]
        public required string Issuer { get; set; }
        [Required]
        public required string Audience { get; set; }
        [Required]
        public required string SecurityKey { get; set; }
    }
}
