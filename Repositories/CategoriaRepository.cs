using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Repository;

public class CategoriaRepository : ICategoriasRepository
{
    private readonly string _connectionString;
    
    public CategoriaRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task Crear(Categoria categoria)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        int id = await connection.QuerySingleAsync<int>(
            @"INSERT INTO Categorias(Nombre,TipoOperacionId,UsuarioId)
                VALUES (@Nombre,@TipoOperacionId,@UsuarioId)
                SELECT SCOPE_IDENTITY();", categoria);
        
        categoria.Id = id;
    }

    public async Task<IEnumerable<Categoria>> Obtener(int usuarioId)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        IEnumerable<Categoria> categorias = await connection.QueryAsync<Categoria>(
            @"SELECT Id, Nombre, TipoOperacionId
                FROM Categorias
                WHERE UsuarioId=@UsuarioId;",
            new {UsuarioId = usuarioId});

        return categorias;
    }
    
}