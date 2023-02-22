using Flunt.Notifications;
using Store.Domain.Commands;
using Store.Domain.Commands.Interfaces;
using Store.Domain.Entities;
using Store.Domain.Handlers.Interface;
using Store.Domain.Repositories;
using Store.Domain.Utils;

namespace Store.Domain.Handlers;

public class OrderHandler : Notifiable<Notification>, IHandler<CreateOrderCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IDeliveryFeeRepository _deliveryFeeRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderHandler(
        ICustomerRepository customerRepository,
        IDeliveryFeeRepository deliveryFeeRepository,
        IDiscountRepository discountRepository,
        IProductRepository productRepository,
        IOrderRepository orderRepository)
    {
        _customerRepository = customerRepository;
        _deliveryFeeRepository = deliveryFeeRepository;
        _discountRepository = discountRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public ICommandResult Handle(CreateOrderCommand command)
    {
        //Fail Fast Validation
        command.Validate();
        if (!command.IsValid)
            return new GenericCommandResult(false, "Pedido inválido", command.Notifications);

        //Recupera o cliente
        var customer = _customerRepository.Get(command.Customer);

        //Calcula a taxa de entrega
        var deliveryFee = _deliveryFeeRepository.Get(command.ZipCode);
        
        //Obtém o cupom de desconto
        var discount = _discountRepository.Get(command.PromoCode);
        
        //Gera o pedido
        var products = _productRepository.Get(ExtractGuids.Extract(command.Items)).ToList();
        var order = new Order(customer, deliveryFee, discount);
        foreach (var item in command.Items)
        {
            var product = products.Where(x => x.Id == item.Product).FirstOrDefault();
            order.AddItem(product, item.Quantity);
        }
        
        //Agrupa as notificações
        AddNotifications(order.Notifications);
        
        //Verifica se tudo deu certo
        if (!IsValid)
            return new GenericCommandResult(false, "Falha ao gerar pedido", Notifications);
        
        //Retorna o resultado
        _orderRepository.Save(order);
        return new GenericCommandResult(true, $"Pedido {order.Number} gerado com sucesso", order);
    }
}