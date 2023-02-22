using System.Text.Encodings.Web;
using Store.Domain.Entities;
using Store.Domain.Enums;

namespace Store.Tests.Entities;

[TestClass]
public class OrderTests
{
    private readonly Customer _customer = new Customer("Eduardo", "email@email.com");
    private readonly Product _product = new Product("Produto 1", 10, true);
    private readonly Discount _discount = new Discount(10, DateTime.Now.AddDays(5));

    [TestMethod]
    [TestCategory("Domain")]
    public void DadoUmNovoPedidoValidoEleDeveGerarUmNumeroCom8Caracteres()
    {
        
        var order = new Order(_customer, 0, _discount);
        Assert.AreEqual(8, order.Number.Length);
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void DadoUmNovoPedidoSeuStatusDeveSerAguardandoPagamento()
    {
        var order = new Order(_customer, 0, _discount);
        Assert.AreEqual(EOrderStatus.WaitingPayment, order.Status);
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void DadoUmPagamentoDoPedidoSeuStatusDeveSerAguardandoEntrega()
    {
        var order = new Order(_customer, 0, null);
        
        order.AddItem(_product, 1);
        order.Pay(10);
        
        Assert.AreEqual(EOrderStatus.WaitingDelivery, order.Status);
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void DadoUmPedidoCanceladoSeuStatusDeveSerCancelado()
    {
        var order = new Order(_customer, 0, null);
        
        order.Cancel();
        
        Assert.AreEqual(EOrderStatus.Canceled, order.Status);
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void DadoUmNovoItemSemProdutoOMesmoNaoDeveSerAdicionado()
    {
        var order = new Order(_customer, 0, null);
        
        order.AddItem(null, 0);
        
        Assert.AreEqual(0, order.Items.Count);
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void DadoUmNovoItemComQuantidadeZeroOuMenorOMesmoNaoDeveSerAdicionado()
    {
        var order = new Order(_customer, 0, null);
        
        order.AddItem(_product, 0);
        
        Assert.AreEqual(0, order.Items.Count);
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void DadoUmNovoPedidoValidoOSeuTotalDeveSer50()
    {
        var order = new Order(_customer, 10, _discount);
        
        order.AddItem(_product, 5);
        
        Assert.AreEqual(50, order.Total());
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void DadoUmDescontoExpiradoOValorDoPedidoDeveSer60()
    {
        var _expiredDiscount = new Discount(10, DateTime.Now.AddDays(-1));
        var order = new Order(_customer, 10, _expiredDiscount);
        
        order.AddItem(_product, 5);
        
        Assert.AreEqual(60, order.Total());
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void DadoUmDescontoInvalidoOValorDoPedidoDeveSer60()
    {
        var order = new Order(_customer, 10, null);
        
        order.AddItem(_product, 5);
        
        Assert.AreEqual(60, order.Total());
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void DadoUmDescontoDe10OValorDoPedidoDeveSer50()
    {
        var order = new Order(_customer, 10, _discount);
        
        order.AddItem(_product, 5);
        
        Assert.AreEqual(50, order.Total());
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void DadoUmaTaxaDeEntregaDe10OValorDoPedidoDeveSer60()
    {
        var order = new Order(_customer, 10, null);
        
        order.AddItem(_product, 5);
        
        Assert.AreEqual(60, order.Total());
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void DadoUmPedidoSemClienteOMesmoDeveSerInvalido()
    {
        var order = new Order(null, 10, null);
        Assert.AreEqual(false, order.IsValid);
    }
}