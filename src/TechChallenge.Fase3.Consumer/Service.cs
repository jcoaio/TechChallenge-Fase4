using TechChallenge.Fase3.Consumer.ContatoServices;

namespace TechChallenge.Fase3.Consumer
{
    public class Service(InserirContato inserirContatoWorker, EditarContato editarContatoWorker, RemoverContato removerContatoWorker) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<Task> tasks = new Task[]
                    {
                        editarContatoWorker.ExecuteAsync(cancellationToken),
                        inserirContatoWorker.ExecuteAsync(cancellationToken),
                        removerContatoWorker.ExecuteAsync(cancellationToken)
                    }
                   .Select(task => task.ContinueWith(task =>
                   {
                       if (task.IsFaulted)
                       {
                           cancellationToken.ThrowIfCancellationRequested();
                       }
                   }));

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }
    }
}
