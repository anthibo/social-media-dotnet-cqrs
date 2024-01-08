using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;

namespace Post.Cmd.Infrastructure.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new();
        public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
        {
            if (_handlers.ContainsKey(typeof(T)))
            {
                throw new IndexOutOfRangeException($"Handler for {typeof(T)} already registered.");
            }

            _handlers.Add(typeof(T), command => handler((T)command));
        }

        public async Task SendAsync(BaseCommand command)
        {
            if (!_handlers.ContainsKey(command.GetType()))
            {
                throw new ArgumentException($"Handler for {command.GetType()} not found.");
            }

            _handlers.TryGetValue(command.GetType(), out var handler);
            await handler(command);
        }
    }
}