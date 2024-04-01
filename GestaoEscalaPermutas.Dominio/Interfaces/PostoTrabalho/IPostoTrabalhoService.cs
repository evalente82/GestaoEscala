using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho;

namespace GestaoEscalaPermutas.Dominio.Interfaces.PostoTrabalho
{
    public interface IPostoTrabalhoService
    {
        Task<PostoTrabalhoDTO> Incluir(PostoTrabalhoDTO postoTrabalhoModel);
        Task<PostoTrabalhoDTO> Alterar(int id, PostoTrabalhoDTO postoTrabalhoModel);
        Task<PostoTrabalhoDTO> Deletar(int id);
        Task<List<PostoTrabalhoDTO>> BuscarTodos();
        Task<PostoTrabalhoDTO[]> IncluirLista(PostoTrabalhoDTO[] postoDTOs);
    }
}
