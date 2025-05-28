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

    public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        IEnumerable<Cuenta> cuentas = await connection.QueryAsync<Cuenta>(
            @"SELECT Cuentas.Id, Cuentas.Nombre, Cuentas.Balance, tc.Nombre as TipoCuenta
                FROM Cuentas
                INNER JOIN TiposCuentas as tc
                ON Cuentas.TipoCuentaId = tc.Id
                WHERE tc.UsuarioId=@UsuarioId;",
                new { UsuarioId = usuarioId });

        return cuentas;
    }
    
    public async Task Actualizar(CuentaCreacionViewModel cuentaVM)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        await connection.ExecuteAsync(
            @"UPDATE Cuentas
                SET Nombre=@Nombre, Balance=@Balance, Descripcion=@Descripcion, TipoCuentaId=@TipoCuentaId
                WHERE Id=@Id",
            cuentaVM);
    }
    
    public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        Cuenta cuenta = await connection.QueryFirstOrDefaultAsync<Cuenta>(
            @"SELECT Cuentas.Id, Cuentas.Nombre, Cuentas.Balance, Cuentas.TipoCuentaId, Cuentas.Descripcion
                FROM Cuentas
                INNER JOIN TiposCuentas as tc
                ON Cuentas.TipoCuentaId = tc.Id
                WHERE tc.UsuarioId=@UsuarioId AND Cuentas.Id=@Id;",
            new { UsuarioId = usuarioId , Id = id });
        
        return cuenta; 
    }
}