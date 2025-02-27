using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using Control;
using Model;



namespace App_Final
{
    public partial class frmAgregarArticulo : Form
    {
        private Articulo articulo = null;

        private OpenFileDialog archivo = null;

        public frmAgregarArticulo()
        {
            InitializeComponent();
        }

        public frmAgregarArticulo(Articulo articulo)
        {
            InitializeComponent();
            this.articulo = articulo;
            Text = "Modifica tu artículo";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();

            try
            {
                if (validarCampo())
                {
                    if (articulo == null)
                        articulo = new Articulo();

                    articulo.Codigo = txtCodigo.Text;
                    articulo.Nombre = txtNombre.Text;
                    articulo.Marca = (Marca)cboxMarca.SelectedItem;
                    articulo.Categoria = (Categoria)cboxCategoria.SelectedItem;
                    articulo.UrlImagen = TxtUrlImagen.Text;
                    articulo.Precio = decimal.Parse(txtPrecio.Text);
                    articulo.Descripcion = txtDescripcion.Text;

                    if (articulo.Id != 0)
                    {
                        negocio.modificar(articulo);
                        MessageBox.Show("Modificado correctamente");
                    }
                    else
                    {
                        negocio.agregar(articulo);
                        MessageBox.Show("Agregado correctamente");
                    }
                    if (archivo != null && !(TxtUrlImagen.Text.ToUpper().Contains("HTTP")))
                    {
                        File.Copy(archivo.FileName, ConfigurationManager.AppSettings["imagesfoldercatalogo"] + archivo.SafeFileName);
                    }

                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void frmAgregarArticulo_Load(object sender, EventArgs e)
        {
            MarcaCategoria negocio = new MarcaCategoria();

            try
            {
                cboxMarca.DataSource = negocio.obtenerMarcas();
                cboxMarca.ValueMember = "Id";
                cboxMarca.DisplayMember = "Descripcion";
                cboxCategoria.DataSource = negocio.obtenerCategorias();
                cboxCategoria.ValueMember = "Id";
                cboxCategoria.DisplayMember = "Descripcion";

                if(articulo != null)
                {
                    txtCodigo.Text = articulo.Codigo;
                    txtNombre.Text = articulo.Nombre;
                    TxtUrlImagen.Text = articulo.UrlImagen;
                    cargarImagen(articulo.UrlImagen);
                    txtDescripcion.Text = articulo.Descripcion;
                    txtPrecio.Text = articulo.Precio.ToString();
                    cboxMarca.SelectedValue = articulo.Marca.Id;
                    cboxCategoria.SelectedValue = articulo.Categoria.Id;
                }
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

        private void TxtUrlImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(TxtUrlImagen.Text);
        }

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg;|png|*.png";
            if(archivo.ShowDialog() == DialogResult.OK)
            {
                TxtUrlImagen.Text = archivo.FileName;
                cargarImagen(archivo.FileName);

            }
        }

        private bool validarCampo()
        {
            bool valido = true;

            lblCodigoError.Text = "";
            lblNombreError.Text = "";
            lblPrecioError.Text = "";

            if(string.IsNullOrEmpty(txtCodigo.Text))
            {
                lblCodigoError.Text = "Este campo es obligatorio";
                valido = false;
            } 
            if(string.IsNullOrEmpty(txtNombre.Text))
            {
                lblNombreError.Text = "Este campo es obligatorio";
                valido = false;
            }
            if(string.IsNullOrEmpty(txtPrecio.Text) || !decimal.TryParse(txtPrecio.Text, out _)) 
            {
                lblPrecioError.Text = "Este campo es obligatoio, solo números";
                valido = false;
            }
            return valido;

        }
    }
}
