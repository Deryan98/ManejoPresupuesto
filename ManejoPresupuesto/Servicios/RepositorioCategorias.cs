using System;
using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
	public interface IRepositorioCategorias
	{
		Task Crear(Categoria categoria);

		Task<IEnumerable<Categoria>> Obtener(int usuarioId);

    }

    public class RepositorioCategorias: IRepositorioCategorias
    {
		private readonly string connectionString;

		public RepositorioCategorias(IConfiguration configuration)
		{
			connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		public async Task Crear(Categoria categoria)
		{
			using var connnection = new SqlConnection(connectionString);
			var id = await connnection.QuerySingleAsync<int>(@"
				INSERT INTO Categorias (Nombre, TipoOperacionId, UsuarioId)
				VALUES (@Nombre, @TipoOperacionId, @UsuarioId);

				SELECT SCOPE_IDENTITY();
			", categoria);

			categoria.Id = id;
		}

		public async Task<IEnumerable<Categoria>> Obtener(int usuarioId)
		{
			using var connection = new SqlConnection(connectionString);
			return await connection.QueryAsync<Categoria>(
				@"SELECT * FROM Categorias WHERE UsuarioId = @usuaroId", new { usuarioId });
		}
	}
}

