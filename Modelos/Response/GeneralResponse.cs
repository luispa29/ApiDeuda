
namespace Modelos.Response
{
    public class GeneralResponse
    {
        public int Contador { get; set; } = 1;
        public int Codigo { get; set; }
        public String? Token { get; set; }
        public String? Mensaje { get; set; }
        public Object? Data { get; set; }

    }
}
