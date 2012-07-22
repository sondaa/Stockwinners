namespace WebSite.Migrations
{
    using System;
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

            Configuration.SeedStockPickTypes(context);

            Configuration.SeedOptionPickTyps(context);
        }

        private static void SeedOptionPickTyps(DatabaseContext context)
        {
            context.OptionPickTypes.AddOrUpdate(optionPickType => optionPickType.Name, new OptionPickType() { Name = "Vertical Call Spread" });
        }

        private static void SeedStockPickTypes(DatabaseContext context)
        {
            context.StockPickTypes.AddOrUpdate(stockPickType => stockPickType.Name,
                new StockPickType() { Name = "Momentum" },
                new StockPickType() { Name = "Fundamentals" },
                new StockPickType() { Name = "Rumour" });
        }

        private static void SeedRoles(DatabaseContext context)
        {
            context.Roles.AddOrUpdate(r => r.Name, new Role() { Name = PredefinedRoles.Member });
            context.Roles.AddOrUpdate(r => r.Name, new Role() { Name = PredefinedRoles.Administrator });
        }

        private static void SeedCountries(DatabaseContext context)
        {
            string countryList = @"Afghanistan
Åland Islands
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
Côte D'ivoire
Croatia
Cuba
Curaçao
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
Réunion
Romania
Russian Federation
Rwanda
Saint Barthélemy
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
                        from countryName in countryList.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                        select new Country() { Name = countryName }
                    )
                    .ToArray()
                );
        }

        private static void SeedSubscriptionTypesAndFrequencies(DatabaseContext context)
        {
            SubscriptionFrequency monthly = new SubscriptionFrequency() { Name = PredefinedSubscriptionFrequencies.Monthly };
            SubscriptionFrequency quarterly = new SubscriptionFrequency() { Name = PredefinedSubscriptionFrequencies.Quarterly };
            SubscriptionFrequency yearly = new SubscriptionFrequency() { Name = PredefinedSubscriptionFrequencies.Yearly };

            context.SubscriptionFrequencies.AddOrUpdate(sf => sf.Name, monthly, quarterly, yearly);

            context.SubscriptionTypes.AddOrUpdate(st => st.Price,
                new SubscriptionType() { SubscriptionFrequencyId = monthly.SubscriptionFrequencyId, SubscriptionFrequency = monthly, Price = 39, IsAvailableToUsers = true },
                new SubscriptionType() { SubscriptionFrequencyId = quarterly.SubscriptionFrequencyId, SubscriptionFrequency = quarterly, Price = 110, IsAvailableToUsers = true },
                new SubscriptionType() { SubscriptionFrequencyId = yearly.SubscriptionFrequencyId, SubscriptionFrequency = yearly, Price = 350, IsAvailableToUsers = true }
                );
        }
    }
}
