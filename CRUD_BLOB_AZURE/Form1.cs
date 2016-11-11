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

        private void btn_envoyer_Click(object sender, EventArgs e)
        {
            if (!(String.IsNullOrEmpty(textBox1.Text)) || !(String.IsNullOrEmpty(textBox3.Text)))
            {
                //envoyer le blob dans le cloud

                // Récupérer le compte de stockage à partir de la chaîne de connexion.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("StorageConnectionString"));

                // Créer le client blob.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Récupère la référence d'un conteneur créé précédemment.
                CloudBlobContainer container = blobClient.GetContainerReference(textBox3.Text);
                
                container.CreateIfNotExists();

                container.SetPermissions(
                    new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(textBox1.Text);
                
                using (var fileStream = System.IO.File.OpenRead(@textBox2.Text))
                {
                    //envoyer le blob dur le cloud
                    blockBlob.UploadFromStream(fileStream);
                }


                MessageBox.Show("Blob envoyé avec succé !");

                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";

            }
        }

        private void btn_supprimer_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Etes vous sur de vouloir supprimer ce blob?", "Verification", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string nom = textBox1.Text;
                    string conteneur = textBox3.Text;
                    
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                        CloudConfigurationManager.GetSetting("StorageConnectionString"));
                    
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    
                    CloudBlobContainer container = blobClient.GetContainerReference(conteneur);
                    
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(nom);

                    // Supprimer le blob.
                    blockBlob.Delete();

                    MessageBox.Show("Blob supprimé avec succé !");

                    this.chargement();

                    textBox1.Text = "";
                }

            }
        }
    }
}
