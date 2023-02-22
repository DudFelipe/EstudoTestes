using Store.Domain.Commands.Interfaces;

namespace Store.Domain.Handlers.Interface;

public interface IHandler<T> where T : ICommand
{
    ICommandResult Handle(T command);
}