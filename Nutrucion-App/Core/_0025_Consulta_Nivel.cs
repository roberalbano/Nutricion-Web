﻿using NA.Dominio.Base.Helpers;
using NA.IServicio.Nivel;
using NA.Servicio.Nivel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nutrucion_App.Core
{
    public partial class _0025_Consulta_Nivel : _0009_Consulta_Base
    {
        private readonly INivelServicio _nivelServicio = new NivelServicio();
        public long planId;

        public _0025_Consulta_Nivel()
            :base("Niveles")
        {
            InitializeComponent();
        }

        public override void ActualizarDatos(string empty)
        {
            if (planId != null)
            {
                ActualizarPorPlan(planId);

            }
            if(planId == null)
            {
                dgvGrilla.DataSource = _nivelServicio.ObtenerPorFiltro(empty);

                FormatiarGrilla(dgvGrilla);
            }
        }

        public void ActualizarPorPlan(long? id)
        {
            dgvGrilla.DataSource = _nivelServicio.ObtenerPorPlanId((long)id);

            FormatiarGrilla(dgvGrilla);
        }

        public override void FormatiarGrilla(DataGridView dgv)
        {
            dgvGrilla.Columns["Id"].Visible = false;
            dgvGrilla.Columns["Eliminado"].Visible = false;
            dgvGrilla.Columns["Imagen"].Visible = false;
            dgvGrilla.Columns["LinkPago"].Visible = false;
            dgvGrilla.Columns["Descripcion"].Visible = false;
            dgvGrilla.Columns["PlanId"].Visible = false;

            dgvGrilla.Columns["Titulo"].Visible = true;
            dgvGrilla.Columns["Titulo"].HeaderText = @"Tipo de Nivel";
            dgvGrilla.Columns["Titulo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; ;

            //dgvGrilla.Columns["Precio"].HeaderText = @"Precio";
            //dgvGrilla.Columns["Precio"].Width = 120;
        }

        public override void Buscar(string cadena)
        {
            dgvGrilla.DataSource = _nivelServicio.ObtenerPorFiltro(cadena);

            base.Buscar(cadena);
        }

      

        //******************************** OPERACIONES ****************************************

        public override bool EjecutarAgregarEntidad()
        {
            var fNivel = new _0026_ABM_Nivel(TipoOperacion.Agregar);
            fNivel.ShowDialog();
            return fNivel.RealizoOperacion;
        }

        public override bool EjecutarModificarEntidad(long? EntidadId)
        {
            var fNivel = new _0026_ABM_Nivel(TipoOperacion.Modificar , EntidadId);
            fNivel.ShowDialog();
            return fNivel.RealizoOperacion;
        }

        public override bool EjecutarEliminarEntidad(long? EntidadId)
        {
            var nivel = _nivelServicio.ObtenerPorId(EntidadId.Value);

            if (MessageBox.Show("Desea Eliminar el Nivel : " + nivel.Titulo, "Eliminar Registro", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                var fEliminar = new _0026_ABM_Nivel(TipoOperacion.Eliminar, EntidadId.Value);
                fEliminar.ShowDialog();
                FormatiarGrilla(dgvGrilla);

                return fEliminar.RealizoOperacion;
            }
            else
            {
                return false;
            }
        }

    }
}
