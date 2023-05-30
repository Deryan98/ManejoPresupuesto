using System;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ManejoPresupuesto.Controllers
{
	public class CategoriasController: Controller
	{
        private readonly ILogger<CategoriasController> logger;
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IServicioUsuarios servicioUsuarios;

        public CategoriasController(ILogger<CategoriasController> logger,
            IRepositorioCategorias repositorioCategorias,
            IServicioUsuarios servicioUsuarios)
		{
            this.logger = logger;
            this.repositorioCategorias = repositorioCategorias;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var categorias = await repositorioCategorias.Obtener(usuarioId);
            return View(categorias);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if(!ModelState.IsValid)
            {
                return View(categoria);
            }

            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            categoria.UsuarioId = usuarioId;
            await repositorioCategorias.Crear(categoria);
            return RedirectToAction("Index");
        }
	}

}

