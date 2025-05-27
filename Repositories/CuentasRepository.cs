using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Repository;

public class CuentasRepository : ICuentasRepository
{
    private readonly string _connectionString;
    
    public CuentasRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task Crear(Cuenta cuenta)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        int cuentaId = await connection.QuerySingleAsync<int>(
            @"INSERT INTO Cuentas(Nombre, Balance, Descripcion, TipoCuentaId)
                VALUES (@Nombre, @Balance, @Descripcion, @TipoCuentaId)
                SELECT SCOPE_IDENTITY();",
                cuenta);
        
        cuenta.Id = cuentaId;
    }
}