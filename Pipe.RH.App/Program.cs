using Pipe.RH.Application.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pipe.RH.App
{
    class Program
    {
        private static ConcurrentDictionary<Guid, Object> ThreadPorId = new ConcurrentDictionary<Guid, object>();

        static void Main(string[] args)
        {
            List<teste> ids = new List<teste>();

            var id3 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id1 = Guid.NewGuid();

            for (int i = 0; i < 4; i++)
            {
                ids.Add(new teste() { id = id1, numero = i });
                ids.Add(new teste() { id = id2, numero = i });
                ids.Add(new teste() { id = id3, numero = i });
            }

            List<Task> tasks = new List<Task>();

            foreach (var id in ids)
            {
                tasks.Add(Task.Run(() =>
                {
                    Console.WriteLine($"Chegou: {id.numero} {id.id}, {DateTime.Now.ToLongTimeString()}");
                    if (ThreadPorId.ContainsKey(id.id))
                        ExcecuteLook(id);

                    else
                    {
                        var obj = new Object();

                        if (ThreadPorId.TryAdd(id.id, obj))
                            Console.WriteLine($"Adicionou {obj.GetHashCode()} para {id.numero} {id.id}");
                        ExcecuteLook(id);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());


            Console.ReadLine();
            //HoleriteService holeriteService = new HoleriteService();holeriteService.ExtrairTextoPDF(@"D:\");
        }

        public static void ExcecuteLook(teste id)
        {
            try
            {
                lock (ThreadPorId[id.id])
                {
                    Console.WriteLine($"Iniciando: {id.numero} {id.id}, {DateTime.Now.ToLongTimeString()}");
                    Thread.Sleep(500);
                    Console.WriteLine($"Tratado: {id.numero} {id.id}, {DateTime.Now.ToLongTimeString()}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                //var removido = new Object();
                //if (ThreadPorId.TryRemove(id.id, out removido))
                //    Console.WriteLine($"Removeu: {removido.GetHashCode()} para {id.numero} {id.id}");

                //else
                //    Console.WriteLine($"Não rolou: {id.numero} {id.id}");
            }
        }
    }

    class teste
    {
        public Guid id { get; set; }
        public int numero { get; set; }
    }
}