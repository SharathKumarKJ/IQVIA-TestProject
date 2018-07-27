using IQVIA.Interfaces;

namespace IQVIA.Controllers
{
    public class Swagger : ISwagger
    {
        public string Id { get; set; }
        public string Stamp { get; set; }
        public string Text { get; set; }
    }
}