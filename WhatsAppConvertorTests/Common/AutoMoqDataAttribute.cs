using AutoFixture;
using AutoFixture.Xunit2;

namespace WhatsAppConvertorTests.Common
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {

        public AutoMoqDataAttribute() : base(FixtureFactory.Create)
        {
        }

        public AutoMoqDataAttribute(IFixture fixture) : base(() => fixture)
        {
        }

        public AutoMoqDataAttribute(IFixture fixture, Type customizationType) : base(() => AddCustomizationsToFixtures(fixture, new Type[] { customizationType }))
        {
        }

        public AutoMoqDataAttribute(IFixture fixture, Type[] customizationTypes) : base(() => AddCustomizationsToFixtures(fixture, customizationTypes))
        {
        }

        public AutoMoqDataAttribute(Type customizationType) : base(() => CreateFixtureWithCustomizations(new Type[] { customizationType }))
        {
        }            

        public AutoMoqDataAttribute(Type[] customizationTypes) : base(() => CreateFixtureWithCustomizations(customizationTypes))
        {
        }        

        private static IFixture CreateFixtureWithCustomizations(Type[] customizationTypes)
        {
            IFixture fixture = FixtureFactory.Create();
            return AddCustomizationsToFixtures(fixture, customizationTypes);
        }

        private static IFixture AddCustomizationsToFixtures(IFixture fixture, Type[] customizationTypes)
        {
            foreach (Type customizationType in customizationTypes)
            {
                if (!(Activator.CreateInstance(customizationType) is ICustomization customization))
                {
                    throw new ArgumentException("type is not an ICustomization");
                }
                fixture.Customize(customization);
            }

            return fixture;
        }
    }
}
