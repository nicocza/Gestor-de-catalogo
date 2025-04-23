using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Control;
using Model;

namespace App_Final
{
    public partial class MainForm : Form
    {
        private List<Articulo> listaArticulo;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            cargar();
            cboxCampo.Items.Add("Código");
            cboxCampo.Items.Add("Nombre");
            cboxCampo.Items.Add("Descripción");
        }

        private void dgvArticulo_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvArticulo.CurrentRow != null)
            {
                Articulo seleccionado = (Articulo)dgvArticulo.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.UrlImagen);
            }        
        }

        private void cargar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();

            try
            {
                listaArticulo = negocio.listar();
                dgvArticulo.DataSource = listaArticulo;
                ocultarColumnas();
                cargarImagen(listaArticulo[0].UrlImagen);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pboxArticulo.Load(imagen);
            }
            catch (Exception ex)
            {
                pboxArticulo.Load("https://www.afim.com.eg/public/images/no-photo.png");
            }
        }

        private void ocultarColumnas()
        {
            dgvArticulo.Columns["UrlImagen"].Visible = false;
            dgvArticulo.Columns["Id"].Visible = false;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAgregarArticulo alta = new frmAgregarArticulo();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if(dgvArticulo.CurrentRow == null)
            {
                MessageBox.Show("No hay artículo seleccionado para modificar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Articulo seleccionado;
            seleccionado = (Articulo)dgvArticulo.CurrentRow.DataBoundItem;
            frmAgregarArticulo modificar = new frmAgregarArticulo(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if(dgvArticulo.CurrentRow == null)
            {
                MessageBox.Show("No hay articulo seleccionado para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            eliminar();
        }

        private void eliminar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            Articulo seleccionado;

            try
            {
                DialogResult respuesta = MessageBox.Show("¿Deseas eliminar este artículo?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Articulo)dgvArticulo.CurrentRow.DataBoundItem;
                    negocio.eliminar(seleccionado.Id);
                    cargar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool validarFiltro()
        {
            if(cboxCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione el campo a filtrar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            if(cboxCriterio.SelectedIndex < 0 && !string.IsNullOrWhiteSpace(txtFiltroAvanzado.Text.Trim()))
            {
                MessageBox.Show("Seleccione el criterio a filtrar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            if (cboxCampo.SelectedItem.ToString() == "Código")
            {
               if(string.IsNullOrEmpty(txtFiltroAvanzado.Text.Trim()))
               {
                    MessageBox.Show("El filtro no puede estar vacio para el campo 'Código'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
               }
            }
            else if(cboxCampo.SelectedItem.ToString() == "Nombre")
            {
                if(string.IsNullOrEmpty(txtFiltroAvanzado.Text.Trim()))
                {
                    MessageBox.Show("El filtro no puede estar vacio para el campo 'Nombre'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
            }
            else if(cboxCampo.SelectedItem.ToString() == "Descripción")
            {
                if(string.IsNullOrEmpty(txtFiltroAvanzado.Text.Trim()))
                {
                    MessageBox.Show("El filtro no puede estar vacio para el campo 'Descripción'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
            }
            return false;    
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                if (validarFiltro())
                    return;
                string campo = cboxCampo.SelectedItem.ToString();
                string criterio = cboxCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text.Trim();
                dgvArticulo.DataSource = negocio.filtrar(campo, criterio, filtro);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al filtrar los articulos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            List<Articulo> listaFiltrada;
            string filtro = txtFiltro.Text;

            if (filtro.Length >= 3)
                listaFiltrada = listaArticulo.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Codigo.ToUpper().Contains(filtro.ToUpper()) || x.Descripcion.ToUpper().Contains(filtro.ToUpper()));
            else
                listaFiltrada = listaArticulo;

            dgvArticulo.DataSource = null;
            dgvArticulo.DataSource = listaFiltrada;
            ocultarColumnas();
        }

        private void cboxCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboxCampo.SelectedItem.ToString();
            cboxCriterio.Items.Clear();
            if (opcion == "Código" || opcion == "Nombre" || opcion == "Descripción")
            {
                cboxCriterio.Items.Add("Comienza con");
                cboxCriterio.Items.Add("Termina con");
                cboxCriterio.Items.Add("Contiene");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            cargar();
        }
    }
}

