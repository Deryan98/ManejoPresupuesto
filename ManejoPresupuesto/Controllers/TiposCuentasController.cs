﻿using System;
using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace ManejoPresupuesto.Controllers
{
	public class TiposCuentasController: Controller
	{
        private readonly ILogger<TiposCuentasController> logger;
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;

        public TiposCuentasController(
			ILogger<TiposCuentasController> logger,
            IRepositorioTiposCuentas repositorioTiposCuentas,
			IServicioUsuarios servicioUsuarios
        )
		{
			
            this.logger = logger;
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }

		public async Task<IActionResult> Index()
		{
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
			return View(tiposCuentas);
		}

		public IActionResult Crear()
		{
			return View();
		}

		[HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
			if(!ModelState.IsValid)
			{
				//para enviar la info con la que envió la data el usuario
				return View(tipoCuenta);
			}
			tipoCuenta.UsuarioId = servicioUsuarios.ObtenerUsuarioId();

            var yaExisteTipoCuenta =
				await repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

			if (yaExisteTipoCuenta)
			{
				ModelState.AddModelError(nameof(tipoCuenta.Nombre),
					$"El nombre {tipoCuenta.Nombre} ya existe");
				return View(tipoCuenta);
			}

			await repositorioTiposCuentas.Crear(tipoCuenta);

            return RedirectToAction("Index");
        }

        [HttpGet]
		public async Task<IActionResult> Editar(int id)
		{
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

			if (tipoCuenta is null)
			{
				return RedirectToAction("NoEncontrado", "Home");
			}

			return View(tipoCuenta);
		}

		[HttpPost]
		public async Task<IActionResult> Editar (TipoCuenta tipoCuenta)
		{
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);
			if(tipoCuentaExiste is null)
			{
				return RedirectToAction("NoEncontrado", "Home");
			}
			await repositorioTiposCuentas.Actualizar(tipoCuenta);
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Borrar (int id)
		{
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
			if(tipoCuenta is null)
			{
				return RedirectToAction("NoEncontrado", "Home");
			}
			return View(tipoCuenta);
		}

		[HttpPost]
		public async Task<IActionResult> BorrarTipoCuenta (int id)
		{
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
			await repositorioTiposCuentas.Borrar(id);
			return RedirectToAction("Index");
        }

        [HttpGet]
		public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
		{
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(nombre, usuarioId);

			if(yaExisteTipoCuenta)
			{
				return Json($"El nombre {nombre} ya existe");
			}

			return Json(true);

		}
		[HttpPost]
		public async Task<IActionResult> Ordenar([FromBody] int[] ids )
		{
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
			var idsTiposCuentas = tiposCuentas.Select(x => x.Id);
			var idsTiposCuentasNoPertencenAlUsuario = ids.Except(idsTiposCuentas).ToList();
			if (idsTiposCuentasNoPertencenAlUsuario.Count > 0)
				return Forbid();
			var tiposCuentasOrdenados = ids.Select((valor, indice) =>
				new TipoCuenta() { Id = valor, Orden = indice + 1 }).AsEnumerable();
			await repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);
			return Ok();
		}
    }
}
