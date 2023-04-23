using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;

namespace WhatsAppConvertorTests.Common
{
    public static class FixtureFactory
    {
        public static IFixture Create()
        {
            IFixture fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            RegisterFixtures();

            return fixture;

            void RegisterFixtures()
            {
                RegisterProblemDetails();

                void RegisterProblemDetails()
                {
                    fixture.Register(() =>
                    {
                        Mock<ProblemDetailsFactory> problemDetailsFactoryMock = new();

                        problemDetailsFactoryMock
                            .Setup(f => f.CreateProblemDetails(
                                It.IsAny<HttpContext>(),
                                It.IsAny<int?>(),
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>())
                            )
                            .Returns((HttpContext httpC, int? statusCode, string title, string type, string detail, string instance) =>
                            {
                                return new ProblemDetails()
                                {
                                    Title = title,
                                    Type = type,
                                    Status = statusCode,
                                    Detail = detail,
                                    Instance = instance
                                };
                            });

                        return problemDetailsFactoryMock.Object;
                    });
                }
            }
        }
    }
}
