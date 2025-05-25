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
            @"SELECT Id, Nombre, Orden
                FROM TiposCuentas
                WHERE UsuarioId=@UsuarioId;", 
            new {usuarioId=usuarioId});
        
        return queryResult;
    }
}