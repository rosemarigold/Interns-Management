using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_App_Bureau
{
    /*
     Classe Programme avec ses attributs et ses fonctions
     */ 
    public class Programme
    {
        /// <summary>
        /// Propriété pour le numéro du Programme
        /// </summary>
        public int NumeroProgramme { get; set; }

        /// <summary>
        ///  Propriété pour le nom du Programme
        /// </summary>
        public string NomProgramme { get; set; }

        /// <summary>
        /// Propriété pour la durée du Programme
        /// </summary>
        public int DureeProgramme { get; set; }

        /// <summary>
        /// Propriété pour la liste des stagiaires du Programme
        /// </summary>
        public List<Stagiaire> ListeStagiaire { get; set; }

        public Programme() { }
        /// <summary>
        /// Constructeur qui prend en paramètre le numéro, le nom et la durée du programme
        /// </summary>
        /// <param name="numero"></param>
        /// <param name="nom"></param>
        /// <param name="duree"></param>
        public Programme(int numero, string nom, int duree)
        {
            NumeroProgramme = numero;
            NomProgramme = nom;
            DureeProgramme = duree;
            ListeStagiaire = new List<Stagiaire>();//Bruno: J'ai initialiser ta lister sinon elle ne va pas fonctionner
        }
     
        /// <summary>
        ///Methode pour ajouter un stagiaire dans la liste des stagiaires du Programme
        /// </summary>
        /// <param name="numero"></param>
        /// <param name="nom"></param>
        /// <param name="prenom"></param>
        /// <param name="dateNaissance"></param>
        /// <param name="sexe"></param>
        /// <param name="nomProgramme"></param>
        public void AjoutStagiaireProgramme(int numero, string nom, string prenom, DateTime dateNaissance, string sexe, string nomProgramme)
        {
            this.ListeStagiaire.Add(new Stagiaire(numero, nom, prenom,dateNaissance,sexe, nomProgramme));//Bruno: pourquoi le stagiaire a un attribut nom programme
        }

        /// <summary>
        /// Methode permettant l'affichage de la liste des stagiaires du programme
        /// </summary>
        /// <param name="P"></param>
        public void AfficherListeStagiaire()
        {
            foreach(Stagiaire stagiaire in ListeStagiaire)
            {
                Console.WriteLine(stagiaire.ToString());
            }
        }

        /// <summary>
        /// Fonction qui compare deux programmes avec le nom du programme.
        /// </summary>
        /// <param name="nom"></param>
        /// <returns>bool</returns>
        public bool ComparerProgrammes(string nom)
        {
            return this.NomProgramme == nom;
        }


        /// <summary>
        /// Fonction qui compare deux programmes avec le numéro du programme.
        /// </summary>
        /// <param name="numero"></param>
        /// <returns>bool</returns>
        public bool ComparerProgrammes(int numero)
        {
            return this.NumeroProgramme == numero;
        }
    }
}
