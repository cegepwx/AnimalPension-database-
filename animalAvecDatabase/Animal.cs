using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;

namespace animalAvecDatabase
{
    class Animal
    {
        private int id;
        private string typeAnimal;
        private string nom;
        private int age;
        private float poids;
        private string couleur;
        private string proprietaire;
        private int id_owner;
        private MySqlConnection cnn;

        public Animal()
        {
            string connectionString = "server = localhost;database=newprojetanimal;uid=root;pwd=;";
            cnn = new MySqlConnection(connectionString);
        }

        public void afficherTitle()
        {
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            Console.WriteLine("ID |TYPE ANIMAL|   NOM   | AGE | POIDS | COULEUR | PROPRIÉTAIRE |");
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
        }

        //**1- Ajouter un animal **//
        public void ajouterUnAnimal()
        {
            if (totalAnimaux() < 30)
            {
                saisirInformation();
                if (trouverProprietaire()==false)
                {
                    ajouterProprietaire();
                    trouverIdProprietaire();
                }
                insertAnimal();
            }
            else
            {
                Console.WriteLine("Il n'y plus de chambre pour votre animal");
            }
        }

        public void saisirInformation()
        {
            Console.WriteLine("Dans la fonction ajouter un animal");
            Console.WriteLine("Veuillez saisir le type de l'animal:");
            typeAnimal = Console.ReadLine();

            Console.WriteLine("Veuillez saisir le nom de l'animal:");
            nom = Console.ReadLine();

            Console.WriteLine("Veuillez saisir l'age de l'animal:");
            string inputAge = Console.ReadLine();
            while (isNumeral(inputAge) == false)
            {
                Console.WriteLine("L'age n'est pas valide");
                Console.WriteLine("Veuillez saisir l'age de l'animal:");
                inputAge = Console.ReadLine();
            }
            age = int.Parse(inputAge);

            Console.WriteLine("Veuillez saisir le poids de l'animal:");
            string inputWeight = Console.ReadLine();
            while (isNumeral(inputWeight) == false)
            {
                Console.WriteLine("Le poids n'est pas valide");
                Console.WriteLine("Veuillez saisir le poids de l'animal:");
                inputWeight = Console.ReadLine();
            }
            poids = int.Parse(inputWeight);

            Console.WriteLine("Veuillez saisir la couleur de l'animal:");
            couleur = Console.ReadLine();

            while (traiterAjoutAnimal(couleur) != true)
            {
                Console.WriteLine("La couleur n'est pas valide");
                Console.WriteLine("Veuillez saisir la couleur de l'animal:");
                couleur = Console.ReadLine();
            }

            Console.WriteLine("Veuillez saisir le nom du propriétaire de l'animal:");
            proprietaire = Console.ReadLine();
        }

        private void insertAnimal()
        {
            try
            {
                if (cnn.State == System.Data.ConnectionState.Closed)
                {
                    cnn.Open();
                }
                
                string sql = "INSERT INTO animal (type_animal, nom, age, poids, couleure, id_proprietaire)" +
                    "VALUES(@type_animal1, @nom1, @age1, @poids1, @couleur1, @id_owner1);";
                using (MySqlCommand command = new MySqlCommand(sql, cnn))
                {
                    command.Parameters.AddWithValue("@type_animal1", typeAnimal);
                    command.Parameters.AddWithValue("@nom1", nom);
                    command.Parameters.AddWithValue("@age1", age);
                    command.Parameters.AddWithValue("@poids1", poids);
                    command.Parameters.AddWithValue("@couleur1", couleur);
                    command.Parameters.AddWithValue("@id_owner1", id_owner);
                    command.ExecuteNonQuery();
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                systemError(ex);
            }
        }

        public bool trouverProprietaire()
        {
            bool isfinded = false;
            try
            {
                if (cnn.State == System.Data.ConnectionState.Closed)
                {
                    cnn.Open();
                }
                MySqlCommand command = new MySqlCommand("SELECT id, prenom FROM proprietaire;", cnn);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (proprietaire == reader.GetString(1))
                            {
                                id_owner = reader.GetInt32(0);
                                isfinded = true;
                                break;
                            }
                            else continue;
                        }
                    }
                    else isfinded = false;
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                systemError(ex);
            }
            return isfinded;
        }

