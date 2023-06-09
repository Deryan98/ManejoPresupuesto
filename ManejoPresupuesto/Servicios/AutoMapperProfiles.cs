﻿using System;
using AutoMapper;
using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Servicios
{
	public class AutoMapperProfiles: Profile
	{
		public AutoMapperProfiles()
		{
			CreateMap<Cuenta, CuentaCreacionViewModel>();
			CreateMap<TransaccionActulizacionViewModel, Transaccion>().ReverseMap();
		}
	}
}

