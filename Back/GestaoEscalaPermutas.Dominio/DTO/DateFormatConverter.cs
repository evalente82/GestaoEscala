using Newtonsoft.Json.Converters;

namespace GestaoEscalaPermutas.Dominio.DTO
{
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
