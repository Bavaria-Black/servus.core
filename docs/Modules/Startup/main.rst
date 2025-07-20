Application Startups
====================

The AppBuilder in Servus.Core simplifies and streamlines the process of
setting up .NET applications by providing a fluent API for common startup
tasks.


Quick Start
-----------

Your **Program.cs** can look as simple as the following:


.. code-block:: csharp

   using CServus.Core;

   var app = AppBuilder
        .Create()
        .WithSetup<GrpcSetupContainer>()
        .Build();

   await app.RunAsync();

The main setup of your DI code can be done in one or more
**SetupContainer**. Those container provide easy to understand
strucutring of your code and allow also for easy re-usability in *UnitTests*.
Implemenation of the *GrpcSetupContainer*:

.. code-block:: csharp

    public class GrpcSetupContainer : ApplicationSetupContainer<WebApplication>, IServiceSetupContainer
    {
        public void SetupServices(IServiceCollection services, IConfiguration configuration)
        {
                services.AddGrpc();
                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen();
        }

        protected override void SetupApplication(WebApplication app)
        {
            app.UseGrpcWeb();

            app.UseSwagger().UseSwaggerUI();

            app.MapGrpcService<GreeterService>()
                .EnableGrpcWeb();
        }
    }
