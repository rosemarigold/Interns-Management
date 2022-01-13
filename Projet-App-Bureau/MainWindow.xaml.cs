/*
 * Groupe 4
 * Participants:  Bruno Tsane, Lamia Ouassaa, Mariam Cisse, Yousra Merzouk.
 * Description: Gestion des programmes et des stagiaires.
 * Date: 2021-11-30
 */

using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

            
            // Clé de connexion:
            conBD = new SqlConnection(@"Server=localhost\SQLEXPRESS;Database=projet-app-bureau;Trusted_Connection=True;");

            // Initialisation du datagrid; pour ne pas avoir des Row vide dans l'interface on le met à false.
            dgSimple.CanUserAddRows = false;
            dgProgramme.CanUserAddRows = false;
            dgStagiaire.CanUserAddRows = false;

            // Charge les comboBoxes dans le menu stagiaire et le menu consulter           
            Charger_programmeConsulter();

            // Charger la liste étudiant
            Charger_Liste_stagiaire();

            // Pour mettre à jour le comboBox "programmes" dans le menu Stagiaire.
            InitialisationDesComposants();
            Charger_Liste_Programme();
        }

        /// <summary>
        /// Remplir les comboBox du menu stagiaire avec la table programme de la BD
        /// </summary>
        private void Charger_programmeStagiaire()
        {
            // Mettre les commandes SQL en spécifiant la connexion à chercher
            // Chercher les noms de programme pour populer le comboBox
            SqlCommand command = new SqlCommand("SELECT nom_programme FROM programme", conBD);

            // Fermer la connexion avec la BD au cas ou qu'elle n'a pas déjà ete fermé
            conBD.Close();

            // Ouvrir la connexion
            conBD.Open();

            // Lire les enregistrements collectées suite à l'exécution de la requête 1
            SqlDataReader dataReader = command.ExecuteReader();

            // Chargement du combobox programme dans stagiaire avec les données de la BD
            while (dataReader.Read())
            {
                comboBoxProgrammes.Items.Add(dataReader[0]);
            }

            // Fermer la connexion avec la BD
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
        /// Méthode qui remplit le datagrid "dtStagiaire" avec le contenu de la table stagiaire de la BD
        /// </summary>
        private void Charger_Liste_stagiaire()
        {
            // Déclaration des titres du dataGrid "stagiaire"
            string id = "ID";
            string nom = "Last name";
            string prenom = "First name";
            string sexe= "Gender";
            string date_naissance = "Date of birth";
            string programme = "ID Program";

            // Mettre la commande SQL en spécifiant la connexion à chercher        
            SqlCommand command = new SqlCommand("SELECT id_stagiaire as '" + id + "', nom_stagiaire as '" + nom + "', prenom_stagiaire as '" + prenom + "', sexe_stagiaire as '" + sexe + "', date_naissance as '" + date_naissance + "', id_programme as '" + programme + "' FROM stagiaire", conBD);

            // S'assurer que la connexion est fermé au cas ou elle serait toujours ouverte
            conBD.Close();

            // Ouvrir la connexion
            conBD.Open(); 

            // Lire les enregistrements collectées suite à l'exécution de la requête
            SqlDataReader dataReader = command.ExecuteReader();

            // Stocker les données lues par le DataReader dans DataTable
            DataTable dt = new DataTable();
            dt.Load(dataReader);

            // Fermer la connexion à la BD
            conBD.Close();

            // Chargement de DataGrid "dgStagiaire" avec les données de la BD
            dgStagiaire.ItemsSource = dt.DefaultView;
        }

        /**************************************** Interface Programmes ************************************************/
        /// <summary>
        /// Méthode associée au Button qui ajoute un nouveau programme à la liste des programmes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjouterProgramme_Click(object sender, RoutedEventArgs e)
        {
            // Valider le nom de programme
            bool programmeValide = true;

            // Vérifier si tous les caractères sont des lettres seulement
            /// Source: https://stackoverflow.com/questions/1181419/verifying-that-a-string-contains-only-letters-in-c-sharp
            foreach (char character in txtNomProgramme.Text.ToString())
            {
                if (!Char.IsLetter(character))
                {
                    programmeValide = false;
                    break;
                }
            }

            //gestions des exceptions pour s'assurer d'entrer des valeurs valides.
            try
            {
                // Si le textBox du "Nom programme" est vide ou s'il est numerique.
                if (txtNomProgramme.Text == "" || int.TryParse(txtNomProgramme.Text, out int n) || programmeValide == false)
                {
                    //mettre la bordure du textbox en rouge pour avertir l'utilisateur
                    txtNomProgramme.BorderBrush = Brushes.Red;
                    MessageBox.Show("SVP veuillez entrez un nom de programme valide !");
                }


                // Si le textBox de "durée programme" n'est pas vide ou n'est pas un nombre positif
                else if (!int.TryParse(txtDureeProgramme.Text, out int dureeProgramme) || int.Parse(txtDureeProgramme.Text) < 0 || txtDureeProgramme.Text == "")
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
            Charger_Liste_Programme();
        }

        /// <summary>
        /// Méthode qui ajoute un nouveau programme dans la base de données.
        /// </summary>
        /// <param name="P"></param>
        private void AjouterProgramme()
        {
            // Vérifier si le nom du programme n'existe pas déjà
            // Source: https://stackoverflow.com/questions/20743121/how-can-i-get-sql-result-into-a-string-variable

            // Query qui cherche le nom du programme
            string query_getNomProgramme = "SELECT nom_programme FROM programme WHERE nom_programme = '" + txtNomProgramme.Text + "';";

            // Exécution du query
            using (var command_NomProgramme = new SqlCommand(query_getNomProgramme, conBD))
            {
                // Ouvrir la connexion à la BD
                conBD.Open();

                // Source: https://stackoverflow.com/questions/42524639/unable-to-cast-object-of-type-system-datetime-to-type-system-string-c-sharp
                // Stocker le résultat du query
                query_getNomProgramme = (string)command_NomProgramme.ExecuteScalar();

                // Si le nom du programme existe déjà
                if ((string)command_NomProgramme.ExecuteScalar() != null)
                {
                    MessageBox.Show("Le nom du programme existe déjà, veuillez en choisir un autre.");
                }
                // Si le nom du programme n'existe pas encore dans la table programme
                else
                {
                    // Ma requête, on les precède de @ POUR DIRE QUE CE SONT DES PARAMÈTRES
                    string maRequete = "INSERT INTO programme VALUES (@nom_programme,@duree_programme)";

                    SqlCommand command = new SqlCommand(maRequete, conBD);  //Définir la requête à exécuter sur la BD

                    command.CommandType = CommandType.Text;

                    // Récupérer les valeurs à mettre dans les paramètres de la requête 
                    command.Parameters.AddWithValue("@nom_programme", formatTitre.ToTitleCase(txtNomProgramme.Text));
                    command.Parameters.AddWithValue("@duree_programme", txtDureeProgramme.Text);

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
            }

        }

        /// <summary>
        /// Méthode qui supprime un programme de la table programme à travers son numéro.
        /// </summary>
        /// <param name="numero"></param>
        private void SupprimerProgramme()
        {
            int.TryParse(txtNumeroProgramme.Text, out int numero);


            // Ma requête, on les precède de @ POUR DIRE QUE CE SONT DES PARAMÈTRES
            //   string maRequete = "DELETE FROM eprogramma WHERE id_programme=txtNumeroProgramme.Txt";
            try
            {
                // Définir la requête à exécuter sur la BD
                SqlCommand command = new SqlCommand("DELETE FROM programme WHERE id_programme = '" + numero + "'", conBD);

                conBD.Open(); //ouvrir la connexion

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
                MessageBox.Show("Opération de suppression échouée !", "Veuillez mettre des entrées valides");
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

            //Effacer le contenu des champs du menu programmme.
            txtNumeroProgramme.Text = "";
            txtNomProgramme.Text = "";
            txtDureeProgramme.Text = "";

            // Pour mettre à jour le comboBox "programmes" dans le menu Stagiaire
            InitialisationDesComposants();
            Charger_Liste_Programme();
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

            // Met à jour le dataGrid "Programme"
            Charger_Liste_Programme();
        }

        private void btnModifierProgramme_Click(object sender, RoutedEventArgs e)
        {
            // Valider le nom de programme
            bool programmeValide = true;

            // Vérifier si tous les caractères sont des lettres seulement
            /// Source: https://stackoverflow.com/questions/1181419/verifying-that-a-string-contains-only-letters-in-c-sharp
            foreach (char character in txtNomProgramme.Text.ToString())
            {
                if (!Char.IsLetter(character))
                {
                    programmeValide = false;
                    break;
                }
                
            }

            //gestions des exceptions pour s'assurer d'entrer des valeurs valides.
            try
            {
                // Si le textBox de "numéro programme" n'est pas vide ou n'est pas un nombre positif
               if (!int.TryParse(txtNumeroProgramme.Text, out int numeroProgramme) || int.Parse(txtNumeroProgramme.Text) < 0 || txtNumeroProgramme.Text == "" )
                {
                    //mettre la bordure du textbox en rouge pour avertir l'utilisateur
                    txtNumeroProgramme.BorderBrush = Brushes.Red;
                    MessageBox.Show("SVP veuillez entrez une durée valide en semaine!");
                }
                // Si le textBox du "Nom programme" est vide ou s'il est numerique.
                else if (txtNomProgramme.Text == "" || int.TryParse(txtNomProgramme.Text, out int n) || programmeValide==false)
                {
                    //mettre la bordure du textbox en rouge pour avertir l'utilisateur
                    txtNomProgramme.BorderBrush = Brushes.Red;
                    MessageBox.Show("SVP veuillez entrez un nom de prgramme valide !");
                }

                // Si le textBox de "durée programme" n'est pas vide ou n'est pas un nombre positif
                else if (!int.TryParse(txtDureeProgramme.Text, out int dureeProgramme) || int.Parse(txtDureeProgramme.Text) < 0 || txtDureeProgramme.Text == "")
                {
                    //mettre la bordure du textbox en rouge pour avertir l'utilisateur
                    txtDureeProgramme.BorderBrush = Brushes.Red;
                    MessageBox.Show("SVP veuillez entrez une durée valide en semaine!");
                }

                // On peut passer à l'étape de création d'un nouveau programme
                else
                {
                    // modifier les informations du programme dans la table programme dans la BD
                    UpdateProgramme();
                }
            }
            catch (FormatException)
            {
                // Affiche un message d'erreur
                MessageBox.Show("SVP veuillez entrez une valeur valide !");
            }

            Charger_Liste_Programme();

        }

        private void UpdateProgramme()
        {
            conBD.Open(); //ouvrir la connexion
            SqlCommand command = new SqlCommand("UPDATE programme SET nom_programme = '" + formatTitre.ToTitleCase(txtNomProgramme.Text) + "' , duree_programme='" + txtDureeProgramme.Text + "' WHERE id_programme = '" + txtNumeroProgramme.Text + "' ", conBD);  //Définir la requête à exécuter sur la BD

            try
            {
                //executer la requête
                command.ExecuteNonQuery();
                //message afficher à l'utilisateur
                MessageBox.Show("Opération réussie !", "Confirmation de mise à jour");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("Opération de mise à jour échouée !", "Veuillez mettre des valeurs valides!");
            }
            finally
            {
                conBD.Close();
                Charger_Liste_Programme();
                txtNumeroProgramme.BorderBrush = Brushes.LightBlue;
                txtNomProgramme.BorderBrush = Brushes.LightBlue;
                txtDureeProgramme.BorderBrush = Brushes.LightBlue;
            }
        }

        // Méthode pour remplir textBoxs des informations de la colonne sélectionné dans le DataGrid
        private void dgProgramme_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DataGrid maDataGrid = (DataGrid)sender; //Récupérer le contenu de la DataGrid
            DataRowView ligne_selectionnee = maDataGrid.SelectedItem as DataRowView; //Récupérer le contenu de la ligne sélectionnée dans le DataGrid

            //Remplir les composants
            if (ligne_selectionnee != null)
            {
                txtNumeroProgramme.Text = ligne_selectionnee["ID Program"].ToString();
                txtNomProgramme.Text = ligne_selectionnee["Name"].ToString();
                txtDureeProgramme.Text = ligne_selectionnee["Duration"].ToString();
            }

            Charger_Liste_Programme();
        }

        /// <summary>
        /// Remplir la liste programme avec la table programme de la BD
        /// </summary>
        private void Charger_Liste_Programme()
        {
            string nom = "Name";
            string id = "ID Program";
            string duree = "Duration";

            //mettre les commande SQL en spécifiant la connexion à chercher        
            SqlCommand command = new SqlCommand("SELECT id_programme as '" + id + "', nom_programme as '" + nom + "', duree_programme as '" + duree + "' FROM programme", conBD);
            conBD.Open(); //ouvrir la connexion

            // lire les enregistrements colelctées suite à l'exécution de la requête
            SqlDataReader dataReader = command.ExecuteReader();

            // stocker les données lues par DataReader dans DataTable
            DataTable dt = new DataTable();
            dt.Load(dataReader);

            conBD.Close(); //Fermer la connexion

            dgProgramme.ItemsSource = dt.DefaultView;  //chargement de DataGrid avec les données de la BD


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
        /// Méthode qui ajoute un nouveau stagiaire dans la liste des programmes.
        /// </summary>
        /// <param name="P"></param>
        private void AjouterStagiaire(string nom, string prénom, string dateDeNaissance, string sexe, string nomProgramme)
        {
            // S'assure de fermer la connexion s'il elle est déjà ouverte pour ne pas crash le programme
            conBD.Close();

            // Ouvrir la connexion
            conBD.Open();

            // Requête SQL pour ajouter get le numéro du programme sélectionné dans le comboBox "Programme"
            string queryGetIdProgramme = "select id_programme from programme where nom_programme = '" + nomProgramme + "'";

            // Commande pour faire exécuter la requête
            SqlCommand command_getIdProgramme = new SqlCommand(queryGetIdProgramme, conBD);

            // Convertir la commande en texte
            command_getIdProgramme.CommandType = CommandType.Text;

            // Exécuter la requête
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

            // Fermer la connexion
            conBD.Close();

            // Charge les comboBoxes dans le menu stagiaire et consulter
            Charger_programmeStagiaire();
            Charger_programmeConsulter();

            // Affiche un message de confirmation à l'utilisateur
            MessageBox.Show("Intern added.", "Operation confirmation");
        }
        /// <summary>
        /// Méthode qui supprime un stagiaire de la table stagiaire à l'aide de son id.
        /// </summary>
        /// <param name="numero"></param>
        private void SupprimerStagiaire()
        {
            // Convertir le ID du stagiaire de string à int
            int.TryParse(textBoxNumero.Text, out int numero);

            // Ouvrir la connexion
            conBD.Open(); 

            try
            {
                // Définir la requête à exécuter sur la BD
                SqlCommand command = new SqlCommand("DELETE FROM stagiaire WHERE id_stagiaire = '" + numero + "'", conBD);

                // Exécuter la requête
                command.ExecuteNonQuery();
                conBD.Close(); //Fermer la connexion

                // Charge les comboboxes dans menu stagiaire et consulter
                Charger_programmeStagiaire();
                Charger_programmeConsulter();

                // Message à afficher à l'utilisateur: La supression du stagiaire a fonctionné
                MessageBox.Show("Successful operation.", "Deletion confirmation");
            }
            catch
            {
                // Message à afficher à l'utilisateur: La supression du stagiaire n'a pas fonctionné
                MessageBox.Show("Operation failed.", "Please enter valid inputs.");
            }
        }
        /// <summary>
        /// Méthode qui met à jour un stagiaire dans la table stagiaire de la BD à l'aide de son id.
        /// </summary>
        private void UpdateStagiaire()
        {
            // Déclaration des variables pour identifier quel radioButton sexe a été sélectionné
            bool radioButtonMasculin = (bool)radioButton_masculin.IsChecked;
            bool radioButtonFeminin = (bool)radioButton_feminin.IsChecked;
            bool radioButtonAutre = (bool)radioButton_autre.IsChecked;
            String gender = "";

            // Obtenir la valeur du radioButton sexe 
            if (radioButtonMasculin)
            {
                gender = "Man";
            }
            else if (radioButtonFeminin)
            {
                gender = "Woman";
            }
            else if (radioButtonAutre)
            {
                gender = "Other";
            }

            // Trouver le id_programme
            // Query pour trouver le id programme correspondant au programme dont le stagiaire fait partie
            string query_getIdProgramme = "SELECT id_programme FROM programme WHERE nom_programme = '" + comboBoxProgrammes.SelectedItem.ToString() + "';";

            // Variable qui stocke le résultat du query (id_programme)
            int query_result_IdProgramme;

            // Exécuter le query
            using (var command_getIDProgramme = new SqlCommand(query_getIdProgramme, conBD))
            {
                // Ouvrir la connexion à la BD
                conBD.Open();

                // Stocker le résultat du query
                query_result_IdProgramme = (int)command_getIDProgramme.ExecuteScalar();
            }

            // Query pour mettre à jour le stagiaire
            SqlCommand command_updateStagiaire = new SqlCommand("UPDATE stagiaire SET nom_stagiaire = '" + textBoxNom.Text + "' , prenom_stagiaire='" + textBoxPrenom.Text + "' , sexe_stagiaire='" + gender + "' , date_naissance='" + datePickerDateDeNaissance.SelectedDate + "' , id_programme = " + query_result_IdProgramme.ToString() + " WHERE id_stagiaire = '" + textBoxNumero.Text + "' ", conBD);

            try
            {
                // Exécuter la requête
                command_updateStagiaire.ExecuteNonQuery();

                // Message à afficher à l'utilisateur
                MessageBox.Show("Update successful.", "Update confirmation");
            }
            catch (SqlException ex)
            {
                // Afficher l'erreur du compilateur
                // MessageBox.Show(ex.Message);

                // Message à afficher à l'utilisateur
                MessageBox.Show("Update failed.", "Please enter valid inputs.");
            }
            finally
            {
                // Enfin, on fermela connexion à la BD
                conBD.Close();

                // Fefresh le datagrid "dtStagiaire" avec le contenu de la table stagiaire de la BD
                Charger_Liste_stagiaire();
                // Mettre à jour les Data Grid
                Charger_Liste_stagiaire();
                Charger_Liste_stagiaire_Consulter();
            }
        }

        /// <summary>
        /// Méthode qui ajoute un stagiaire dans la base de donnée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAjouterStagiaire_Click(object sender, RoutedEventArgs e)
        {
            // ***************** Section 1: Déclaration des variables du stagiaire ******************
            //int numero = 0;
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
            if (textBoxNom.Text.Length == 0 ||
                textBoxPrenom.Text.Length == 0 ||
                datePickerDateDeNaissance.SelectedDate == null ||
                ((bool)!radioButton_masculin.IsChecked &&
                (bool)!radioButton_feminin.IsChecked &&
                (bool)!radioButton_autre.IsChecked) ||
                comboBoxProgrammes.SelectedValue == null)
            {
                // Affiche un message d'erreur
                MessageBox.Show("Please fill all fields.");
            }
            // #2 S'il n'y a aucun input vide
            else
            {
                try
                {
                    // valider le nom
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
                        MessageBox.Show("Please enter a valid last name.");
                        textBoxNom.Text = "";
                    }

                    // Valider le prénom
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
                        MessageBox.Show("Please enter a valid first name.");
                        textBoxPrenom.Text = "";
                    }

                    // Valider le datePicker Date
                    if (datePickerDateDeNaissance.SelectedDate != null)
                    {
                        dateDeNaissance = datePickerDateDeNaissance.SelectedDate.Value.Date;
                    }

                    // Valider radioButton sexe 
                    if (radioButtonMasculin)
                    {
                        sexe = "Man";
                    }
                    else if (radioButtonFeminin)
                    {
                        sexe = "Woman";
                    }
                    else if (radioButtonAutre)
                    {
                        sexe = "Other";
                    }

                    // ComboBox programmes
                    if (comboBoxProgrammes.SelectedValue != null)
                    {
                        nomProgramme = comboBoxProgrammes.SelectedValue.ToString();
                    }

                    // **************** Section 3: Créer un nouveau stagiaire *****************************
                    if (nomValide && prenomValide)
                    {
                        // Ajouter le stagiaire
                        AjouterStagiaire(nom, prenom, dateDeNaissance.ToString(), sexe, nomProgramme);

                        // Mettre à jour le DataGrid "Stagiaire"
                        Charger_Liste_stagiaire();
                        // Mettre à jour le Data Grid Consulter                     
                        Charger_Liste_stagiaire_Consulter();

                        // Fermer la connexion avec la BD
                        conBD.Close();

                        // Effacer le contenu de tous les champs dans le menu stagiaires
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
                    MessageBox.Show("Please enter a valid input.");
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

            // Mettre à jour le DataGrid "Stagiaire"
            Charger_Liste_stagiaire();
            // Mettre à jour le Data Grid Consulter          
            Charger_Liste_stagiaire_Consulter();

            // Efface le contenu de tous les champs du menu stagiaire
            textBoxNumero.Text = "";
            textBoxPrenom.Text = "";
            textBoxNom.Text = "";
            datePickerDateDeNaissance.SelectedDate = null;
            radioButton_masculin.IsChecked = false;
            radioButton_feminin.IsChecked = false;
            radioButton_autre.IsChecked = false;
            comboBoxProgrammes.SelectedIndex = -1;  // Remet le comboBox à son état initial

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

        // Méthode qui remplit les textBoxs du menu Stagiaire
        // des informations de la ligne sélectionné dans le DataGrid "dgStagiaire"
        private void dgStagiaire_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Récupérer le contenu de la DataGrid
            DataGrid maDataGrid = (DataGrid)sender;

            // Récupérer le contenu de la ligne sélectionnée dans le DataGrid
            DataRowView ligne_selectionnee = maDataGrid.SelectedItem as DataRowView;

            // Variables à déclarer pour pouvoir remplir le radioButton "Sexe"
            String sexe = "";
            bool radioButtonMasculin = (bool)radioButton_masculin.IsChecked;
            bool radioButtonFeminin = (bool)radioButton_feminin.IsChecked;
            bool radioButtonAutre = (bool)radioButton_autre.IsChecked;

            // Valider radioButton sexe 
            if (radioButtonMasculin)
            {
                sexe = "Man";
            }
            else if (radioButtonFeminin)
            {
                sexe = "Woman";
            }
            else if (radioButtonAutre)
            {
                sexe = "Other";
            }

            // Si une ligne est sélectionné dans le DataGrid, on remplit les composants des informations du stagiaire sélectionné
            if (ligne_selectionnee != null)
            {
                // 1-Fill le textBox "Numéro" 
                textBoxNumero.Text = ligne_selectionnee["ID"].ToString();

                // 2-Fill le textBox "Nom"
                textBoxNom.Text = ligne_selectionnee["Last name"].ToString();

                // 3-Fill le textBox "Prénom"
                textBoxPrenom.Text = ligne_selectionnee["First name"].ToString();

                // ******************4-Fill le datePicker "Date de naissance"*********************
                // Source: https://stackoverflow.com/questions/20743121/how-can-i-get-sql-result-into-a-string-variable

                // Query qui cherche la date de naissance du stagiaire
                string query_getDateNaissance = "SELECT date_naissance FROM stagiaire WHERE id_stagiaire = " + ligne_selectionnee["ID"].ToString() + ";";

                // Variable qui va contenir le resultat du query sous fourme de DateTime
                // pour ensuite mettre ce resultat dans le DatePicker "Date de naissance"
                DateTime dateTime;

                // Exécution du query
                using (var command = new SqlCommand(query_getDateNaissance, conBD))
                {
                    // Ouvrir la connexion à la BD
                    conBD.Open();

                    // Source: https://stackoverflow.com/questions/42524639/unable-to-cast-object-of-type-system-datetime-to-type-system-string-c-sharp
                    // Stocker le résultat du query
                    dateTime = (DateTime)command.ExecuteScalar();
          
                }
                // Assigner la date de naissance du stagiaire au DatePicker
                datePickerDateDeNaissance.SelectedDate = dateTime;


                // ******************* 5-Fill le radioButton "Sexe"******************
                // Source: https://stackoverflow.com/questions/20743121/how-can-i-get-sql-result-into-a-string-variable

                // Query qui cherche le sexe du stagiaire
                string query_getSexe = "SELECT sexe_stagiaire FROM stagiaire WHERE id_stagiaire = " + ligne_selectionnee["ID"].ToString() + ";";

                // Variable qui va contenir le sexe du stagiaire
                string query_result_getSexe = "";

                // Exécution du query
                using (var command = new SqlCommand(query_getSexe, conBD))
                {
                    // Stocker le résultat du query
                    query_result_getSexe = (string)command.ExecuteScalar();
                }

                // Assigner le sexe du stagiaire au radioButton "Sexe"
                if (query_result_getSexe == "Man")
                {
                    radioButton_masculin.IsChecked = true;
                }
                else if (query_result_getSexe == "Woman")
                {
                    radioButton_feminin.IsChecked = true;
                }
                else if (query_result_getSexe == "Other")
                {
                    radioButton_autre.IsChecked = true;
                }

                // ****************** 6-Fill le comboBox "Programme" ******************

                // Query pour trouver le id_programme du stagiaire
                string query_getIdProgramme = "SELECT id_programme FROM stagiaire WHERE id_stagiaire = " + ligne_selectionnee["ID"].ToString() + ";";

                // Variable qui va stocker le id programme du stagiaire
                int query_result_getIdProgramme;

                // Exécution du query
                using (var command = new SqlCommand(query_getIdProgramme, conBD))
                {
                    // Stocker le résultat du query
                    query_result_getIdProgramme = (int)command.ExecuteScalar();
                }

                // Query pour trouver le nom du programme à l'aide du ID programme
                string query_getNomProgramme = "SELECT nom_programme FROM programme WHERE id_programme = " + query_result_getIdProgramme.ToString() + ";";

                // Variable qui va stocker le nom du programme du stagiaire
                string query_result_getNomProgramme = "";

                // Exécution du query
                using (var command = new SqlCommand(query_getNomProgramme, conBD))
                {
                    // Stocker le résultat du query
                    query_result_getNomProgramme = (string)command.ExecuteScalar();
                }

                // Set le comboBox avec le nom du programme du stagiaire
                comboBoxProgrammes.SelectedValue = query_result_getNomProgramme;            
            }

            // Méthode qui remplit le datagrid "dtStagiaire" avec le contenu de la table stagiaire de la BD
            Charger_Liste_stagiaire();
        }

        public bool TestStringValide(string s)
        {
            bool NomValide = true;

            foreach (char character in s)
            {
                if (!Char.IsLetter(character))
                {
                    NomValide = false;
                    break;
                }

            }
            return NomValide;
        }

        // Méthode qui modifie le stagiaire sélectionné et le met à jour dans la BD
        private void buttonModifierStagiaire_Click(object sender, RoutedEventArgs e)
        {
            
            // Gestions des exceptions pour s'assurer d'entrer des valeurs valides.
            // Vérifier si tous les caractères sont des lettres seulement
            /// Source: https://stackoverflow.com/questions/1181419/verifying-that-a-string-contains-only-letters-in-c-sharp
            
             // valider le nom
                bool nomValide = true;

                // Vérifier si tous les caractères sont des lettres seulement
                /// Source: https://stackoverflow.com/questions/1181419/verifying-that-a-string-contains-only-letters-in-c-sharp
                foreach (char character in textBoxNom.Text.ToString())
                {
                    if (!Char.IsLetter(character))
                    {
                        nomValide = false;
                        break;
                    }
                }
                
                // Valider le prénom
                bool prenomValide = true;

                // Vérifier si tous les caractères sont des lettres seulement
                /// Source: https://stackoverflow.com/questions/1181419/verifying-that-a-string-contains-only-letters-in-c-sharp
                foreach (char character in textBoxPrenom.Text.ToString())
                {
                    if (!Char.IsLetter(character))
                    {
                        prenomValide = false;
                        break;
                    }
                }
                

            try
            {

                // Si un utilisateur est en train de créé un nouveau stagiaire
                // mais clique sur le bouton modifier
                if (!int.TryParse(textBoxNumero.Text, out int numeroStagiaire) || int.Parse(textBoxNumero.Text) < 0 || textBoxNumero.Text == "")
                {
                    // Mettre la bordure du textbox en rouge pour avertir l'utilisateur
                    textBoxNumero.BorderBrush = Brushes.Red;
                    MessageBox.Show("Please enter a valid intern ID.");
                }
                // #2 Nom
                // Si le textBox nom du stagiaire est vide ou s'il est numerique.
                else if (textBoxNom.Text == "" || int.TryParse(textBoxNom.Text, out int n) || nomValide == false)
                {
                    // Mettre la bordure du textbox en rouge pour avertir l'utilisateur
                    textBoxNom.BorderBrush = Brushes.Red;
                    MessageBox.Show("Please enter a valid intern last name.");
                }
                // #3 Prénom
                // Si le textBox prénom du stagiaire est vide ou s'il est numerique.
                else if (textBoxPrenom.Text == "" || int.TryParse(textBoxPrenom.Text, out int nn) || prenomValide == false)
                {
                    // Mettre la bordure du textbox en rouge pour avertir l'utilisateur
                    textBoxPrenom.BorderBrush = Brushes.Red;
                    MessageBox.Show("Please enter a valid intern last name.");
                }
                else
                {
                    // Mettre à jour la table stagiaire dans la BD 
                    UpdateStagiaire();
                }
            }
            catch (FormatException)
            {
                // Affiche un message d'erreur
                MessageBox.Show("Please enter a valid input.");
            }

            // Mettre à jour les Data Grid
            Charger_Liste_stagiaire();
            Charger_Liste_stagiaire_Consulter();

            // Méthode qui efface le contenu de tous les champs dans le menu stagiaires
            textBoxNumero.Text = "";
            textBoxPrenom.Text = "";
            textBoxNom.Text = "";
            datePickerDateDeNaissance.SelectedDate = null;
            radioButton_masculin.IsChecked = false;
            radioButton_feminin.IsChecked = false;
            radioButton_autre.IsChecked = false;
            comboBoxProgrammes.SelectedIndex = -1;  // Remet le comboBox à son état initial
            textBoxPrenom.BorderBrush = Brushes.LightBlue;
            textBoxNom.BorderBrush = Brushes.LightBlue;

        }

      
        /// <summary>
        /// Remplir le datagrid avec la table stagiaire de la BD
        /// </summary>
        private void Charger_Liste_stagiaire_Consulter()
        {     
                conBD.Close(); //Fermer la connexion
            
                if (comboBoxConsulter.SelectedIndex != -1)
                {
                    //Ma requête
                    string maRequete = "SELECT id_stagiaire,nom_stagiaire, prenom_stagiaire,DATEDIFF(month,date_naissance,GETDATE())/12 AS age_stagiaire,sexe_stagiaire,nom_programme FROM stagiaire JOIN programme p ON p.id_programme=stagiaire.id_programme where p.id_programme= " + comboBoxConsulter.SelectedItem.ToString();

                    //Ma commande
                    SqlCommand cmd = new SqlCommand(maRequete, conBD); //Définir la requête à exécuter sur la BD
                    conBD.Open(); //Ouvrir la connexion
                    SqlDataReader dr = cmd.ExecuteReader();//Lire les enregistrements collectés suite à l'exécution de la requête

                    //Stockage des données lues par DataReader dans DataTable
                    DataTable dt = new DataTable();
                    dt.Load(dr);

                    conBD.Close(); //Fermer la connexion

                    dgSimple.ItemsSource = dt.DefaultView; //Chargement de DataGrid avec les données de la BD
                }
          

        }
        

        /// <summary>
        /// Remplir DataGrid selon la selection dans ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxConsulter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Charger_Liste_stagiaire_Consulter();
            }
            catch(Exception)
            {
                MessageBox.Show("Please select an ID.");
            }
        }

        
    }
}
