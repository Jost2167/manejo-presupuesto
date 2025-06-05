using System.Data;
using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Repository;

public class TransaccionRepository: ITransaccionRepository
{
    private readonly string _connectionString;
    
    public TransaccionRepository(IConfiguration configuration)
    {
        _connectionString  =configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task Crear(Transaccion transaccion)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        int id = await connection.QuerySingleAsync<int>(
            @"Transacciones_Insertar",
            new
            {
                transaccion.Fecha,
                transaccion.Monto,
                transaccion.Nota,
                transaccion.CategoriaId,
                transaccion.CuentaId,
                transaccion.UsuarioId,
            },
            commandType: CommandType.StoredProcedure);
        
        transaccion.Id = id;
    }

    public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            @"Transacciones_Actualizar",
            new
            {
                transaccion.Id,
                transaccion.Fecha,
                transaccion.Monto,
                montoAnterior,
                transaccion.CuentaId,
                cuentaAnteriorId,
                transaccion.CategoriaId,
                transaccion.Nota,
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);

        Transaccion transaccion = await connection.QueryFirstOrDefaultAsync<Transaccion>(
            @"SELECT tra.*, cat.TipoOperacionId
                FROM Transacciones as tra
                INNER JOIN Categorias as cat 
                ON cat.Id = tra.CategoriaId
                WHERE tra.UsuarioId = @id AND tra.Id = @usuarioId;",
            new { id, usuarioId});

        return transaccion;
    }
    
}