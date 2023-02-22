using Store.Domain.Entities;
using Store.Domain.Queries;

namespace Store.Tests.Queries;

[TestClass]
public class ProductQueriesTests
{
    private IList<Product> _products;

    public ProductQueriesTests()
    {
        _products = new List<Product>();
        _products.Add(new Product("Produto 01", 10, true));
        _products.Add(new Product("Produto 02", 20, true));
        _products.Add(new Product("Produto 03", 30, true));
        _products.Add(new Product("Produto 04", 40, false));
        _products.Add(new Product("Produto 05", 50, false));
    }
    
    [TestMethod]
    [TestCategory("Queries")]
    public void DadoAConsultaDeProdutosAtivosDeveRetornar3()
    {
        var result = _products.AsQueryable().Where(ProductQueries.GetActiveProduct());
        Assert.AreEqual(result.Count(), 3);
    }
    
    [TestMethod]
    [TestCategory("Queries")]
    public void DadoAConsultaDeProdutosInativosDeveRetornar2()
    {
        var result = _products.AsQueryable().Where(ProductQueries.GetInactiveProducts());
        Assert.AreEqual(result.Count(), 2);
    }
}