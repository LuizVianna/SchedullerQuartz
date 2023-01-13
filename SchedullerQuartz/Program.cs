using System;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;


    public class Program
    {
        private static void Main(string[] args)
        {
            Run().GetAwaiter().GetResult();
        }

        private static async Task Run()
        {
            try
            {
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };

                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await factory.GetScheduler();

                //agendador iniciado
                await scheduler.Start();

                //setar configurações (job e trigger), na prática isso é montado dinamicamente via configurações
                IJobDetail job = JobBuilder.Create<Teste1Job>()
                    .WithIdentity("Teste1Job", "groupTeste1")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("triggerTeste1", "groupTeste1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(5)
                        .RepeatForever())
                    .Build();

                await scheduler.ScheduleJob(job, trigger);

                //infinito
                await Task.Delay(-1);
                await scheduler.Shutdown();

            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }

    }

    //Nossa tarefa teste, tem por objetivo printar no console "Hello" a cada 5 segundos
    public class Teste1Job : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync($"Hello! ¬¬ {DateTime.Now.ToString("hh:mm:ss")}");
        }
    }

