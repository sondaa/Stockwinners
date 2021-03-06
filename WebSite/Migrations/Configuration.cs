namespace WebSite.Migrations
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebSite.Database;
    using WebSite.Models;
    using WebSite.Models.Data.Picks;

    internal sealed class Configuration : DbMigrationsConfiguration<DatabaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DatabaseContext context)
        {
            // This method will be called after migrating to the latest version.

            // Ensure default subscriptions are available
            Configuration.SeedSubscriptionTypesAndFrequencies(context);

            // Ensure default roles are available
            Configuration.SeedRoles(context);

            // Ensure countries are available
            Configuration.SeedCountries(context);

            Configuration.SeedAdministrators(context);
        }

        private static void SeedAdministrators(DatabaseContext context)
        {
            StockwinnersMember ameen = new StockwinnersMember() { EmailAddress = "ameen.tayyebi@gmail.com", FirstName = "Ameen", LastName = "Tayyebi", IsLegacyMember = false, Password = WebSite.Infrastructure.MembershipProvider.HashPassword("madmoney") };
            StockwinnersMember mehdi = new StockwinnersMember() { EmailAddress = "s.mehdi.ghaffari@gmail.com", FirstName = "Mehdi", LastName = "Ghaffari", IsLegacyMember = false, Password = WebSite.Infrastructure.MembershipProvider.HashPassword("madmoney") };
            StockwinnersMember dayee = new StockwinnersMember() { EmailAddress = "seyed@stockwinners.com", FirstName = "Mohammad", LastName = "Mohammadi", IsLegacyMember = false, Password = WebSite.Infrastructure.MembershipProvider.HashPassword("madmoney") };

            //context.StockwinnersMembers.AddOrUpdate(member => member.EmailAddress, ameen);
            //context.StockwinnersMembers.AddOrUpdate(member => member.EmailAddress, mehdi);
            context.StockwinnersMembers.AddOrUpdate(member => member.EmailAddress, dayee);

            // Make any user with ameen.tayyebi@gmail.com or s.mehdi.ghaffari@gmail.com or seyed@stockwinners.com an admin
            string[] adminEmails = new string[] { "ameen.tayyebi@gmail.com", "s.mehdi.ghaffari@gmail.com", "seyed@stockwinners.com" };
            Role adminRole = context.Roles.First(role => role.Name == PredefinedRoles.Administrator);

            foreach (string adminEmail in adminEmails)
            {
                foreach (User admin in context.Users.Include("Roles").Where(user => user.EmailAddress == adminEmail))
                {
                    if (admin != null)
                    {
                        ICollection<Role> roles = admin.Roles ?? new List<Role>();

                        if (!roles.Any(role => role.RoleId == adminRole.RoleId))
                        {
                            roles.Add(adminRole);
                        }
                    }
                }
            }
        }

        private static void SeedRoles(DatabaseContext context)
        {
            context.Roles.AddOrUpdate(r => r.Name, new Role() { Name = PredefinedRoles.Member });
            context.Roles.AddOrUpdate(r => r.Name, new Role() { Name = PredefinedRoles.Administrator });

            context.SaveChanges();
        }

        private static void SeedCountries(DatabaseContext context)
        {
            string countryList = @"Afghanistan
�land Islands
Albania
Algeria
American Samoa
Andorra
Angola
Anguilla
Antarctica
Antigua And Barbuda
Argentina
Armenia
Aruba
Australia
Austria
Azerbaijan
Bahamas
Bahrain
Bangladesh
Barbados
Belarus
Belgium
Belize
Benin
Bermuda
Bhutan
Bolivia, Plurinational State Of
Bonaire, Sint Eustatius And Saba
Bosnia And Herzegovina
Botswana
Bouvet Island
Brazil
British Indian Ocean Territory
Brunei Darussalam
Bulgaria
Burkina Faso
Burundi
Cambodia
Cameroon
Canada
Cape Verde
Cayman Islands
Central African Republic
Chad
Chile
China
Christmas Island
Cocos (Keeling) Islands
Colombia
Comoros
Congo
Congo, The Democratic Republic Of The
Cook Islands
Costa Rica
C�te D'ivoire
Croatia
Cuba
Cura�ao
Cyprus
Czech Republic
Denmark
Djibouti
Dominica
Dominican Republic
Ecuador
Egypt
El Salvador
Equatorial Guinea
Eritrea
Estonia
Ethiopia
Falkland Islands (Malvinas)
Faroe Islands
Fiji
Finland
France
French Guiana
French Polynesia
French Southern Territories
Gabon
Gambia
Georgia
Germany
Ghana
Gibraltar
Greece
Greenland
Grenada
Guadeloupe
Guam
Guatemala
Guernsey
Guinea
Guinea-Bissau
Guyana
Haiti
Heard Island And Mcdonald Islands
Holy See (Vatican City State)
Honduras
Hong Kong
Hungary
Iceland
India
Indonesia
Iran, Islamic Republic Of
Iraq
Ireland
Isle Of Man
Israel
Italy
Jamaica
Japan
Jersey
Jordan
Kazakhstan
Kenya
Kiribati
Korea, Democratic People's Republic Of
Korea, Republic Of
Kuwait
Kyrgyzstan
Lao People's Democratic Republic
Latvia
Lebanon
Lesotho
Liberia
Libya
Liechtenstein
Lithuania
Luxembourg
Macao
Macedonia, The Former Yugoslav Republic Of
Madagascar
Malawi
Malaysia
Maldives
Mali
Malta
Marshall Islands
Martinique
Mauritania
Mauritius
Mayotte
Mexico
Micronesia, Federated States Of
Moldova, Republic Of
Monaco
Mongolia
Montenegro
Montserrat
Morocco
Mozambique
Myanmar
Namibia
Nauru
Nepal
Netherlands
New Caledonia
New Zealand
Nicaragua
Niger
Nigeria
Niue
Norfolk Island
Northern Mariana Islands
Norway
Oman
Pakistan
Palau
Palestinian Territory, Occupied
Panama
Papua New Guinea
Paraguay
Peru
Philippines
Pitcairn
Poland
Portugal
Puerto Rico
Qatar
R�union
Romania
Russian Federation
Rwanda
Saint Barth�lemy
Saint Helena, Ascension And Tristan Da Cunha
Saint Kitts And Nevis
Saint Lucia
Saint Martin (French Part)
Saint Pierre And Miquelon
Saint Vincent And The Grenadines
Samoa
San Marino
Sao Tome And Principe
Saudi Arabia
Senegal
Serbia
Seychelles
Sierra Leone
Singapore
Sint Maarten (Dutch Part)
Slovakia
Slovenia
Solomon Islands
Somalia
South Africa
South Georgia And The South Sandwich Islands
South Sudan
Spain
Sri Lanka
Sudan
Suriname
Svalbard And Jan Mayen
Swaziland
Sweden
Switzerland
Syrian Arab Republic
Taiwan, Province Of China
Tajikistan
Tanzania, United Republic Of
Thailand
Timor-Leste
Togo
Tokelau
Tonga
Trinidad And Tobago
Tunisia
Turkey
Turkmenistan
Turks And Caicos Islands
Tuvalu
Uganda
Ukraine
United Arab Emirates
United Kingdom
United States
United States Minor Outlying Islands
Uruguay
Uzbekistan
Vanuatu
Venezuela, Bolivarian Republic Of
Viet Nam
Virgin Islands, British
Virgin Islands, U.S.
Wallis And Futuna
Western Sahara
Yemen
Zambia
Zimbabwe";

            context.Countries.AddOrUpdate(
                country => country.Name, 
                    (
                        from countryName in countryList.Split(new string[] { "\n" }, StringSplitOptions.None)
                        select new Country() { Name = countryName.Trim() }
                    )
                    .ToArray()
                );
        }

        private static void SeedSubscriptionTypesAndFrequencies(DatabaseContext context)
        {
            SubscriptionFrequency monthly = context.SubscriptionFrequencies.FirstOrDefault(st => st.Name == PredefinedSubscriptionFrequencies.Monthly);

            if (monthly == null)
            {
                monthly = new SubscriptionFrequency() { Name = PredefinedSubscriptionFrequencies.Monthly };
                context.SubscriptionFrequencies.Add(monthly);
            }

            SubscriptionFrequency quarterly = context.SubscriptionFrequencies.FirstOrDefault(st => st.Name == PredefinedSubscriptionFrequencies.Quarterly);

            if (quarterly == null)
            {
                quarterly = new SubscriptionFrequency() { Name = PredefinedSubscriptionFrequencies.Quarterly };
                context.SubscriptionFrequencies.Add(quarterly);
            }

            SubscriptionFrequency yearly = context.SubscriptionFrequencies.FirstOrDefault(st => st.Name == PredefinedSubscriptionFrequencies.Yearly);

            if (yearly == null)
            {
                yearly = new SubscriptionFrequency() { Name = PredefinedSubscriptionFrequencies.Yearly };
                context.SubscriptionFrequencies.Add(yearly);
            }

            context.SaveChanges();
            
            context.SubscriptionTypes.AddOrUpdate(st => st.Price,
                new SubscriptionType() { SubscriptionFrequencyId = monthly.SubscriptionFrequencyId, SubscriptionFrequency = monthly, Price = 39, IsAvailableToUsers = false, IsAddOn = false, Name = "Monthly Subscription" },
                new SubscriptionType() { SubscriptionFrequencyId = quarterly.SubscriptionFrequencyId, SubscriptionFrequency = quarterly, Price = 110, IsAvailableToUsers = false, IsAddOn = false, Name = "Quarterly Subscription" },
                new SubscriptionType() { SubscriptionFrequencyId = yearly.SubscriptionFrequencyId, SubscriptionFrequency = yearly, Price = 350, IsAvailableToUsers = false, IsAddOn = false, Name = "Yearly Subscription" },
                new SubscriptionType() { SubscriptionFrequencyId = monthly.SubscriptionFrequencyId, SubscriptionFrequency = monthly, Price = 10, IsAvailableToUsers = true, IsAddOn = true, Name = "Auto Trading", Description = "Auto trading enables you to have our registered brokers trade our selections on your behalf. With auto trading, our selections are automatically traded on your behalf based on allocation rules that you specify. We've partnered with EOption to enable this functionality and its use requires having an account with EOption. Please refer to http://eoption.com/auto_trading.html for more information on auto trading." }
                );
        }
    }
}
