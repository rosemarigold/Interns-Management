/*
 * Groupe 4
 * Participants:  Bruno Tsane, Lamia Ouassaa, Mariam Cisse, Yousra Merzouk.
 * Description: Gestion des programmes et des stagiaires.
 * Date: 2021-11-30
 */



using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Data;
using System.Data.SqlClient;

namespace Projet_App_Bureau
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // déclaration d'une variable pour initialiser la connexion à la BD
        private SqlConnection conBD;

        //Déclaration et initialisation de la liste des programmes.
       // private List<Programme> listeProgrammes = new List<Programme>();

        // Lien pour transformer un string en format titre
        // Source : https://docs.microsoft.com/en-us/dotnet/api/system.globalization.textinfo.totitlecase?view=net-5.0
        // Instanciation de l'objet permettant de mettre en majuscule le début des mot
        TextInfo formatTitre = new CultureInfo("en-US", false).TextInfo;
        public MainWindow()
        {
            //initialisation des combos box et datagrid 
            InitializeComponent();

            //Configuration de la chaine de connexion à la BD
            //on met le @ pour echapper (changer la direction du \)   
 
            conBD = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=(localdb)\MSSQLLocalDB;Integrated Security=True;Pooling=False");

            //initialisation du datagrid, pour ne pas avoir des Row vide dans l'interface on le met a false.
            dgSimple.CanUserAddRows = false;

            //charge les comboboxes dans menu stagiaire et consulter
            Charger_programmeStagiaire();
            Charger_programmeConsulter();

            //charger la liste etudiant
            Charger_Liste_stagiaire();

            // Pour mettre à jour le comboBox "programmes" dans le menu Stagiaire.
            InitialisationDesComposants();
        }

        /// <summary>
        /// Remplir les comboBox du menu stagiaire avec la table programme de la BD
        /// </summary>
        private void Charger_programmeStagiaire()
        {
            //mettre les commande SQL en spécifiant la connexion à chercher
            //chercher les noms de programme pour populer le combobox
            SqlCommand command = new SqlCommand("SELECT nom_programme FROM programme", conBD);
            conBD.Open(); //ouvrir la connexion

            // lire les enregistrements colelctées suite à l'exécution de la requête 1
            SqlDataReader dataReader = command.ExecuteReader();

            //chargement du combobox programme dans stagiaire avec les données de la BD
            while (dataReader.Read())
            {
                comboBoxProgrammes.Items.Add(dataReader[0]);
            }

            //fermer la connexion avec la BD
            conBD.Close();
        }


        /// <summary>
        /// Remplir les comboBox du menu consulter avec la table programme de la BD
        /// </summary>
        private void Charger_programmeConsulter()
        {         
            //mettre les commande SQL en spécifiant la connexion à chercher
            //chercher les id de programme pour populer le combobox
            SqlCommand command = new SqlCommand("SELECT Id_programme FROM programme", conBD);
            conBD.Open(); //ouvrir la connexion
           
            // lire les enregistrements colelctées suite à l'exécution de la requête 2
            SqlDataReader dataReader = command.ExecuteReader();

            //chargement du combobox consulter avec les données de la BD
            while (dataReader.Read())
            {
                comboBoxConsulter.Items.Add(dataReader[0]);
            }

            //fermer la connexion avec la BD
            conBD.Close();
        }
        /// <summary>
        /// Remplir le datagrid avec la table stagiaire de la BD
        /// </summary>
        private void Charger_Liste_stagiaire()
        {
            //mettre les commande SQL en spécifiant la connexion à chercher
            //chercher les informations de stagiaire du programme selectionné pour populer le datagrid
            // SqlCommand command = new SqlCommand("SELECT * FROM stagiaire S, DATEDIFF(year, 'date_naissance', GETDATE()) AS age" +
            //    "JOIN programme p ON P.nom_programme=s.nom_programme WHERE p.Id_programme='" + comboBoxConsulter.SelectedValue.ToString() +"'" , conBD);

            SqlCommand command = new SqlCommand("SELECT * FROM stagiaire",conBD);
            conBD.Open(); //ouvrir la connexion
            

            // lire les enregistrements colelctées suite à l'exécution de la requête
            SqlDataReader dataReader = command.ExecuteReader();

            // stocker les données lues par DataReader dans DataTable
            DataTable dt = new DataTable();
            dt.Load(dataReader);

            conBD.Close(); //Fermer la connexion

            dgSimple.ItemsSource = dt.DefaultView;  //chargement de DataGrid avec les données de la BD


        }

        /**************************************** Interface Programmes ************************************************/
        /// <summary>
        /// Méthode associée au Button qui ajoute un nouveau programme à la liste des programmes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjouterProgramme_Click(object sender, RoutedEventArgs e)
        {
            //déclaration et initialisation des variables.
            string nomProgramme;
            int numeroProgramme, dureeProgramme;

            //gestions des exceptions pour s'assurer d'entrer des valeurs valides.
            try
            {

                // Si le textBox du "numéro programme" n'est pas vide ou n'est pas un nombre positif
                if (!int.TryParse(txtNumeroProgramme.Text, out numeroProgramme) || int.Parse(txtNumeroProgramme.Text) <= 0 || txtNumeroProgramme.Text == "")
                {
                    //mettre la bordure du textbox en rouge pour avertir l'utilisateur
                    txtNumeroProgramme.BorderBrush = Brushes.Red;               
                    MessageBox.Show("SVP veuillez entrez un numéro valide positif !");
                }

                // Si le textBox du "Nom programme" est vide ou s'il est numerique.
                else if (txtNomProgramme.Text == "" || int.TryParse(txtNomProgramme.Text, out int n))
                {
                    //mettre la bordure du textbox en rouge pour avertir l'utilisateur
                    txtNomProgramme.BorderBrush = Brushes.Red;                  
                    MessageBox.Show("SVP veuillez entrez un nom valide !");
                }

                // Si le textBox de "durée programme" n'est pas vide ou n'est pas un nombre positif
                else if (!int.TryParse(txtDureeProgramme.Text, out dureeProgramme) || int.Parse(txtDureeProgramme.Text) < 0 || txtDureeProgramme.Text=="")
                {
                    //mettre la bordure du textbox en rouge pour avertir l'utilisateur
                    txtDureeProgramme.BorderBrush = Brushes.Red;               
                    MessageBox.Show("SVP veuillez entrez une durée valide en semaine!");
                }

                // on peut passer à l'étape de création d'un nouveau programme
                else
                {   
                    //ajouter un nouveau programme dans la table programme dans la BD
                    AjouterProgramme();
                }
            }
            catch (FormatException)
            {
                // Affiche un message d'erreur
                MessageBox.Show("SVP veuillez entrez une valeur valide !");
            }

            // remettre la bordure des textboxes à leur couleur initiale
            txtNumeroProgramme.BorderBrush = Brushes.LightBlue;
            txtDureeProgramme.BorderBrush = Brushes.LightBlue;
            txtNomProgramme.BorderBrush = Brushes.LightBlue;  
            
            // Pour mettre à jour le comboBox "programmes" dans le menu Stagiaire
            InitialisationDesComposants();
        }

        /// <summary>
        /// Méthode qui ajoute un nouveau programme dans la liste des programmes.
        /// </summary>
        /// <param name="P"></param>
        private void AjouterProgramme()
        { 
            // Ma requête, on les precède de @ POUR DIRE QUE CE SONT DES PARAMÈTRES
            string maRequete = "INSERT INTO programme VALUES (@nom,@duree)";

            SqlCommand command = new SqlCommand(maRequete, conBD);  //Définir la requête à exécuter sur la BD

            command.CommandType = CommandType.Text;

            //Récupérer les valeurs à mettre dans les paramètres de la requête 
            command.Parameters.AddWithValue("@nom", txtNomProgramme.Text);
            command.Parameters.AddWithValue("@duree", txtDureeProgramme.Text);            

            conBD.Open(); //ouvrir la connexion
                            //executer la requête
            command.ExecuteNonQuery();

            conBD.Close(); //Fermer la connexion

            //charge les comboboxes dans menu stagiaire et consulter
            Charger_programmeStagiaire();
            Charger_programmeConsulter();

            //message afficher à l'utilisateur
            MessageBox.Show("Opération réussie !", "Confirmation d'ajout");
        }

        /// <summary>
        /// Méthode qui supprime un programme de la table programme à travers son numéro.
        /// </summary>
        /// <param name="numero"></param>
        private void SupprimerProgramme()
        {
            int.TryParse(txtNumeroProgramme.Text, out int numero);
            conBD.Open(); //ouvrir la connexion

            // Ma requête, on les precède de @ POUR DIRE QUE CE SONT DES PARAMÈTRES
            //   string maRequete = "DELETE FROM eprogramma WHERE id_programme=txtNumeroProgramme.Txt";
            try {
                //Définir la requête à exécuter sur la BD
                SqlCommand command = new SqlCommand("DELETE FROM programme WHERE Id_programme = " + numero + "", conBD);  
            
            //executer la requête
            command.ExecuteNonQuery();
            conBD.Close(); //Fermer la connexion

            //charge les comboboxes dans menu stagiaire et consulter
            Charger_programmeStagiaire();
            Charger_programmeConsulter();

                //message afficher à l'utilisateur
                MessageBox.Show("Opération réussie !", "Confirmation de suppression");
            }
            catch
            {
                MessageBox.Show("Opération échoué !", "Veuillez mettre des entrées valides");
            }
        }

        /// <summary>
        /// Méthode associée au Button qui supprime un programme dans la liste des programmes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerProgramme_Click(object sender, RoutedEventArgs e)
        {
            SupprimerProgramme();

            dgSimple.ItemsSource = "";

            // Pour mettre à jour le comboBox "programmes" dans le menu Stagiaire
            InitialisationDesComposants();
        }

        /// <summary>
        /// Méthode associée au Button qui efface le contenu des champs dans le menu programmme.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEffacerProgramme_Click(object sender, RoutedEventArgs e)
        {
            //Effacer le contenu des champs dans le menu programmme.
            if (txtNumeroProgramme.Text != null)
            {
                txtNumeroProgramme.Text = "";
            }

            if (txtNomProgramme.Text != null)
            {
                txtNomProgramme.Text = "";
            }

            if (txtDureeProgramme.Text != null)
            {
                txtDureeProgramme.Text = "";
            }
        }

        

        /// <summary>
        /// Faire une mise a jour du combos Box chaque fois qu'on modifie la liste.
        /// </summary>
        void InitialisationDesComposants() 
        {
            //Initialiser le combo box avec les numéro de programme.
            //nom temporaire pour stocker le numero du programme
            string temp = comboBoxConsulter.Text;
            comboBoxProgrammes.Items.Clear();
            //rafraichir le datagrid
            dgSimple.Items.Refresh();
            comboBoxConsulter.Items.Clear();

            //charge les comboboxes dans menu stagiaire et consulter
            Charger_programmeStagiaire();
            Charger_programmeConsulter();

            comboBoxConsulter.Text = temp;
        }

        private void buttonAjouterStagiaire_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonSupprimerStagiaire_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonEffacerStagiaire_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonRecherche_Click(object sender, RoutedEventArgs e)
        {
            Charger_Liste_stagiaire();
        }
    }
}
