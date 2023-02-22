using Store.Domain.Commands;
using Store.Domain.Handlers;
using Store.Domain.Repositories;
using Store.Tests.Repositories;

namespace Store.Tests.Handlers;

[TestClass]
public class OrderHandlerTests
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IDeliveryFeeRepository _deliveryFeeRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    OrderHandler handler;
    
    public OrderHandlerTests()
    {
        _customerRepository = new FakeCustomerRepository();
        _deliveryFeeRepository = new FakeDeliveryFeeRepository();
        _discountRepository = new FakeDiscountRepository();
        _productRepository = new FakeProductRepository();
        _orderRepository = new FakeOrderRepository();
        
        handler = new OrderHandler(
            _customerRepository,
            _deliveryFeeRepository,
            _discountRepository,
            _productRepository,
            _orderRepository);
    }

    [TestMethod]
    [TestCategory("Handlers")]
    public void DadoUmClienteInexistenteOPedidoNaoDeveSerGerado()
    {
        var command = new CreateOrderCommand();
        command.Customer = "00000000000";
        command.ZipCode = "04564050";
        command.PromoCode = "12345678";
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
        command.Validate();
        
        Assert.AreEqual(command.IsValid, false);
    }
    
    [TestMethod]
    [TestCategory("Handlers")]
    public void DadoUmCepInvalidoOPedidoDeveSerGeradoNormalmente()
    {
        var command = new CreateOrderCommand();
        command.Customer = "43926715855";
        command.ZipCode = "00000000";
        command.PromoCode = "12345678";
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
        command.Validate();

        handler.Handle(command);
        Assert.AreEqual(handler.IsValid, true);
    }
    
    [TestMethod]
    [TestCategory("Handlers")]
    public void DadoUmPromocodeInexistenteOPedidoDeveSerGeradoNormalmente()
    {
        var command = new CreateOrderCommand();
        command.Customer = "43926715855";
        command.ZipCode = "";
        command.PromoCode = "12345678";
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
        command.Validate();

        handler.Handle(command);
        Assert.AreEqual(handler.IsValid, true);
    }
    
    [TestMethod]
    [TestCategory("Handlers")]
    public void DadoUmPedidoSemItensOMesmoNaoDeveSerGerado()
    {
        var command = new CreateOrderCommand();
        command.Customer = "43926715855";
        command.ZipCode = "04564050";
        command.PromoCode = "12345678";
        command.Validate();
        
        Assert.AreEqual(command.IsValid, false);
    }
    
    [TestMethod]
    [TestCategory("Handlers")]
    public void DadoUmComandoInvalidoOPedidoNaoDeveSerGerado()
    {
        var command = new CreateOrderCommand();
        command.Customer = "";
        command.ZipCode = "04564050";
        command.PromoCode = "12345678";
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
        command.Validate();
        
        Assert.AreEqual(command.IsValid, false);
    }
    
    [TestMethod]
    [TestCategory("Handlers")]
    public void DadoUmComandoValidoOPedidoDeveSerGerado()
    {
        var command = new CreateOrderCommand();
        command.Customer = "43926715855";
        command.ZipCode = "04564050";
        command.PromoCode = "12345678";
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
        command.Validate();

        handler.Handle(command);
        Assert.AreEqual(handler.IsValid, true);
    }
}