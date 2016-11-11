using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Azure; // Namespace de CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace de CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace de Blob storage types

namespace CRUD_BLOB_AZURE
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Initialiser le composant pour parcourir les fichiers
        OpenFileDialog ofd = new OpenFileDialog();

        public void chargement()
        {
            dataGridView1.Rows.Clear();
            textBox1.Text = "";
            // exemple qui choisi le conteneur et affiche les informations des blob dedan

            // Récupérer le compte de stockage à partir de la chaîne de connexion.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Créer le client blob.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Récupère la référence d'un conteneur créé précédemment.
            CloudBlobContainer container = blobClient.GetContainerReference(textBox3.Text);

            // Créez le conteneur s'il n'existe pas déjà.
            container.CreateIfNotExists();

            // Pérmissions
            container.SetPermissions(
                new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            // Boucle des éléments dans le conteneur.
            foreach (IListBlobItem item in container.ListBlobs(null, false))
            {
                //retourner le nom du conteneur, le nom du blob, la taille du blob et son URI.
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    this.dataGridView1.Rows.Add(blob.Container.Name, blob.Name, blob.Properties.Length, blob.Uri);
                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;
                    this.dataGridView1.Rows.Add(pageBlob.Container.Name, pageBlob.Name, pageBlob.Properties.Length, pageBlob.Uri);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btn_parcourir_Click(object sender, EventArgs e)
        {
            //si on ouvre un fichier
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // on rempli les champs avec le nom et le path du fichier
                textBox2.Text = ofd.FileName;
                textBox1.Text = ofd.SafeFileName;
            }
        }
    }
}