        public void trouverIdProprietaire()
        {
            try
            {
                if (cnn.State == System.Data.ConnectionState.Closed)
                {
                    cnn.Open();
                }
                string sql = "SELECT id FROM proprietaire WHERE prenom=@prenom2;";
                MySqlCommand command = new MySqlCommand(sql, cnn);
                
                command.Parameters.AddWithValue("@prenom2", proprietaire);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            id_owner = reader.GetInt32(0);
                        }
                    }
                                            
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                systemError(ex);
            }
        }

        public void ajouterProprietaire()
        {
            try
            {
                if (cnn.State == System.Data.ConnectionState.Closed)
                {
                    cnn.Open();
                }
                string sql = "INSERT INTO proprietaire (prenom) VALUES (@prenom1);";
                using (MySqlCommand command = new MySqlCommand(sql, cnn))
                {
                    command.Parameters.AddWithValue("@prenom1", proprietaire);
                    command.ExecuteNonQuery();
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                systemError(ex);
            }
        }

    //**2- Voir la liste de tous les animaux en pension**//
    public void voirListeAnimauxPension()
        {
            afficherTitle();
            try
            {
                if (cnn.State == System.Data.ConnectionState.Closed)
                {
                    cnn.Open();
                }
                MySqlCommand command = new MySqlCommand("SELECT a.id, type_animal, a.nom, age, poids, couleure, p.prenom " +
                    "FROM animal AS a INNER JOIN proprietaire AS p ON p.id=a.id_proprietaire ORDER BY a.id;", cnn);
                cnn.Close();
                voirselecteddata(command);
            }
            catch (Exception ex)
            {
                systemError(ex);
            }
        }

        public void voirselecteddata(MySqlCommand command)
        {
            try
            {
                if (cnn.State == System.Data.ConnectionState.Closed)
                {
                    cnn.Open();
                }
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            id = reader.GetInt32(0);
                            typeAnimal = reader.GetString(1);
                            nom = reader.GetString(2);
                            age = reader.GetUInt16(3);
                            poids = reader.GetFloat(4);
                            couleur = reader.GetString(5);
                            proprietaire = reader.GetString(6);
                            Console.WriteLine("{0,-4}{1,-12}{2,-11}{3,-6}{4,-8}{5,-11}{6,-16}",
                                id, typeAnimal, nom, age, poids, couleur, proprietaire);
                        }
                    }
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                systemError(ex);
            }

        }

        //**3- Voir la liste de tous les propriétaires**//
        public void voirListePropriétaire()
        {
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            Console.WriteLine("| PROPRIÉTAIRE |");
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            try
            {
                if (cnn.State == System.Data.ConnectionState.Closed)
                {
                    cnn.Open();
                }
                MySqlCommand command = new MySqlCommand("SELECT DISTINCT prenom FROM proprietaire AS p INNER JOIN animal AS a ON p.id=a.id_proprietaire;", cnn);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            proprietaire = reader.GetString(0);
                            Console.WriteLine(proprietaire);
                        }
                    }
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                systemError(ex);
            }
        }

        //**4- Voir le nombre total d’animaux en pension**//
        public void voirNombreTotalAnimaux()
        {
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            Console.WriteLine("|  NOMBRE ANIMAUX  |");
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");

            Console.WriteLine(totalAnimaux());
        }

        public int totalAnimaux()
        {
            int totalNumber=0;
            try
            {
                if (cnn.State == System.Data.ConnectionState.Closed)
                {
                    cnn.Open();
                }
                string sql = "SELECT COUNT(*) FROM animal;";
                MySqlCommand command = new MySqlCommand(sql, cnn);
                totalNumber = Convert.ToInt32(command.ExecuteScalar());  // the result of request.//
                cnn.Close();
                return totalNumber;
            }
            catch (Exception ex)
            {
                systemError(ex);
                return totalNumber;
            }
        }

        //**5- Voir le poids total de tous les animaux en pension**//
        public void voirPoidsTotalAnimaux()
        {
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            Console.WriteLine("|  POIDS TOTAL  |");
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");

            try
            {
                float totalPoids = 0;
                if (cnn.State == System.Data.ConnectionState.Closed)
                {
                    cnn.Open();
                }
                string sql = "SELECT poids FROM animal;";
                MySqlCommand command = new MySqlCommand(sql, cnn);
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        totalPoids += reader.GetFloat(0);
                    }
                }
                Console.WriteLine(totalPoids);   
                cnn.Close();
            }
            catch (Exception ex)
            {
                systemError(ex);
            }
        }

        //**6- Voir la liste des animaux d’une couleur **//
        public void extraireAnimauxSelonCouleurs()
        {
            Console.WriteLine("VEUILLEZ SAISIR LA COULEUR DE RECHERCHE");
            string inputColor = Console.ReadLine();
            traiterCouleur(inputColor);
            try
            {
                if (cnn.State == System.Data.ConnectionState.Closed)
                {
                    cnn.Open();
                }
                string sql = "SELECT id, type_animal, nom, couleure FROM animal WHERE couleure=@color1;";
                MySqlCommand command = new MySqlCommand(sql, cnn);
                command.Parameters.AddWithValue("@color1", inputColor);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int resultID = reader.GetInt32(0);
                        string resultType = reader.GetString(1);
                        string resultName = reader.GetString(2);
                        string color = reader.GetString(3);
                        Console.WriteLine("{0,-4}{1,-12}{2,-8}{3,-11}", resultID, resultType, resultName, color);
                    }
                    Console.WriteLine();
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                systemError(ex);
            }
        }

        public void traiterCouleur(string color)
        {
            while (traiterAjoutAnimal(color) != true)
            {
                Console.WriteLine("La couleur n'est pas valide");
                Console.WriteLine("Veuillez saisir la couleur de l'animal:");
                color = Console.ReadLine();
            }
            Console.WriteLine("Dans la fonction voir liste animaux pension");
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            Console.WriteLine("ID |TYPE ANIMAL|  NOM  | COULEUR |");
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
        }

        //**7- Retirer un animal de la liste**//
        public void retirerUnAnimalDeListe()
        {
            Console.WriteLine("VEUILLEZ SAISIR LE ID DE L'ANIMAL");
            string inputId = Console.ReadLine();
            int deletId = Convert.ToInt16(inputId);

            afficherTitle();
           
            try
            {
                if (cnn.State == System.Data.ConnectionState.Closed)
                {
                    cnn.Open();
                }
                string sqltemp = "SELECT a.id, type_animal, a.nom, age, poids, couleure, p.prenom " +
                    "FROM animal AS a INNER JOIN proprietaire AS p ON a.id_proprietaire = p.id WHERE a.id = @id1;";
                MySqlCommand command = new MySqlCommand(sqltemp, cnn);
                command.Parameters.AddWithValue("@id1", deletId);
                command.ExecuteReader();
                cnn.Close();

                voirselecteddata(command);
                confirmerDelete(command, deletId);
            }
            catch (Exception ex)
            {
                systemError(ex);
            }
        }

        public void confirmerDelete(MySqlCommand command, int deletId)
        {
            try
            {
                string choix;
                do
                {
                    Console.WriteLine("Veuillez confirmer de retirer cet animal: O/N");
                    choix = Console.ReadLine();
                    choix = choix.ToLower();
                }
                while (!(choix == "o" || choix == "n"));

                if (cnn.State == System.Data.ConnectionState.Closed)
                {
                    cnn.Open();
                }
                if (choix == "o")
                {
                    string sql = "DELETE FROM animal WHERE id=@id1;";
                    command = new MySqlCommand(sql, cnn);
                    command.Parameters.AddWithValue("@id1", deletId);
                    command.ExecuteReader();
                    cnn.Close();
                    voirListeAnimauxPension();
                }
                else
                {
                    cnn.Close();
                }
            }
            catch (Exception ex)
            {
                systemError(ex);
            }
        }

        //**8- Modifier un animal de la liste**//
        public void modifierListe()
        {
            try
            {
                int animalId = modifierQuelAnimal();

                Console.WriteLine("Nouveau nom de l’animal : ");
                string animalNom = Console.ReadLine();

                if (cnn.State == System.Data.ConnectionState.Closed)
                {
                    cnn.Open();
                }
                string sqltemp = "UPDATE animal SET nom=@nom1 WHERE id=@id1;";
                MySqlCommand command = new MySqlCommand(sqltemp, cnn);
                command.Parameters.AddWithValue("@id1", animalId);
                command.Parameters.AddWithValue("@nom1", animalNom);
                command.ExecuteReader();
                cnn.Close();

                Console.WriteLine("Le nom du pensionnaire {0} a été modifié.", animalId);
            }
            catch (Exception ex)
            {
                systemError(ex);
            }
        }

        public int modifierQuelAnimal()
        {
            Console.WriteLine("Numéro de l’animal à modifier :");
            string inputId = Console.ReadLine();
            while (isNumeral(inputId) == false)
            {
                Console.WriteLine("Le numéro de l’animal n'est pas valide.");
                Console.WriteLine("Numéro de l’animal à modifier : ");
                inputId = Console.ReadLine();
            }
            return Convert.ToInt32(inputId);
        }

        private static bool isNumeral(string input)
        {
            foreach (char ch in input)
            {
                if (ch < '0' && ch > '9')
                {
                    return false;
                }
            }
            return true;
        }

        private bool traiterAjoutAnimal(string inputColor)
        {
            if (inputColor != "rouge" & inputColor != "bleu" & inputColor != "violet")
            {
                return false;
            }
            return true;
        }

        public void systemError(Exception ex)
        {
            Console.WriteLine("Vous avez une problème. Veuillez connecter le resposable. " + ex.Message);
        }
    }
}
