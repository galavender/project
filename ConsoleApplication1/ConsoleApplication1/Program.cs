﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApplication1
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Initialisation du dictionnaire Personne
            var listePersonne = new Dictionary<string, Personnes>();
            InitPersonne(ref listePersonne);

            //Initialisation du Data Access Layer
            var Genomica = new DAL(listePersonne);
            Genomica.ChargerDonnées();                          //Chargement des données contenues dans le fichier données.txt
            SortedDictionary<string, Taches> ActiAne = new SortedDictionary<string, Taches>();

            //Initialisation des activités annexes
            InitActivitésAnnexes(ref ActiAne);

            while (true)
            {
                Console.ReadKey();
                Console.Clear();
                Console.WriteLine("Quel action voulez-vous effectuer ? \n r : resultats\n a : Ajouter une activité annexe\n ");
                switch (Console.ReadLine())                                     //Choix des actions à effectuer
                {
                    case "r":
                        resultat(Genomica, listePersonne);                      //Affichage de statistique sur les versions et les personnes qui travaillent dessus
                        break;
                    case "a":
                        InitActivitésAnnexes(ref ActiAne);                      //Enregistrement d'activités annexes
                        break;
                    default:
                        Console.WriteLine("Ce n'est pas une action, appuyez sur une touche");
                        break;
                }
            }
        }

        public static void InitPersonne(ref Dictionary<string, Personnes> listePersonne)
        {
            //Génération des instances personnes d'après fichier dans le dictionnaire associé
            string chemin = @"..\..\Personne.txt";
            int compteur = 0;
            using (StreamReader str = new StreamReader(chemin,System.Text.Encoding.Default))
            {
                string ligne;
                while ((ligne = str.ReadLine()) != null) //Boucle tant qu'il y a des lignes à lire
                {
                    compteur++;
                    var tab = ligne.Split('\t');
                    try
                    {
                        //Initialise une instance de Personne
                        var Personne = new Personnes
                        {
                            Code = tab[0],
                            Prenom = tab[1],
                            Nom = tab[2],
                            Métier = (CodeMetiers)Enum.Parse(typeof(CodeMetiers), tab[3])
                        };
                        //Ajout de l'instance DonnéesGestionTaches à la collection de données
                        listePersonne.Add(Personne.Code, Personne);
                    }
                    catch (FormatException)
                    {
                        //Lève une exception si le format des données du fichier n'est pas bon.
                        throw new FormatException("Une erreur de format a été identifié dans le fichier de données à la ligne");
                    }
                }
            }
        }

        static void InitActivitésAnnexes(ref SortedDictionary<string, Taches> ActiAne)
        {
            bool PlusDActiAnn = false;

            string libellé = string.Empty;

            while (!PlusDActiAnn)
            {
                Console.WriteLine("Veuillez saisir le code d'une activité annexe :");
                string code = Console.ReadLine();
                if (!ActiAne.ContainsKey(code))
                {
                    Console.WriteLine("Veuillez saisir le nom de l'activité annexe correspondante:");
                    libellé = Console.ReadLine();
                    ActiAne.Add(code, new Taches { Libellé = libellé });                //Ajout de l'activité dans le dictionnaire
                    }
                else
                {
                    Console.WriteLine("Ce code existe déjà, veuillez saisir un code différent");
                }
                Console.WriteLine("Voulez-vous saisir une autre activité annexe ? o/n");
                if (Console.ReadLine() == "n")
                    PlusDActiAnn = true;
            }

            Console.Clear();                                                            //Affichage des activités annexes triées par code
            foreach (var a in ActiAne)
            {
                Console.WriteLine("Activité n°{0} : {1}", a.Key, a.Value.Libellé);
            }

        }


        static void resultat(DAL Genomica, Dictionary<string, Personnes> listePersonne)
        {
            bool verif = false;
            Console.WriteLine("Quel resultat voulez-vous ?\n p : durées de travail réalisée et restante d’une personne sur une version\n n : nombre de jours et le pourcentage d’avance ou de retard sur une version \n d : durées totales de travail réalisées sur la production d’une version, pour chaque activité");
            switch (Console.ReadLine())
            {
                case "d":               
                    while (!verif)
                    {
                        Console.WriteLine("Sur quelle version");
                        string version = Console.ReadLine();
                        if (version == "1.00" || version == "2.00")
                        {                                                       //Affichage des durées de travail réalisées sur une version par activité
                            Console.WriteLine(Results.TotalTravailRéa(version, Genomica));      
                            verif = true;
                        }
                        else
                            Console.WriteLine("Cette version n'existe pas");
                    }
                    break;
                case "n":
                    while (!verif)
                    {
                        Console.WriteLine("Sur quelle version");
                        string version = Console.ReadLine();
                        if (version == "1.00" || version == "2.00")
                        {                                                           //Affichage du nombre de jours d'avance ou de retard sur une version
                            Console.WriteLine(Results.RetardVersion(version, Genomica));        
                            verif = true;
                        }
                        else
                            Console.WriteLine("Cette version n'existe pas");
                    }
                    break;
                case "p":
                    while (!verif)
                    {
                        Console.WriteLine("Quelles sont les initiales de la personne?");
                        string initial = Console.ReadLine().ToUpper();
                        if (listePersonne.ContainsKey(initial))
                        {
                            while (!verif)
                            {
                                Console.WriteLine("Sur quelle version");
                                string version = Console.ReadLine();
                                if (version == "1.00" || version == "2.00")
                                {                               //Affichage des durées de travail réalisée et restante qur une version par une personne
                                    Console.WriteLine(Results.DuréeTravail(listePersonne[initial], version, Genomica));
                                    verif = true;
                                }
                                else
                                    Console.WriteLine("Cette version n'existe pas");
                            }
                        }
                        else
                            Console.WriteLine("Cette personne n'est pas dans la base de données");
                    }
                    break;
                default:
                    Console.WriteLine("Ce n'est pas un résultat, appuyez sur une touche");
                    break;
            }
        }
    }
}
