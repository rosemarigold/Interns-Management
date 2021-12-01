using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_App_Bureau
{
    public class Stagiaire
    {
        /// <summary>
        /// Propriété pour le numéro du Stagiaire
        /// </summary>
        public int NumeroStagiaire { get; set; }

        /// <summary>
        /// Propriété pour le nom du Stagiaire
        /// </summary>
        public string NomStagiaire { get; set; }

        /// <summary>
        /// Propriété pour le nom du Stagiaire
        /// </summary>
        public int AgeStagiaire { get; set; }


        /// <summary>
        /// Propriété pour le prénom du Stagiaire
        /// </summary>
        public string PrenomStagiaire { get; set; }

        /// <summary>
        /// Propriété pour la date de naissance du Stagiaire
        /// </summary>
        public DateTime DateNaissanceStagiaire { get; set; }

        /// <summary>
        /// Propriété pour le genre (sexe) du Stagiaire
        /// </summary>
        public string SexeStagiaire { get; set; }

        /// <summary>
        /// Propriété pour le programme du Stagiaire
        /// </summary>
        public string NomProgramme { get; set; }

        /// <summary>
        /// Constructeur qui prend en paramètre le numéro, le nom
        /// la date de naissance, le sexe et le programme du stagiaire
        /// </summary>
        /// <param name="numero"></param>
        /// <param name="nom"></param>
        /// <param name="prenom"></param>
        /// <param name="dateNaissance"></param>
        /// <param name="sexe"></param>
        /// <param name="programme"></param>
        public Stagiaire(int numero, string nom, string prenom, DateTime dateNaissance, string sexe, string nomProgramme) 
        {
            NumeroStagiaire = numero;
            NomStagiaire = nom;
            PrenomStagiaire = prenom;
            DateNaissanceStagiaire = dateNaissance;
            SexeStagiaire = sexe;
            AgeStagiaire = CalculerAgeStagiaire(dateNaissance);
            NomProgramme = nomProgramme;
        }

        /// <summary>
        /// Fonction qui calcule l'âge du stagiaire
        /// </summary>
        /// <param name="dateNaissance"></param>
        /// <returns></returns>
        public int CalculerAgeStagiaire(DateTime dateNaissance)
        {
            int age = DateTime.Today.Year - dateNaissance.Year;
            return age;
        }

        /// <summary>
        /// Fonction qui compare deux programmes avec le numéro du programme.
        /// </summary>
        /// <param name="numero"></param>
        /// <returns>bool</returns>
        public bool ComparerStagiaires(int numero)
        {
            return this.NumeroStagiaire == numero;
        }
    

    }
}
