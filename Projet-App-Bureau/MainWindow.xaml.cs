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

            // Clé de connexion: Pour Mariam
            // conBD = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=(localdb)\MSSQLLocalDB;Integrated Security=True;Pooling=False");

            // Clé de connexion: Pour MS SQL Server Management Studio
            conBD = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=projet-app-bureau;Trusted_Connection=True;");

            // Initialisation du datagrid; pour ne pas avoir des Row vide dans l'interface on le met à false.
            dgSimple.CanUserAddRows = false;

            // Charge les comboBoxes dans le menu stagiaire et le menu consulter           
            Charger_programmeConsulter();

            // Charger la liste étudiant
            Charger_Liste_stagiaire();

            // Pour mettre à jour le comboBox "programmes" dans le menu Stagiaire.
            InitialisationDesComposants();
        }

        /// <summary>
        /// Remplir les comboBox du menu stagiaire avec la table programme de la BD
        /// </summary>
        private void Charger_programmeStagiaire()
        {
            // Mettre les commandes SQL en spécifiant la connexion à chercher
            // Chercher les noms de programme pour populer le comboBox
            SqlCommand command = new SqlCommand("SELECT nom_programme FROM programme", conBD);

            // Ouvrir la connexion
            conBD.Open();

            // Lire les enregistrements collectées suite à l'exécution de la requête 1
            SqlDataReader dataReader = command.ExecuteReader();

            // Chargement du combobox programme dans stagiaire avec les données de la BD
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
            // Mettre les commande SQL en spécifiant la connexion à chercher
            // Chercher les id de programme pour populer le combobox
            SqlCommand command = new SqlCommand("SELECT id_programme FROM programme", conBD);

            // Ouvrir la connexion
            conBD.Open(); 

            // Lire les enregistrements colelctées suite à l'exécution de la requête 2
            SqlDataReader dataReader = command.ExecuteReader();

            // Chargement du combobox consulter avec les données de la BD
            while (dataReader.Read())
            {
                comboBoxConsulter.Items.Add(dataReader[0]);
            }

            // Fermer la connexion avec la BD
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

            SqlCommand command = new SqlCommand("SELECT * FROM stagiaire", conBD);

            // Ouvrir la connexion
            conBD.Open();

            // Lire les enregistrements collectées suite à l'exécution de la requête
            SqlDataReader dataReader = command.ExecuteReader();

            // Stocker les données lues par DataReader dans DataTable
            DataTable dt = new DataTable();
            dt.Load(dataReader);

            // Fermer la connexion
            conBD.Close();

            // Chargement de DataGrid avec les données de la BD
            dgSimple.ItemsSource = dt.DefaultView;  
        }

        /**************************************** Interface Programmes ************************************************/
        /// <summary>
        /// Méthode associée au Button qui ajoute un nouveau programme à la liste des programmes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjouterProgramme_Click(object sender, RoutedEventArgs e)
        {
            // Déclaration et initialisation des variables.
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
                else if (!int.TryParse(txtDureeProgramme.Text, out dureeProgramme) || int.Parse(txtDureeProgramme.Text) < 0 || txtDureeProgramme.Text == "")
                {
                    //mettre la bordure du textbox en rouge pour avertir l'utilisateur
                    txtDureeProgramme.BorderBrush = Brushes.Red;
                    MessageBox.Show("SVP veuillez entrez une durée valide en semaine!");
                }

                // On peut passer à l'étape de création d'un nouveau programme
                else
                {
                    // Ajouter un nouveau programme dans la table programme dans la BD
                    AjouterProgramme();
                }
            }
            catch (FormatException)
            {
                // Affiche un message d'erreur
                MessageBox.Show("SVP veuillez entrez une valeur valide !");
            }

            // Remettre la bordure des textboxes à leur couleur initiale
            txtNumeroProgramme.BorderBrush = Brushes.LightBlue;
            txtDureeProgramme.BorderBrush = Brushes.LightBlue;
            txtNomProgramme.BorderBrush = Brushes.LightBlue;

            // Pour mettre à jour le comboBox "programmes" dans le menu Stagiaire
            InitialisationDesComposants();
        }

        /// <summary>
        /// Méthode qui ajoute un nouveau programme dans la base de données.
        /// </summary>
        /// <param name="P"></param>
        private void AjouterProgramme()
        {
            // Ma requête, on les precède de @ POUR DIRE QUE CE SONT DES PARAMÈTRES
            string maRequete = "INSERT INTO programme VALUES (@nom,@duree)";

            SqlCommand command = new SqlCommand(maRequete, conBD);  //Définir la requête à exécuter sur la BD

            command.CommandType = CommandType.Text;

            // Récupérer les valeurs à mettre dans les paramètres de la requête 
            command.Parameters.AddWithValue("@nom", txtNomProgramme.Text);
            command.Parameters.AddWithValue("@duree", txtDureeProgramme.Text);

            // Ouvrir la connexion
            conBD.Open(); 

            // Executer la requête
            command.ExecuteNonQuery();

            // Fermer la connexion
            conBD.Close(); 

            // Charge les comboboxes dans menu stagiaire et consulter
            Charger_programmeStagiaire();
            Charger_programmeConsulter();

            // Message à afficher à l'utilisateur
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
            try
            {
                // Définir la requête à exécuter sur la BD
                SqlCommand command = new SqlCommand("DELETE FROM programme WHERE Id_programme = " + numero + "", conBD);

                //executer la requête
                command.ExecuteNonQuery();
                conBD.Close(); //Fermer la connexion

                // Charge les comboboxes dans menu stagiaire et consulter
                Charger_programmeStagiaire();
                Charger_programmeConsulter();

                // Message à afficher à l'utilisateur
                MessageBox.Show("Opération réussie !", "Confirmation de suppression");

            }
            catch
            {
                MessageBox.Show("Opération échoué !", "Veuillez mettre des entrées valides");
            }
        }


        /// <summary>
        /// Méthode qui ajoute un nouveau stagiaire dans la liste des programmes.
        /// </summary>
        /// <param name="P"></param>
        private void AjouterStagiaire(int numéro, string nom, string prénom, string dateDeNaissance, string sexe, string nomProgramme)
        {
            // S'assure de fermer un connexion s'il elle est déjà ouverte pour ne pas crash le programme
            conBD.Close();
            // Ouvrir la connexion
            conBD.Open();

            // Requête SQL pour ajouter un nouveau stagiaire, @paramètres
            string queryGetIdProgramme = "select id_programme from programme where nom_programme = '" + nomProgramme + "';";

            // Commande pour faire executer la requête
            SqlCommand command_getIdProgramme = new SqlCommand(queryGetIdProgramme, conBD);
            command_getIdProgramme.CommandType = CommandType.Text;

            // Executer la requête
            command_getIdProgramme.ExecuteNonQuery();
            
            // Insérer le résultat de la requête SQL dans une variable
            String result = command_getIdProgramme.ExecuteScalar().ToString();

            //***********************************

            // Requête SQL pour ajouter un nouveau stagiaire, @paramètres
            string queryAddStagiaire = "INSERT INTO stagiaire VALUES (@nom_stagiaire,@prenom_stagiaire,@sexe_stagiaire,@date_naissance,@id_programme)";

            // Définir la requête à exécuter sur la BD
            SqlCommand command = new SqlCommand(queryAddStagiaire, conBD);

            // Convertir la commande en texte
            command.CommandType = CommandType.Text;

            // Récupérer les valeurs à mettre dans les paramètres de la requête 
            command.Parameters.AddWithValue("@nom_stagiaire", nom);
            command.Parameters.AddWithValue("@prenom_stagiaire", prénom);
            command.Parameters.AddWithValue("@sexe_stagiaire", sexe);
            command.Parameters.AddWithValue("@date_naissance", dateDeNaissance);
            command.Parameters.AddWithValue("@id_programme", result);

            // Fermer la connexion
            conBD.Close();

            // Réouvrir la connexion
            conBD.Open();
            // Exécuter la requête SQL
            command.ExecuteNonQuery();
            //command.ExecuteReader();

            // Fermer la connexion
            conBD.Close(); 

            // Charge les comboBoxes dans le menu stagiaire et consulter
            Charger_programmeStagiaire();
            Charger_programmeConsulter();

            // Affiche un message de confirmation à l'utilisateur
            MessageBox.Show("Stagiaire ajouté !", "Confirmation d'ajout");
        }
        /// <summary>
        /// Méthode qui supprime un programme de la table programme à travers son numéro.
        /// </summary>
        /// <param name="numero"></param>
        private void SupprimerStagiaire()
        {
            int.TryParse(textBoxNumero.Text, out int numero);
            conBD.Open(); //ouvrir la connexion

            try
            {
                //Définir la requête à exécuter sur la BD
                SqlCommand command = new SqlCommand("DELETE FROM stagiaire WHERE id_stagiaire = " + numero + "", conBD);

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

        // **************************************** Interface Stagiaires ************************************************
        /// <summary>
        /// Méthode qui ajoute un stagiaire dans la base de donnée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAjouterStagiaire_Click(object sender, RoutedEventArgs e)
        {
            // ***************** Section 1: Déclaration des variables du stagiaire ******************
            int numero = 0;
            string nom = formatTitre.ToTitleCase(textBoxNom.Text);
            string prenom = formatTitre.ToTitleCase(textBoxPrenom.Text);
            DateTime dateDeNaissance = new DateTime();
            string sexe = "";
            string nomProgramme = "";

            // ***************** Section 2: Vérification des inputs ********************************

            bool radioButtonMasculin = (bool)radioButton_masculin.IsChecked;
            bool radioButtonFeminin = (bool)radioButton_feminin.IsChecked;
            bool radioButtonAutre = (bool)radioButton_autre.IsChecked;

            // #1 S'il y a un input vide
            if (textBoxNumero.ToString() == "" ||
                textBoxNom.Text.Length == 0 ||
                textBoxPrenom.Text.Length == 0 ||
                datePickerDateDeNaissance.SelectedDate == null ||
                ((bool)!radioButton_masculin.IsChecked &&
                (bool)!radioButton_feminin.IsChecked &&
                (bool)!radioButton_autre.IsChecked) ||
                comboBoxProgrammes.SelectedValue == null)
            {
                // Affiche un message d'erreur
                MessageBox.Show("SVP veuillez remplire toutes les cases !");
            }
            // #2 S'il n'y a aucun input vide
            else
            {
                try
                {
                    // #1 valider le numéro
                    numero = Int32.Parse(textBoxNumero.Text);

                    // #2 valider le nom
                    bool nomValide = true;

                    // Vérifier si tous les caractères sont des lettres seulement
                    /// Source: https://stackoverflow.com/questions/1181419/verifying-that-a-string-contains-only-letters-in-c-sharp
                    foreach (char character in textBoxNom.Text.ToString())
                    {
                        if (!Char.IsLetter(character))
                        {
                            nomValide = false;
                        }
                    }
                    // Si le nom n'est pas valide
                    if (nomValide == false)
                    {
                        MessageBox.Show("SVP veuillez entrez un nom valide !");
                        textBoxNom.Text = "";
                    }

                    // #3 Valider le prénom
                    bool prenomValide = true;

                    // Vérifier si tous les caractères sont des lettres seulement
                    /// Source: https://stackoverflow.com/questions/1181419/verifying-that-a-string-contains-only-letters-in-c-sharp
                    foreach (char character in textBoxPrenom.Text.ToString())
                    {
                        if (!Char.IsLetter(character))
                        {
                            prenomValide = false;
                        }
                    }
                    // Si le prénom n'est pas valide
                    if (prenomValide == false)
                    {
                        MessageBox.Show("SVP veuillez entrez un prénom valide !");
                        textBoxPrenom.Text = "";
                    }

                    // #4 Valider le datePicker Date
                    if (datePickerDateDeNaissance.SelectedDate != null)
                    {
                        dateDeNaissance = datePickerDateDeNaissance.SelectedDate.Value.Date;
                    }

                    // #5 Valider radioButton sexe 
                    if (radioButtonMasculin)
                    {
                        sexe = "Masculin";
                    }
                    else if (radioButtonFeminin)
                    {
                        sexe = "Féminin";
                    }
                    else if (radioButtonAutre)
                    {
                        sexe = "Autre";
                    }

                    // #6 ComboBox programmes
                    if (comboBoxProgrammes.SelectedValue != null)
                    {
                        nomProgramme = comboBoxProgrammes.SelectedValue.ToString();
                    }

                    // **************** Section 3: Créer un nouveau stagiaire *****************************
                    if (nomValide && prenomValide)
                    {
                        // Query qui va au travers de chque "id_stagiaire" de la table stagiaire
                        SqlCommand command_GetIdStagiaire = new SqlCommand("SELECT id_stagiaire FROM stagiaire", conBD);

                        // Get id_stagiaire
                        conBD.Close();
                        conBD.Open();
                        // Requête SQL pour ajouter un nouveau stagiaire,  @paramètres
                        command_GetIdStagiaire.CommandType = CommandType.Text;

                        // Executer la commande GetIdStagiaire
                        command_GetIdStagiaire.ExecuteNonQuery();
                        //command_getIdProgramme.ExecuteReader();
                        String result = command_GetIdStagiaire.ExecuteScalar().ToString();


                        // Si le query ne retourne aucun résultat, on peut ajouter le nouveau stagiaire
                        // Source: https://stackoverflow.com/questions/36447590/check-if-query-result-is-null-in-c-sharp
                        if (result != null)
                        {
                            AjouterStagiaire(numero, nom, prenom, dateDeNaissance.ToString(), sexe, nomProgramme);
                        }
                        // Si le stagiaire existe déjà, on retourne une erreur
                        else if (result == null)
                        {
                            // Affiche un message d'erreur 
                            MessageBox.Show("Ce numéro de stagiaire existe déjà, veuillez utilisé un autre numéro.");
                        }

                        //fermer la connexion avec la BD
                        conBD.Close();

                        // #4  Effacer le contenu de tous les champs dans le menu stagiaires
                        textBoxNumero.Text = "";
                        textBoxPrenom.Text = "";
                        textBoxNom.Text = "";
                        datePickerDateDeNaissance.SelectedDate = null;
                        radioButton_masculin.IsChecked = false;
                        radioButton_feminin.IsChecked = false;
                        radioButton_autre.IsChecked = false;
                        comboBoxProgrammes.SelectedIndex = -1;  // Remet le comboBox à son état initial
                    }
                }
                catch (FormatException)
                {
                    // Affiche un message d'erreur
                    MessageBox.Show("SVP veuillez entrez une valeur valide !");
                }

                // reinitailise les combosBox et le dataGrid
                InitialisationDesComposants();
            }
        }
        /// <summary>
        /// Méthode qui supprime un stagiaire dans la base de donnée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSupprimerStagiaire_Click(object sender, RoutedEventArgs e)
        {
            SupprimerStagiaire();

            dgSimple.ItemsSource = "";

            // Pour mettre à jour le comboBox "programmes" dans le menu Stagiaire
            InitialisationDesComposants();
        }
        /// <summary>
        /// Méthode qui efface le contenu de tous les champs dans le menu stagiaires
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEffacerStagiaire_Click(object sender, RoutedEventArgs e)
        {
            textBoxNumero.Text = "";
            textBoxPrenom.Text = "";
            textBoxNom.Text = "";
            datePickerDateDeNaissance.SelectedDate = null;
            radioButton_masculin.IsChecked = false;
            radioButton_feminin.IsChecked = false;
            radioButton_autre.IsChecked = false;
            comboBoxProgrammes.SelectedIndex = -1;  // Remet le comboBox à son état initial
        }

        private void ButtonRecherche_Click(object sender, RoutedEventArgs e)
        {
            Charger_Liste_stagiaire();
        }
    }
}
