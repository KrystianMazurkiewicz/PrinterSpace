global using Xunit;
using BachelorsProject.DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;


namespace WebApiIntegrationTesting.IntegrationTests.ControllerTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<ISystemRepository> IsystemRepositoryMock { get; set; }

        public CustomWebApplicationFactory()
        {
            IsystemRepositoryMock = new Mock<ISystemRepository>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(IsystemRepositoryMock.Object);
            });
        }
    }
}