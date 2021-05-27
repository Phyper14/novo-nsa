using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace DIO
{
    class Program
    {

        static void Main(string[] args)
        {
            List<Aluno> alunos = new List<Aluno>();
            string optionUser = getOptionUser();
            int indexAluno = 1;
            bool controleBanco = false;

            while (optionUser.ToUpper() != "X")
            {
                switch (optionUser)
                {
                    case "1":
                        Console.WriteLine("Informe o nome do aluno: ");
                        Aluno aluno = new Aluno();
                        string localName = Console.ReadLine();
                        if (AlunoExist(alunos, localName))
                        {
                            Console.WriteLine("Aluno já existe");
                            optionUser = "x";
                        }
                        else
                        {
                            aluno.Name = localName;
                            Console.WriteLine("Insira a nota do aluno: ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal nota) && nota <= 10)
                            {
                                aluno.Note = nota;
                                aluno.SetConceito(CalcularConceito((double)nota));
                            }
                            else
                            {
                                throw new ArgumentException("Valor deve ser decimal");
                            }

                            alunos.Add(aluno);
                            indexAluno++;

                            Console.WriteLine();
                            Console.WriteLine($"Aluno >{aluno.Name}< Cadastrado com sucesso");
                        }
                        break;

                    case "2":
                        Console.Write("Nome do aluno a ser removido -> ");
                        string name = Console.ReadLine();
                        if (AlunoExist(alunos, name))
                        {
                            for (int i = 0; i < alunos.Count; i++)
                            {
                                if (alunos[i].Name == name)
                                {
                                    alunos.Remove(alunos[i]);
                                }
                            }
                        }
                        break;

                    case "3":
                        Conceito conceitoAluno;
                        foreach (var ser in alunos)
                        {
                            if (!string.IsNullOrEmpty(ser.Name))
                            {
                                conceitoAluno = ser.GetConceito();
                                Console.WriteLine($"ALUNO: {ser.Name} - NOTA: {ser.Note} - Conceito: {conceitoAluno}");
                            }
                        }
                        break;

                    case "4":
                        double soma = 0;
                        int numeroAlunos = 0;

                        foreach (var ser in alunos)
                        {
                            if (!string.IsNullOrEmpty(ser.Name))
                            {
                                soma += (double)ser.Note;
                                numeroAlunos++;
                            }
                        }

                        double media = soma / numeroAlunos;
                        Conceito conceitoGeral = CalcularConceito(media);

                        Console.WriteLine($"Média: {media} - Conceito: {conceitoGeral}");
                        break;

                    case "5":
                        using (Stream acesso = new FileStream("data.json", FileMode.Create))
                        {
                            DataContractJsonSerializer converter = new DataContractJsonSerializer(typeof(List<Aluno>));
                            converter.WriteObject(acesso, alunos);
                        }
                        Console.WriteLine("O Backup foi salvo.");
                        break;

                    case "6":
                        using (Stream acesso = new FileStream("data.json", FileMode.Open))
                        {
                            DataContractJsonSerializer converter = new DataContractJsonSerializer(typeof(List<Aluno>));
                            List<Aluno> banco = (List<Aluno>)converter.ReadObject(acesso);
                            for (int i = 0; i < banco.Count; i++)
                            {
                                Aluno xAluno = banco[i];
                                xAluno.SetConceito(CalcularConceito((double)xAluno.Note));
                                banco.RemoveAt(0);
                                banco.Add(xAluno);
                            }
                            if (alunos.Count > 0 && !controleBanco)
                            {
                                Console.WriteLine("Quer sobrescrever os dados atuais?");
                                Console.WriteLine("1- Sim");
                                Console.WriteLine("2- Não");
                                Console.WriteLine("3- Cancelar");
                                Console.Write("->");
                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        alunos = banco;
                                        controleBanco = true;
                                        break;

                                    case "2":
                                        foreach (Aluno bAluno in banco)
                                        {
                                            alunos.Add(bAluno);
                                        }
                                        controleBanco = true;
                                        break;

                                    case "3":
                                        optionUser = "x";
                                        break;

                                    default:
                                        throw new Exception("Erro de leitura, valor invalido.");
                                }
                            }
                            else if (!controleBanco)
                            {
                                alunos = banco;
                                controleBanco = true;
                            }
                            else
                            {
                                Console.WriteLine("O Backup já foi carregado.");
                            }
                            Console.WriteLine("Backup carregado com sucesso.");
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();

                }
                optionUser = getOptionUser();
            }
        }

        private static bool AlunoExist(List<Aluno> alunos, string name)
        {
            for (int i = 0; i < alunos.Count; i++)
            {
                if (alunos[i].Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        private static Conceito CalcularConceito(double media)
        {
            Conceito conceitoGeral;

            if (media < 3)
            {
                conceitoGeral = Conceito.E;
            }
            else if (media < 5)
            {
                conceitoGeral = Conceito.D;
            }
            else if (media < 7)
            {
                conceitoGeral = Conceito.C;
            }
            else if (media < 9)
            {
                conceitoGeral = Conceito.B;
            }
            else
            {
                conceitoGeral = Conceito.A;
            }

            return conceitoGeral;
        }

        private static string getOptionUser()
        {
            Console.WriteLine();
            Console.WriteLine("Informe a ação");
            Console.WriteLine("1- Inserir aluno");
            Console.WriteLine("2- Remover aluno");
            Console.WriteLine("3- Listar alunos");
            Console.WriteLine("4- Calcular média geral");
            Console.WriteLine("5- Salvar");
            Console.WriteLine("6- Carregar");
            Console.WriteLine("X- Sair");
            Console.Write("-> ");

            string optionUser = Console.ReadLine();
            Console.WriteLine();
            return optionUser;
        }
    }
}