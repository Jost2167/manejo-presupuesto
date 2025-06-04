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
}