using TechChallenge.Fase3.DataTransfer.Utils;
using TechChallenge.Fase3.DataTransfer.Utils.Enumeradores;

namespace TechChallenge.Fase3.DataTransfer.Usuarios.Request
{
    public class UsuarioListarRequest : PaginacaoFiltro
    {
        public string? NomeUsuario { get; set; }
        public string? Email { get; set; }
        public UsuarioListarRequest() : base("nome", TipoOrdernacao.Desc)
        {
        }
    }
}
