using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho;

namespace GestaoEscalaPermutas.Dominio.Interfaces.PostoTrabalho
{
    public interface IPostoTrabalhoService
    {
        Task<PostoTrabalhoDTO> Incluir(PostoTrabalhoDTO postoTrabalhoModel);
        Task<PostoTrabalhoDTO> Alterar(Guid id, PostoTrabalhoDTO postoTrabalhoModel);
        Task<PostoTrabalhoDTO> Deletar(Guid id);
        Task<List<PostoTrabalhoDTO>> BuscarTodos();
        Task<PostoTrabalhoDTO[]> IncluirLista(PostoTrabalhoDTO[] postoDTOs);
        Task<List<PostoTrabalhoDTO>> BuscarTodosAtivos();
    }
}
