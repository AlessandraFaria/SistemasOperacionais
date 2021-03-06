using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Barbeiro {
    public class Principal {
        //Trabalho Final Sistemas Operacionais
        //Exercício 6.3
        //Data: 29/05/2018
        //Grupo: Alessandra Faria, Cleuba Alves, Lucas Amancio, Mirella Avelino e Yanna Paula 
        //Descrição: Problema do barbeiro adormecido utilizando monitor.

        static void Main(string[] args) {
            Console.WriteLine("********************************************************");
            Console.WriteLine("Exercício 6.3 - Barbeiro Adormecido com Monitor");
            Console.WriteLine("Grupo: Alessandra Faria Abreu 573831\nCleuba Alves Ribeiro612542\nLucas Amancio Mantini 590982\nMirella Avelino Soares 590983\nYanna Paula Araújo Silva 601282");
            Console.WriteLine("********************************************************");



            Random random = new Random();

            //Cria instância da classe barbearia
            Barbearia barbearia = new Barbearia();

            //Cria instância da classe cliente
            Cliente cliente = new Cliente(barbearia, random);
            //Cria instância da classe barbeiro
            Barbeiro barbeiro = new Barbeiro(barbearia, random);

            // Inicializa todas suas threads
            Thread threadCliente = new Thread(new ThreadStart(cliente.adicionar));
            threadCliente.Name = "Cliente";
            Thread threadBarbeiro = new Thread(new ThreadStart(barbeiro.cortar));
            threadBarbeiro.Name = "Barbeiro";

            //Inicia execução
            threadCliente.Start();
            threadBarbeiro.Start();

            Console.ReadKey();
        }
    }

    //Classe que representa a barbearia
    public class Barbearia {
        private int capacidadeBarbearia = 5;
        private int cadeirasEsperaOcupadas = 0;
        private int barbeiro = 1;

        public int Barbeiro {
            //Representa atividades do barbeiro
            get {
                // Garantir apenas uma thread em execucao
                lock(this) {
                    //Verifica se tem cliente na Barbearia
                    if (cadeirasEsperaOcupadas <= 0 && barbeiro == 1) {
                        //Libera o bloqueio de um objeto e bloqueia o thread atual até que ele precise do bloqueio.
                        Console.WriteLine("\nBarbeiro dormindo...");
                        Monitor.Wait(this);
                    }
                    //Libera um cliente, barbeiro em ação
                    --cadeirasEsperaOcupadas;
                    Console.WriteLine("\nBarbeiro atende cliente. Cortando o cabelo. \nClientes restantes: " + cadeirasEsperaOcupadas);
                    barbeiro = 1;

                    //Notifica thread na fila de espera de uma alteração no estado do objeto bloqueado.
                    Monitor.Pulse(this);
                    return barbeiro;
                }
            }
            //Representa atividades do cliente
            set {
                // Garantir apenas uma thread em execucao
                lock(this) {
                    //Verifica se barbearia está cheia
                    if (cadeirasEsperaOcupadas == capacidadeBarbearia) {
                        //Libera o bloqueio de um objeto e bloqueia o thread atual até que ele precise do bloqueio.
                        Console.WriteLine("\nBarbearia cheia, o cliente foi embora!");
                        Monitor.Wait(this);
                    }

                    //Adiciona um novo cliente a barbearia
                    ++cadeirasEsperaOcupadas;
                    Console.WriteLine("\nUm cliente chegou e aguarda...\nClientes esperando: " + cadeirasEsperaOcupadas);

                    //Caso, haja clientes e o barbeiro desocupado
                    if (cadeirasEsperaOcupadas > 0 && barbeiro == 1) {
                        //Notifica thread na fila de espera de uma alteração no estado do objeto bloqueado.
                        Monitor.Pulse(this);
                    }

                    //Caso Barbeiro esteja ocupado
                    if (barbeiro == 0) {
                        //Libera o bloqueio de um objeto e bloqueia o thread atual até que ele precise do bloqueio.
                        Console.WriteLine("\nBarbeiro ocupado");
                        Monitor.Wait(this);
                    }

                    barbeiro = 1;
                    //Notifica thread na fila de espera de uma alteração no estado do objeto bloqueado.
                    Monitor.Pulse(this);
                }
            }
        }
    }

    // Barbeiro
    public class Barbeiro {
        private Barbearia barber;
        private Random randomSleepTime;

        // Instancia
        public Barbeiro(Barbearia b, Random random) {
            this.barber = b;
            this.randomSleepTime = random;
        }

        // Cortar cabelo
        public void cortar() {
            int barbeiro;
            //O Barbeiro corta cabelo 10 vezes
            for (int i = 0; i <= 10; i++) {
                barbeiro = barber.Barbeiro;
                Thread.Sleep(randomSleepTime.Next(1, 100));
            }

            Console.WriteLine("Fim do dia, a barbearia foi fechada.");
        }

    }

    //Cliente
    public class Cliente {
        private Barbearia barber;
        private Random randomSleepTime;

        //Instancia
        public Cliente(Barbearia b, Random random) {
            this.barber = b;
            this.randomSleepTime = random;
        }

        //Adicionar cliente na barbearia
        public void adicionar() {
            //Chegam 10 clientes
            for (int i = 0; i <= 10; i++) {
                Thread.Sleep(randomSleepTime.Next(1, 80));
                barber.Barbeiro = i;
            }
        }
    }
}
