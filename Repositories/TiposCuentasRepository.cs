using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Repository;

public class TiposCuentasRepository: ITiposCuentasRepository
{
    private readonly string _connnectionString;
    
    public TiposCuentasRepository(IConfiguration configuration)
    {
        _connnectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task Crear(TipoCuenta tipoCuenta)
    {
        using SqlConnection connection = new SqlConnection(_connnectionString);
        connection.Open();
        int resultId = await connection.QuerySingleAsync<int>(
            @"INSERT INTO TiposCuentas(Nombre, UsuarioId, Orden)  
                VALUES (@Nombre, @UsuarioId, @Orden) 
                SELECT SCOPE_IDENTITY();", tipoCuenta);  //Devuelve el id con el cual se inserto la fila

        tipoCuenta.Id = resultId;
    }

    public async Task<bool> YaExiste(string nombre, int usuarioId)
    {
        using SqlConnection connection = new SqlConnection(_connnectionString);
        connection.Open();
        int result = await connection.QueryFirstOrDefaultAsync<int>(
            @"SELECT 1
                FROM TiposCuentas
                WHERE UsuarioId=@UsuarioId AND Nombre=@Nombre;", 
            new { UsuarioId = usuarioId, Nombre = nombre});

        return result == 1;
    }

    public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
    {
        using SqlConnection connection = new SqlConnection(_connnectionString);
        connection.Open();
        
        IEnumerable<TipoCuenta> queryResult = await connection.QueryAsync<TipoCuenta>(
            @"SELECT Id, Nombre
                FROM TiposCuentas
                WHERE UsuarioId=@UsuarioId;", 
            new {usuarioId=usuarioId});
        
        return queryResult;
    }

    public async Task Actualizar(TipoCuenta tipoCuenta)
    {
        using SqlConnection connection = new SqlConnection(_connnectionString);
        connection.Open();

        await connection.ExecuteAsync(
            @"UPDATE TiposCuentas
                SET Nombre=@Nombre
                WHERE id=@Id",
                tipoCuenta);
    }

    public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
    {
        using SqlConnection connection = new SqlConnection(_connnectionString);
        connection.Open();

        TipoCuenta tipoCuenta = await connection.QueryFirstOrDefaultAsync<TipoCuenta>(
            @"SELECT id, Nombre, Orden
                FROM TiposCuentas
                WHERE id=@Id AND UsuarioId=@UsuarioId;",
            new { Id = id, UsuarioId = usuarioId });

        return tipoCuenta;
    }

    public async Task Eliminar(int id)
    {
        using SqlConnection connection = new SqlConnection(_connnectionString);
        connection.Open();

        await connection.ExecuteAsync(
            @"DELETE TiposCuentas
                WHERE Id=@Id",
            new { Id = id });
    }
    
    
}